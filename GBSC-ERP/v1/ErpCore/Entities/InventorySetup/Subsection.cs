﻿using ErpCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ErpCore.Entities.InventorySetup
{
    public class Subsection : BaseClass
    {
        public Subsection()
        {
            Stores = new HashSet<Store>();
        }

        [Key]
        public long SubsectionId { get; set; }

        [Required]
        public string Name { get; set; }

        public long SectionId { get; set; }

        public Section Section { get; set; }

        public long? UserId { get; set; }

        public User User { get; set; }

        public IEnumerable<Store> Stores { get; set; }
    }
}
