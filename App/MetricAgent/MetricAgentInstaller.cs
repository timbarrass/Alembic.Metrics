using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace MetricAgent
{
    [RunInstaller(true)]
    public class MetricAgentInstaller : Installer
    {
        private ServiceInstaller serviceInstaller1;
        private ServiceProcessInstaller processInstaller;

        public MetricAgentInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller1 = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller1.StartType = ServiceStartMode.Automatic;

            serviceInstaller1.ServiceName = "Metric Agent";

            Installers.Add(serviceInstaller1);
            Installers.Add(processInstaller);
        }
    }
}
