using System.Web;
using System.Web.Optimization;

namespace WebLayer
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.signalR-2.2.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/switAlert").Include(
                      "~/Scripts/switAlert.js"));

            bundles.Add(new ScriptBundle("~/bundles/ColorAdmin2").Include(
                      "~/Scripts/ColorAdmin/jquery.slimscroll.js",
                      //"~/Scripts/ColorAdmin/jquery.cookie.js",
                      "~/Scripts/ColorAdmin/jquery.gritter.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/ColorAdmin").Include(
                      "~/Scripts/ColorAdmin/dashboard.js",                     
                      "~/Scripts/ColorAdmin/apps.js"));

            bundles.Add(new ScriptBundle("~/bundles/Maps").Include(
                      "~/Scripts/Map/map.js"));
            
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/switAlert.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css",
                      //"~/Content/Maps/bootstrapmap.css" ,
                      "~/Content/ColorAdmin/animate.css",
                      "~/Content/ColorAdmin/style.css",
                      "~/Content/ColorAdmin/style-responsive.css",
                      "~/Content/ColorAdmin/theme/blue.css",
                      "~/Content/ColorAdmin/jquery.gritter.css"

                      ));

            bundles.Add(new StyleBundle("~/Content/ColorAdmin").Include(
                 

                      ));
        }
    }
}
