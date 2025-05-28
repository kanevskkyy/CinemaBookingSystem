namespace CinemaBookingSystemAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next; 
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Unhandled exception caught.");
                context.Response.StatusCode = ex switch
                {
                    InvalidOperationException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.ContentType = "application/json";

                var response = new
                {
                    error = ex.Message,
                    statusCode = context.Response.StatusCode
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
