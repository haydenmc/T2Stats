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

        public Guid ServerId { get; set; }

        [ForeignKey("ServerId")]
        [InverseProperty("Matches")]
        public Server Server { get; set; }

        public string MapName { get; set; }

        public string GameType { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        [InverseProperty("Match")]
        public virtual ICollection<Event> Events { get; set; }
    }
}