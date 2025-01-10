using Microsoft.AspNetCore.Http;
namespace Shared.Utils
{
    public static class FileUtils
    {
        public static IFormFile ConvertByteArrayToIFormFile(byte[] byteArray, string fileName, string contentType)
        {
            var stream = new MemoryStream(byteArray);
            var formFile = new FormFile(stream, 0, byteArray.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }
    }
}
