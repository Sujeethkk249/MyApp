using MyApp.MiddlewareInterface;
using System.Net.Http.Headers;
using System.Text;

namespace MyApp.Middleware
{
    public class BasicAuthenticationMiddleware : IAuthenticationMiddeleware
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly RequestDelegate _next;
        private readonly ILogger<BasicAuthenticationMiddleware> _logger;
        private readonly string _username;
        private readonly string _password;


        public BasicAuthenticationMiddleware(RequestDelegate next, ILogger<BasicAuthenticationMiddleware> logger, string username, string password)
        {
            _next = next;
            _logger = logger;
            _username = username;
            _password = password;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                _logger.LogWarning("Missing Authorization header.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing Authorization header.");
                return;
            }

            if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var authHeader) || authHeader.Scheme != "Basic")
            {
                _logger.LogWarning("Invalid Authorization header format.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid Authorization header.");
                return;
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter ?? string.Empty)).Split(':', 2);

            if (credentials.Length != 2 || credentials[0] != _username || credentials[1] != _password)
            {
                _logger.LogWarning("Invalid credentials.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Secure Area\"";
                await context.Response.WriteAsync("Unauthorized: Invalid credentials.");
                return;
            }

            _logger.LogInformation("Authorization successful for username: {Username}", credentials[0]);

            await _next(context);
        }
    }
}

