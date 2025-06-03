using Microsoft.EntityFrameworkCore;
using ServiceFactory;
using WebApi.Filters;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<ModelStateValidationFilter>();
})
.ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddServices(connectionString);
builder.Services.AddSingleton<IBusinessLogic.ITransformerService, BusinessLogic.TransformerService>();

builder.Services.AddScoped<AuthorizationFilter>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddResponseCaching();

var app = builder.Build();

app.UseCors();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
