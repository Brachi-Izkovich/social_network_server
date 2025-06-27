using AutoMapper;
using Common.Dto;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MyMapper:Profile
    {
        string path = Path.Combine(Environment.CurrentDirectory, "Images/");
        //string path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Images");

        public MyMapper()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.ArrImageProfile, opt => opt.MapFrom(src => File.ReadAllBytes(path + src.ImageProfileUrl)))
                .ForMember(dest => dest.ImageProfileUrl, opt => opt.MapFrom(src => src.ImageProfileUrl));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.ImageProfileUrl, opt => opt.MapFrom(src => src.fileImageProfile.FileName));
       
            //CreateMap<User, UserDto>()
            //      .ForMember("ArrImageProfile", x => x.MapFrom(y => File.ReadAllBytes(path+ y.ImageProfileUrl)));
            //CreateMap<UserDto, User>()
            //      .ForMember("ImageProfileUrl", x => x.MapFrom(y => y.fileImageProfile.FileName));
            
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Feedback, FeedbackDto>().ReverseMap();
            CreateMap<Message, MessageDto>().ReverseMap();
            CreateMap<Topic, TopicDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}