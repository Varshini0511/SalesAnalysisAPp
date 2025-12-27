using Microsoft.EntityFrameworkCore;
using SalesAnalysisApp.Domain;
using Microsoft.OpenApi.Models;
using SalesAnalysisApp.Repository;
using SalesAnalysisApp.Service;
using SalesAnalysisApp;
using System;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SalesDbContext>(options =>
options.UseSqlServer("server=VARCHINIDEVI\\SQLEXPRESS; database=SaleAnalysisDb;Encrypt=false; Trusted_Connection=true",
        sqlServerOptionsAction =>
        {
            // Enable retry logic for transient failures
            sqlServerOptionsAction.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    )
);


builder.Services.AddScoped<ISalesRepository,SalesRepository>();
builder.Services.AddScoped<IDataRefreshService, DataRefreshService>();

builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowAll",

       builder => builder.AllowAnyOrigin()
       .AllowAnyHeader().AllowAnyMethod());
   });

   var app = builder.Build();

       // Configure the HTTP request pipeline.
       if (app.Environment.IsDevelopment())
       {
           app.UseSwagger();
           app.UseSwaggerUI();
       }

       app.UseCors("AllowAll");
       app.UseExceptionHandlingMiddleware();
       app.UseHttpsRedirection();

       app.UseAuthorization();

       app.MapControllers();

       app.Run();
   
