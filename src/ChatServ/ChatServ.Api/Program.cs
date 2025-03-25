using ChatServ.Api;
using ChatServ.Api.Configuration;
using ChatServ.Core;
using ChatServ.Core.Configuration;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("CHATSERV_");

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Settings"));
builder.Services.Configure<BasicNonTextHouseOptions>(builder.Configuration.GetSection("House"));

// Add services to the container.
builder.Services.AddSingleton<IHouse, BasicNonTextHouse>();

builder.Services.AddHostedService<HouseService>();

var ops = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
};

builder.Services.AddWebSockets(config => config = ops);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().WithOrigins("https://localhost:5003", "https://identityserver:5003", "wss://localhost:5003"));



app.UseWebSockets();
app.MapControllers();

app.Run();
