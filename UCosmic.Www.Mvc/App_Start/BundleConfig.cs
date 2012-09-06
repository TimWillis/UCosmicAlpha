﻿using System.Web.Optimization;

namespace UCosmic.Www.Mvc
{
    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/scripts/jquery-{version}.js"));

            // jQuery UI
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/scripts/jquery-ui-{version}.js"));

            // jquery validation, plus ms unobtrusive validation and ajax
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/scripts/jquery.unobtrusive*",
                        "~/scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/scripts/knockout-*"));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                        "~/scripts/jquery-{version}.js",
                        "~/scripts/knockout-*",
                        "~/scripts/sammy/sammy.js",
                        "~/scripts/oss/jquery.placeholder*",
                        "~/scripts/oss/jquery.animate-enhanced*",
                        "~/scripts/app/app.js",
                        "~/scripts/app/fixed-scroll.js",
                        "~/scripts/app/side-swiper.js",
                        "~/scripts/T4MvcJs/T4MvcJs.js",
                        "~/scripts/T4MvcJs/WebApiRoutes.js",
                        "~/models/BaseViewModel.js",
                        "~/models/FlasherViewModel.js",
                        "~/scripts/oss/jquery.palceholder*"));

            // bootstrap css bundles
            var tenants = new[]
            {
                "_default",
                "suny.edu",
                "uc.edu",
                "umn.edu",
                "lehigh.edu",
                "usf.edu",
            };
            foreach (var tenant in tenants)
            {
                bundles.Add(new StyleBundle(string.Format("~/styles/tenants/{0}/main", tenant)).Include(
                    "~/styles/reset.css",
                    string.Format("~/styles/tenants/{0}/layout.css", tenant),
                    string.Format("~/styles/tenants/{0}/forms.css", tenant)));

                bundles.Add(new StyleBundle(string.Format("~/styles/tenants/{0}/ie8", tenant)).Include(
                    string.Format("~/styles/tenants/{0}/ie8.css", tenant)));
            }

            // internet explorer 8 sucks
            bundles.Add(new StyleBundle("~/bundles/legacy/ie8").Include(
                "~/styles/ie8/layout.css"
            ));

            // jQuery UI theme
            bundles.Add(new StyleBundle("~/content/themes/base/css").Include(
                        "~/content/themes/base/jquery.ui.core.css",
                        "~/content/themes/base/jquery.ui.resizable.css",
                        "~/content/themes/base/jquery.ui.selectable.css",
                        "~/content/themes/base/jquery.ui.accordion.css",
                        "~/content/themes/base/jquery.ui.autocomplete.css",
                        "~/content/themes/base/jquery.ui.button.css",
                        "~/content/themes/base/jquery.ui.dialog.css",
                        "~/content/themes/base/jquery.ui.slider.css",
                        "~/content/themes/base/jquery.ui.tabs.css",
                        "~/content/themes/base/jquery.ui.datepicker.css",
                        "~/content/themes/base/jquery.ui.progressbar.css",
                        "~/content/themes/base/jquery.ui.theme.css"));
        }
    }
}