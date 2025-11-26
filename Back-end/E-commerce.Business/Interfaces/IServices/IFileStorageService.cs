using System.IO;

namespace Dunder_Store.Interfaces.IServices
{
    public interface IFileStorageService
    {
        string? SaveProductImage(Stream content, string fileName, string? oldImageUrl, string baseUrl);
    }
}