using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class TipoDocumentoTipoProdutoBusiness : UniCadBusinessBase<TipoDocumentoTipoProduto>
    {
        public List<TipoDocumentoTipoProdutoView> ListarTipoProdutoPorTipoDocumento(int IDTipoDocumento)
        {
            using (UniCadDalRepositorio<TipoDocumentoTipoProduto> repositorio = new UniCadDalRepositorio<TipoDocumentoTipoProduto>())
            {
                var query = GetQuery(repositorio, IDTipoDocumento);

                return query.ToList();
            }
        }

        private IQueryable<TipoDocumentoTipoProdutoView> GetQuery(UniCadDalRepositorio<TipoDocumentoTipoProduto> repositorio, int IDTipoDocumento)
        {
            var tipoProdutos = from TipoDocumentoTipoProduto in repositorio.ListComplex<TipoDocumentoTipoProduto>().AsNoTracking()
                           join tipoProduto in repositorio.ListComplex<TipoProduto>().AsNoTracking() on TipoDocumentoTipoProduto.IDTipoProduto equals tipoProduto.ID
                           where TipoDocumentoTipoProduto.IDTipoDocumento == IDTipoDocumento
                               select new TipoDocumentoTipoProdutoView { ID = TipoDocumentoTipoProduto.ID, IDTipoProduto = TipoDocumentoTipoProduto.IDTipoProduto, IDTipoDocumento = TipoDocumentoTipoProduto.IDTipoDocumento, Nome = tipoProduto.Nome };

            return tipoProdutos;


        }
    }
}

