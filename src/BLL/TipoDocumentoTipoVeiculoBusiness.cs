using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class TipoDocumentoTipoVeiculoBusiness : UniCadBusinessBase<TipoDocumentoTipoVeiculo>
    {
        public List<TipoDocumentoTipoVeiculoView> ListarTipoVeiculoPorTipoDocumento(int IDTipoDocumento)
        {
            using (UniCadDalRepositorio<TipoDocumentoTipoVeiculo> repositorio = new UniCadDalRepositorio<TipoDocumentoTipoVeiculo>())
            {
                var query = GetQuery(repositorio, IDTipoDocumento);

                return query.ToList();
            }
        }

        private IQueryable<TipoDocumentoTipoVeiculoView> GetQuery(UniCadDalRepositorio<TipoDocumentoTipoVeiculo> repositorio, int IDTipoDocumento)
        {
            var TipoVeiculos = from TipoDocumentoTipoVeiculo in repositorio.ListComplex<TipoDocumentoTipoVeiculo>().AsNoTracking()
                               join TipoVeiculo in repositorio.ListComplex<TipoVeiculo>().AsNoTracking() on TipoDocumentoTipoVeiculo.IDTipoVeiculo equals TipoVeiculo.ID
                               where TipoDocumentoTipoVeiculo.IDTipoDocumento == IDTipoDocumento
                               select new TipoDocumentoTipoVeiculoView { ID = TipoDocumentoTipoVeiculo.ID, IDTipoVeiculo = TipoDocumentoTipoVeiculo.IDTipoVeiculo, IDTipoDocumento = TipoDocumentoTipoVeiculo.IDTipoDocumento, Nome = TipoVeiculo.Nome };

            return TipoVeiculos;


        }
    }
}

