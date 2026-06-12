using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShoexEcommerce.Application.Interfaces.Media
{
    public interface ICloudinaryService
    {
        Task<(string Url, string PublicId)> UploadAsync(
            Stream stream,
            string fileName,
            string folder,
            CancellationToken ct = default
        );

        Task<bool> DeleteAsync(
            string publicId,
            CancellationToken ct = default
        );
    }
}
