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
namespace T4MVC
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
                public readonly string _GoogleMapsScript = "_GoogleMapsScript";
                public readonly string _GoogleMapsToolsOverlay = "_GoogleMapsToolsOverlay";
                public readonly string _Layout = "_Layout";
                public readonly string _LayoutCss = "_LayoutCss";
                public readonly string _SampleSidebarNav = "_SampleSidebarNav";
                public readonly string _SampleStylingBib = "_SampleStylingBib";
                public readonly string _SampleVerticalContent = "_SampleVerticalContent";
                public readonly string _UserVoiceLink = "_UserVoiceLink";
                public readonly string _UserVoiceScript = "_UserVoiceScript";
                public readonly string Error = "Error";
            }
            public readonly string _GoogleMapsScript = "~/Views/Shared/_GoogleMapsScript.cshtml";
            public readonly string _GoogleMapsToolsOverlay = "~/Views/Shared/_GoogleMapsToolsOverlay.cshtml";
            public readonly string _Layout = "~/Views/Shared/_Layout.cshtml";
            public readonly string _LayoutCss = "~/Views/Shared/_LayoutCss.cshtml";
            public readonly string _SampleSidebarNav = "~/Views/Shared/_SampleSidebarNav.cshtml";
            public readonly string _SampleStylingBib = "~/Views/Shared/_SampleStylingBib.cshtml";
            public readonly string _SampleVerticalContent = "~/Views/Shared/_SampleVerticalContent.cshtml";
            public readonly string _UserVoiceLink = "~/Views/Shared/_UserVoiceLink.cshtml";
            public readonly string _UserVoiceScript = "~/Views/Shared/_UserVoiceScript.cshtml";
            public readonly string Error = "~/Views/Shared/Error.cshtml";
            static readonly _EditorTemplatesClass s_EditorTemplates = new _EditorTemplatesClass();
            public _EditorTemplatesClass EditorTemplates { get { return s_EditorTemplates; } }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public partial class _EditorTemplatesClass
            {
                public readonly string Collection = "Collection";
                public readonly string tinymce_full = "tinymce_full";
                public readonly string tinymce_full_compressed = "tinymce_full_compressed";
            }
        }
    }

}

#endregion T4MVC
#pragma warning restore 1591
