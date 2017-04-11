using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class KillEvent : Event
    {
        [MaxLength(16)]
        [NotMapped]
        public string KillerTribesGuid { get; set; }

        [MaxLength(128)]
        [NotMapped]
        public string KillerName { get; set;}

        [ForeignKey("KillerId")]
        public Player Killer { get; set; }

        public Guid? KillerId { get; set; }

        [MaxLength(16)]
        [NotMapped]
        public string VictimTribesGuid { get; set; }

        [MaxLength(128)]
        [NotMapped]
        public string VictimName { get; set; }

        [ForeignKey("VictimId")]
        public Player Victim { get; set; }
        
        public Guid? VictimId { get; set; }

        [MaxLength(128)]
        public string KillType { get; set; }

        [MaxLength(128)]
        public string Weapon { get; set; }

        public override bool EventCompareTo(Event other)
        {
            if (!(other is KillEvent))
            {
                return false;
            }
            var otherKillEvent = other as KillEvent;
            return (
                KillerTribesGuid == otherKillEvent.KillerTribesGuid &&
                VictimTribesGuid == otherKillEvent.VictimTribesGuid &&
                KillType == otherKillEvent.KillType &&
                Weapon == otherKillEvent.Weapon 
            );
        }
    }
}