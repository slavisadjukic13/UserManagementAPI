using Microsoft.Extensions.Options;
using System.Net;
using UserManagementAPI.Config;

namespace UserManagementAPI.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;
        private readonly TokenSettings _tokenSettings;

        // For demo purposes: replace with your real validation logic
        private const string ValidToken = "my-secret-token";

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger, IOptions<TokenSettings> tokenSettings)
        {
            _next = next;
            _logger = logger;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check for Authorization header
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                await RejectRequest(context, "Authorization header missing.");
                return;
            }

            // Example: Expecting "Bearer my-secret-token"
            var token = authHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

            if (string.IsNullOrEmpty(token) || token != _tokenSettings.ValidToken)
            {
                _logger.LogWarning("Invalid or missing token.");
                await RejectRequest(context, "Invalid or missing token.");
                return;
            }

            // Token is valid, continue pipeline
            await _next(context);
        }

        private static async Task RejectRequest(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new { error = message };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
