using Common.Dto;
using Common.Dto.User;
using Microsoft.Extensions.DependencyInjection;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using Service.Services.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public static class ExtensionService
    {
        public static IServiceCollection AddServices(this IServiceCollection services) {
            services.AddRepository();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IService<FeedbackDto>, FeedbackService>();
            services.AddScoped<IService<MessageDto>, MessageService>();
            //services.AddScoped<IService<TopicDto>, TopicService>();
            services.AddScoped<IService<CategoryDto>, CategoryService>();

            services.AddScoped<IOwner, FeedbackService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILoginService, UserService>();

            services.AddAutoMapper(typeof(MyMapper));

            services.AddScoped<UserRepository>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddTransient<EmailService>();

            //services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ITopicService, TopicService>();

            services.AddScoped<ISearchService,SearchService>();

            return services;
        }
    }
}
