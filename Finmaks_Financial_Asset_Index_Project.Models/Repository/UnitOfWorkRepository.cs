﻿using Finmaks_Financial_Asset_Index_Project.DataAccess.Data;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Repository
{
    public class UnitOfWorkRepository : IUnitOfWorksRepository
    {
        private readonly ApplicationDbContext _db;
        public IExchangeRepository ExchangeRepository { get; private set; }

        public UnitOfWorkRepository(ApplicationDbContext db)
        {
            _db = db;
            ExchangeRepository = new ExchangeRepository(_db);
        }  

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
