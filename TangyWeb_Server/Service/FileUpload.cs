using Microsoft.AspNetCore.Components.Forms;

using TangyWeb_Server.Service.IService;

namespace TangyWeb_Server.Service;

public class FileUpload : IFileUpload
{
    private readonly IWebHostEnvironment _environment;
    public FileUpload(IWebHostEnvironment webHostEnvironment)
    {
        _environment = webHostEnvironment;
    }
    public bool DeleteFile(string filePath)
    {
        if(File.Exists(_environment.WebRootPath+filePath))
        {
            File.Delete(_environment.WebRootPath+filePath);
            return true;
        }
        return false;
    }

    async Task<string> IFileUpload.UploadFile(IBrowserFile file)
    {
        FileInfo fileInfo = new(file.Name);
        var fileName = Guid.NewGuid().ToString()+fileInfo.Extension;
        var folderDirectory = $"{_environment.WebRootPath}\\images\\product";

        if(!Directory.Exists(folderDirectory))
        {
            Directory.CreateDirectory(folderDirectory);
        }

        var filePath = Path.Combine(folderDirectory, fileName);

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.OpenReadStream().CopyToAsync(fs);

        var fullPath = $"/images/product/{fileName}";
        return fullPath;
    }
}
