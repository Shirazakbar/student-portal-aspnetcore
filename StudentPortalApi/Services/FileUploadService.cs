using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudentPortalApi.Services
{
    public static class FileUploadService
    {
        public static async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var fileName = System.Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filepath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filepath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }
    }
}
