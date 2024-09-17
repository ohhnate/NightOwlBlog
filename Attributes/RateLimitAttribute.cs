using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleBlogMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private readonly int _seconds;
        private readonly int _maxRequests;

        public RateLimitAttribute(int seconds, int maxRequests)
        {
            _seconds = seconds;
            _maxRequests = maxRequests;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var memoryCache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
            var key = $"{context.ActionDescriptor.RouteValues["controller"]}_{context.ActionDescriptor.RouteValues["action"]}_{context.HttpContext.Connection.RemoteIpAddress}";

            if (!memoryCache.TryGetValue(key, out int requestCount))
            {
                requestCount = 1;
            }
            else
            {
                requestCount++;
            }

            if (requestCount > _maxRequests)
            {
                context.Result = new ContentResult
                {
                    Content = "Rate limit exceeded. Please try again later.",
                    StatusCode = 429
                };
                return;
            }

            memoryCache.Set(key, requestCount, TimeSpan.FromSeconds(_seconds));
        }
    }
}