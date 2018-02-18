using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Code11.Model
{
    [DataContract]
    public abstract class Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
    }
}