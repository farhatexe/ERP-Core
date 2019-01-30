﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ErpCore.Entities.HR.Payroll.PayrollSetup
{
    public class Allowance : BaseClass
    {
        public Allowance()
        {
            AllowanceRates = new HashSet<AllowanceRate>();
        }

        [Key]
        public long AllowanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool? IsDefaultAllowance { get; set; } //From payroll admin
         
        public IEnumerable<AllowanceRate> AllowanceRates { get; set; }
    }
}
