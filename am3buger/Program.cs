﻿using am3burger.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using static am3burger.Models.MailSetting;

var builder = WebApplication.CreateBuilder(args);

// 注入am3burgerContext的類別
builder.Services.AddDbContext<Am3burgerContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("am3burger")));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 加入本機分散式記憶體快取服務
builder.Services.AddDistributedMemoryCache();

// 加入redis分散式快取服務
//builder.Services.AddSingleton<IConnectionMultiplexer>(
//    ConnectionMultiplexer.Connect(
//        new ConfigurationOptions()
//        {
//            EndPoints = { { "localhost", 6379 } }
//        }
//    )
// );

// redis win11安裝教學
//https://redis.io/blog/install-redis-windows-11/

// CORS跨來源共用設定
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5173","https://am3burger.sakuyaonline.uk", "https://sanae.am3buger-vue.pages.dev/").WithHeaders("*").WithMethods("*").AllowCredentials();
    });
});

// 從appsettings.json當中取得寄件人設定
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<am3burger.Helper.IMailService, am3burger.Helper.MailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 呼叫 MapSwagger().RequireAuthorization 來保護 Swagger UI 端點。
app.MapSwagger().RequireAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
