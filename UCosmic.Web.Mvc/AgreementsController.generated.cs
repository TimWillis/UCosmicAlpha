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
    public partial class AgreementsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected AgreementsController(Dummy d) { }

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
        public System.Web.Mvc.ActionResult Created()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Created);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public AgreementsController Actions { get { return MVC.Agreements; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Agreements";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Agreements";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string New = "New";
            public readonly string Created = "Created";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string New = "New";
            public const string Created = "Created";
        }


        static readonly ActionParamsClass_Created s_params_Created = new ActionParamsClass_Created();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Created CreatedParams { get { return s_params_Created; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Created
        {
            public readonly string location = "location";
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
                public readonly string _FormAndCards = "_FormAndCards";
                public readonly string _FormCodesAndCategorySection = "_FormCodesAndCategorySection";
                public readonly string _FormLocationSection = "_FormLocationSection";
                public readonly string _FormNamesSection = "_FormNamesSection";
                public readonly string _FormParentSection = "_FormParentSection";
                public readonly string _FormSideBar = "_FormSideBar";
                public readonly string _FormUrlsSection = "_FormUrlsSection";
                public readonly string _SearchAndResults = "_SearchAndResults";
                public readonly string _SearchSideBar = "_SearchSideBar";
                public readonly string EstablishmentSearch = "EstablishmentSearch";
                public readonly string Form = "Form";
                public readonly string Index = "Index";
            }
            public readonly string _Bib = "~/Views/Agreements/_Bib.cshtml";
            public readonly string _FormAndCards = "~/Views/Agreements/_FormAndCards.cshtml";
            public readonly string _FormCodesAndCategorySection = "~/Views/Agreements/_FormCodesAndCategorySection.cshtml";
            public readonly string _FormLocationSection = "~/Views/Agreements/_FormLocationSection.cshtml";
            public readonly string _FormNamesSection = "~/Views/Agreements/_FormNamesSection.cshtml";
            public readonly string _FormParentSection = "~/Views/Agreements/_FormParentSection.cshtml";
            public readonly string _FormSideBar = "~/Views/Agreements/_FormSideBar.cshtml";
            public readonly string _FormUrlsSection = "~/Views/Agreements/_FormUrlsSection.cshtml";
            public readonly string _SearchAndResults = "~/Views/Agreements/_SearchAndResults.cshtml";
            public readonly string _SearchSideBar = "~/Views/Agreements/_SearchSideBar.cshtml";
            public readonly string EstablishmentSearch = "~/Views/Agreements/EstablishmentSearch.cshtml";
            public readonly string Form = "~/Views/Agreements/Form.cshtml";
            public readonly string Index = "~/Views/Agreements/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_AgreementsController : UCosmic.Web.Mvc.Controllers.AgreementsController
    {
        public T4MVC_AgreementsController() : base(Dummy.Instance) { }

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

        public override System.Web.Mvc.ActionResult Created(string location)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Created);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "location", location);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
