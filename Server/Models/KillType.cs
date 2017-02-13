using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public class KillType
    {
        [Key]
        public Guid KillTypeId { get; set; }

        [MaxLength(128)]
        public string Type { get; set; }

        [MaxLength(128)]
        public string FriendlyName { get; set; }
    }
}