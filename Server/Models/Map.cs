using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public class Map
    {
        [Key]
        public Guid MapId { get; set; }

        public string Name { get; set; }
    }
}