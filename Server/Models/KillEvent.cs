using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class KillEvent : Event
    {
        [MaxLength(16)]
        public string KillerTribesGuid { get; set; }

        [MaxLength(128)]
        public string KillerName { get; set;}

        [MaxLength(16)]
        public string VictimTribesGuid { get; set; }

        [MaxLength(128)]
        public string VictimName { get; set; }

        [MaxLength(128)]
        public string KillType { get; set; }

        [MaxLength(128)]
        public string Weapon { get; set; }
    }
}