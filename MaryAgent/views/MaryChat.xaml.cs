using MaryAgent.extensions;
using MaryAgent.Service;
using MaryAgent.Service.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MaryAgent.views;

public partial class MaryChat : ContentPage, IQueryAttributable
{
    public static Assesment assesment { get; set; }

    private readonly MarkdownRenderer _markdownRenderer;

    public MaryChat()
	{
		InitializeComponent();
        BindingContext = this;

        _markdownRenderer = new MarkdownRenderer();
        

    }


    private void ScrollToBottom()
    {
        if (assesment.Messages.Count > 0)
        {
            Dispatcher.Dispatch(() =>
            {
                MessagesCollectionView.ScrollTo(assesment.Messages[assesment.Messages.Count - 1], position: ScrollToPosition.End, animate: true);
            });
        }
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(MessageEntry.Text))
        {
            Button button = (Button)sender;
            button.IsEnabled = false;

            assesment.Messages.Add(new Message { Text = MessageEntry.Text, IsUserMessage = true });
            MessageEntry.Text = string.Empty;

            try
            {
                Message message = new Message { Text = "Thinking...", IsUserMessage = false };

                assesment.Messages.Add(message);

                List<string> file_ids = new List<string>();
                
                foreach (var file in assesment.Files)
                {
                    if (file.FileID != null)
                        file_ids.Add(file.FileID);
                }

                string full_text = "";

                await foreach (var maryResponse in MaryService.Chat(assesment.ThreadID, assesment.Messages[assesment.Messages.Count - 2].Text, file_ids))
                {
                    full_text += maryResponse.response;
                    message.Text = full_text;
                    System.Diagnostics.Debug.WriteLine(message.Text);
                }

                //Messages.RemoveAt(Messages.Count - 1); // Remove the last thinking message

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                button.IsEnabled = true;
                await AssesmentStorage.SaveAssesmentsAsync(AssesmentsPage.assesments);
            }

            
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Assesment"))
        {
            assesment = query["Assesment"] as Assesment;

            this.Title = assesment.ThreadID ;

            MessagesCollectionView.ItemsSource = assesment.Messages;

            assesment.Messages.CollectionChanged += (sender, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    ScrollToBottom();
                }
            };
            // do something with the threadID
        }
    }

    private  async void OnFileClicked(object sender, EventArgs e)
    {
       await Shell.Current.GoToAsync($"/{nameof(AssesmentFilesPage)}", true, new Dictionary<string, object>() {
            //initialize to the assesment object
            { "Assesment", assesment }
        });

    }
}

