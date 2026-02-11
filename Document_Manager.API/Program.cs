using Document_Manager.API.Middlewares;
using Document_Manager.Application.DependencyInjection;
using Document_Manager.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Controllers
builder.Services.AddControllers();

// Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS Configuration for Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
    );
});


builder.Services.AddAuthorization();

//Services Identity 
builder.Services.AddIdentityServices(builder.Configuration);

//Registro JWT 
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),

            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmins", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("AdminOrManager", policy =>
        policy.RequireRole("Admin", "Manager"));
});


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ClockSkew = TimeSpan.Zero,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

//            // Esto le dice a .NET dónde buscar el ID del usuario
//            NameClaimType = ClaimTypes.NameIdentifier,
//            RoleClaimType = ClaimTypes.Role
//        };
//        options.AddPolicy("OnlyAdmins", policy =>
//           policy.RequireRole("Admin"));
//    });

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtSettings["Issuer"],
//            ValidAudience = jwtSettings["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(key),

//            NameClaimType = ClaimTypes.NameIdentifier,
//            RoleClaimType = ClaimTypes.Role
//        };

//        options.MapInboundClaims = false;
//    });


var app = builder.Build();

// Middleware de manejo de excepciones (Siempre de los primeros)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Blazor 
app.UseCors("AllowBlazor");

//Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

