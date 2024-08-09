using MaryAgent.Service;
using MaryAgent.Service.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MaryAgent.views;


public partial class AssesmentsPage : ContentPage
{
	public static ObservableCollection<Assesment> assesments { get; set; }

	public AssesmentsPage()
	{
		InitializeComponent();

	}

    override protected async void OnAppearing()
    {
        base.OnAppearing();

		if (assesments == null)
		{
			assesments = await AssesmentStorage.LoadAssesmentsAsync();//new ObservableCollection<Assesment>();

			this.AssessmentCollectionView.ItemsSource = assesments;
		}
    }


    private async void New_Assesment_Button_Clicked(object sender, EventArgs e)
	{
		// open an input dialog to get the name of the new assesment
		// TODO: add validation and dont move on on cancel
		string assesmentName = await DisplayPromptAsync("New Assesment", "Name:", placeholder: "Fancy Assesment");

		string threadID = await MaryService.CreateAssesment();

		Assesment assesment = new Assesment { Name = assesmentName, Date = DateTime.Now, ThreadID = threadID };
		assesments.Add(assesment);

		await AssesmentStorage.SaveAssesmentsAsync(assesments);

		await Shell.Current.GoToAsync($"/{nameof(MaryChat)}", true, new Dictionary<string, object>() { 
			{ "Assesment", assesment }
		});
	}

	private async void Delete_Assesment_Button_Clicked(object sender, EventArgs e)
	{
		var _sender = (Button)sender;
		Assesment s = _sender.Parent.BindingContext as Assesment;
		bool deleted = await MaryService.DeleteAssesment(s.ThreadID);

		if (deleted)
		{
			assesments.Remove(s);
			await AssesmentStorage.SaveAssesmentsAsync(assesments);
		}
		else
		{
			bool delete = await DisplayAlert("Error", "Assesment not found in server", "Delete", "Cancel");
			if (delete)
			{
				assesments.Remove(s);
				await AssesmentStorage.SaveAssesmentsAsync(assesments);
			}
		}

	}

	private async void Edit_Assesment_Button_Clicked(object sender, EventArgs e)
	{
		var _sender = (Button)sender;
		Assesment assesment = _sender.Parent.BindingContext as Assesment;

		if (assesment != null)
			await Shell.Current.GoToAsync($"/{nameof(MaryChat)}", true, new Dictionary<string, object>() {
				//initialize to the assesment object
				{ "Assesment", assesment }
			});
		else
		{
			await DisplayAlert("Error", "Assesment not found", "OK");
		}
	}
}