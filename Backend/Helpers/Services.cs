using Backend.Services;

namespace Backend.Helpers;

public static class Services
{
    public static void Initialize(WebApplicationBuilder builder)
    {
        JWT.Init(builder.Configuration);
        Mail.Init(builder.Configuration);
        
        builder.Services.AddScoped<IAdminService, AdminService>();
    } 
}