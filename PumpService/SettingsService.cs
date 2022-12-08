using PumpService.Interfaces;

namespace PumpService
{
    public class SettingsService : ISettingsService
    {
        public string FileName { get; set; }
    }
}