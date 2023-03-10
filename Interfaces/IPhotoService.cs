using CloudinaryDotNet.Actions;

namespace SellYourStuffWebApi.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile photo);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
