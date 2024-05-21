using Microsoft.EntityFrameworkCore;
using BitBeakAPI.Models;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configurações de logging
var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger<Program>();

// Verificar configurações de SMTP
var smtpServer = builder.Configuration["Smtp:Server"];
var smtpPort = builder.Configuration["Smtp:Port"];
var smtpUser = builder.Configuration["Smtp:User"];
var smtpPass = builder.Configuration["Smtp:Pass"];

logger.LogInformation($"SMTP Server: {smtpServer}");
logger.LogInformation($"SMTP Port: {smtpPort}");
logger.LogInformation($"SMTP User: {smtpUser}");
logger.LogInformation($"SMTP Pass: {smtpPass}");

// Adicionar serviços ao contêiner
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContext<BitBeakContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar o serviço de envio de email
builder.Services.AddTransient<EmailService>(provider => new EmailService(
    smtpServer!,
    int.Parse(smtpPort!),
    smtpUser!,
    smtpPass!
));

// Adicionar Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BitBeakAPI", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configurar o Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BitBeakAPI v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
