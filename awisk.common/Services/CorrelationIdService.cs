using awisk.common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace awisk.common.Services
{
    public class CorrelationIdService(IHttpContextAccessor httpContextAccessor) : ICorrelationIdService
    {
        public const string HeaderName = "X-Correlation-ID";

        public string CorrelationId =>
            httpContextAccessor.HttpContext?.Items[HeaderName]?.ToString()
            ?? Guid.NewGuid().ToString();
    }
}
