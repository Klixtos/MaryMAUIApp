using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MaryAgent.extensions;

namespace MaryAgent.Service.Models
{
    public class Assesment : INotifyPropertyChanged
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        private string _threadID;

        [JsonPropertyName("threadID")]
        public string ThreadID
        {
            get => _threadID;
            set
            {
                _threadID = value;
                OnPropertyChanged(nameof(ThreadID));
            }
        }

        [JsonPropertyName("files")]
        public ObservableCollection<AssesmentFile> Files { get; set; } = new ObservableCollection<AssesmentFile>();

        [JsonPropertyName("messages")]
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AssesmentFile : INotifyPropertyChanged
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        private string _fileID;

        [JsonPropertyName("fileID")]
        public string FileID
        {
            get => _fileID;
            set
            {
                _fileID = value;
                OnPropertyChanged(nameof(FileID));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool NotUploaded => string.IsNullOrEmpty(FileID);
    }

    public class Message : INotifyPropertyChanged
    {
        private string _text;

        [JsonPropertyName("text")]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
                HtmlContent = new MarkdownRenderer().RenderMarkdownToHtml(_text);
            }
        }

        private string _htmlContent;
        public string HtmlContent
        {
            get => _htmlContent;
            private set
            {
                _htmlContent = value;
                OnPropertyChanged();
            }
        }



        [JsonPropertyName("isusermessage")]
        public bool IsUserMessage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
