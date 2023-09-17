using Bulky.DataAccess.Repository.IRepository;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository
{
    public interface IExchangeRepository:IRepository<Exchange>
    {
        void Update(Exchange exchange);
    }
}
