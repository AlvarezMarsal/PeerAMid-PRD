using System.Web.Optimization;

namespace YardStickPortal;

public class BundleConfig
{
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(
            new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

        bundles.Add(
            new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

        // Use the development version of Modernizr to develop with and learn from. Then, when you're
        // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(
            new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

        bundles.Add(
            new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

        bundles.Add(
            new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

        ////

        //bundles.Add(new ScriptBundle("~/Scripts/angular/modules/home-app").Include(
        //           "~/Scripts/angular/modules/home-app.js"));

        //bundles.Add(new ScriptBundle("~/Scripts/angular/homeService").Include(
        //           "~/Scripts/angular/services/homeService.js"));

        //bundles.Add(new ScriptBundle("~/Scripts/angular/controllers/home/homeController").Include(
        //           "~/Scripts/angular/controllers/home/homeController.js"));

        //bundles.Add(new ScriptBundle("~/Scripts/angular/filters/filters").Include(
        //           "~/Scripts/angular/filters/filters.js"));

        //bundles.Add(new StyleBundle("~/Content/home-custom-css").Include(
        //          "~/Content/animate.css",
        //          "~/Content/custom.css"));

        //BundleTable.EnableOptimizations = true;
    }
}