using System.Collections.Generic;

namespace DuplicateFinder
{
    public interface IConfigService
    {
        public void SetFilterExtension(string[] extensionToFilter);
        public IEnumerable<string> GetFilterExtension();
    }

    public class ConfigService : IConfigService
    {
        public ConfigService()
        {
            SetDefaultConfig();
        }

        private void SetDefaultConfig()
        {
            SetFilterExtension(new[] {".mp4", ".jpg", ".mp3", ".doc", ".webm", ".odg"});
        }

        private string[] ExtensionToFilter { get; set; }

        public void SetFilterExtension(string[] extensionToFilter)
        {
            ExtensionToFilter = extensionToFilter;
        }

        public IEnumerable<string> GetFilterExtension()
        {
            return ExtensionToFilter;
        }
    }
}
