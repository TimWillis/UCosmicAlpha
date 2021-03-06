// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace UCosmic.Web.Mvc.Controllers
{
    public partial class FacultyStaffController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected FacultyStaffController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Institution()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Institution);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public FacultyStaffController Actions { get { return MVC.FacultyStaff; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "FacultyStaff";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "FacultyStaff";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string Institution = "Institution";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string Institution = "Institution";
        }


        static readonly ActionParamsClass_Institution s_params_Institution = new ActionParamsClass_Institution();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Institution InstitutionParams { get { return s_params_Institution; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Institution
        {
            public readonly string institutionId = "institutionId";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _Bib = "_Bib";
                public readonly string _SearchSideBar = "_SearchSideBar";
                public readonly string _StatsSideBar = "_StatsSideBar";
                public readonly string Index = "Index";
            }
            public readonly string _Bib = "~/Views/FacultyStaff/_Bib.cshtml";
            public readonly string _SearchSideBar = "~/Views/FacultyStaff/_SearchSideBar.cshtml";
            public readonly string _StatsSideBar = "~/Views/FacultyStaff/_StatsSideBar.cshtml";
            public readonly string Index = "~/Views/FacultyStaff/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_FacultyStaffController : UCosmic.Web.Mvc.Controllers.FacultyStaffController
    {
        public T4MVC_FacultyStaffController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Institution(int institutionId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Institution);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "institutionId", institutionId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
