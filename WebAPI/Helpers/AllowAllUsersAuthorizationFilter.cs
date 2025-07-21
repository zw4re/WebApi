using Hangfire.Dashboard;

namespace KapParser.API.Helpers
{ 
public class AllowAllUsersAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; // Geliştirme ortamı için uygundur
    }
}

}
