using System.ServiceProcess;

namespace OnOffPCService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new OnOffService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}