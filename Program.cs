using Play.Catalog.Service.Settings;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Inventory.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(MongoDbSettings.SettingName));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection(ServiceSettings.SettingName));
var rabbitMQSettings = builder.Configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
builder.Services.
    AddMongo(mongoDbSettings)
    .AddMongoRepository<InventoryItem>("inventoryitems")
    .AddMongoRepository<CatalogItem>("catalogitems")
    .AddMassTransitWithRabbitMQ(rabbitMQSettings, serviceSettings);
    




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
