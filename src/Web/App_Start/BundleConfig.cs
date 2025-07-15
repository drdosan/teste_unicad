using System;
using System.Web.Optimization;

namespace Raizen.UniCad.Web
{
    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(System.Web.Optimization.BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CSS
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

         
            // Version - 0.15.2
            bundles.Add(new StyleBundle("~/Content/selectize").Include(
                   "~/Content/css/selectize/selectize.css",
                   "~/Content/css/selectize/selectize.bootstrap5.css"
                   ));

            //bundles.Add(new StyleBundle("~/Content/css/sistema").Include(
            //         "~/Content/css/jquery-ui/jquery-ui-1.10.3.custom.min.css",
            //         "~/Content/css/cloud-admin.min.css",
            //         "~/Content/css/font-awesome.min.css",
            //         "~/Content/css/responsive.css"
            //         ));

            bundles.Add(new ScriptBundle("~/bundles/plugin/selectize").Include(
               "~/Content/js/third-party/selectize.js"
               ));

            
            //CSS base do Cloud Admin
            bundles.Add(new StyleBundle("~/Content/css/cloud-admin").Include(
                "~/Content/css/cloud-admin.min.css"));

            //CSS responsive
            bundles.Add(new StyleBundle("~/Content/css/responsive").Include(
                "~/Content/css/responsive.min.css"));

            //CSS Themas das Empresas
            //bundles.Add(new StyleBundle("~/Content/css/theme/neutro").Include("~/Content/css/themes/neutro.css"));
            bundles.Add(new StyleBundle("~/Content/css/theme/raizen").Include("~/Content/css/themes/raizen.css"));
            //bundles.Add(new StyleBundle("~/Content/css/theme/cosan").Include("~/Content/css/themes/cosan.css"));
            //bundles.Add(new StyleBundle("~/Content/css/theme/comgas").Include("~/Content/css/themes/comgas.css"));
            //bundles.Add(new StyleBundle("~/Content/css/theme/rumo").Include("~/Content/css/themes/rumo.css"));

            //CSS das Fontes Awesome e Opensans
            //Obs.: NÃO alterar o nome do bundle. O Font-Awesome usa, internamente em seus css, redirecionamento
            //relativo (../). Usar CssRewriteUrlTransformation neste caso não resolve para sites rodando como subsite.
            // Como o bundles ainda não possui suporte para resolver Urls internas nos css, temos que nomear o bundle
            //com a mesma estrutura de pastas, mudando apenas o final (nome do bundle em si, note que da pasta css para
            //trás é a mesma estrutura física de pastas do site).
            //Referência: http://stackoverflow.com/questions/11355935/mvc4-stylebundle-not-resolving-images
            //Vide comentário Hao Kung Jul 13 '12 at 22:56
            bundles.Add(
                new StyleBundle("~/Content/font-awesome/css/font-awesome")
                .Include("~/Content/font-awesome/css/font-awesome.min.css")
                .Include("~/Content/font-opensans/css/font-opensans.css"));

            //CSS do jQuery UI
            bundles.Add(new StyleBundle("~/Content/css/jqueryui").Include(
                "~/Content/js/third-party/jquery-ui-1.10.3.custom/css/custom-theme/jquery-ui-1.10.3.custom.min.css"));

            //CSS do componente DateRangePicker
            bundles.Add(new StyleBundle("~/Content/js/third-party/bootstrap-daterangepicker/daterangepickercss").Include(
                "~/Content/js/third-party/bootstrap-daterangepicker/daterangepicker-bs3.css"));

            //CSS do componente DateRangePicker
            bundles.Add(new StyleBundle("~/Content/js/third-party/multipledatespicker/multipledatespickercss").Include(
                "~/Content/js/third-party/multipledatespicker/jquery-ui.multidatespicker.css"));

            //CSS de DataTables
            bundles.Add(new StyleBundle("~/Content/css/datatables").Include(
                "~/Content/js/third-party/tablecloth/css/tablecloth.min.css",
                "~/Content/js/third-party/datatables/media/css/jquery.dataTables.min.css",
                "~/Content/js/third-party/datatables/media/assets/css/datatables.min.css",
                "~/Content/js/third-party/datatables/extras/TableTools/media/css/TableTools.min.css"));

            //CSS do AutoComplete
            bundles.Add(new StyleBundle("~/Content/css/typeahead").Include(
                "~/Content/js/typeahead/typeahead.css"));

            //DatePicker
            bundles.Add(new StyleBundle("~/Content/js/third-party/datepicker/themes/datepicker").Include(
                "~/Content/js/third-party/datepicker/themes/default.min.css",
                "~/Content/js/third-party/datepicker/themes/default.date.min.css",
                "~/Content/js/third-party/datepicker/themes/default.time.min.css"));

            //CSS do TreeView
            bundles.Add(new StyleBundle("~/Content/css/treeview").Include(
                "~/Content/js/third-party/fuelux-tree/fuelux.min.css",
                "~/Content/js/third-party/uniform/css/uniform.default.min.css",
                "~/Content/js/third-party/jsTree/themes/default/style.min.css",
                "~/Content/js/third-party/select2/select2.min.css",
                "~/Content/css/Site.css"));

            bundles.Add(new StyleBundle("~/Content/css/hubspot-messenger").Include(
                "~/Content/js/third-party/hubspot-messenger/css/messenger.min.css",
                "~/Content/js/third-party/hubspot-messenger/css/messenger-spinner.min.css",
                "~/Content/js/third-party/hubspot-messenger/css/messenger-theme-air.min.css",
                "~/Content/js/third-party/hubspot-messenger/css/messenger-theme-block.min.css",
                "~/Content/js/third-party/hubspot-messenger/css/messenger-theme-flat.min.css",
                "~/Content/js/third-party/hubspot-messenger/css/messenger-theme-future.min.css",
                "~/Content/js/third-party/hubspot-messenger/css/messenger-theme-ice.min.css"));

            //CSS jqGrid
            bundles.Add(new StyleBundle("~/Content/css/jqgridCSS").Include(
            "~/Content/js/third-party/jquery.jqGrid/ui.jqgrid.min.css"
            ));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // SCRIPTS
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //SCRIPTS de compatibildiade com IE 8
            bundles.Add(new ScriptBundle("~/Content/js/third-party/compatibilidade").Include(
                        "~/Content/js/third-party/flot/excanvas.min.js",
                        "~/Content/js/third-party/compatibility/html5.js",
                        "~/Content/js/third-party/compatibility/css3-mediaqueries.js"));

            //SCRIPTS jQuery
            bundles.Add(new ScriptBundle("~/Content/js/third-party/jquery/jquery").Include(
                        "~/Content/js/third-party/jquery/jquery-2.0.3.min.js"));

            //SCRIPTS jQuery UI
            bundles.Add(new ScriptBundle("~/Content/js/third-party/jqueryui").Include(
                        "~/Content/js/third-party/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.min.js"));

            //SCRIPTS jQuery Validation
            bundles.Add(new ScriptBundle("~/Content/js/third-party/jqueryval").Include(
                        "~/Content/js/third-party/Customizado/jquery.validate.js",
                        "~/Content/js/third-party/Customizado/jquery.unobtrusive-ajax.js",
                        "~/Content/js/third-party/Customizado/jquery.validate.unobtrusive.js",
                        "~/Content/js/third-party/Customizado/jquery.validate.custom.pt-br.js"));

            //SCRIPTS Bootstrap
            bundles.Add(new ScriptBundle("~/Content/js/third-party/bootstrapdist").Include(
                        "~/Content/js/third-party/bootstrap-dist/js/bootstrap.min.js"));

            //SCRIPTS Cloud Admin Custom Script
            bundles.Add(new ScriptBundle("~/Content/js/third-party/custom-script-js").Include(
                        "~/Content/js/third-party/script.js"));

            //SCRIPTS Bootstrap DateRangePicker
            bundles.Add(new ScriptBundle("~/Content/js/third-party/bootstrap-daterangepicker/daterangepicker").Include(
                        "~/Content/js/third-party/bootstrap-daterangepicker/moment.min.js",
                        "~/Content/js/third-party/bootstrap-daterangepicker/daterangepicker.min.js"));

            //SCRIPTS Bootstrap MultipleDateRangePicker
            bundles.Add(new ScriptBundle("~/Content/js/third-party/multipledatespicker/multipledatespicker").Include(
                        "~/Content/js/third-party/multipledatespicker/jquery-ui.multidatespicker.js"));

            //SCRIPTS Bootstrap DateRangePicker
            bundles.Add(new ScriptBundle("~/Content/js/third-party/modernizr/modernizr-2.6.2").Include(
                        "~/Content/js/third-party/modernizr-2.6.2/modernizr-2.6.2.js"));

            //SCRIPTS Bootstrap DatePicker
            bundles.Add(new ScriptBundle("~/Content/js/third-party/datepicker/datepicker").Include(
                        "~/Content/js/third-party/datepicker/ui.datepicker-pt-BR.js"));

            //SCRIPTS Bootstrap-wysiwyg
            //bundles.Add(new ScriptBundle("~/Content/js/third-party/bootstrap-wysiwyg/bootstrap-wysiwyg").Include(
            //            "~/Content/js/third-party/bootstrap-wysiwyg/bootstrap-wysiwyg.min.js",
            //            "~/Content/js/third-party/bootstrap-wysiwyg/jquery.hotkeys.min.js"));

            //SCRIPTS Bootstrap CKEditor
            //bundles.Add(new ScriptBundle("~/Content/js/third-party/ckeditor/ckeditor").Include(
            //            "~/Content/js/third-party/ckeditor/ckeditor.js"));


            //SCRIPTS jQuery TableCloth
            bundles.Add(new ScriptBundle("~/Content/js/third-party/tablecloth/js/tablecloth").Include(
                "~/Content/js/third-party/tablecloth/js/jquery.tablecloth.js",
                "~/Content/js/third-party/tablecloth/js/jquery.tablesorter.min.js"));

            //SCRIPTS jQuery SlimScroll
            bundles.Add(new ScriptBundle("~/Content/js/third-party/jQuery-slimScroll").Include(
                        "~/Content/js/third-party/jQuery-slimScroll-1.3.0/jquery.slimscroll.min.js",
                        "~/Content/js/third-party/jQuery-slimScroll-1.3.0/slimScrollHorizontal.min.js"));

            //SCRIPTS jQuery jqGrid

            bundles.Add(new ScriptBundle("~/Content/js/third-party/jqGrid").Include(
              "~/Content/js/third-party/jquery.jqGrid/jquery.jqGrid.min.js ",
              "~/Content/js/third-party/jquery.jqGrid/grid.locale-pt-br.js "
              ));

            //SCRIPTS jQuery Select2

            bundles.Add(new ScriptBundle("~/Content/js/third-party/select2/select2").Include("~/Content/js/third-party/select2/select2.min.js"));

            //SCRIPTS jQuery BlockUI
            bundles.Add(new ScriptBundle("~/Content/js/third-party/jQuery-BlockUI/jquery-blockui").Include(
                        "~/Content/js/third-party/jQuery-BlockUI/jquery.blockUI.min.js"));

            //SCRIPTS jQuery Cookie
            bundles.Add(new ScriptBundle("~/Content/js/third-party/jQuery-Cookie/jquery-cookie").Include(
                        "~/Content/js/third-party/jQuery-Cookie/jquery.cookie.min.js"));

            //SCRIPTS jQuery Data Tables
            bundles.Add(new ScriptBundle("~/Content/js/third-party/datatables/datatables").Include(
                        "~/Content/js/third-party/datatables/media/js/jquery.dataTables.min.js",
                        "~/Content/js/third-party/datatables/media/assets/js/datatables.min.js",
                        "~/Content/js/third-party/datatables/extras/TableTools/media/js/TableTools.min.js",
                        "~/Content/js/third-party/datatables/extras/TableTools/media/js/ZeroClipboard.min.js"));


            //SCRIPTS Mask
            bundles.Add(new ScriptBundle("~/Content/js/third-party/mask").Include(
            "~/Content/js/third-party/bootstrap-inputmask.js",
            "~/Content/js/third-party/jquery.maskMoney.js"
            ));


            bundles.Add(new ScriptBundle("~/Content/js/third-party/jquerymask").Include(
                        "~/Content/js/third-party/Customizado/jquery.maskMoney.min.js",
                        "~/Content/js/third-party/Customizado/jquery.mask.js"));

            //SCRIPTS para o plugin de mensagens hubspot-messenger
            bundles.Add(new ScriptBundle("~/Content/js/third-party/hubspot-messenger/hubspot-messenger").Include(
                        "~/Content/js/third-party/hubspot-messenger/js/messenger.min.js",
                        "~/Content/js/third-party/hubspot-messenger/js/messenger-theme-flat.js",
                        "~/Content/js/third-party/hubspot-messenger/js/messenger-theme-future.js"));

            bundles.Add(new ScriptBundle("~/Content/js/third-party/typeahead/typeahead").Include(
                        "~/Content/js/third-party/typeahead/typeahead.min.js"));

            bundles.Add(new ScriptBundle("~/Content/js/third-party/selectize").Include(
                        "~/Content/js/third-party/selectize.js"));

            //SCRIPTS Namespace
            bundles.Add(new ScriptBundle("~/Content/js/third-party/namespace").Include(
                        "~/Content/js/Namespace.js"));
            ////Print
            //bundles.Add(new ScriptBundle("~/Content/js/jquery-print").Include(
            //    "~/Content/js/third-party/jquery-print/jQuery.print.js"));

            //SCRIPTS FRAMEWORK RAIZEN
            //Container
            bundles.Add(new ScriptBundle("~/Content/js/raizen-init-container").Include(
                        "~/Content/js/Raizen/Raizen.Init.js"));



            //Mascaras
            bundles.Add(new ScriptBundle("~/Content/js/raizen-mask").Include(
                        "~/Content/js/Raizen/Raizen.Mask.js"));

          

            //Menus
            bundles.Add(new ScriptBundle("~/Content/js/raizen-menus").Include(
                        "~/Content/js/Raizen/Raizen.Menus.js"));

            //JS - UniCad
            //bundles.Add(new ScriptBundle("~/bundles/UniCad").Include(
            //        "~/Content/js/Raizen.UniCad.Tools.js",
            //        "~/Content/js/Raizen.UniCad.AJAX.js",
            //        "~/Content/js/Raizen.UniCad.View.Helper.js",
            //        "~/Content/js/Raizen/UniCad/Raizen.UniCad.Helper.js"
            //        ));

            //Domínios
            bundles.Add(new ScriptBundle("~/Content/js/raizen-dominios").Include(
                        "~/Content/js/Raizen/Raizen.Dominios.js"));


            //Helpers
            bundles.Add(new ScriptBundle("~/Content/js/raizen-helpers").Include(
                        "~/Content/js/Raizen/Raizen.Helpers.js"));


            BundleTable.EnableOptimizations = true;
        }




        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null) throw new ArgumentNullException("ignoreList");

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
        }
    }
}