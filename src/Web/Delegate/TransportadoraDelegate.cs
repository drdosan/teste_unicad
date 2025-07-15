using Raizen.Framework.UserSystem.Client;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Raizen.UniCad
{
    public class TransportadoraDelegate
    {
        public static List<ClienteTransportadoraView> Listar(string operacao, int linhaNegocio, int idPais)
        {
            var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var user = new UsuarioBusiness().Selecionar(p => p.Login == login);
            int? idUsuario = null;
            if (user != null && user.Externo && !user.Perfil.IsQuality())
                idUsuario = user.ID;    

            var lista = new List<ClienteTransportadoraView>();
            List<ClienteTransportadoraView> listaTransp;
            if (idUsuario.HasValue)
                listaTransp = new TransportadoraBusiness().ListarPorUsuario(idUsuario, operacao,linhaNegocio, idPais).OrderBy(p => p.RazaoSocial).ToList();
            else
                listaTransp = new TransportadoraBusiness().ListarSemUsuario(operacao,linhaNegocio, idPais).OrderBy(p => p.RazaoSocial).ToList();

            listaTransp.ForEach(p => p.RazaoSocial = string.Format("{0} - {1} - {2}", p.IBM, p.RazaoSocial, p.CPF_CNPJ));
            lista.AddRange(listaTransp);

            return lista;
        }
    }
}