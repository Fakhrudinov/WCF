using System.ServiceModel;
using WcfPumpService.StatisticService;

namespace WcfPumpService
{
    [ServiceContract]
    public interface IPumpServiceCallback
    {
        [OperationContract]
        void UpdateStatistics(StatisticsService statistics);
    }
}
