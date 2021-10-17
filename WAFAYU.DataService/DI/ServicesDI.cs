using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.Services;
using WAFAYU.DataService.UnitOfWorks;

namespace WAFAYU.DataService.DI
{
    public static class ServicesDI
    {
        public static void ConfigServicesDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DbContext, wafayuContext>();

            services.AddScoped<IBoxRepository, BoxRepository>();
            services.AddScoped<IBoxService, BoxService>();

            services.AddScoped<IShelfRepository, ShelfRepository>();
            services.AddScoped<IShelfService, ShelfService>();

            services.AddScoped<IStorageRepository, StorageRepository>();
            services.AddScoped<IStorageService, StorageService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ISubOrderSerivce, SubOrderService>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<ISpacePackageRepository, SpacePackageRepository>();
            services.AddScoped<ISpacePackageService, SpacePackageService>();

            services.AddScoped<IPendingOrderRepository, PendingOrderRepository>();
            services.AddScoped<IPendingOrderService, PendingOrderService>();

            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IImageService, ImageService>();

            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IFeedbackService, FeedbackService>();

            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<ISubOrderDetailService, SubOrderDetailService>();
        }
    }
}
