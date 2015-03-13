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

       
        public IList<StudentActivity> getStudentActivities()
        {
            SqlConnectionFactory connectionFactory = new SqlConnectionFactory();
            const string sql = "select *  FROM [vw_MobilityDetail]";
            
            IList<StudentActivity> studentActivities = connectionFactory.SelectList<StudentActivity>(DB.UCosmic, sql);

            return studentActivities;
        }





        public int[] uploadStudents(IList<StudentImportApi> students){
            
            SqlConnectionFactory connectionFactory = new SqlConnectionFactory();


            int success = 0;
            int fail = 0;

            foreach(StudentImportApi student in students){

                StudentMobilityData record = new StudentMobilityData();

                /*
                record.status;
                */

                //Check if term data exists
                
                
                record.termId = getTermId(student, connectionFactory);                
                 
                 string sql = @"select RevisionId from [Establishments].[Establishment] where RevisionId=@UCosmicCode";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                     record.establishmentId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     //Return "establishment does not exist error"
                 }

                 sql = @"select RevisionId from [Establishments].[Establishment] where RevisionId=@ucosmicForeignCode";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                     record.foreignEstablishmentId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     //do nothing currently
                 }


                //Look for leveldata object
                StudentLevelData leveldata = new StudentLevelData();
                leveldata.establishmentId=(int)record.establishmentId;
                leveldata.name = student.level;
                leveldata.rank = student.rank;
                
                 sql = @"select id from [Students].[StudentLevel] where establishmentId=@establishmentId and name=@name";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, leveldata, System.Data.CommandType.Text);
                     record.levelId = id;
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

                 StudentProgramData progData = new StudentProgramData();
                 progData.establishmentId = (int)record.establishmentId;
                 progData.isStandard = false;
                 progData.name = student.progDescription;
                 progData.code = student.progCode;

                 sql = @"select id from [Students].[StudentProgramData] where code=@code and establishmentId=@establishmentId";
                 try
                 {
                     int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, progData, System.Data.CommandType.Text);
                     record.programId = id;
                 }
                 catch (System.InvalidOperationException e)
                 {
                     
                     sql = @"insert into [Students].[StudentProgramData] (code,name,isStandard,establishmentId) VALUES(@code, @name, @isStandard, @establishmentId)" +
                           @"SELECT CAST(SCOPE_IDENTITY() as int)";
                     record.programId = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, progData, System.Data.CommandType.Text);

                 }


                 record.studentId = student.externalId;
                 record.status = student.status;
                 

                 //Check if record exists
                 sql = @"select TOP 1 id from [Students].[StudentMobility] where studentId=@studentId and termId=@termId";

                 try
                 {
                     //Entry is a dupicate, do not add
                     connectionFactory.ExecuteWithId(DB.UCosmic, sql, record, System.Data.CommandType.Text);
                     fail += 1;

                 }
                 catch(System.InvalidOperationException e)
                 {
                     //If the entry does not exist, add it
                     sql = @"insert into [Students].[StudentMobility]" +
                     @"VALUES(@studentId, @status, @levelId, @termId, @placeId, @programId, @establishmentId, @foreignEstablishmentId)";
                     connectionFactory.Execute(DB.UCosmic, sql, record, System.Data.CommandType.Text);
                     success += 1;
                 }

                 

            }

            return new int[] { success, fail };
        }

        public int getTermId(StudentImportApi student, SqlConnectionFactory connectionFactory)
        {
            string sql = @"select id from [Students].[TermData] where name=@termDescription and startDate=@termStart and endDate=@termEnd";

            try
            {
                int id = (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
                return id;
            }
            catch (System.InvalidOperationException e)
            {
                //ID did not exist
                sql = @"insert into [Students].[TermData] VALUES(@termDescription, @termStart, @termEnd)" +
                      @"SELECT CAST(SCOPE_IDENTITY() as int)";
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
            }
        }

        
    }
}