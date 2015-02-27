using System.Collections.Generic;
//using UCosmic.Exceptions;
using UCosmic.Interfaces;
using UCosmic.Factories;
using System.Web.Security;
using System;
using System.Data.Entity;
using UCosmic.Domain.Places;
using UCosmic.Web.Mvc.Models;
using System.Data.SqlClient;


namespace UCosmic.Repositories
{

    public class StudentActivityRepository
    {

       
        public IList<StudentImportApi> getStudentActivities()
        {
            SqlConnectionFactory connectionFactory = new SqlConnectionFactory();
            const string sql = "select *  FROM [studentActivity] ";
            
            IList<StudentImportApi> studentActivities = connectionFactory.SelectList<StudentImportApi>(DB.UCosmic, sql);

            return studentActivities;
        }



        public void uploadStudents(IList<StudentImportApi> students){
            
            SqlConnectionFactory connectionFactory = new SqlConnectionFactory();


            foreach(StudentImportApi student in students){

                string sql = @"insert into [studentActivity] (externalId, status, level,
                                    termDescription, termEnd, termStart, countryCode, ucosmicOfficialName,
                                    ucosmicCode, uCosmicForeignOfficialName, ucosmicForeignCode, progCode, progDescription) 
                                    VALUES( @externalId, @status, @level, @termDescription, @termEnd, @termStart, @countryCode, @ucosmicOfficialName,
                                    @ucosmicCode,@ucosmicForeignOfficialName, @ucosmicForeignCode,@progCode, @progDescription)";/*student.externalId.ToString() + "," + student.status + "," +
                                    student.level + "," + student.termDescription + "," + student.termEnd.ToString() +
                                    "," + student.termStart.ToString() + "," + student.countryCode + "," +
                                    student.ucosmicOfficialName + "," + student.ucosmicCode + "," + student.ucosmicForeignCode +
                                    "," + student.progCode + "," + student.progDescription + ");";*/
                connectionFactory.Execute(DB.UCosmic, sql, student, System.Data.CommandType.Text);

            }

        }
    }
}