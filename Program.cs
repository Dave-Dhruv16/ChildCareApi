using ChildCareApi.Repositories.Interfaces;
using ChildCareApi.Repositories.Implementation;
using System.Data.SqlClient;
using FluentValidation;
using FluentValidation.AspNetCore;
using ChildCareApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddFluentValidation();

builder.Services.AddTransient<IValidator<User>, UserValidator>();
builder.Services.AddTransient<IValidator<Tutor>, TutorValidator>();
builder.Services.AddTransient<IValidator<TutoringRequest>, TutoringRequestValidator>();
builder.Services.AddTransient<IValidator<Session>, SessionValidator>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<ITutoringRequestRepository, TutoringRequestRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddTransient(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }
    return new SqlConnection(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = new { message = "An unexpected error occurred. Please try again later." };
        await context.Response.WriteAsJsonAsync(error);
    });
});

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 400 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "Validation failed. Check your inputs." });
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
