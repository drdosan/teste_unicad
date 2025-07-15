using Raizen.UniCad.Model.View;
using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Raizen.UniCad.SAL
{
    public class WsEasyQuery
    {
        public string CriarNovoTicket(EasyQueryView parametros)
        {
            var client = new WsEasyquery.wsEasyQueryACS();

            string UserName = ConfigurationManager.AppSettings["usuarioWsEasyQuery"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSEasyQuery"];
            string URL = ConfigurationManager.AppSettings["urlWSEasyQUery"];

            client.Url = URL;
            client.AuthenticatorValue = new WsEasyquery.Authenticator();
            client.AuthenticatorValue.Username = UserName;
            client.AuthenticatorValue.Password = PassWord;

            string retorno = client.CreateTicketByUnicad(parametros.idSubcategoria,
                    parametros.ResolutionGroupID,
                    parametros.Description,
                    parametros.Subject,
                    parametros.ContactEmail,
                    true);

            if (SolicitacaoFoiCriada(retorno))
            {
                return retorno;
            }
            else
            {
                throw new Exception(retorno);
            }
        }

        private bool SolicitacaoFoiCriada(string ret)
        {
            var reg = new Regex("[0-9]{7}-[0-9]{7}");
            return (reg.IsMatch(ret));
        }
    }
}
