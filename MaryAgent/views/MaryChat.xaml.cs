using MaryAgent.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                Message message = new Message { Text = "Thinking...", IsUserMessage = false };

                Messages.Add(message);


                await foreach ( var maryResponse in maryService.Chat(Messages[Messages.Count - 2].Text))
                {
                    message.Text += maryResponse.response;
                    await Task.Delay(100);

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
            }

            
        }
    }
}

public class Message : INotifyPropertyChanged
{
    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }
    public bool IsUserMessage { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}