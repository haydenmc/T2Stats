using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public class GameType
    {
        [Key]
        public Guid GameTypeId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
    }
}