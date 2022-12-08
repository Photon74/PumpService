using PumpClient.PumpServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PumpClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var client = new PumpServiceClient(new InstanceContext(new CallbackHandler()));

            client.UpdateAndCompileScript(@"C:\Users\Photo\source\repos\PumpService\PumpService\Scripts\Sample.script");
            client.RunScript();

            Console.ReadLine();
            client.Close();
        }
    }
}
