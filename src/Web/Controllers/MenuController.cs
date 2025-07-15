using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
    public class MenuController : Controller
    {
        [HttpGet]
        public ActionResult AtualizarItemMenuSelecionado(string Id)
        {
            this.Session.Add("UltimoItemMenuSelecionado", Id);
            return null;
        }

        [HttpGet]
        public JsonResult GetItemMenuSelecionado()
        {
            string retorno = "";

            if (this.Session["UltimoItemMenuSelecionado"] != null)
            {
                retorno = this.Session["UltimoItemMenuSelecionado"].ToString();
            }
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }
    }
}
