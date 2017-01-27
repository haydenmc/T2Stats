using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class Match
    {
        [Key]
        public Guid MatchId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        public Guid MapId { get; set; }

        [ForeignKey("MapId")]
        public Map Map { get; set; }

        public Guid ServerId { get; set; }

        [InverseProperty("Matches")]
        [ForeignKey("ServerId")]
        public Server Server { get; set; }

        [InverseProperty("Match")]
        public virtual ICollection<Kill> Kills { get; set; }
    }
}