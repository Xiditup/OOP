namespace PhoneService.BLL.Services
{
    public class ImageService
    {
        private string _rootPath = "";
        public ImageService()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _rootPath = Path.Combine(documentsPath, "PhoneService");
            _rootPath = Path.Combine(_rootPath, "images");

        }
        public string Upload(FileInfo file)
        {
            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.Name;

            var filePath = Path.Combine(_rootPath, uniqueFileName);

            file.CopyTo(filePath);

            var relativeFilePath = Path.Combine("images", uniqueFileName);
            return relativeFilePath;

        }

        public IEnumerable<string> UploadMany(IEnumerable<FileInfo> files)
        {
            List<string> filePathes = [];
            foreach (var file in files)
            {
                filePathes.Add(Upload(file));
            }
            return filePathes;
        }
    }
}
