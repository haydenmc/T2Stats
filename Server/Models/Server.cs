using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Stats.Models
{
    public class Server
    {
        [Key]
        public Guid ServerId { get; set; }

        [MaxLength(128)]
        public string IpAddress { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(6)]
        public int Port { get; set; }

        [InverseProperty("Server")]
        public virtual ICollection<Match> Matches { get; set; }
    }
}