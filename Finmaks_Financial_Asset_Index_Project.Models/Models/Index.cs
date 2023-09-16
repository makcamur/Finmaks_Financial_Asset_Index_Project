﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.Models.Models
{
    public class Index
    {
        public int Id { get; set; }
        public decimal IndexValue { get; set; }
        public DateTime Date { get; set; }

        //navigation property
        public int AssetId { get; set; }
        public Asset? Asset { get; set; }
    }
}
