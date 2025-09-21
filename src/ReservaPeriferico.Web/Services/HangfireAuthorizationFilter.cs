using Hangfire.Dashboard;

namespace ReservaPeriferico.Web.Services;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Em desenvolvimento, permitir acesso a todos
        var httpContext = context.GetHttpContext();
        return httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
    }
}




