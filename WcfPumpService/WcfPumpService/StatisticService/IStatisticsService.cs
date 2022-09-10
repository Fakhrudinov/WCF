namespace WcfPumpService.StatisticService
{
    public interface IStatisticsService
    {
        int SuccessTacts { get; set; }
        int ErrorTacts { get; set; }
        int AllTacts { get; set; }
    }
}
