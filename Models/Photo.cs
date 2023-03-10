using MessagePack;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellYourStuffWebApi.Models
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        [Required]
        public string PublicId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int AdId { get; set; }
        public Ad? Ad { get; set; }

    }
}