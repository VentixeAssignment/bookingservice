using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Repositories;

public class BookingRepository
{
    protected readonly DataContext _context;
    protected readonly DbSet<BookingEntity> _table;
    private IDbContextTransaction _transaction = null!;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(DataContext context, ILogger<BookingRepository> logger)
    {
        _context = context;
        _table = _context.Set<BookingEntity>();
        _logger = logger;
    }


    #region CRUD

    public virtual async Task<Result<BookingEntity>> CreateAsync(BookingEntity entity)
    {
        if (entity == null)
            return new Result<BookingEntity> { Success = false, ErrorMessage = "Invalid data." };

        try
        {
            await BeginTransactionAsync();
            var result = await _context.AddAsync(entity);

            return result != null
                ? new Result<BookingEntity> { Success = true, StatusCode = 201, Data = entity }
                : new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to create event." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to create entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Something went wrong creating entity." };
        }
    }

    public async Task<Result<BookingEntity>> GetAllAsync()
    {
        var entities = await _table.ToListAsync();

        return entities.Count > 0
            ? new Result<BookingEntity> { Success = true, StatusCode = 200, DataList = entities }
            : new Result<BookingEntity> { Success = false, StatusCode = 404, ErrorMessage = "No entities were found." };
    }

    public async Task<Result<BookingEntity>> GetOneAsync(Expression<Func<BookingEntity, bool>> expression)
    {
        if (expression == null)
            return new Result<BookingEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid expression." };

        var result = await _table.FirstOrDefaultAsync(expression);

        return result != null
            ? new Result<BookingEntity> { Success = true, StatusCode = 200, Data = result }
            : new Result<BookingEntity> { Success = false, StatusCode = 404, ErrorMessage = "No entity found matching input expression." };
    }

    public Result<BookingEntity> Update(BookingEntity entity)
    {
        if (entity == null)
            return new Result<BookingEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid input data." };

        try
        {
            var result = _table.Update(entity);

            return result == null
                ? new Result<BookingEntity> { Success = true, StatusCode = 200, Data = result?.Entity }
                : new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to update entity." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to update entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Something went wrong updating entity." };
        }
    }

    public Result<BookingEntity> DeleteAsync(BookingEntity entity)
    {
        if (entity == null)
            return new Result<BookingEntity> { Success = false, StatusCode = 400, ErrorMessage = "Invalid input." };

        try
        {
            var result = _table.Remove(entity);

            return result != null
                ? new Result<BookingEntity> { Success = true, StatusCode = 200 }
                : new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to delete entity." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to update entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Something went wrong deleting entity." };
        }
    }

    #endregion



    #region Transactions

    public async Task<Result<BookingEntity>> BeginTransactionAsync()
    {
        if (_transaction != null)
            return new Result<BookingEntity> { Success = false, StatusCode = 400, ErrorMessage = "Failed to begin transaction because a transaction is already started." };
    
        try
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return new Result<BookingEntity> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "\nFailed to start new transaction for entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong when starting transation." };
        }
    }

    public async Task<Result<BookingEntity>> CommitTransactionAsync()
    {
        if (_transaction == null)
            return new Result<BookingEntity> { Success = false, StatusCode = 400, ErrorMessage = "Another transaction is already in use." };

        try
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;

            return new Result<BookingEntity> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            await _transaction.RollbackAsync();
            _logger.LogError(ex.Message, "Failed to commit transaction for entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = $"Something went wrong when committing transation." };
        }
    }

    public async Task<Result<BookingEntity>> RollbackTransactionAsync()
    {
        if (_transaction == null)
            return new Result<BookingEntity> { Success = false, StatusCode = 400, ErrorMessage = "There is no transaction to roll back." };

        try
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;

            return new Result<BookingEntity> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            _transaction = null!;
            _logger.LogError(ex.Message, "Failed to roll back transaction for entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to rollback transaction." };
        }
    }

    public async Task<Result<BookingEntity>> SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            return new Result<BookingEntity> { Success = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Failed to save changes for entity of type {EntityType}", typeof(BookingEntity).Name);
            return new Result<BookingEntity> { Success = false, StatusCode = 500, ErrorMessage = "Failed to save changes." };
        }
    }

    #endregion

}
