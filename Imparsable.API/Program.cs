using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Toolchain.LSP;

var builder = WebApplication
    .CreateBuilder(args)
    .AddServiceDefaults(
        tracer: tracer => tracer.AddSource(typeof(LanguageServer).Assembly.GetName().Name!)
    );

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCalculatorLsp();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

// app.UseHttpsRedirection();
app.UseWebSockets(new() { KeepAliveInterval = TimeSpan.FromMinutes(2) });
app.MapControllers();
app.MapDefaultEndpoints();

await app.RunAsync();