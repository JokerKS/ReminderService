using Code11.Model;
using System.ServiceModel;

namespace Code11
{

    interface IReminderCallback
    {
        [OperationContract(IsOneWay = true)]
        void ExecuteReminder(Reminder reminder);
    }
}
