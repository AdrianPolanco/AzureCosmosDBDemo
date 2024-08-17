using Azure.Identity;
using AzureCosmosDBDemo.Interfaces;
using AzureCosmosDBDemo.Models;
using AzureCosmosDBDemo.Models.Options;
using AzureCosmosDBDemo.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

//Packages required:
//Microsoft.Azure.Cosmos
//Azure.Extensions.AspNetCore.Configuration.Secrets
//Azure.Identity

//IMPORTANT: We have to specify a partition key when creating a container in CosmosDB, so we have to add the partition key path to the model and have no problems
//when creating the CRUD in the container

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//IMPORTANT: Getting the Azure Key Vault Url
var keyVaultUrl = new Uri(builder.Configuration.GetSection("KeyVaultUrl").Value!);
//IMPORTANT: Getting the credentials from the current account that is signed in Visual Studio, which is the same that owns the Azure Key Vault resource
var azureCredentials = new DefaultAzureCredential();

//IMPORTANT: Adding Azure Key Vault services
builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);
//Configuring the CosmosOptions for using the Options pattern
builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("CosmosDb"));

//Providing the dependency injection for the CosmosClient
builder.Services.AddSingleton(sp =>
{
    //var options = sp.GetRequiredService<IOptions<CosmosOptions>>().Value;

    //IMPORTANT: Getting the CosmosDB connection string from Azure Key Vault
    string cosmosConnectionString = builder.Configuration.GetSection("CosmosConnectionString").Value!;
    return new CosmosClient(cosmosConnectionString);
});
builder.Services.AddScoped<ICosmosService, CosmosService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/items", async (ICosmosService service) =>
{
    var response = await service.GetMultipleAsync("SELECT * FROM c");
    return Results.Ok(response);
});

app.MapPost("/items", async (ICosmosService service, Item item) =>
{
    var response = await service.AddAsync(item);
    return Results.Ok(response);
}); 

app.MapGet("/items/{id}", async (ICosmosService service, string id) =>
{
    var response = await service.GetAsync(id);
    return response is not null ? Results.Ok(response) : Results.NotFound();
});

app.MapPut("/items/{id}", async (ICosmosService service, string id, Item item) =>
{
    var response = await service.UpdateAsync(id, item);
    return Results.Ok(response);
});

app.MapDelete("/items/{id}", async (ICosmosService service, string id) =>
{
    var response = await service.DeleteAsync(id);
    return Results.Ok(response);
});

app.Run();