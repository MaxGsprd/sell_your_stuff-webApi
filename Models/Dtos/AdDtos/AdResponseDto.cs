﻿using SellYourStuffWebApi.Models.Dtos.UserDtos;

namespace SellYourStuffWebApi.Models.Dtos.AdDtos
{
    public class AdResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Price { get; set; }
        public UserResponseDto User { get; set; }
        public Address? Address { get; set; }
        public Category Category { get; set; }
        public Condition? Condition { get; set; }
        public ICollection<PhotoDto> Photos { get; set; } = new List<PhotoDto>();
    }
}
