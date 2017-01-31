using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public class Map
    {
        [Key]
        public Guid MapId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
    }
}