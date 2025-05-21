using Microsoft.Win32;
using System.IO;

namespace PhoneService.Models
{
    public class PhotoBrowser
    {
        public static FileInfo? GetPhoto()
        {
            OpenFileDialog opf = new()
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };
            if (opf.ShowDialog() == true)
            {
                if (!opf.FileName.EndsWith(".png") &&
                    !opf.FileName.EndsWith(".jpeg") &&
                    !opf.FileName.EndsWith(".jpg")) return null;
                return new(opf.FileName);
            }
            return null;
        }

        public static Uri GetFullPathUri(string relativePath)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "PhoneService");
            return new Uri(Path.Combine(path, relativePath));
        }
    }
}
