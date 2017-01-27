using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class Kill
    {
        [Key]
        public Guid KillId { get; set; }
        
        public Guid KillerId { get; set; }

        [ForeignKey("KillerId")]
        public Player Killer { get; set; }

        public Guid VictimId { get; set; }

        [ForeignKey("VictimId")]
        public Player Victim { get; set; }

        public Guid ReporterId { get; set; }

        [ForeignKey("ReporterId")]
        public Player Reporter { get; set; }

        public Guid WeaponId { get; set; }

        [ForeignKey("WeaponId")]
        public Weapon Weapon { get; set; }

        public TimeSpan MatchTime { get; set; }

        public Guid MatchId { get; set; }

        [InverseProperty("Kills")]
        [ForeignKey("MatchId")]
        public Match Match { get; set; }
    }
}