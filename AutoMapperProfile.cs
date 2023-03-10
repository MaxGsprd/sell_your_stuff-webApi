using SellYourStuffWebApi.Models;
using AutoMapper;
using SellYourStuffWebApi.Models.Dtos.AdDtos;
using SellYourStuffWebApi.Models.Dtos.MessageDtos;
using SellYourStuffWebApi.Models.Dtos.UserDtos;
using SellYourStuffWebApi.Models.Dtos;

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
            CreateMap<Photo, PhotoDto>();
        }
    }
}
