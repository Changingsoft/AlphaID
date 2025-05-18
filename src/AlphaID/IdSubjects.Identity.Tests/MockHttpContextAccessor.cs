using Microsoft.AspNetCore.Http;

namespace IdSubjects.Identity.Tests;
internal class MockHttpContextAccessor(IServiceProvider provider) : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; } = new DefaultHttpContext()
    {
        RequestServices = provider,
    };
}
