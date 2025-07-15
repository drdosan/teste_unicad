using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Raizen.UniCad.DAL.Repositories
{
    public class PlacaDocumentoRepository : Repository<PlacaDocumento>, IPlacaDocumentoRepository
    {
        public PlacaDocumentoRepository(UniCadContexto contexto) : base(contexto)
        {
        }

        public List<PlacaDocumentoView> GetDocumentosAVencer(DateTime data)
        {
            var placaDocumentos = from placaDocumento in DbContext.Set<PlacaDocumento>().AsNoTracking()
                                  join placa in DbContext.Set<Placa>().AsNoTracking() on placaDocumento.IDPlaca equals placa.ID
                                  join composicao in DbContext.Set<Composicao>().AsNoTracking() on (int)EnumStatusComposicao.Aprovado equals composicao.IDStatus
                                  join tipoDocumento in DbContext.Set<TipoDocumento>().AsNoTracking() on placaDocumento.IDTipoDocumento equals tipoDocumento.ID

                                  join placaCliente in DbContext.Set<PlacaCliente>().AsNoTracking() on placa.ID equals placaCliente.IDPlaca into pct
                                  from pc in pct.DefaultIfEmpty()

                                  join usuarioCliente in DbContext.Set<UsuarioCliente>().AsNoTracking() on pc.IDCliente equals usuarioCliente.IDCliente into uct
                                  from uc in uct.DefaultIfEmpty()

                                  join usuario in DbContext.Set<Usuario>().AsNoTracking() on uc.IDUsuario equals usuario.ID into ucli1
                                  from u1 in ucli1.DefaultIfEmpty()

                                  join cliente in DbContext.Set<Cliente>().AsNoTracking() on uc.IDCliente equals cliente.ID into cli1
                                  from c1 in cli1.DefaultIfEmpty()

                                  join transportadora1 in DbContext.Set<Transportadora>().AsNoTracking() on placa.IDTransportadora equals transportadora1.ID into tran1
                                  from t1 in tran1.DefaultIfEmpty()

                                  join usuarioTransportadora1 in DbContext.Set<UsuarioTransportadora>().AsNoTracking() on t1.ID equals usuarioTransportadora1.IDTransportadora into utran1
                                  from utr1 in utran1.DefaultIfEmpty()

                                  join usuarioT1 in DbContext.Set<Usuario>().AsNoTracking() on utr1.IDUsuario equals usuarioT1.ID into utra1
                                  from ut1 in utra1.DefaultIfEmpty()

                                  join transportadora2 in DbContext.Set<Transportadora>().AsNoTracking() on placa.IDTransportadora2 equals transportadora2.ID into tran2
                                  from t2 in tran2.DefaultIfEmpty()

                                  join usuarioTransportadora2 in DbContext.Set<UsuarioTransportadora>().AsNoTracking() on t2.ID equals usuarioTransportadora2.IDTransportadora into utran2
                                  from utr2 in utran2.DefaultIfEmpty()

                                  join usuarioT2 in DbContext.Set<Usuario>().AsNoTracking() on utr1.IDUsuario equals usuarioT2.ID into utra2
                                  from ut2 in utra2.DefaultIfEmpty()

                                  where
                                        tipoDocumento.Status
                                        && tipoDocumento.DocumentoPossuiVencimento != false
                                        && ((tipoDocumento.Alerta1 > 0 && !placaDocumento.Alerta1Enviado && placaDocumento.DataVencimento == DbFunctions.AddDays(data, tipoDocumento.Alerta1)) ||
                                            (tipoDocumento.Alerta2 > 0 && !placaDocumento.Alerta2Enviado && placaDocumento.DataVencimento == DbFunctions.AddDays(data, tipoDocumento.Alerta2)))
                                        && placaDocumento.DataVencimento != null
                                        && (composicao.IDPlaca1 == placa.ID || composicao.IDPlaca2 == placa.ID || composicao.IDPlaca3 == placa.ID || composicao.IDPlaca4 == placa.ID)
                                  select new PlacaDocumentoView
                                  {
                                      ID = placaDocumento.ID,
                                      IDPlaca = placaDocumento.IDPlaca,
                                      DataVencimento = placaDocumento.DataVencimento,
                                      Operacao = placa.Operacao,
                                      Placa = placa.PlacaVeiculo,
                                      IdEmpresa = composicao.IDEmpresa,
                                      Sigla = tipoDocumento.Sigla,
                                      DiasVencimento = tipoDocumento.Alerta1,
                                      DiasVencimentoA2 = tipoDocumento.Alerta2,
                                      Alerta1Enviado = placaDocumento.Alerta1Enviado,
                                      Alerta2Enviado = placaDocumento.Alerta2Enviado,
                                      IBM = c1.IBM,
                                      Email = u1.Email,
                                      RazaoSocial = c1.RazaoSocial,
                                      IbmTransportadora1 = t1.IBM,
                                      EmailCif = ut1.Email,
                                      RazaoSocialTransportadora1 = t1.RazaoSocial,
                                      IbmTransportadora2 = t2.IBM,
                                      EmailEmpresaAmbos = ut2.Email,
                                      RazaoSocialTransportadora2 = t2.RazaoSocial,
                                      IdPais = placa.IDPais,
                                      TipoPlaca = (EnumTipoVeiculo)placa.IDTipoVeiculo,
                                      Documento = tipoDocumento.Descricao,
                                      QtdeAlertas = tipoDocumento.qtdeAlertas
                                  };

            return placaDocumentos.Distinct().ToList();
        }


        //public List<PlacaDocumentoView> GetDocumentosBloqueados(DateTime data)
        //{
        //    int statusComposicaoAprovado = (int)EnumStatusComposicao.Aprovado;

        //    var placaDocumentos = from placaDocumento in DbContext.Set<PlacaDocumento>().AsNoTracking()
        //                          join placa in DbContext.Set<Placa>().AsNoTracking() on placaDocumento.IDPlaca equals placa.ID
        //                          join composicao in DbContext.Set<Composicao>().AsNoTracking() on statusComposicaoAprovado equals composicao.IDStatus
        //                          join tipoDocumento in DbContext.Set<TipoDocumento>().AsNoTracking() on placaDocumento.IDTipoDocumento equals tipoDocumento.ID

        //                          join placaCliente in DbContext.Set<PlacaCliente>().AsNoTracking() on placa.ID equals placaCliente.IDPlaca into pct
        //                          from pc in pct.DefaultIfEmpty()

        //                          join usuarioCliente in DbContext.Set<UsuarioCliente>().AsNoTracking() on pc.IDCliente equals usuarioCliente.IDCliente into uct
        //                          from uc in uct.DefaultIfEmpty()

        //                          join usuarioFob in DbContext.Set<Usuario>().AsNoTracking() on uc.IDUsuario equals usuarioFob.ID into ufobt
        //                          from ufob in ufobt.DefaultIfEmpty()

        //                          join cliente in DbContext.Set<Cliente>().AsNoTracking() on uc.IDCliente equals cliente.ID into cli1
        //                          from c1 in cli1.DefaultIfEmpty()

        //                          join transportadora1 in DbContext.Set<Transportadora>().AsNoTracking() on placa.IDTransportadora equals transportadora1.ID into tran1
        //                          from t1 in tran1.DefaultIfEmpty()

        //                          join transportadoraUsuario in DbContext.Set<UsuarioTransportadora>().AsNoTracking() on t1.ID equals transportadoraUsuario.IDTransportadora into tut
        //                          from tu in tut.DefaultIfEmpty()

        //                          join usuarioCif in DbContext.Set<Usuario>().AsNoTracking() on tu.IDUsuario equals usuarioCif.ID into ucift
        //                          from ucif in ucift.DefaultIfEmpty()

        //                          join transportadora2 in DbContext.Set<Transportadora>().AsNoTracking() on placa.IDTransportadora2 equals transportadora2.ID into tran2
        //                          from t2 in tran2.DefaultIfEmpty()

        //                          join transportadoraUsuario2 in DbContext.Set<UsuarioTransportadora>().AsNoTracking() on t2.ID equals transportadoraUsuario2.IDTransportadora into tu2t
        //                          from tu2 in tu2t.DefaultIfEmpty()

        //                          join usuarioCif2 in DbContext.Set<Usuario>().AsNoTracking() on tu.IDUsuario equals usuarioCif2.ID into ucif2t
        //                          from ucif2 in ucif2t.DefaultIfEmpty()

        //                          where
        //                                tipoDocumento.Status
        //                                && (tipoDocumento.DocumentoPossuiVencimento != false)
        //                                && (!placaDocumento.Bloqueado)
        //                                && (placaDocumento.DataVencimento.HasValue && placaDocumento.DataVencimento < data)
        //                                && (!placaDocumento.Processado)
        //                                && (composicao.IDPlaca1 == placa.ID || composicao.IDPlaca2 == placa.ID || composicao.IDPlaca3 == placa.ID || composicao.IDPlaca4 == placa.ID)
        //                          select new PlacaDocumentoView
        //                          {
        //                              ID = placaDocumento.ID,
        //                              IDPlaca = placaDocumento.IDPlaca,
        //                              IDComposicao = composicao.ID,
        //                              Operacao = placa.Operacao,
        //                              Placa = placa.PlacaVeiculo,
        //                              Sigla = tipoDocumento.Sigla,
        //                              QuantidadeDiasBloqueio = tipoDocumento.QtdDiasBloqueio ?? 0,
        //                              Email = ufob.Email,
        //                              IBM = c1.IBM,
        //                              RazaoSocial = c1.RazaoSocial,
        //                              IDUsuario = ufob == null ? 0 : ufob.ID,
        //                              EmailCif = ucif.Email,
        //                              IbmTransportadora1 = t1.IBM,
        //                              RazaoSocialTransportadora1 = t1.RazaoSocial,
        //                              IDUsuarioTransportadora1 = ucif == null ? 0 : ucif.ID,
        //                              EmailEmpresaAmbos = ucif2.Email,
        //                              IbmTransportadora2 = t2.IBM,
        //                              RazaoSocialTransportadora2 = t2.RazaoSocial,
        //                              IDUsuarioTransportadora2 = ucif2 == null ? 0 : ucif2.ID,
        //                              Documento = tipoDocumento.Descricao,
        //                              TipoPlaca = (EnumTipoVeiculo)placa.IDTipoVeiculo,
        //                              TipoAcaoVencimento = (EnumTipoAcaoVencimento)tipoDocumento.TipoAcaoVencimento,
        //                              DataVencimento = placaDocumento.DataVencimento,
        //                              TipoBloqueioImediato = (tipoDocumento.BloqueioImediato == null || tipoDocumento.BloqueioImediato == 2) ?
        //                                                      EnumTipoBloqueioImediato.Nao :
        //                                                      (EnumTipoBloqueioImediato)tipoDocumento.BloqueioImediato,
        //                              IdPais = placa.IDPais,
        //                              DiasVencimento = tipoDocumento.QtdDiasBloqueio ?? 0,
        //                              QtdeAlertas = tipoDocumento.qtdeAlertas
        //                          };

        //    return placaDocumentos.Distinct().ToList();
        //}

        public List<PlacaDocumentoView> GetDocumentosBloqueados(DateTime data)
        {
            SqlParameter paramInicio = new SqlParameter("@DataVencimento", SqlDbType.DateTime);

            paramInicio.Value = data;

            using (UniCadDalRepositorio<PlacaDocumentoView> repositorio = new UniCadDalRepositorio<PlacaDocumentoView>())
            {
                return repositorio.ExecutarProcedureComRetorno("[dbo].[Proc_GetDocumentosBloqueados] @DataVencimento", new object[] { paramInicio });
            }
        }
    }
}
