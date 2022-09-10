using System.ServiceModel;
using WcfPumpService.ScriptService;
using WcfPumpService.SettingsService;
using WcfPumpService.StatisticService;

namespace WcfPumpService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class PumpService : IPumpService
    {
        private readonly IScriptService _scriptService;
        private readonly IStatisticsService _statisticsService;
        private readonly ISettingsService _serviceSettings;

        public PumpService()
        {
            _statisticsService = new StatisticsService();
            _serviceSettings = new SettingsService.SettingsService();
            _scriptService = new ScriptService.ScriptService(Callback, _serviceSettings, _statisticsService);
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

        // возврат уведомлений клиенту.
        // для использования в web.config сделать дуал: <add binding="wsDualHttpBinding" scheme="http" />
        IPumpServiceCallback Callback
        {
            get
            {
                if (OperationContext.Current != null)
                    return OperationContext.Current.GetCallbackChannel<IPumpServiceCallback>();
                else
                    return null;
            }
        }
    }
}
