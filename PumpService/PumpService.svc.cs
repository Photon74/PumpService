using PumpService.Interfaces;
using System.ServiceModel;

namespace PumpService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "PumpService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы PumpService.svc или PumpService.svc.cs в обозревателе решений и начните отладку.
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class PumpService : IPumpService
    {
        private readonly IScriptService _scriptService;
        private readonly IStatisticsService _statisticsService;
        private readonly ISettingsService _serviceSettings;

        public PumpService()
        {
            _statisticsService = new StatisticsService();
            _serviceSettings = new SettingsService();
            _scriptService = new ScriptService(_statisticsService, _serviceSettings, Callback);
        }

        public void RunScript()
        {
            _scriptService.Run(10);
        }

        public void UpdateAndCompileScript(string fileName)
        {
            _serviceSettings.FileName = fileName;
            _scriptService.Compile();
        }

        IPumpServiceCallback Callback
        {
            get
            {
                return OperationContext.Current?.GetCallbackChannel<IPumpServiceCallback>();
            }
        }
    }
}
