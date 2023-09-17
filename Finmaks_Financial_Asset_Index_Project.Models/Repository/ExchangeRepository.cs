using Bulky.DataAccess.Repository;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Repository
{
    public class ExchangeRepository : Repository<Exchange>, IExchangeRepository
    {
        private readonly ApplicationDbContext _db;
        public ExchangeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Exchange exchange)
        {
            _db.Update(exchange);
        }
    }
}
