var builder = WebApplication.CreateBuilder(args);
    
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddOpenApi();

// Bind KafkaConfig from configuration and register as singleton
builder.Services.Configure<IngressApi.Models.KafkaConfig>(
    builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<IngressApi.Models.KafkaConfig>>().Value);

// Register Kafka consumer background service
builder.Services.AddHostedService<IngressApi.Services.KafkaConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
