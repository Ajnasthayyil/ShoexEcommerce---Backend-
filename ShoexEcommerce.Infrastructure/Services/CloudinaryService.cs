using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ShoexEcommerce.Application.Interfaces.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
            _cloudinary.Api.Secure = true;
        }

        public async Task<(string Url, string PublicId)> UploadAsync(
            Stream stream,
            string fileName,
            string folder,
            CancellationToken ct = default)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams, ct);
            if (result.Error != null) throw new System.Exception(result.Error.Message);

            return (result.SecureUrl.ToString(), result.PublicId);
        }

        public async Task<bool> DeleteAsync(string publicId, CancellationToken ct = default)
        {
            var deleteParams = new CloudinaryDotNet.Actions.DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok" || result.Result == "not found";
        }


        public async Task<(string Url, string PublicId)> AddImagesAsync(Stream stream, string fileName, string folder, CancellationToken ct = default)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return (result.SecureUrl.ToString(), result.PublicId);
        }

        public async Task<bool> DeleteImageAsync(string publicId, CancellationToken ct = default)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok" || result.Result == "not found";
        }
    }
}
