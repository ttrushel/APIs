using ClientApi.Callouts;
using ClientApi.Data;
using ClientApi.Helpers;
using ClientApi.Repositories;
using ClientApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IClientCalloutService, ClientCalloutService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddAutoMapper(typeof(Program));

var _configuration = builder.Configuration;
builder.Services.AddDbContext<ClientContext>(options =>
    options.UseSqlServer(_configuration.GetConnectionString("ClientDemo")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();