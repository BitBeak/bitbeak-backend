using Microsoft.EntityFrameworkCore;
using BitBeakAPI.Models;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using BitBeakAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger<Program>();

// Verificar configurações de SMTP
var smtpServer = builder.Configuration["Smtp:Server"];
var smtpPort = builder.Configuration["Smtp:Port"];
var smtpUser = builder.Configuration["Smtp:User"];
var smtpPass = builder.Configuration["Smtp:Pass"];
logger.LogInformation($"SMTP Server: {smtpServer}");
logger.LogInformation($"SMTP Port: {smtpPort}");
logger.LogInformation($"SMTP User: {smtpUser}");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContext<BitBeakContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<EmailService>(provider => new EmailService(
    smtpServer!,
    int.Parse(smtpPort!),
    smtpUser!,
    smtpPass!
));

builder.Services.AddScoped<QuestaoService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BitBeakAPI", Version = "v1" });
    
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Urls.Add("http://0.0.0.0:5159");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BitBeakAPI");
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
