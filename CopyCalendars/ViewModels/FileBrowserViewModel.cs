using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CopyCalendars.ViewModels
{
    public class FileBrowserViewModel : ViewModelBase
    {
        private List<DirectoryModel> _directoryInfo;
        public List<DirectoryModel> DirectoryInfo
        {
            get { return _directoryInfo; }
            set { _directoryInfo = value; OnPropertyChanged(); }
        }

        private List<FileModel> _fileInfo;
        public List<FileModel> FileInfo
		{
			get { return _fileInfo; }
			set { _fileInfo = value; OnPropertyChanged(); }
		}



        public FileBrowserViewModel()
        {            
        }

        public async Task GetFiles(string path)
        {
            await Task.Run(() => {
                FileInfo = Directory.EnumerateFiles(path).Where(x => !new FileInfo(x).Name.StartsWith(".")).Select(x => new FileModel(x)).ToList();
            });
        }
        public async Task GetFolders(string path)
        {
            await Task.Run(() => {
                DirectoryInfo = Directory.EnumerateDirectories(path).Where(x => !new DirectoryInfo(x).Name.StartsWith(".")).Select(x => new DirectoryModel(x)).ToList();
                DirectoryInfo.Insert(0, new DirectoryModel(path).GetParent("../"));
            });
        }
    }

    public class DirectoryModel
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public DirectoryModel Parent { get; set; }
        public DirectoryModel(string Path, string CustomName = null)
        {
            var info = new DirectoryInfo(Path);

            this.FullName = info.FullName;
            this.Name = info.Name;

            if (CustomName != null)
            {
                this.Name = CustomName;
            }

            if (info.Parent != null)
            {
                Parent = new DirectoryModel(info.Parent.FullName);
            }
        }
        public DirectoryModel GetParent(string CustomName = null)
        {
			if (Parent == null)
			{
                Parent = new DirectoryModel("/");
			}

            if (CustomName != null)
            {
                Parent.Name = CustomName;
            }

            return Parent;
        }

}
    public class FileModel
    {
		public string FullName { get; set; }
		public string Name { get; set; }
        public FileModel(string Path)
        {
            var info = new FileInfo(Path);

			this.FullName = info.FullName;
			this.Name = info.Name;
        }
}
}
