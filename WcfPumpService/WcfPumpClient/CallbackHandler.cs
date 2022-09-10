using System;
using WcfPumpClient.PumpServiceReference;

namespace WcfPumpClient
{
    internal class CallbackHandler : IPumpServiceCallback
    {
        public void UpdateStatistics(StatisticsService statistics)
        {
            Console.Clear();
            Console.WriteLine("Обновление по статистике выполнения скрипта");
            Console.WriteLine($"Всего     тактов: {statistics.AllTacts}");
            Console.WriteLine($"Успешных  тактов: {statistics.SuccessTacts}");
            Console.WriteLine($"Ошибочных тактов: {statistics.ErrorTacts}");
        }
    }
}
