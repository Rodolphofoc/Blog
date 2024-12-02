using System.Net;
using System.Text;
using Applications.Interceptions;
using Applications.Socket;
using CrossCuting;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configura o Kestrel para ouvir na porta 7104
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 7104);
});

ConfigureAuthentication(builder.Services, builder.Configuration);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();

void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });
}

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Serviços padrão do ASP.NET
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Dependências de projeto
    services.AddScoped<IResponse, Response>();
    services.AddInfrastructure(configuration);
    services.AddRepository(configuration);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
    services.AddHttpContextAccessor();

    // Configuração de CORS
    services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost",
            builder => builder
                .WithOrigins("http://127.0.0.1:5500") // Permitir apenas a origem específica
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // Use a política de CORS configurada
    app.UseCors("AllowLocalhost");
}

void ConfigureEndpoints(WebApplication app)
{
    AppContext.SetSwitch("System.Globalization.Invariant", false);

    app.MapControllers();
    app.MapHub<PostHub>("/postHub"); // Para WebSocket
}
