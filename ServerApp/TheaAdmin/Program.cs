using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thea.Json;
using TheaAdmin;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDomainServices(builder.Configuration);

var frontendUrl = builder.Configuration["FrontendUrl"];
string[] urls = new[] { frontendUrl };
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(urls).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.IncludeErrorDetails = true;

        var rsaPublicKey = builder.Configuration["JwtAuth:RsaPublicKey"];
        var rsa = RSA.Create();
        rsa.ImportFromPem(rsaPublicKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.Name,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidIssuer = "thea",
            ValidateAudience = true,
            ValidAudiences = new string[] { "thea" },
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
        options.SaveToken = true;
    });

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonIntegerConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonLongConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonFloatConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonDoubleConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonDecimalConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonNullableIntegerConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonNullableLongConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonNullableFloatConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonNullableDoubleConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonNullableDecimalConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonNullableDateTimeConverter());
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization format : Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme}
                },
                new string[] { }
        }
    });
    options.OrderActionsBy(o => o.RelativePath);
});

var app = builder.Build();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
