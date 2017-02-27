using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public abstract class Event
    {
        [Key]
        public Guid EventId { get; set; }

        [MaxLength(128)]
        public string ReporterName { get; set; }

        [MaxLength(16)]
        public string ReporterTribesGuid { get; set; }

        public TimeSpan MatchTime { get; set; }

        public DateTimeOffset MatchStartTime { get; set; }

        public TimeSpan MatchDuration { get; set; }

        [MaxLength(128)]
        public string MatchGameType { get; set; }

        [MaxLength(128)]
        public string MatchMapName { get; set; }

        [MaxLength(128)]
        public string ServerName { get; set; }
        
        [MaxLength(16)]
        public string ServerIpAddress { get; set; }

        [MaxLength(6)]
        public int ServerPort { get; set; }
    }
}