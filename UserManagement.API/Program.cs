using FluentValidation;
using FluentValidation.AspNetCore;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Validators;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Services;
using UserManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture Setup
// 1. Domain/Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserManagement.Application.Interfaces.IPasswordHasher).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddFluentValidationAutoValidation();

// 2. Infrastructure
var mongoConn = builder.Configuration.GetValue<string>("MongoDB:ConnectionString") ?? "mongodb://localhost:27017";
var mongoDb = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "UserManagementDb";

builder.Services.AddSingleton(new MongoDbContext(mongoConn, mongoDb));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Exception Handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception is UserDomainException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = exception.Message });
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An internal server error occurred." });
        }
    });
});

app.UseAuthorization();
app.MapControllers();

app.Run();
