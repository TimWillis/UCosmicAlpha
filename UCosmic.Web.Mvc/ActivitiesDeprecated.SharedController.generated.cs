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
namespace T4MVC.ActivitiesDeprecated
{
    public class SharedController
    {

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
                public readonly string _activity_info = "_activity-info";
                public readonly string _short_list = "_short-list";
                public readonly string _tag_list = "_tag-list";
                public readonly string _tag_menu = "_tag-menu";
                public readonly string activities_page = "activities-page";
                public readonly string activity_delete = "activity-delete";
                public readonly string activity_form = "activity-form";
                public readonly string activity_info = "activity-info";
                public readonly string activity_results = "activity-results";
            }
            public readonly string _activity_info = "~/Areas/ActivitiesDeprecated/Views/Shared/_activity-info.cshtml";
            public readonly string _short_list = "~/Areas/ActivitiesDeprecated/Views/Shared/_short-list.cshtml";
            public readonly string _tag_list = "~/Areas/ActivitiesDeprecated/Views/Shared/_tag-list.cshtml";
            public readonly string _tag_menu = "~/Areas/ActivitiesDeprecated/Views/Shared/_tag-menu.cshtml";
            public readonly string activities_page = "~/Areas/ActivitiesDeprecated/Views/Shared/activities-page.cshtml";
            public readonly string activity_delete = "~/Areas/ActivitiesDeprecated/Views/Shared/activity-delete.cshtml";
            public readonly string activity_form = "~/Areas/ActivitiesDeprecated/Views/Shared/activity-form.cshtml";
            public readonly string activity_info = "~/Areas/ActivitiesDeprecated/Views/Shared/activity-info.cshtml";
            public readonly string activity_results = "~/Areas/ActivitiesDeprecated/Views/Shared/activity-results.cshtml";
            static readonly _EditorTemplatesClass s_EditorTemplates = new _EditorTemplatesClass();
            public _EditorTemplatesClass EditorTemplates { get { return s_EditorTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _EditorTemplatesClass
            {
                public readonly string Tag = "Tag";
                public readonly string TinyMceContent = "TinyMceContent";
            }
        }
    }

}

#endregion T4MVC
#pragma warning restore 1591