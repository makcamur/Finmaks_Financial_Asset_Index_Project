using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Commons
{
    public abstract class Base
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
