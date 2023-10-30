using SmartInventorySystemApi.Api.Middlewares;
using SmartInventorySystemApi.Application;
using SmartInventorySystemApi.Infrastructure;
using SmartInventorySystemApi.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMapper();
builder.Services.AddRepositories();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddJWTTokenAuthentication(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalUserCustomMiddleware>();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }