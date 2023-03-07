using SellYourStuffWebApi.Models.Dtos;
using SellYourStuffWebApi.Models;
using AutoMapper;

namespace SellYourStuffWebApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponseDto>();
            CreateMap<UserRequestDto, User>();
            CreateMap<User, UserResponseForMessageDto>();
            CreateMap<Ad, AdResponseDto>();
            CreateMap<AdRequestDto, Ad>();
            CreateMap<MessageRequestDto, Message>();
            CreateMap<Message, MessageResponseDto>();
        }
    }
}
