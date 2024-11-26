using MyApp.Middleware;
using MyApp.MiddlewareInterface;
using MyApp.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Logging.AddSystemdConsole();
builder.Services.Configure<BasicAuthOptions>(builder.Configuration.GetSection("BasicAuth"));
var app = builder.Build();
var basicAuthOptions = app.Services.GetRequiredService<IConfiguration>().GetSection("BasicAuth").Get<BasicAuthOptions>();
app.MapWhen(context => context.Request.Path.StartsWithSegments("/Events", StringComparison.OrdinalIgnoreCase),
    AuthApp =>
    {
        AuthApp.UseMiddleware<BasicAuthenticationMiddleware>(basicAuthOptions.Username, basicAuthOptions.Password);

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
    );



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
