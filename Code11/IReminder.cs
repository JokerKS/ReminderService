using Code11.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Code11
{
    [ServiceContract(CallbackContract = typeof(IReminderCallback))]
    public interface IReminder
    {
        [OperationContract]
        [FaultContract(typeof(NotCorrectUsernameOrPassword))]
        User Login(string username, string password);
        [OperationContract]
        bool SignOut(int userId);
        [OperationContract]
        [FaultContract(typeof(NotCorrectUsernameOrPassword))]
        bool Register(string username, string password, string email);
        [OperationContract]
        [FaultContract(typeof(ArgumentNullException))]
        bool UpdateUser(User user);
        [OperationContract]
        [FaultContract(typeof(UserNotFound))]
        bool RemoveUser(int userId);


        [OperationContract]
        Reminder GetReminder(int userId, int reminderId);
        [OperationContract]
        List<Reminder> GetAllReminders(int userId);


        [OperationContract]
        [FaultContract(typeof(NotCorrectDataAndTime))]
        int AddReminder(int userId, Reminder reminder);
        [OperationContract]
        [FaultContract(typeof(ReminderNotFound))]
        bool UpdateReminder(int userId, int reminderId, Reminder reminder);
        [OperationContract]
        bool RemoveReminder(int userId, int reminderId);

        [OperationContract]
        [FaultContract(typeof(ReminderNotFound))]
        bool ActivateReminder(int userId, int reminderId);
        [OperationContract]
        [FaultContract(typeof(ReminderNotFound))]
        bool DeactivateReminder(int userId, int reminderId);

        [OperationContract(IsOneWay = true)]
        void ConfirmReceipt(int reminderId);
    }
}
