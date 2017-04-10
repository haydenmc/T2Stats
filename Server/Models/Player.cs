using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class Player
    {
        [Key]
        public Guid PlayerId { get; set; }

        public string TribesGuid { get; set; }

        public string Name { get; set; }

        [InverseProperty("Player")]
        public ICollection<EventReporter> EventReports { get; set; }
    }
}