using System;
using System.ServiceModel;
using WcfPumpClient.PumpServiceReference;

namespace WcfPumpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
            PumpServiceClient client = new PumpServiceClient(instanceContext);

            client.UpdateAndCompileScript(@"e:\Downloads\Обучение\C#\DZ_Repozitoriy\SOApRESTgRPC\GeekBrainsWCF\WCF\WcfPumpService\WcfPumpService\Scripts\Sample.script");
            client.RunScript();

            Console.WriteLine("Please, Enter to exit ...");
            Console.ReadKey(true);
            client.Close();
        }
    }
}
