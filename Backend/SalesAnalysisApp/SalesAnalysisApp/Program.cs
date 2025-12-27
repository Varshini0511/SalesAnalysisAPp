using Microsoft.EntityFrameworkCore;
using SalesAnalysisApp.Domain;
using Microsoft.OpenApi.Models;
using SalesAnalysisApp.Repository;
using SalesAnalysisApp.Service;
using SalesAnalysisApp;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SalesDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
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
   
