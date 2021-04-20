
using RestWebApiServer.Data;

namespace RestWebApiServer
{

internal static class DbInitializer
{
    public static void Initialize(RestWebApiServerContext context)
    {
        context.Database.EnsureCreated();
    }
}

}