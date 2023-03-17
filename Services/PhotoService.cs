using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SellYourStuffWebApi.Interfaces;

namespace SellYourStuffWebApi.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary cloudinary;


        public PhotoService(IConfiguration configuration)
        {
            var cloudinarySettingsCloudName = Environment.GetEnvironmentVariable("CloudinarySettingsCloudName");
            var cloudinarySettingsApiKey = Environment.GetEnvironmentVariable("CloudinarySettingsApiKey");
            var cloudinarySettingsApiSecret = Environment.GetEnvironmentVariable("CloudinarySettingsApiSecret");

            Account account = new Account(
                        cloudinarySettingsCloudName,
                        cloudinarySettingsApiKey,
                        cloudinarySettingsApiSecret
                    );

            cloudinary = new Cloudinary( account );
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile photo)
        {
            var uploadResult = new ImageUploadResult();
            if ( photo.Length > 0)
            {
                using var stream = photo.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(photo.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500)
                };
                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await cloudinary.DestroyAsync(deleteParams);
            return result;
        }

    }
}
