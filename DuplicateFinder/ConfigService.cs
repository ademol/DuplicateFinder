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
