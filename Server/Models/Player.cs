using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public class Player
    {
        [Key]
        public Guid PlayerId { get; set; }

        [MaxLength(32)]
        public string TribesGuid { get; set; }
    }
}