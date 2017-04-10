using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class EventReporter
    {
        public Guid EventId { get; set; }

        [ForeignKey("EventId")]
        [InverseProperty("EventReports")]
        public Event Event { get; set; }

        public Guid PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        [InverseProperty("EventReports")]
        public Player Player { get; set; }
    }
}