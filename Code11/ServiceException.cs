using System.Runtime.Serialization;

namespace Code11
{
    [DataContract]
    public class ServiceException
    {
        [DataMember]
        public string Message { get; private set; }
        public ServiceException(string message)
        {
            Message = message;
        }
    }

    [DataContract]
    public class NotCorrectUsernameOrPassword : ServiceException
    {
        public NotCorrectUsernameOrPassword(string message) : base(message)
        {

        }
    }

    [DataContract]
    public class UserNotFound : ServiceException
    {
        public UserNotFound(string message) : base(message)
        {

        }
    }

    [DataContract]
    public class ReminderNotFound : ServiceException
    {
        public ReminderNotFound(string message) : base(message)
        {

        }
    }

    [DataContract]
    public class NotCorrectDataAndTime : ServiceException
    {
        public NotCorrectDataAndTime(string message) : base(message)
        {

        }
    }
}