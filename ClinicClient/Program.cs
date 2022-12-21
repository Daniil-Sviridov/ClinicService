
using ClinicService.Proto;
using ClinicServiceNamespace;
using Grpc.Core;
using Grpc.Net.Client;
using static ClinicService.Proto.AuthenticateService;
using static ClinicServiceNamespace.ClinicService;

//AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

//using var channel = GrpcChannel.ForAddress("http://localhost:5001");

using var channel = GrpcChannel.ForAddress("https://localhost:5002");

//ClinicServiceClient clinicServiceClient = new ClinicServiceClient(channel);

AuthenticateServiceClient authenticateServiceClient = new AuthenticateServiceClient(channel);
var authenticationResponse = authenticateServiceClient.Login(new AuthenticationRequest
{
    UserName = "daniil@gmail.com",
    Password = "12345"
});

if (authenticationResponse.Status != 0)
{
    Console.WriteLine("Authentication error.");
    Console.ReadKey();
    return;
}

Console.WriteLine($"Session token: {authenticationResponse.SessionContext.SessionToken}");

var callCredentials = CallCredentials.FromInterceptor((c, m) =>
{
    m.Add("Authorization",
        $"Bearer {authenticationResponse.SessionContext.SessionToken}");
    return Task.CompletedTask;
});

var protectedChannel = GrpcChannel.ForAddress("https://localhost:5002", new GrpcChannelOptions
{
    Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials)
});

ClinicServiceClient clinicServiceClient = new ClinicServiceClient(protectedChannel);

var createClientResponse = clinicServiceClient.CreateClinet(new CreateClientRequest
{
    Document = "Паспорт 1",
    FirstName = "Имя 3",
    Patronymic = "Отчество 2",
    Surname = "Фамилия 1"
});

if (createClientResponse.ErrCode == 0)
{
    Console.WriteLine($"Client #{createClientResponse.ClientId} created successfully.");
}
else
{
    Console.WriteLine($"Create client error.\nErrorCode: {createClientResponse.ErrCode}\nErrorMessage: {createClientResponse.ErrMessage}");
}

var getClientResponse = clinicServiceClient.GetClients(new GetClientsRequest());

if (createClientResponse.ErrCode == 0)
{
    Console.WriteLine("Clients");
    Console.WriteLine("=======\n");

    foreach (var client in getClientResponse.Clients)
    {
        Console.WriteLine($"#{client.ClientId} {client.Document} {client.Surname} {client.FirstName} {client.Patronymic}");
    }
}
else
{
    Console.WriteLine($"Get clients error.\nErrorCode: {getClientResponse.ErrCode}\nErrorMessage: {getClientResponse.ErrMessage}");
}

Console.ReadKey();