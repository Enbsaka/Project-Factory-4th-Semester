using Dunder_Store.Interfaces.IServices;
using System.IO;

namespace Dunder_Store.Data.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        public string? SaveProductImage(Stream content, string fileName, string? oldImageUrl, string baseUrl)
        {
            string nomePasta = "produtos";
            string caminhoDaPasta = Path.Combine("wwwroot", nomePasta);
            Directory.CreateDirectory(caminhoDaPasta);

            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                try
                {
                    var nomeAntigo = Path.GetFileName(new Uri(oldImageUrl).AbsolutePath);
                    var caminhoAntigo = Path.Combine(caminhoDaPasta, nomeAntigo);
                    if (File.Exists(caminhoAntigo)) File.Delete(caminhoAntigo);
                }
                catch { }
            }

            string nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            string caminhoCompleto = Path.Combine(caminhoDaPasta, nomeArquivo);

            using var fs = new FileStream(caminhoCompleto, FileMode.Create);
            content.CopyTo(fs);

            var url = $"{baseUrl}/{nomePasta}/{nomeArquivo}";
            return url;
        }
    }
}