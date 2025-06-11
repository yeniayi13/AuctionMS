using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using AuctionMS;
using AuctionMS.Application;
using AuctionMS.Infrastructure;
using AuctionMS.Infrastructure.Settings;
using AuctionMS.Common.AutoMapper;
using AuctionMS.Core.Database;
using AuctionMS.Infrastructure.Database.Context.Mongo;
using AuctionMS.Core.RabbitMQ;
using AuctionMS.Infrastructure.RabbitMQ.Connection;
using AuctionMS.Infrastructure.RabbitMQ.Consumer;
using AuctionMS.Infrastructure.RabbitMQ;
using AuctionMS.Common.Dtos.Auction.Request;
using AuctionMS.Common.Dtos.Auction.Response;
using AuctionMS.Core.Service.Auction;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Core.Service.User;
using AuctionMS.Infrastructure.Services.Auction;
//using AuctionMS.Core.Service.Product;
using AuctionMS.Infrastructure.Services.User;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication();

// Registrar el serializador de GUID
BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
// Registro de los perfiles de AutoMapper
var profileTypes = new[]
{
 
     typeof(AuctionProfile)
};

foreach (var profileType in profileTypes)
{
    builder.Services.AddAutoMapper(profileType);
}


builder.Services.AddSingleton<IApplicationDbContextMongo>(sp =>
{
    const string connectionString = "mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    var databaseName = "AuctionMS";
    return new ApplicationDbContextMongo(connectionString, databaseName);
});

builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();
builder.Services.AddSingleton<IConnectionRabbbitMQ, RabbitMQConnection>();

builder.Services.AddHostedService<RabbitMQBackgroundService>();


builder.Services.AddScoped(sp =>
{
    var dbContext = sp.GetRequiredService<IApplicationDbContextMongo>();
    return dbContext.Database.GetCollection<AuctionEntity>("Auction"); // Nombre de la colección en MongoDB
});


builder.Services.AddSingleton<IConnectionFactory>(provider =>
{
    return new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
    };
});

// ?? Registrar `RabbitMQConnection` pasando `IConnectionFactory` en el constructor
builder.Services.AddSingleton<IConnectionRabbbitMQ>(provider =>
{
    var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
    var rabbitMQConnection = new RabbitMQConnection(connectionFactory);
    rabbitMQConnection.InitializeAsync().Wait(); // ?? Ejecutar inicialización antes de inyectarlo
    return rabbitMQConnection;
}); builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();

// ?? Ahora los Producers pueden usar `RabbitMQConnection`

builder.Services.AddSingleton<IEventBus<CreateAuctionDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    return new RabbitMQProducer<CreateAuctionDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<UpdateAuctionDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    return new RabbitMQProducer<UpdateAuctionDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<GetAuctionDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    return new RabbitMQProducer<GetAuctionDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IMongoCollection<GetAuctionDto>>(provider =>
{
    var mongoClient = new MongoClient("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
    var database = mongoClient.GetDatabase("AuctionMS");
    return database.GetCollection<GetAuctionDto>("Auction");
});

// ?? Registrar `RabbitMQConsumer` solo una vez
builder.Services.AddSingleton<RabbitMQConsumer>(provider =>
{

    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    var auctionCollection = provider.GetRequiredService<IMongoCollection<GetAuctionDto>>();
    return new RabbitMQConsumer(rabbitMQConnection, auctionCollection);
});


// Iniciar el consumidor automáticamente con `RabbitMQBackgroundService`
builder.Services.AddHostedService<RabbitMQBackgroundService>();


//* Para que funcione el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen();


var _appSettings = new AppSettings();
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
_appSettings = appSettingsSection.Get<AppSettings>();
builder.Services.Configure<AppSettings>(appSettingsSection);

builder.Services.Configure<HttpClientUrl>(
    builder.Configuration.GetSection("HttpClientAddress"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddHttpClient<IProductService, ProductService>();

//Configurar Firebase Storage Settings desde appsettings.json


// Agregar el cliente de Firebase Storage
//builder.Services.AddSingleton<IFirebaseStorageService, FirebaseStorageService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.MapGet("/", () => "Connected!");

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();



