using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopyCalendars.ViewModels;
using Xamarin.Forms;

namespace CopyCalendars.Views
{
    public partial class SettingsPage : ContentPage
    {
        SettingsPageViewModel _viewModel;
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new SettingsPageViewModel();

            _viewModel.LoadSettingsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void MasterTemplate_Clicked(object sender, System.EventArgs e)
        {
            var browser = new FileBrowser();

            browser.Disappearing += (s, ea) => {
                _viewModel.MasterTemplateFile = ((FileBrowser)s).SelectedFile?.FullName;
            };

            await Navigation.PushModalAsync(browser);
        }
		async void Proof_Clicked(object sender, System.EventArgs e)
		{
			var browser = new FileBrowser();
			browser.Disappearing += (s, ea) =>
			{
                _viewModel.ProofingFolder = ((FileBrowser)s).SelectedDirectory?.FullName;
			};

			await Navigation.PushModalAsync(browser);
		}
		async void Working_Clicked(object sender, System.EventArgs e)
		{
			var browser = new FileBrowser();
			browser.Disappearing += (s, ea) =>
			{
                _viewModel.WorkingCalendarFolder = ((FileBrowser)s).SelectedDirectory?.FullName;
			};

			await Navigation.PushModalAsync(browser);
		}

        void Save_Clicked(object sender, System.EventArgs e)
        {
            _viewModel.SaveSettingsCommand.Execute(null);
        }
    }

}
