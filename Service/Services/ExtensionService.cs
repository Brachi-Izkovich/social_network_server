using Common.Dto;
using Microsoft.Extensions.DependencyInjection;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
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
            services.AddScoped<IService<UserDto>, UserService>();
            services.AddScoped<IService<FeedbackDto>, FeedbackService>();
            services.AddScoped<IService<MessageDto>, MessageService>();
            services.AddScoped<IService<TopicDto>, TopicService>();
            services.AddScoped<IService<CategoryDto>, CategoryService>();

            services.AddAutoMapper(typeof(MyMapper));//הגדרת התלויות....
            return services;
        }
    }
}
