using Microsoft.AspNetCore.Authentication;
using RyberID.Identity.Api.Configuration;
using RyberID.Identity.Api.Endpoints;
using RyberID.Identity.Api.Middleware;
using RyberID.Identity.Application.Authentication;
using RyberID.Identity.Application.Passkeys;
using RyberID.Identity.Application.Sessions;
using RyberID.Identity.Application.Users;
using RyberID.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            SessionAuthenticationDefaults.Scheme;

        options.DefaultChallengeScheme =
            SessionAuthenticationDefaults.Scheme;

        options.DefaultForbidScheme =
            SessionAuthenticationDefaults.Scheme;
    })
    .AddScheme<
        AuthenticationSchemeOptions,
        SessionAuthenticationHandler>(
            SessionAuthenticationDefaults.Scheme,
            _ =>
            {
            });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<
    SessionCookieManager>();

builder.Services.AddScoped<CreateUser>();
builder.Services.AddScoped<BeginPasskeyRegistration>();
builder.Services.AddScoped<CompletePasskeyRegistration>();
builder.Services.AddScoped<BeginPasskeyAuthentication>();
builder.Services.AddScoped<CompletePasskeyAuthentication>();
builder.Services.AddScoped<CreateSession>();
builder.Services.AddScoped<GetSessionState>();
builder.Services.AddScoped<RevokeSession>();
builder.Services.AddScoped<SignInWithPasskey>();
builder.Services.AddScoped<ResolveActiveSession>();
builder.Services.AddScoped<RevokeCurrentSession>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var api =
    app.MapGroup(
        "/api/v1");

api.MapPasskeyAuthenticationEndpoints();
api.MapSessionEndpoints();

app.Run();
