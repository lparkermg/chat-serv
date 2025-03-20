using ChatServ.Api;
using ChatServ.Api.Configuration;
using ChatServ.Core;
using ChatServ.Core.Configuration;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("CHATSERV_");

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Settings"));
builder.Services.Configure<BasicNonTextHouseOptions>(builder.Configuration.GetSection("House"));

// Add services to the container.
builder.Services.AddSingleton<IHouse, BasicNonTextHouse>();

builder.Services.AddHostedService<HouseService>();


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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
