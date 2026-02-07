using Application.Abstractions;
using Infrastructure.AI;
using Infrastructure.Persistence;
using Application.Abstractions;
using Infrastructure.AI;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SqlitePragmaInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, opt) =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=budget.db");
    opt.AddInterceptors(sp.GetRequiredService<SqlitePragmaInterceptor>());
});
// App abstractions
builder.Services.AddScoped<IAppDbContext, EfAppDbContext>();
builder.Services.AddScoped<AiCommandExecutor>();

// Ollama
var ollamaOpt = builder.Configuration.GetSection("Ollama").Get<OllamaOptions>() ?? new OllamaOptions();
builder.Services.AddSingleton(ollamaOpt);

builder.Services.AddHttpClient<OllamaClient>(http =>
{
    http.BaseAddress = new Uri(ollamaOpt.BaseUrl);
});

builder.Services.AddScoped<IAiCommandParser, OllamaAiCommandParser>();

// CORS for Blazor WASM
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("ui", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("ui");
app.MapControllers();

// Ensure DB created/migrated (simple approach)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
