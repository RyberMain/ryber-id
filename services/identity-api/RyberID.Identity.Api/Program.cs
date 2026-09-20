using RyberID.Identity.Application.Passkeys;
using RyberID.Identity.Application.Users;
using RyberID.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<CreateUser>();
builder.Services.AddScoped<BeginPasskeyRegistration>();
builder.Services.AddScoped<CompletePasskeyRegistration>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();


