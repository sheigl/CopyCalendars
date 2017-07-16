using System;
using System.Collections.Generic;
using CopyCalendars.ViewModels;
using Xamarin.Forms;
using System.IO;
using System.Windows.Input;

namespace CopyCalendars.Views
{
    public partial class FileBrowser : ContentPage
    {
        private FileBrowserViewModel _viewModel;

        public ICommand LoadFiles;
        public ICommand LoadFolders;

		public DirectoryModel SelectedDirectory;
		public FileModel SelectedFile;

        public FileBrowser()
        {
            InitializeComponent();
            BindingContext = _viewModel = new FileBrowserViewModel();

            LoadFiles = new Command(async (path) => await _viewModel.GetFiles((string)path));
            LoadFolders = new Command(async (path) => await _viewModel.GetFolders((string)path));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadFolders.Execute(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            LoadFiles.Execute(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

        void Directory_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var d = e.SelectedItem as DirectoryModel;
            SelectedDirectory = d;

            LoadFolders.Execute(d.FullName);
            LoadFiles.Execute(d.FullName);
        }

		void File_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
            var f = e.SelectedItem as FileModel;
            SelectedFile = f;
		}
    }
}
