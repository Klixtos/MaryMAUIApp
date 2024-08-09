using MaryAgent.Service;
using MaryAgent.Service.Models;

namespace MaryAgent.views;

public partial class AssesmentFilesPage : ContentPage, IQueryAttributable
{
    public Assesment assesment { get; set; }

    public AssesmentFilesPage()
	{
        InitializeComponent();
    }

    private async void AddFileClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Please select a file"
            });

            if (result != null)
            {
                // Handle the selected file
                await DisplayAlert("File Selected", $"File name: {result.FileName}", "OK");

                assesment.Files.Add(new AssesmentFile { FileName = result.FileName, FilePath = result.FullPath });

                await AssesmentStorage.SaveAssesmentsAsync(AssesmentsPage.assesments);
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during file picking
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void DeleteFileClicked(object sender, EventArgs e)
    {
        var _sender = (Button)sender;
        AssesmentFile assesmentFile = _sender.Parent.BindingContext as AssesmentFile;

        if (assesmentFile != null)
            if (!string.IsNullOrEmpty(assesmentFile.FileID))
            {
                bool deleted = await MaryService.DeleteFile(assesmentFile.FileID);
                if (deleted)
                    assesment.Files.Remove(assesmentFile);
                else
                {
                    bool consent = await DisplayAlert("Error", "File not found in sever", "OK", cancel: "Cancel");
                    if (consent)
                        assesment.Files.Remove(assesmentFile);
                }
            }
            else
                assesment.Files.Remove(assesmentFile);
        else
            await DisplayAlert("Error", "Assesment not found", "OK");

        await AssesmentStorage.SaveAssesmentsAsync(AssesmentsPage.assesments);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Assesment"))
        {
            assesment = query["Assesment"] as Assesment;

            this.Title = $"Files {assesment.ThreadID}";
            this.FileCollectionView.ItemsSource = assesment.Files;
        }
    }

    private async void UploadFileClicked(object sender, EventArgs e)
    {
        var _sender = (Button)sender;
        AssesmentFile assesmentFile = _sender.Parent.BindingContext as AssesmentFile;
        assesmentFile.FileID = await MaryService.Uploadfile(assesmentFile.FilePath);
        _sender.IsVisible = assesmentFile.NotUploaded;

        await AssesmentStorage.SaveAssesmentsAsync(AssesmentsPage.assesments);
    }
}