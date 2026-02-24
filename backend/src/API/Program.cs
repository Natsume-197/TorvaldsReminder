using MongoDB.Driver;
using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Application.Services;
using TorvaldsReminder.Infrastructure.Repositories;
using TorvaldsReminder.Infrastructure.Seed;
using TorvaldsReminder.Infrastructure.Services;
using TorvaldsReminder.Infrastructure.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoSettings>()!;
var emailSettings = builder.Configuration.GetSection("Email").Get<EmailSettings>()!;

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton(emailSettings);

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IClientRepository,  ClientRepository>();
builder.Services.AddScoped<IEmailService,       EmailService>();
builder.Services.AddScoped<InvoiceProcessingService>();
builder.Services.AddSingleton<DatabaseSeeder>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

await app.Services.GetRequiredService<DatabaseSeeder>().SeedAsync();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
