

namespace foodOrderingApp.services
{
    public class UploadFiles
    {
        public static string Photo(IFormFile file){
            string uploadsFolder = "uploads";
            string uniqueFileName = Guid.NewGuid()+"_"+file.FileName;
            string filePath = Path.Combine(uploadsFolder,uniqueFileName);
            file.CopyTo(new FileStream(filePath,FileMode.Create));
            return uniqueFileName;
        }
    }
}