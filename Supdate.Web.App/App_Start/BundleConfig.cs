using System.Web.Optimization;

namespace Supdate.Web.App
{
  public class BundleConfig
  {
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {
      // JavaScript
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.validate*"));

      bundles.Add(new ScriptBundle("~/bundles/media-queries").Include(
                  "~/Scripts/html5shiv.js",
                  "~/Scripts/respond.js"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap-plus").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.dcjqaccordion.2.7.js",
                "~/Scripts/jquery.scrollTo.min.js",
                "~/Scripts/jquery.nicescroll.js",
                "~/Scripts/jquery.glow.js",
                "~/Scripts/slidebars.min.js",
                "~/Scripts/slidebars.min.js",
                "~/Scripts/bootstrap-prettyfile.js",
                "~/Assets/bootstrap-datepicker/js/bootstrap-datepicker.js",
                "~/Assets/toastr-master/toastr.js",
                "~/Assets/Ladda/spin.min.js",
                "~/Assets/Ladda/ladda.min.js",
                "~/Assets/tinymce/tinymce.min.js",
                "~/Scripts/jquery-ui-1.10.4.custom.min.js",
                "~/Scripts/jQuery.microTemplate.js",
                "~/Scripts/jquery.signalR-2.2.0.min.js",
                "~/Scripts/autolinker.js",
                "~/Scripts/autosize.js",
                "~/Scripts/common-scripts.js",
                "~/Scripts/app.site.js",
                "~/Scripts/app.settings.js",
                "~/Scripts/app.reports.js",
                "~/Scripts/app.discussion.js",
                "~/Scripts/app.metricdataedit.js",
                "~/Scripts/app.reportView.js"));

      bundles.Add(new ScriptBundle("~/bundles/graphs").Include(
                  "~/Assets/raphael-2.2.7/raphael.min.js",
                  "~/Assets/morris.js-0.5.1/morris.min.js",
                  "~/Scripts/app.charts.js",
                  "~/Scripts/app.reportData.js"));

      // App styles
      bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-reset.css",
                "~/Content/slidebars.css",
                "~/Assets/bootstrap-datepicker/css/datepicker.css",
                "~/Assets/toastr-master/toastr.css",
                "~/Assets/Ladda/ladda-themeless.min.css",
                "~/Assets/morris.js-0.5.1/morris.css",
                "~/Content/style.css",
                "~/Content/style-responsive.css",
                "~/Content/site.css",
                "~/Content/report.css"));

      bundles.Add(new StyleBundle("~/Content/report-print-css").Include(
                "~/Content/report-print.css"));
    }
  }
}
