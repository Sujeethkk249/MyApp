using MyApp.MiddlewareInterface;

namespace MyApp.Middleware
{
    public class BasicAuthenticationMiddleware : IAuthenticationMiddeleware
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly RequestDelegate _next;
        private readonly ILogger<BasicAuthenticationMiddleware> _logger;
        private readonly IConfiguration _configuration;


        public BasicAuthenticationMiddleware(RequestDelegate next, IHttpContextAccessor contextAccessor, ILogger<BasicAuthenticationMiddleware> logger, IConfiguration configuration )
        {
            _contextAccessor = contextAccessor;
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.
    }
}
