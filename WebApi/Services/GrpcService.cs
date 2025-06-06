using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using WebApi.Protos;
using static Grpc.Core.Metadata;

namespace WebApi.Services;

public class GrpcService
{
    private readonly IConfiguration _config;
    private readonly ILogger<GrpcService> _logger ;
    private readonly BookingHandler.BookingHandlerClient _client;

    public GrpcService(IConfiguration config, ILogger<GrpcService> logger)
    {
        _logger = logger;
        _config = config; var uri = _config["GrpcUri"];
        logger.LogInformation("Using GrpcUri: {Uri}", uri);
        var channel = GrpcChannel.ForAddress(_config["GrpcUri"]);
        _client = new BookingHandler.BookingHandlerClient(channel);
        _logger = logger;

        Console.WriteLine($"gRPC channel state: {channel.State}");
    }

    public async Task<SeatsReply> UpdateSeats(SeatsRequest request)
    {
        var seatsResult = await _client.UpdateSeatsLeftAsync(request);
        return seatsResult;
    }
}
