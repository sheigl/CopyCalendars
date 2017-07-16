using System;
using System.Collections.Generic;
using CopyCalendars.ViewModels;
using Xamarin.Forms;

namespace CopyCalendars.Views
{
    public partial class MainPage : ContentPage
    {
		MainPageViewModel viewModel;

		public MainPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new MainPageViewModel();
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Items.Count == 0)
				viewModel.LoadItemsCommand.Execute(null);
		}
    }
}
