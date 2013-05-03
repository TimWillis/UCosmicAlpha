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
    public partial class MyProfileController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected MyProfileController(Dummy d) { }

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
        public System.Web.Mvc.ActionResult ActivityEdit()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ActivityEdit);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public MyProfileController Actions { get { return MVC.MyProfile; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "MyProfile";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "MyProfile";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string ActivityEdit = "ActivityEdit";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string ActivityEdit = "ActivityEdit";
        }


        static readonly ActionParamsClass_ActivityEdit s_params_ActivityEdit = new ActionParamsClass_ActivityEdit();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ActivityEdit ActivityEditParams { get { return s_params_ActivityEdit; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ActivityEdit
        {
            public readonly string activityId = "activityId";
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
                public readonly string _Activities = "_Activities";
                public readonly string _Affiliations = "_Affiliations";
                public readonly string _EmailAddresses = "_EmailAddresses";
                public readonly string _FormalEducations = "_FormalEducations";
                public readonly string _GeographicExpertises = "_GeographicExpertises";
                public readonly string _LanguageExpertises = "_LanguageExpertises";
                public readonly string _PersonalInfo = "_PersonalInfo";
                public readonly string ActivityEdit = "ActivityEdit";
                public readonly string Index = "Index";
            }
            public readonly string _Activities = "~/Views/MyProfile/_Activities.cshtml";
            public readonly string _Affiliations = "~/Views/MyProfile/_Affiliations.cshtml";
            public readonly string _EmailAddresses = "~/Views/MyProfile/_EmailAddresses.cshtml";
            public readonly string _FormalEducations = "~/Views/MyProfile/_FormalEducations.cshtml";
            public readonly string _GeographicExpertises = "~/Views/MyProfile/_GeographicExpertises.cshtml";
            public readonly string _LanguageExpertises = "~/Views/MyProfile/_LanguageExpertises.cshtml";
            public readonly string _PersonalInfo = "~/Views/MyProfile/_PersonalInfo.cshtml";
            public readonly string ActivityEdit = "~/Views/MyProfile/ActivityEdit.cshtml";
            public readonly string Index = "~/Views/MyProfile/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_MyProfileController : UCosmic.Web.Mvc.Controllers.MyProfileController
    {
        public T4MVC_MyProfileController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ActivityEdit(int activityId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ActivityEdit);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "activityId", activityId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
