using Microsoft.Extensions.Caching.Distributed;
using RssServiceApi.Extensions;
using System.Net;

namespace RssServiceApi.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private IDistributedCache _cache;

        public TokenBlacklistMiddleware(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var currentToken = context.Request.Headers["Authorization"].ToString().Split(" ")[1];

            var isBlacklisted = await _cache.GetRecordAsync<string>($"bl_{currentToken}") != null;
            
            if (!isBlacklisted)
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
