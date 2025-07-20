using DangPatterns;
using DangPatterns.DesignPatterns.RepositoyUoW.Implementations;
using DangPatterns.DesignPatterns.RepositoyUoW.Interfaces;
using DangPatterns.HelperModels.ExceptionHandling;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Configure Serilog

Log.Logger = new LoggerConfiguration()
    // MINIMUM LEVELS: Control what gets logged
    .MinimumLevel.Information() // Only log Information and above (Info, Warning, Error, Fatal)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Reduce Microsoft framework noise
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information) // Keep startup/shutdown info
    .MinimumLevel.Override("System", LogEventLevel.Warning) // Reduce system noise

    // ENRICHERS: Add context to every log entry
    .Enrich.FromLogContext() // Allows adding properties within using scope
    .Enrich.WithProperty("Application", Assembly.GetExecutingAssembly().GetName().Name) // App name
    .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version?.ToString()) // App version
    .Enrich.WithEnvironmentName() // Development/Staging/Production
    .Enrich.WithMachineName() // Server name
    .Enrich.WithProcessId() // Process ID
    .Enrich.WithThreadId() // Thread ID for debugging

    // CONSOLE SINK: For development debugging
    .WriteTo.Console(
        restrictedToMinimumLevel: LogEventLevel.Information,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj} {Properties}{NewLine}{Exception}")

    // MAIN LOG FILE: All logs in readable format
    .WriteTo.File(
        path: @"D:\logs\api-.txt",
        rollingInterval: RollingInterval.Day, // New file each day
        retainedFileCountLimit: 30, // Keep 30 days of logs
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB max per file
        rollOnFileSizeLimit: true, // Create new file when size limit reached
        restrictedToMinimumLevel: LogEventLevel.Information,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj} {Properties}{NewLine}{Exception}")

    // ERROR-ONLY LOG FILE: Critical issues in separate file
    .WriteTo.File(
        path: @"D:\logs\errors\api-errors-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error, // Only errors and fatal
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 60, // Keep error logs longer
        fileSizeLimitBytes: 50 * 1024 * 1024, // 50MB for errors
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}{NewLine}CorrelationId: {CorrelationId}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}---{NewLine}")

    // JSON LOG FILE: For log analysis tools (ELK, Splunk, etc.)
    .WriteTo.File(
        new JsonFormatter(),
        path: @"D:\logs\json\api-json-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 20 * 1024 * 1024, // 20MB for JSON logs
        rollOnFileSizeLimit: true)

    .CreateLogger();

// Use Serilog for all logging
builder.Host.UseSerilog();

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository Pattern services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

#region Pipeline for exception logs

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
    };
});
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

#endregion

app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("=== Application Starting Up ===");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
    Log.Information("Content Root: {ContentRoot}", app.Environment.ContentRootPath);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "=== Application terminated unexpectedly ===");
}
finally
{
    Log.Information("=== Application Shutting Down ===");
    Log.CloseAndFlush(); // Ensure all logs are written before exit
}
