using SkyFlow.Console.Application.Services;
using SkyFlow.Console.Infrastructure.Data;

var connectionString = Environment.GetEnvironmentVariable("SKYFLOW_CONNECTION_STRING")
	?? "Server=localhost;Database=SkyFlowDb;Trusted_Connection=True;TrustServerCertificate=True;";

var repository = new SqlServerDataRepository(connectionString);
var app = new SkyFlowApp(repository);

try
{
	app.Run();
}
catch (Exception ex)
{
	Console.WriteLine("Application error: " + ex.Message);
	Console.WriteLine("Check that SQL Server is running and the SKYFLOW_CONNECTION_STRING value is correct.");
}
