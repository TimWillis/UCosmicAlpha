using System.Collections.Generic;
//using UCosmic.Exceptions;
using UCosmic.Interfaces;
using UCosmic.Factories;
using System.Web.Security;
using System;
using UCosmic.Domain.Places;
using UCosmic.Web.Mvc.Models;


namespace UCosmic.Repositories
{
    public class StudentActivityApiReturn
    {
        public int externalId;
        public string status;
        public string level;
        public string termDescription;
        public DateTime termEnd;
        public DateTime termStart;

    }
    public class StudentActivityRepository
    {

       
        public IList<StudentImportApi> StudentActivityTypes()
        {
            SqlConnectionFactory connectionFactory = new SqlConnectionFactory();
            const string sql = "select *  FROM [studentActivity] ";
            
            IList<StudentImportApi> studentActivities = connectionFactory.SelectList<StudentImportApi>(DB.UCosmic, sql);

            return studentActivities;
        }
    }
}