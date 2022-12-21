using ClinicService.Data;
using ClinicServiceNamespace;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicService.Services.Impl
{
    [Authorize]
    public class ClinicService : ClinicServiceBase
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<ClinicService> _logger;

        public ClinicService(ClinicServiceDbContext dbContext, ILogger<ClinicService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override Task<CreateClientResponse> CreateClinet(CreateClientRequest request, ServerCallContext context)
        {
            try
            {
                var client = new Client
                {
                    Document = request.Document,
                    Surname = request.Surname,
                    FirstName = request.FirstName,
                    Patronymic = request.Patronymic
                };

                _dbContext.Clients.Add(client);
                _dbContext.SaveChanges();

                var response = new CreateClientResponse
                {
                    ClientId = client.Id,
                    ErrCode = 0,
                    ErrMessage = ""
                };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                var response = new CreateClientResponse
                {
                    ErrCode = 500,
                    ErrMessage = "Err service"
                };
                return Task.FromResult(response);
            }
        }

        public override Task<GetClientsResponse> GetClients(GetClientsRequest request, ServerCallContext context)
        {
            try
            {
                var clients = _dbContext.Clients.Select(client => new ClientResponse
                {
                    ClientId = client.Id,
                    Document = client.Document,
                    FirstName = client.FirstName,
                    Patronymic = client.Patronymic,
                    Surname = client.Surname
                }).ToList();

                var response = new GetClientsResponse();
                response.Clients.AddRange(clients);

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                var response = new GetClientsResponse
                {
                    ErrCode = 501,
                    ErrMessage = "Err service"
                };
                return Task.FromResult(response);
            }
        }
    }
}
