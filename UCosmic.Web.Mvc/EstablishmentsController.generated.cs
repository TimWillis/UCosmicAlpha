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
    public partial class EstablishmentsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected EstablishmentsController(Dummy d) { }

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
        public System.Web.Mvc.ActionResult Show()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Show);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ViewResult Edit()
        {
            return new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Edit);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ViewResult Update()
        {
            return new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Update);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ViewResult Delete()
        {
            return new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Delete);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ViewResult Destroy()
        {
            return new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Destroy);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public EstablishmentsController Actions { get { return MVC.Establishments; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Establishments";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Establishments";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string New = "New";
            public readonly string Create = "Create";
            public readonly string Show = "Show";
            public readonly string Edit = "Edit";
            public readonly string Update = "Update";
            public readonly string Delete = "Delete";
            public readonly string Destroy = "Destroy";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string New = "New";
            public const string Create = "Create";
            public const string Show = "Show";
            public const string Edit = "Edit";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string Destroy = "Destroy";
        }


        static readonly ActionParamsClass_Show s_params_Show = new ActionParamsClass_Show();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Show ShowParams { get { return s_params_Show; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Show
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Edit s_params_Edit = new ActionParamsClass_Edit();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Edit EditParams { get { return s_params_Edit; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Edit
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Update s_params_Update = new ActionParamsClass_Update();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Update UpdateParams { get { return s_params_Update; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Update
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Delete s_params_Delete = new ActionParamsClass_Delete();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Delete DeleteParams { get { return s_params_Delete; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Delete
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Destroy s_params_Destroy = new ActionParamsClass_Destroy();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Destroy DestroyParams { get { return s_params_Destroy; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Destroy
        {
            public readonly string id = "id";
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
                public readonly string _FormAndCards = "_FormAndCards";
                public readonly string _FormLocationSection = "_FormLocationSection";
                public readonly string _FormNamesSection = "_FormNamesSection";
                public readonly string _FormSideBar = "_FormSideBar";
                public readonly string _FormUrlsSection = "_FormUrlsSection";
                public readonly string _SearchAndResults = "_SearchAndResults";
                public readonly string _SearchSideBar = "_SearchSideBar";
                public readonly string Form = "Form";
                public readonly string Index = "Index";
            }
            public readonly string _FormAndCards = "~/Views/Establishments/_FormAndCards.cshtml";
            public readonly string _FormLocationSection = "~/Views/Establishments/_FormLocationSection.cshtml";
            public readonly string _FormNamesSection = "~/Views/Establishments/_FormNamesSection.cshtml";
            public readonly string _FormSideBar = "~/Views/Establishments/_FormSideBar.cshtml";
            public readonly string _FormUrlsSection = "~/Views/Establishments/_FormUrlsSection.cshtml";
            public readonly string _SearchAndResults = "~/Views/Establishments/_SearchAndResults.cshtml";
            public readonly string _SearchSideBar = "~/Views/Establishments/_SearchSideBar.cshtml";
            public readonly string Form = "~/Views/Establishments/Form.cshtml";
            public readonly string Index = "~/Views/Establishments/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_EstablishmentsController : UCosmic.Web.Mvc.Controllers.EstablishmentsController
    {
        public T4MVC_EstablishmentsController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ViewResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult New()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.New);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult Create()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Create);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Show(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Show);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult Edit(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Edit);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult Update(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Update);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult Delete(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Delete);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ViewResult Destroy(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ViewResult(Area, Name, ActionNames.Destroy);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
