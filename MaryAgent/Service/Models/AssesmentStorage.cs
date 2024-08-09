using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MaryAgent.Service.Models;

namespace MaryAgent.Service
{
    public static class AssesmentStorage
    {
        private static readonly string filePath = GetFilePath();

        private static string GetFilePath()
        {
#if ANDROID
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "assesments.json");
#elif IOS
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "assesments.json");
#elif WINDOWS
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "assesments.json");
#else
            throw new PlatformNotSupportedException("Platform not supported");
#endif
        }

        public static async Task SaveAssesmentsAsync(ObservableCollection<Assesment> assesments)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(assesments);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<ObservableCollection<Assesment>> LoadAssesmentsAsync()
        {
            if (!File.Exists(filePath))
                return new ObservableCollection<Assesment>();

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<ObservableCollection<Assesment>>(json);
        }
    }
}
