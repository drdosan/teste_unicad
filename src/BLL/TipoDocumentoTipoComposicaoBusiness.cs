using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class TipoDocumentoTipoComposicaoBusiness : UniCadBusinessBase<TipoDocumentoTipoComposicao>
    {
        private readonly EnumPais _pais;

        public TipoDocumentoTipoComposicaoBusiness()
        {
            _pais = EnumPais.Brasil;
        }

        public TipoDocumentoTipoComposicaoBusiness(EnumPais pais)
        {
            _pais = pais;
        }

        public IEnumerable<TipoDocumentoTipoComposicaoPlacaView> ListarTipoComposicaoPorTipoDocumento(int id)
        {
            List<TipoDocumentoTipoComposicao> composicoes = ListarComComposicoes(x => x.IDTipoDocumento == id);
            foreach (TipoDocumentoTipoComposicao composicao in composicoes)
            {
                if (composicao.Placa1.HasValue && composicao.Placa1.Value)
                {
                    yield return new TipoDocumentoTipoComposicaoPlacaView()
                    {
                        IdComposicao = composicao.IDTipoComposicao,
                        IdPlaca = "Placa1",
                        NomePlaca = Traducao.GetTextoPorLingua("Placa 1", "Tractor", _pais),
                        NomeComposicao = composicao.TipoComposicao.Nome
                    };
                }

                if (composicao.Placa2.HasValue && composicao.Placa2.Value)
                {
                    yield return new TipoDocumentoTipoComposicaoPlacaView()
                    {
                        IdComposicao = composicao.IDTipoComposicao,
                        IdPlaca = "Placa2",
                        NomePlaca = Traducao.GetTextoPorLingua("Placa 2", "Semiremolque 1", _pais),
                        NomeComposicao = composicao.TipoComposicao.Nome
                    };
                }

                if (composicao.Placa3.HasValue && composicao.Placa3.Value)
                {
                    yield return new TipoDocumentoTipoComposicaoPlacaView()
                    {
                        IdComposicao = composicao.IDTipoComposicao,
                        IdPlaca = "Placa3",
                        NomePlaca = Traducao.GetTextoPorLingua("Placa 3", "Semiremolque 2", _pais),
                        NomeComposicao = composicao.TipoComposicao.Nome
                    };
                }

                if (composicao.Placa4.HasValue && composicao.Placa4.Value)
                {
                    yield return new TipoDocumentoTipoComposicaoPlacaView()
                    {
                        IdComposicao = composicao.IDTipoComposicao,
                        IdPlaca = "Placa4",
                        NomePlaca = Traducao.GetTextoPorLingua("Placa 4", "Patente 4", _pais),
                        NomeComposicao = composicao.TipoComposicao.Nome
                    };
                }
            }
        }

        private List<TipoDocumentoTipoComposicao> ListarComComposicoes(Expression<Func<TipoDocumentoTipoComposicao, bool>> predicate)
        {
            using (UniCadDalRepositorio<TipoDocumentoTipoComposicao> repositorio = new UniCadDalRepositorio<TipoDocumentoTipoComposicao>("UniCadContext"))
            {
                return repositorio.ListComplex<TipoDocumentoTipoComposicao>().Include(x => x.TipoComposicao).Where(predicate).ToList();
            }
        }

        public IList<TipoDocumentoTipoComposicaoPlacaView> ListarComposicaoPorTipoDocumentoSemPlaca(int id)
        {
            List<TipoDocumentoTipoComposicao> composicoes = ListarComComposicoes(x => x.IDTipoDocumento == id);
            return composicoes.Select(x => new TipoDocumentoTipoComposicaoPlacaView() { IdComposicao = x.IDTipoComposicao, NomeComposicao = x.TipoComposicao.Nome }).ToList();
        }
    }
}
