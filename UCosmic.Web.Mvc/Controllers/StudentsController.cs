using System;
using System.Linq.Expressions;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using UCosmic.Domain.Home;
using UCosmic.Domain.People;
using UCosmic.Web.Mvc.Models;
using System.Collections.Generic;
using UCosmic.Domain.Establishments;
using UCosmic.Repositories;
using System.Web;
using LinqToExcel;

namespace UCosmic.Web.Mvc.Controllers
{
    public partial class StudentsController : Controller
    {

        [GET("/students/")]
        public virtual ActionResult Index()
        {
            StudentActivityRepository students = new StudentActivityRepository();
            IList<StudentImportApi> content = students.StudentActivityTypes();
            return View();
        }

        [GET("/students/new")]
        public virtual ActionResult New()
        {
            return View();
        }

        [POST("/students/new")]
        public ActionResult New(HttpPostedFileBase file)
        {
            //Check if the file is null - we'll probably want to return an error in this case
            if (file != null && file.ContentLength > 0 && file.ContentLength < 10000000)
           { 
                var excel = new ExcelQueryFactory("C:/Users/trex/Desktop/testdata.xlsx");
                var data = from c in excel.Worksheet("StudentActivity") select c;
                string test = "debug";
            }


            




          
            return View();
        }
        
    }
}

