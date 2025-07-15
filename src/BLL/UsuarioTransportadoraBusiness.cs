using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class UsuarioTransportadoraBusiness : UniCadBusinessBase<UsuarioTransportadora>
    {
        public List<UsuarioTransportadoraView> ListarTransportadorasPorUsuario(int IDUsuario, int? IDEmpresa = null)
        {
            using (UniCadDalRepositorio<UsuarioTransportadora> repositorio = new UniCadDalRepositorio<UsuarioTransportadora>())
            {
                var query = GetQuery(repositorio, IDUsuario, IDEmpresa);

                return query.ToList();
            }
        }

        private IQueryable<UsuarioTransportadoraView> GetQuery(UniCadDalRepositorio<UsuarioTransportadora> repositorio, int IDUsuario, int? IDEmpresa)
        {
            var Transportadoras = from usuarioTransportadora in repositorio.ListComplex<UsuarioTransportadora>().AsNoTracking()
                                  join Transportadora in repositorio.ListComplex<Transportadora>().AsNoTracking() on usuarioTransportadora.IDTransportadora equals Transportadora.ID
                                  where usuarioTransportadora.IDUsuario == IDUsuario
                                  && (Transportadora.IDEmpresa == IDEmpresa.Value || !IDEmpresa.HasValue)
                                  select new UsuarioTransportadoraView
                                  {
                                      ID = usuarioTransportadora.ID,
                                      IDTransportadora = usuarioTransportadora.IDTransportadora,
                                      IDUsuario = usuarioTransportadora.IDUsuario,
                                      IBM = Transportadora.IBM,                                   
                                      Nome = Transportadora.RazaoSocial,
                                      CNPJCPF = Transportadora.CNPJCPF
                                  };

            return Transportadoras;
        }
    }
}

