using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLComponents;

using System.Data;
using System.Diagnostics;
using DataAccessLayer;
using SecurityBL;

namespace WebLayer.Areas.Maps.Controllers
{
    public class TiposDocumentos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
    }

    
    public class MonitoreoController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}