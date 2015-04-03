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

        private StudentQueryParameters param = new StudentQueryParameters();

        [GET("/students/")]
        public virtual ActionResult Index()
        {
            StudentActivityRepository students = new StudentActivityRepository();
            param.order = "TermStart";
            param.orderDirection = "ASC";
            IList<StudentActivity> content = students.getStudentActivities(param);

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
        [GET("{domain}/students/table/")]
        public virtual ActionResult Table(string domain, ActivitySearchInputModel input, int? page, int?pageSize, string orderby, string orderDirection)
        {
            StudentActivityRepository students = new StudentActivityRepository();
            param.order = (orderby!=null) ? orderby:"TermStart";
            param.orderDirection = (orderDirection!=null) ? orderDirection:"ASC";
            param.page = (page != null) ? (int)page : 1;
            param.pageSize = (pageSize != null) ? (int)pageSize : 10;

            IList<StudentActivity> content = students.getStudentActivities(param);
            ViewBag.Count = students.getStudentActivityCount(param);

            ViewBag.Page = param.page;
            ViewBag.PageSize = param.pageSize;
            ViewBag.PageStart = ((param.page - 1) * param.pageSize) + 1;
            ViewBag.PageEnd = param.page * param.pageSize;



            int r = (ViewBag.Count % param.pageSize) > 0 ? 1 : 0;
            ViewBag.LastPage = (ViewBag.Count / param.pageSize) + r;
             
            return View("Table", "_Layout2", content);
        }

        [GET("/api/students/")]
        public JsonResult getTableJson(string domain, ActivitySearchInputModel input, int? page, int? pageSize, string orderby, string orderDirection)
        {
            StudentActivityRepository students = new StudentActivityRepository();
            param.order = (orderby != null) ? orderby : "TermStart";
            param.orderDirection = (orderDirection != null) ? orderDirection : "ASC";
            param.page = (page != null) ? (int)page : 1;
            param.pageSize = (pageSize != null) ? (int)pageSize : 10;
            IList<StudentActivity> content = students.getStudentActivities(param);
            StudentPager s = new StudentPager(content,param.page,param.pageSize,students.getStudentActivityCount(param), param.order,param.orderDirection);
            return Json(s, JsonRequestBehavior.AllowGet);
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

