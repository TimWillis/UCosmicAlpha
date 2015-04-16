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
    public class StudentQueryParameters
    {
        public string order { get;set; }
        public string orderDirection { get; set; }
        public int pageSize { get; set; }
        public int page{get;set;}
        public string institution { get; set; }
        public string campus { get; set; }
        public DateTime FStartDate { get; set; }
        public DateTime FEndDate { get; set; }
        public string FCountry { get; set; }
        public string FContinent { get; set; }
        public string FDegree { get; set; }
        public string FLevel { get; set; }
        public string FInstitution { get; set; }
        public string FStatus { get; set; }
    }

    public class StudentActivityRepository
    {
        public string StatusOUT = "OUT";
        private SqlConnectionFactory connectionFactory = new SqlConnectionFactory();
        private const string from_conditions = @" FROM [vw_MobilityDetail]";
        private const string paginated_from =
            @" FROM (SELECT ROW_NUMBER() OVER( " + order_conditions + @") AS
            rownum, * FROM vw_MobilityDetail WHERE " + filter_conditions +" )AS vw_MobilityDetail1 ";
        //

        private const string filter_conditions = @" status like @FStatus and termStart >= @FStartDate and termStart <= @FEndDate and campus like @campus and
  Country like @FCountry and continent like @FContinent and program like @FDegree and level like @FLevel and (  ((status = 'IN') and localEstablishmentName like @FInstitution) OR ((status = 'OUT') and foreignEstablishmentName like @FInstitution) ) ";
        private const string paginated_where =
            @" WHERE (rownum > ((@page-1)*@pageSize)) and (rownum <= (@page*@pageSize)) and termStart >= @FStartDate and " + filter_conditions;

        private const string order_conditions =
            @" order by 
                CASE WHEN @orderDirection='ASC' THEN
	                CASE @order  --string
	                WHEN 'Country' THEN Country 
                    WHEN 'status' THEN status
                    WHEN 'program' then program
	                END 
                END ASC,
                CASE WHEN @orderDirection='ASC' THEN
	                CASE @order --date
	                WHEN 'TermStart' THEN TermStart 
	                END
                END ASC,
                CASE WHEN @orderDirection='ASC' THEN
	                CASE @order --int
	                WHEN 'rank' THEN rank 
	                END
                END ASC,
                CASE WHEN @orderDirection='DESC' THEN
	                CASE @order --string
	                WHEN 'Country' THEN Country
                    WHEN 'status' THEN status
                    WHEN 'program' then program 
	                END 
                END DESC,
                CASE WHEN @orderDirection='DESC' THEN
	                CASE @order --date
	                WHEN 'TermStart' THEN TermStart 
	                END
                END DESC,
                CASE WHEN @orderDirection='DESC' THEN
	                CASE @order --int
	                WHEN 'rank' THEN rank 
	                END
                END DESC
                ";
        public IList<StudentActivity> getStudentActivities(StudentQueryParameters param)
        {

            const string sql = @"select *" + paginated_from + paginated_where + order_conditions; 
            IList<StudentActivity> studentActivities = connectionFactory.SelectList<StudentActivity>(DB.UCosmic, sql, param);
            return studentActivities;
        }

        public int getStudentActivityCount(StudentQueryParameters param)
        {
            const string sql = @"select COUNT(*)" + from_conditions + " WHERE " + filter_conditions;
            return connectionFactory.SelectList<int>(DB.UCosmic, sql, param)[0];
        }

        public int[] uploadStudents(IList<StudentImportApi> students)
        {
            //Initialize return values
            int success,fail,duplicate;
            success = fail = duplicate = 0;

            //Attempt to upload each student
            foreach (StudentImportApi student in students)
            {
                StudentMobilityData record = new StudentMobilityData();

                record.termId = getTermId(student);
                record.establishmentId = getEstablishmentId(student);
                record.foreignEstablishmentId = getForeignEstablishmentId(student);

                if (record.establishmentId == -1)
                {
                    fail += 1;
                    continue;
                }

                //TODO: foreignEstablishmentId is nullable but I'm not handling that properly
                if (record.foreignEstablishmentId == -1)
                {
                    fail += 1;
                    continue;
                }


                record.levelId = getLevelId(student, (int)record.establishmentId);
                record.placeId = getPlaceId(student);
                record.programId = getProgramId(student, (int)record.establishmentId);
                record.studentId = student.externalId;
                record.status = student.status;

                if (isDuplicate(record)) duplicate += 1;
                else
                {
                    commitRecord(record);
                    success += 1;
                }
            }
            return new int[] { success, fail, duplicate };
        }
        public int getTermId(StudentImportApi student)
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
        public int getEstablishmentId(StudentImportApi student)
        {
            string sql = @"select RevisionId from [Establishments].[Establishment] where RevisionId=@UCosmicCode";
            try
            {
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
            }
            catch (System.InvalidOperationException e)
            {
                //Return "establishment does not exist error"
                return -1;
            }
        }
        public int getForeignEstablishmentId(StudentImportApi student)
        {
            string sql = @"select RevisionId from [Establishments].[Establishment] where RevisionId=@ucosmicForeignCode";
            try
            {
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
            }
            catch (System.InvalidOperationException e)
            {
                return -1;
            }
        }
        public int getLevelId(StudentImportApi student, int establishmentId)
        {
            //Look for leveldata object
            StudentLevelData leveldata = new StudentLevelData();
            leveldata.establishmentId = establishmentId;
            leveldata.name = student.level;
            leveldata.rank = student.rank;

            string sql = @"select id from [Students].[StudentLevel] where establishmentId=@establishmentId and name=@name";
            try
            {
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, leveldata, System.Data.CommandType.Text);
            }
            catch (System.InvalidOperationException e)
            {
                sql = @"insert into [Students].[StudentLevel] VALUES(@establishmentId, @name, @rank)" +
                    @"SELECT CAST(SCOPE_IDENTITY() as int)";
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, leveldata, System.Data.CommandType.Text);
            }
        }
        public int getPlaceId(StudentImportApi student)
        {
            string sql = @"select GeoNameId from [Places].[GeoNamesCountry] where Code=@countryCode";
            try
            {
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, student, System.Data.CommandType.Text);
            }
            catch (System.InvalidOperationException e)
            {
                return -1;
            }
        }
        public int getProgramId(StudentImportApi student, int establishmentId)
        {
            StudentProgramData progData = new StudentProgramData();
            progData.establishmentId = establishmentId;
            progData.isStandard = false;
            progData.name = student.progDescription;
            progData.code = student.progCode;

            string sql = @"select id from [Students].[StudentProgramData] where code=@code and establishmentId=@establishmentId";
            try
            {
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, progData, System.Data.CommandType.Text);
            }
            catch (System.InvalidOperationException e)
            {

                sql = @"insert into [Students].[StudentProgramData] (code,name,isStandard,establishmentId) VALUES(@code, @name, @isStandard, @establishmentId)" +
                      @"SELECT CAST(SCOPE_IDENTITY() as int)";
                return (int)connectionFactory.ExecuteWithId(DB.UCosmic, sql, progData, System.Data.CommandType.Text);
            }
        }
        public bool isDuplicate(StudentMobilityData record)
        {
            //Check if record exists
            string sql = @"select TOP 1 id from [Students].[StudentMobility] where studentId=@studentId and termId=@termId";

            try
            {
                //Entry is a dupicate, do not add
                connectionFactory.ExecuteWithId(DB.UCosmic, sql, record, System.Data.CommandType.Text);
                return true;
            }
            catch (System.InvalidOperationException e)
            {
                //If the entry does not exist, add it   
                return false;
            }
        }
        public void commitRecord(StudentMobilityData record)
        {
            try
            {
                string sql = @"insert into [Students].[StudentMobility]" +
              @"VALUES(@studentId, @status, @levelId, @termId, @placeId, @programId, @establishmentId, @foreignEstablishmentId)";
                connectionFactory.Execute(DB.UCosmic, sql, record, System.Data.CommandType.Text);
            }
            catch (System.InvalidOperationException e)
            {
                //TODO: Don't fail silently.
            }
        }
       
        public IList<StudentProgramData> getPrograms(string institution)
        {
            const string sql = @" SELECT program as name, code
                                    FROM [UCosmicTest].[dbo].[vw_MobilityDetail]
                                    where institution=@institution
                                    group by program,code
                                ";
            StudentQueryParameters param = new StudentQueryParameters();
            param.institution = institution;
            return connectionFactory.SelectList<StudentProgramData>(DB.UCosmic, sql, param);

        }

        public IList<StudentTermData> getTerms(string institution){
            const string sql = @" SELECT [term] as name,[termStart] as startDate
                                FROM [UCosmicTest].[dbo].[vw_MobilityDetail]
                                where institution=@institution
                                group by term,termStart
                                order by termStart desc
                                ";
            StudentQueryParameters param = new StudentQueryParameters();
            param.institution = institution;
            return connectionFactory.SelectList<StudentTermData>(DB.UCosmic, sql, param);

        }
            
            
    }
}