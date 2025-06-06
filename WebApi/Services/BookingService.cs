using WebApi.Dto;
using WebApi.Factories;
using WebApi.Models;
using WebApi.Protos;
using WebApi.Repositories;

namespace WebApi.Services;

public class BookingService(BookingRepository repository, ILogger<BookingModel> logger, GrpcService grpc)
{
    private readonly BookingRepository _repository = repository;
    private readonly ILogger<BookingModel> _logger = logger;
    //private readonly BookingHandler.BookingHandlerClient _client = client;
    private readonly GrpcService _grpc = grpc;



    public async Task<Result<BookingModel>> CreateAsync(BookingRegForm dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("CreateAsync method was called with null DTO.");
            return new Result<BookingModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entity = BookingFactory.EntityFromDto(dto);

            var newEntityResult = await _repository.CreateAsync(entity);

            if (!newEntityResult.Success || newEntityResult.Data == null)
            {
                await _repository.RollbackTransactionAsync();
                _logger.LogWarning($"Failed to create booking in repository: {newEntityResult.ErrorMessage}");
                return new Result<BookingModel> { Success = false, StatusCode = newEntityResult.StatusCode, ErrorMessage = newEntityResult.ErrorMessage };
            }

            var seatsRequest = new SeatsRequest
            {
                Id = entity.EventId,
                SeatsOrdered = entity.Seats
            };


            var seatsResult = await _grpc.UpdateSeats(seatsRequest);

            if (!seatsResult.Success)
            {
                await _repository.RollbackTransactionAsync();
                _logger.LogWarning($"Failed to update seats available.\n{seatsResult.Message}");
                return new Result<BookingModel> { Success = false, StatusCode = 400, ErrorMessage = seatsResult.Message };
            }

            await _repository.SaveChangesAsync();
            await _repository.CommitTransactionAsync();

            var newModel = BookingFactory.ModelFromEntity(newEntityResult.Data);
            return newModel != null
                ? new Result<BookingModel> { Success = true, StatusCode = 201, Data = newModel }
                : new Result<BookingModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create booking." };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError($"Something went wrong creating booking the event: \n{ex}\n{ex.Message}");
            return new Result<BookingModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong creating booking.\n{ex.Message}" };
        }
    }

    public async Task<Result<BookingModel>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        if (!result.Success || result.DataList == null)
        {
            _logger.LogWarning("Failed to fetch bookings {Error}", result.ErrorMessage);
            return new Result<BookingModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        var categories = result.DataList.Select(x => BookingFactory.ModelFromEntity(x)).ToList();

        return categories.Any()
            ? new Result<BookingModel> { Success = true, DataList = categories }
            : new Result<BookingModel> { Success = false, StatusCode = 404, ErrorMessage = "No bookings was found." };
    }

    public async Task<Result<BookingModel>> GetOneAsync(string id)
    {
        var result = await _repository.GetOneAsync(x => x.Id == id);
        if (!result.Success || result.Data == null)
        {
            _logger.LogWarning("Failed to fetch booking {Error}", result.ErrorMessage);
            return new Result<BookingModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        var category = BookingFactory.ModelFromEntity(result.Data);

        return category != null
            ? new Result<BookingModel> { Success = true, StatusCode = 200, Data = category }
            : new Result<BookingModel> { Success = false, StatusCode = 404, ErrorMessage = $"No booking with id {id} was found." };
    }

    public async Task<Result<BookingModel>> UpdateAsync(BookingModel model)
    {
        if (model == null)
        {
            _logger.LogWarning("Update async method was called with invalid data.");
            return new Result<BookingModel> { Success = false, StatusCode = 400, ErrorMessage = "Required fields can not be empty." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entityToUpdate = BookingFactory.EntityFromModel(model);
            var result = _repository.Update(entityToUpdate);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to update booking in repository {Error}", result.ErrorMessage);
                return new Result<BookingModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            var updatedModel = BookingFactory.ModelFromEntity(entityToUpdate);
            return updatedModel != null
                ? new Result<BookingModel> { Success = true, StatusCode = 200, Data = updatedModel }
                : new Result<BookingModel> { Success = false, StatusCode = 500, ErrorMessage = "Failed to convert entity to model." };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError($"Failed to update booking: \n{ex}\n{ex.Message}");
            return new Result<BookingModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong updating booking.\n{ex.Message}" };
        }
    }

    public async Task<Result<BookingModel>> DeleteAsync(string id)
    {
        if (id == null)
        {
            _logger.LogWarning("DeleteAsync method was called with null ID.");
            return new Result<BookingModel> { Success = false, StatusCode = 400, ErrorMessage = "Invalid id." };
        }

        try
        {
            await _repository.BeginTransactionAsync();

            var entityResult = await _repository.GetOneAsync(x => x.Id == id);
            if (!entityResult.Success || entityResult.Data == null)
            {
                _logger.LogWarning("Failed to fetch event to delete from repository {Error}", entityResult.ErrorMessage);
                return new Result<BookingModel> { Success = false, StatusCode = entityResult.StatusCode, ErrorMessage = entityResult.ErrorMessage };
            }

            var result = _repository.DeleteAsync(entityResult.Data);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Failed to delete booking {Error}", result.ErrorMessage);
                return new Result<BookingModel> { Success = false, StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
            }

            await _repository.CommitTransactionAsync();
            await _repository.SaveChangesAsync();

            return new Result<BookingModel> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _logger.LogError($"Failed to delete booking: \n{ex}\n{ex.Message}");
            return new Result<BookingModel> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong deleting booking.\n{ex.Message}" };
        }
    }
}
