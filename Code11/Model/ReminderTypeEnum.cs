using System.Runtime.Serialization;

namespace Code11.Model
{
    [DataContract(Name = "ReminderType")]
    public enum ReminderTypeEnum
    {
        [EnumMember]
        EveryDay = 1,
        [EnumMember]
        EveryWeek = 2,
        [EnumMember]
        EveryYear = 3,
        [EnumMember]
        Once = 4,

    }
}