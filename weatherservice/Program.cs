using MongoDB.Driver;
using weatherservice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// MongoDB client registration
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    return new MongoClient("mongodb://localhost:27017");
});

builder.Services.AddSingleton(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    return WeatherApiService.GetInstance(mongoClient);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

