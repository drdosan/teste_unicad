using System.Web.Mvc;
using Raizen.Framework.UserSystem.Client;
using Raizen.SSO.BLL;

namespace Raizen.UniCad.Web.Controllers
{
    public class LogOutController : Controller
    {
        [HttpGet]
        public JsonResult LogOut()
        {
            SignOnBusiness.LogOut();
            UserSession.LogOut();
            return Json("true", JsonRequestBehavior.AllowGet);
        }
    }
}
