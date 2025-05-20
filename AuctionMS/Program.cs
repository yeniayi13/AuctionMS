using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
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
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Core.Service.Firebase;
using AuctionMS.Infrastructure.Services.Firebase;



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
    const string connectionString = "";
    var databaseName = "AuctionMS";
    return new ApplicationDbContextMongo(connectionString, databaseName);
});


builder.Services.AddScoped(sp =>
{
    var dbContext = sp.GetRequiredService<IApplicationDbContextMongo>();
    return dbContext.Database.GetCollection<AuctionEntity>("Auction"); // Nombre de la colección en MongoDB
});


builder.Services.AddSingleton<RabbitMQConnection>(provider =>
{
    var rabbitMQConnection = new RabbitMQConnection();
    rabbitMQConnection.InitializeAsync().Wait(); //  Inicialización segura
    return rabbitMQConnection;
});




builder.Services.AddSingleton<IEventBus<CreateAuctionDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<CreateAuctionDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<UpdateAuctionDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<UpdateAuctionDto>(rabbitMQConnection);
});


builder.Services.AddSingleton<IEventBus<GetAuctionDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<GetAuctionDto>(rabbitMQConnection);
});




//  Usa la misma instancia de `RabbitMQConnection` para el Consumer
builder.Services.AddSingleton<RabbitMQConsumer>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQConsumer(rabbitMQConnection);
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
//builder.Services.AddHttpClient<IUserService, UserService>();

//Configurar Firebase Storage Settings desde appsettings.json
builder.Services.Configure<FirebaseStorageSettings>(builder.Configuration.GetSection("Firebase"));

// Agregar el cliente de Firebase Storage
builder.Services.AddSingleton<IFirebaseStorageService, FirebaseStorageService>();


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