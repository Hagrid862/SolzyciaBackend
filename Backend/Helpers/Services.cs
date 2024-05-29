using Backend.Data;
using Backend.Services;

namespace Backend.Helpers;

public static class Services
{
    public static void Initialize(WebApplicationBuilder builder)
    {
        //jwt is config and database context
        JWT.Init(builder.Configuration, builder.Services.BuildServiceProvider().GetService<MainDbContext>());
        Mail.Init(builder.Configuration);

        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IEventService, EventService>();
    }
}