using Document_Manager.API.Middlewares;
using Document_Manager.Application.DependencyInjection;
using Document_Manager.Application.Validators;
using Document_Manager.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Service Configuration
// =======================

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//Services Identity 
builder.Services.AddIdentityServices(builder.Configuration);

//builder.Services.AddValidatorsFromAssembly(
//    typeof(CreateDocumentDtoValidator).Assembly
//);

var app = builder.Build();


// Middleware de manejo de excepciones (Siempre de los primeros)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// El middleware de CORS debe ir después de UseRouting (si se usa) 
// y siempre ANTES de UseAuthorization y MapControllers
app.UseCors("AllowBlazor");

app.UseAuthorization();

app.MapControllers();

app.Run();