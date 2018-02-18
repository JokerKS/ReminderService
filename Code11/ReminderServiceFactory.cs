using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;

namespace Code11
{
    public class ReminderServiceFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string service, Uri[] baseAddresses)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(ReminderService), baseAddresses);

            serviceHost.Opened += new EventHandler(ServiceHost_Opened);

            return serviceHost;
        }

        private void ServiceHost_Opened(object sender, EventArgs e)
        {
            Thread MainThread = new Thread(Model.ReminderManager.CheckUpdate);
            MainThread.IsBackground = true;
            MainThread.Start();

            Thread mainCheckThread = new Thread(Model.ReminderManager.CheckAddingToQueue);
            mainCheckThread.IsBackground = true;
            mainCheckThread.Start();
        }
    }
}