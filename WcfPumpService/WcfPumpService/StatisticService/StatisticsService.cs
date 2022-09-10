namespace WcfPumpService.StatisticService
{
    public class StatisticsService : IStatisticsService
    {
        public int SuccessTacts { get; set; }
        public int ErrorTacts { get; set; }
        public int AllTacts { get; set; }
    }
}