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
using Dapper;


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

                StudentMobilityData record = new StudentMobilityData();

                /*
                record.status;
                */

                //Check if term data exists
                string sql = @"select id from [Students].[TermData] where name=@termDescription and startDate=@termStart and endDate=@termEnd";

                try
                {
                    int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                }
                catch (System.InvalidOperationException e)
                {
                    //ID did not exist
                    sql = @"insert into [Students].[TermData] VALUES(@termDescription, @termStart, @termEnd)" +
                          @"SELECT CAST(SCOPE_IDENTITY() as int)";
                    record.termId = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                }

                 //TODO: Find out what makes a program code unique
                 sql = @"select id from [Students].[StudentProgramData] where code=@progCode";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                     record.programId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                    //I don't have anything to insert here currently
                 }

                 
                 sql = @"select RevisionId from [Establishments].[Establishment] where UCosmicCode=@UCosmicCode";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                     record.establishmentId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     //Return "establishment does not exist error"
                 }

                 sql = @"select RevisionId from [Establishments].[Establishment] where UCosmicCode=@ucosmicForeignCode";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                     record.foreignEstablishmentId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     //do nothing currently
                 }


                //Level is a bit tricky due to dependence on establishmentId and code... Should I create a new object to pack these together?

                //Create StudentLevelData object
                StudentLevelData leveldata = new StudentLevelData();
                leveldata.establishmentId=(int)record.establishmentId;
                leveldata.name = student.level;
                leveldata.rank = student.rank;
                
                 sql = @"select id from [Students].[StudentLevel] where establishmentId=@establishmentId and name=@name";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, leveldata, System.Data.CommandType.Text);
                     record.foreignEstablishmentId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     sql = @"insert into [Students].[StudentLevel] VALUES(@establishmentId, @name, @rank)" +
                         @"SELECT CAST(SCOPE_IDENTITY() as int)";
                     record.levelId = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, leveldata, System.Data.CommandType.Text);
                 }

                 sql = @"select GeoNameId from [Places].[GeoNamesCountry] where Code=@countryCode";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                     record.placeId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     //do nothing currently
                 }


                 record.studentId = student.externalId;
                 record.status = student.status;

                 sql = @"insert into [Students].[StudentMobility]"+
                     @"VALUES(@studentId, @status, @levelId, @termId, @placeId, @programId, @establishmentId, @foreignEstablishmentId)";
                 connectionFactory.Execute(DB.UCosmic, sql, record, System.Data.CommandType.Text);

                



                

                
                /*
                //TODO: Do I need to check if affiliation exists?
                string sql = @"insert into Students.AffiliationsData (@studentId, @localEstablishmentId)";
                connectionFactory.ExecuteWithId(DB.UCosmic, sql, s)


                string sql = @"insert into [studentActivity] (externalId, status, level,
                                    termDescription, termEnd, termStart, countryCode, ucosmicOfficialName,
                                    ucosmicCode, uCosmicForeignOfficialName, ucosmicForeignCode, progCode, progDescription) 
                                    VALUES( @externalId, @status, @level, @termDescription, @termEnd, @termStart, @countryCode, @ucosmicOfficialName,
                                    @ucosmicCode,@ucosmicForeignOfficialName, @ucosmicForeignCode,@progCode, @progDescription)";/*student.externalId.ToString() + "," + student.status + "," +
                                    student.level + "," + student.termDescription + "," + student.termEnd.ToString() +
                                    "," + student.termStart.ToString() + "," + student.countryCode + "," +
                                    student.ucosmicOfficialName + "," + student.ucosmicCode + "," + student.ucosmicForeignCode +
                                    "," + student.progCode + "," + student.progDescription + ");";*/
               // connectionFactory.Execute(DB.UCosmic, sql, student, System.Data.CommandType.Text);

            }

        }
    }
}