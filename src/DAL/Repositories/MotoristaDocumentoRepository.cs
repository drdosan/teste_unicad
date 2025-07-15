using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Raizen.UniCad.DAL.Repositories
{
    public class MotoristaDocumentoRepository : Repository<MotoristaDocumento>, IMotoristaDocumentoRepository
    {
        public MotoristaDocumentoRepository(UniCadContexto contexto) : base(contexto)
        {
        }

        public List<MotoristaDocumentoView> GetDocumentosAVencer(DateTime data)
        {
            var motoristaDocumentos = from motoristaDocumento in DbContext.Set<MotoristaDocumento>().AsNoTracking()
                                      join motorista in DbContext.Set<Motorista>().AsNoTracking() on motoristaDocumento.IDMotorista equals motorista.ID
                                      join tipoDocumento in DbContext.Set<TipoDocumento>().AsNoTracking() on motoristaDocumento.IDTipoDocumento equals tipoDocumento.ID

                                      join motoristaBrasil in DbContext.Set<MotoristaBrasil>().AsNoTracking() on motorista.ID equals motoristaBrasil.IDMotorista into motoBr
                                      from subMotoristaBrasil in motoBr.DefaultIfEmpty()

                                      join motoristaArgentina in DbContext.Set<MotoristaArgentina>().AsNoTracking() on motorista.ID equals motoristaArgentina.IDMotorista into motoArg
                                      from subMotoristaArgentina in motoArg.DefaultIfEmpty()

                                      join motoristaCliente in DbContext.Set<MotoristaCliente>().AsNoTracking() on motorista.ID equals motoristaCliente.IDMotorista into mtc
                                      from mc in mtc.DefaultIfEmpty()

                                      join usuarioCliente in DbContext.Set<UsuarioCliente>().AsNoTracking() on mc.IDCliente equals usuarioCliente.IDCliente into uct
                                      from uc in uct.DefaultIfEmpty()

                                      join usuario in DbContext.Set<Usuario>().AsNoTracking() on uc.IDUsuario equals usuario.ID into ucli1
                                      from u1 in ucli1.DefaultIfEmpty()

                                      join cliente in DbContext.Set<Cliente>().AsNoTracking() on uc.IDCliente equals cliente.ID into cli1
                                      from c1 in cli1.DefaultIfEmpty()

                                      join transportadora in DbContext.Set<Transportadora>().AsNoTracking() on motorista.IDTransportadora equals transportadora.ID into tran
                                      from t in tran.DefaultIfEmpty()

                                      join usuarioTransportadora in DbContext.Set<UsuarioTransportadora>().AsNoTracking() on t.ID equals usuarioTransportadora.IDTransportadora into utran
                                      from utr in utran.DefaultIfEmpty()

                                      join usuarioT in DbContext.Set<Usuario>().AsNoTracking() on utr.IDUsuario equals usuarioT.ID into utra
                                      from ut in utra.DefaultIfEmpty()

                                      where
                                            tipoDocumento.Status
                                            && tipoDocumento.DocumentoPossuiVencimento != false
                                            && motorista.IDStatus == (int)EnumStatusMotorista.Aprovado
                                            && ((tipoDocumento.Alerta1 > 0 && !motoristaDocumento.Alerta1Enviado && motoristaDocumento.DataVencimento == DbFunctions.AddDays(data, tipoDocumento.Alerta1)) ||
                                                (tipoDocumento.Alerta2 > 0 && !motoristaDocumento.Alerta2Enviado && motoristaDocumento.DataVencimento == DbFunctions.AddDays(data, tipoDocumento.Alerta2)))
                                            && motoristaDocumento.DataVencimento != null
                                      select new MotoristaDocumentoView
                                      {
                                          ID = motoristaDocumento.ID,
                                          IDMotorista = motoristaDocumento.IDMotorista,
                                          Operacao = motorista.Operacao,
                                          Sigla = tipoDocumento.Sigla,
                                          DiasVencimento = tipoDocumento.Alerta1,
                                          DiasVencimentoA2 = tipoDocumento.Alerta2,
                                          Alerta1Enviado = motoristaDocumento.Alerta1Enviado,
                                          Alerta2Enviado = motoristaDocumento.Alerta2Enviado,
                                          IBM = c1.IBM,
                                          Email = u1.Email,
                                          RazaoSocial = c1.RazaoSocial,
                                          IbmTransportadora = t.IBM,
                                          EmailTransportadora = ut.Email,
                                          RazaoSocialTransportadora = t.RazaoSocial,
                                          Documento = tipoDocumento.Descricao,
                                          Nome = motorista.Nome,
                                          TipoAcaoVencimento = (EnumTipoAcaoVencimento)tipoDocumento.TipoAcaoVencimento,
                                          CPF = (subMotoristaBrasil != null ? subMotoristaBrasil.CPF : null),
                                          DNI = (subMotoristaArgentina != null ? subMotoristaArgentina.DNI : null),
                                          IdPais = motorista.IdPais,
                                          DataVencimento = motoristaDocumento.DataVencimento,
                                          QtdeAlertas = tipoDocumento.qtdeAlertas
                                      };

            return motoristaDocumentos.Distinct().ToList();
        }

        public List<MotoristaDocumentoView> GetDocumentosBloqueados (DateTime data)
        {
            var motoristaDocumentos = from motoristaDocumento in DbContext.Set<MotoristaDocumento>().AsNoTracking()
                                      join motorista in DbContext.Set<Motorista>().AsNoTracking() on motoristaDocumento.IDMotorista equals motorista.ID
                                      join tipoDocumento in DbContext.Set<TipoDocumento>().AsNoTracking() on motoristaDocumento.IDTipoDocumento equals tipoDocumento.ID

                                      join motoristaBrasil in DbContext.Set<MotoristaBrasil>().AsNoTracking() on motorista.ID equals motoristaBrasil.IDMotorista into motoBr
                                      from subMotoristaBrasil in motoBr.DefaultIfEmpty()

                                      join motoristaArgentina in DbContext.Set<MotoristaArgentina>().AsNoTracking() on motorista.ID equals motoristaArgentina.IDMotorista into motoArg
                                      from subMotoristaArgentina in motoArg.DefaultIfEmpty()

                                      join motoristaCliente in DbContext.Set<MotoristaCliente>().AsNoTracking() on motorista.ID equals motoristaCliente.IDMotorista into mtc
                                      from mc in mtc.DefaultIfEmpty()

                                      join usuarioCliente in DbContext.Set<UsuarioCliente>().AsNoTracking() on mc.IDCliente equals usuarioCliente.IDCliente into uct
                                      from uc in uct.DefaultIfEmpty()

                                      join usuario in DbContext.Set<Usuario>().AsNoTracking() on uc.IDUsuario equals usuario.ID into ucli1
                                      from u1 in ucli1.DefaultIfEmpty()

                                      join cliente in DbContext.Set<Cliente>().AsNoTracking() on uc.IDCliente equals cliente.ID into cli1
                                      from c1 in cli1.DefaultIfEmpty()

                                      join transportadora in DbContext.Set<Transportadora>().AsNoTracking() on motorista.IDTransportadora equals transportadora.ID into tran
                                      from t in tran.DefaultIfEmpty()

                                      join usuarioTransportadora in DbContext.Set<UsuarioTransportadora>().AsNoTracking() on t.ID equals usuarioTransportadora.IDTransportadora into utran
                                      from utr in utran.DefaultIfEmpty()

                                      join usuarioT in DbContext.Set<Usuario>().AsNoTracking() on utr.IDUsuario equals usuarioT.ID into utra
                                      from ut in utra.DefaultIfEmpty()

                                      where
                                            tipoDocumento.Status
                                            && (tipoDocumento.DocumentoPossuiVencimento != false)
                                            && (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado)
                                            && (!motoristaDocumento.Bloqueado)
                                            && (motoristaDocumento.DataVencimento.HasValue && motoristaDocumento.DataVencimento < data)
                                            && (!motoristaDocumento.Processado)
                                      select new MotoristaDocumentoView
                                      {
                                          ID = motoristaDocumento.ID,
                                          IDMotorista = motoristaDocumento.IDMotorista,
                                          Operacao = motorista.Operacao,
                                          Sigla = tipoDocumento.Sigla,
                                          IBM = c1.IBM,
                                          Email = u1.Email,
                                          RazaoSocial = c1.RazaoSocial,
                                          IbmTransportadora = t.IBM,
                                          EmailTransportadora = ut.Email,
                                          RazaoSocialTransportadora = t.RazaoSocial,
                                          Documento = tipoDocumento.Descricao,
                                          Nome = motorista.Nome,
                                          TipoAcaoVencimento = (EnumTipoAcaoVencimento)tipoDocumento.TipoAcaoVencimento,
                                          CPF = (subMotoristaBrasil != null ? subMotoristaBrasil.CPF : null),
                                          DNI = (subMotoristaArgentina != null ? subMotoristaArgentina.DNI : null),
                                          IdPais = motorista.IdPais,
                                          DataVencimento = motoristaDocumento.DataVencimento,
                                          BloqueioImediato = tipoDocumento.BloqueioImediato,
                                          QuantidadeDiasBloqueio = tipoDocumento.QtdDiasBloqueio ?? 0,
                                          DiasVencimento = tipoDocumento.QtdDiasBloqueio ?? 0,
                                          TipoBloqueioImediato = (tipoDocumento.BloqueioImediato == null || tipoDocumento.BloqueioImediato == 2) ?
                                                              EnumTipoBloqueioImediato.Nao :
                                                              (EnumTipoBloqueioImediato)tipoDocumento.BloqueioImediato,
                                          QtdeAlertas = tipoDocumento.qtdeAlertas
                                      };

            return motoristaDocumentos.Distinct().ToList();
        }
    }
}
