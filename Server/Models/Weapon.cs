using System;
using System.ComponentModel.DataAnnotations;

namespace T2Stats.Models
{
    public class Weapon
    {
        [Key]
        public Guid WeaponId { get; set; }

        public string Name { get; set; }
    }
}