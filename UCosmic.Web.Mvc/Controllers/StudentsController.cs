using System;
using System.IO;
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
    public partial class StudentsController : Controller{

        [GET("/students/")]
        public virtual ActionResult Index()
        {
            StudentActivityRepository students = new StudentActivityRepository();
            IList<StudentActivity> content = students.getStudentActivities();

            return View(content);
        }

        [GET("/students/new")]
        public virtual ActionResult New()
        {
            return View();
        }

        [POST("/students/new")]
        public virtual ActionResult New(HttpPostedFileBase file)
        {
            //Check if the file is null - we'll probably want to return an error in this case
            if (file != null && file.ContentLength > 0 && file.ContentLength < 10000000)
           {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);

                var excel = new ExcelQueryFactory(path);
                IList<StudentImportApi> data = (from c in excel.Worksheet<StudentImportApi>("StudentActivity") select c).ToList<StudentImportApi>();

                StudentActivityRepository repository = new StudentActivityRepository();
                int[] rows_changed = repository.uploadStudents(data);
                ViewBag.Success = rows_changed[0];
                ViewBag.Failure = rows_changed[1];
            }
            return View();
        }

        [CurrentModuleTab(ModuleTab.Students)]
        [GET("{domain}/students/table")]
        public virtual ActionResult Table(string domain, ActivitySearchInputModel input)
        {
            StudentActivityRepository students = new StudentActivityRepository();
            IList<StudentActivity> content = students.getStudentActivities();
            return View(content);
        }

        [CurrentModuleTab(ModuleTab.Students)]
        [GET("{domain}/students/map")]
        public virtual ActionResult Map(string domain, ActivitySearchInputModel input)
        {
           
            ViewBag.EmployeesDomain = domain;
            return View();
        }

        [CurrentModuleTab(ModuleTab.Students)]
        [GET("{domain}/students")]
        public virtual ActionResult TenantIndex(string domain)
        {
            return View();
        }
        
    }
}

