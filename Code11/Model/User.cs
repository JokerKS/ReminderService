using Code11.Model;
using Code11.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Code11
{
    [DataContract]
    [Table("Users")]
    public class User : Entity, IValidate
    {
        [Required]
        [DataMember(IsRequired = true)]
        [Index(IsUnique = true)]
        [StringLength(35)]
        public string UserName { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string Password { get; set; }
        [DataMember]
        [StringLength(254)]
        public string Email { get; set; }

        [DataMember]
        public List<Reminder> Reminders { get; set; }

        public DateTime LastLoginDate { get; set; }
        public DateTime RegisterDate { get; set; }

        public User(){ }
        public User(string username, string password, string email)
        {
            UserName = username;
            Password = password;
        }
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return true;
                }
                else
                {
                    try
                    {
                        var mail = new System.Net.Mail.MailAddress(Email);
                        return true;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
            }
        }
    }
}