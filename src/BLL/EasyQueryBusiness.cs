using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;

namespace Raizen.UniCad.BLL
{
    public class EasyQueryBusiness
    {
        private WsEasyQuery wsEasyQuery = new WsEasyQuery();
         
        public string CriarNovoTicket(EasyQueryView parametros)
        {
            return wsEasyQuery.CriarNovoTicket(parametros);
        }
    }
}
