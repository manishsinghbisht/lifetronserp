﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;

namespace Lifetrons.Erp.Web.Models.Repository
{
    public interface IAdministratorsRepository
    {
        IList<Administrator> All();
    }

    public class AdministratorsRepository : IAdministratorsRepository
    {
        public IList<Administrator> All()
        {
            using (var streamReader = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Data/SMSNumbers.csv")))
            {
                var reader = new CsvReader(streamReader);
                return reader.GetRecords<Administrator>().ToList();
            }
        }
    }
}