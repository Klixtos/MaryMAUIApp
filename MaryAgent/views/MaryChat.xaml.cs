using MaryAgent.Service;
using System.Collections.ObjectModel;

namespace MaryAgent.views;

public partial class MaryChat : ContentPage
{
    public ObservableCollection<Message> Messages { get; set; }
    MaryService maryService;
    public MaryChat()
	{
		InitializeComponent();
        Messages = new ObservableCollection<Message>();
        BindingContext = this;

        Messages.CollectionChanged += (sender, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ScrollToBottom();
            }
        };

        maryService = new MaryService();
    }

    private void ScrollToBottom()
    {
        if (Messages.Count > 0)
        {
            Dispatcher.Dispatch(() =>
            {
                MessagesCollectionView.ScrollTo(Messages[Messages.Count - 1], position: ScrollToPosition.End, animate: true);
            });
        }
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(MessageEntry.Text))
        {
            Button button = (Button)sender;
            button.IsEnabled = false;

            Messages.Add(new Message { Text = MessageEntry.Text, IsUserMessage = true });
            MessageEntry.Text = string.Empty;

            try
            {
                Messages.Add(new Message { Text = "Thinking...", IsUserMessage = false });

                var response = await maryService.Chat(Messages[Messages.Count - 2].Text);

                Messages.RemoveAt(Messages.Count - 1);
                Messages.Add(new Message { Text = response.response, IsUserMessage = false });

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                button.IsEnabled = true;
            }

            
        }
    }
}

public class Message
{
    public string Text { get; set; }
    public bool IsUserMessage { get; set; }
}