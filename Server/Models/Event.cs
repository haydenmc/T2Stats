using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public abstract class Event
    {
        [Key]
        public Guid EventId { get; set; }

        [InverseProperty("Event")]
        public virtual ICollection<EventReporter> EventReports { get; set; }

        public Guid MatchId { get; set; }

        [ForeignKey("MatchId")]
        [InverseProperty("Events")]
        public Match Match { get; set; }

        public TimeSpan MatchTime { get; set; }

        public abstract bool EventCompareTo(Event other);
    }
}