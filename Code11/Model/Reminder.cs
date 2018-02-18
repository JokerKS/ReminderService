using Code11.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Code11.Model
{
    [DataContract]
    public class Reminder : Entity, IValidate
    {
        [Required]
        [DataMember(IsRequired = true)]
        public string Name { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [DataMember]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [DataMember(IsRequired = true)]
        [NotMapped]
        public ReminderTypeEnum ReminderType { get; set; }

        public int TypeNumber { get; set; }

        [DataMember]
        public DateTime DateAndTime { get; set; } = new DateTime(2017, 12, 1);
        [DataMember]
        public string Days { get; set; }

        [DataMember]
        [StringLength(20)]
        public string Status { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) 
                || DateAndTime == default(DateTime))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}