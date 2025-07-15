using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;
using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.Model.Filtro;
using Raizen.Framework.Models;
using System.Data.Entity;

namespace Raizen.UniCad.BLL
{
    public class LogSincronizacaoBusiness : UniCadBusinessBase<SincronizacaoMotoristas>
    {
        public string Sincronizar()
        {
            var listMotoritas = ListarAgendamentoTerminal();
            string retorno = string.Empty;
            if (listMotoritas != null && listMotoritas.Any())
            {              
                foreach (var mot in listMotoritas)
                {
                    //TODO: Verificar como será realizado este JOB no lado da Argentina, vistos que os campos disponiveis no SAP hoje são todos referentes ao Brasil
                    if (mot.MotoristaBrasil != null)
                    {
                        var sincronizacao = new SincronizacaoMotoristas();
                        retorno = IntegrarSAP(mot);

                        sincronizacao.IsOk = string.IsNullOrEmpty(retorno);
                        sincronizacao.Mensagem = string.IsNullOrEmpty(retorno) ? "OK": retorno;
                        sincronizacao.IDMotorista = mot.ID;
                        sincronizacao.Data = DateTime.Now;                    
                        new LogSincronizacaoBusiness().Adicionar(sincronizacao);
                    }
                }
            }

            if (string.IsNullOrEmpty(retorno))
                return "1";
            else
                return "Nem todos os motoristas foram sincronizados, verifique o Log de Sincronização para mais detalhes";
        }

        private string IntegrarSAP(Motorista motorista)
        {
            var tipoIntegracao = EnumTipoIntegracaoSAP.Inclusao;
            var transp = new TransportadoraBusiness().Selecionar(p => p.ID == motorista.IDTransportadora);
            if (transp != null)
            {
                motorista.NomeTransportadora = transp.RazaoSocial;
            }

            WsIntegraSAP integraSAP = new WsIntegraSAP();
            string retorno = integraSAP.IntegrarMotorista(motorista, tipoIntegracao);
            return retorno;
        }

        public List<Motorista> ListarAgendamentoTerminal()
        {
            using (UniCadDalRepositorio<SincronizacaoMotoristas> repositorio = new UniCadDalRepositorio<SincronizacaoMotoristas>())
            {
                return
                    (from motorista in repositorio.ListComplex<Motorista>().AsNoTracking()
                     join motoristaBrasil in repositorio.ListComplex<MotoristaBrasil>().AsNoTracking() on motorista.ID equals motoristaBrasil.IDMotorista into motoBR
                     from subMotoristaBrasil in motoBR.DefaultIfEmpty()
                     join sincronizacaoMotorista in repositorio.ListComplex<SincronizacaoMotoristas>().AsNoTracking() on motorista.ID equals sincronizacaoMotorista.IDMotorista into m
                     from mot in m.DefaultIfEmpty()
                     where mot.Mensagem != "OK"
                      && (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado)
                      && (motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                     select new
                     {
                         aMotorista = motorista,
                         aMotoristaBrasil = subMotoristaBrasil
                     }
                    )
                    .AsEnumerable()
                    .Select(m =>
                        new Motorista()
                        {
                            ID = m.aMotorista.ID,
                            IDEmpresa = m.aMotorista.IDEmpresa,
                            IDStatus = m.aMotorista.IDStatus,
                            IDMotorista = m.aMotorista.IDMotorista,
                            IDTransportadora = m.aMotorista.IDTransportadora,
                            Nome = m.aMotorista.Nome,
                            Operacao = m.aMotorista.Operacao,
                            Telefone = m.aMotorista.Telefone,
                            Email = m.aMotorista.Email,
                            Anexo = m.aMotorista.Anexo,
                            CodigoEasyQuery = m.aMotorista.CodigoEasyQuery,
                            CodigoSalesForce = m.aMotorista.CodigoSalesForce,
                            DataAtualizazao = m.aMotorista.DataAtualizazao,
                            Ativo = m.aMotorista.Ativo,
                            Observacao = m.aMotorista.Observacao,
                            LoginUsuario = m.aMotorista.LoginUsuario,
                            Justificativa = m.aMotorista.Justificativa,
                            PIS = m.aMotorista.PIS,
                            UsuarioAlterouStatus = m.aMotorista.UsuarioAlterouStatus,
                            IdPais = m.aMotorista.IdPais,
                            MotoristaBrasil = m.aMotoristaBrasil
                        }
                    )
                    .ToList();
            }

        }

        public int ListarCount(SincronizacaoMotoristasFiltro filtro)
        {
            using (UniCadDalRepositorio<SincronizacaoMotoristas> repositorio = new UniCadDalRepositorio<SincronizacaoMotoristas>())
            {
                IQueryable<SincronizacaoMotoritasView> query = GetQuery(filtro, repositorio);
                return query.Count();
            }
        }

        public List<SincronizacaoMotoritasView> Listar(SincronizacaoMotoristasFiltro filtro, PaginadorModel paginador)
        {
            using (UniCadDalRepositorio<SincronizacaoMotoristas> repositorio = new UniCadDalRepositorio<SincronizacaoMotoristas>())
            {
                IQueryable<SincronizacaoMotoritasView> query = GetQuery(filtro, repositorio)
                    .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                    .OrderBy(i => i.ID)
                    .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }
        }
        private IQueryable<SincronizacaoMotoritasView> GetQuery(SincronizacaoMotoristasFiltro filtro, UniCadDalRepositorio<SincronizacaoMotoristas> repositorio)
        {
            IQueryable<SincronizacaoMotoritasView> query =
                (from app in repositorio.ListComplex<SincronizacaoMotoristas>().AsNoTracking().OrderBy(i => i.ID)
                 join moto in repositorio.ListComplex<Motorista>().AsNoTracking() on app.IDMotorista equals moto.ID
                 where (app.Mensagem.Contains(string.IsNullOrEmpty(filtro.Mensagem) ? app.Mensagem : filtro.Mensagem))
                 && (moto.MotoristaBrasil.RG.Contains(string.IsNullOrEmpty(filtro.RG) ? moto.MotoristaBrasil.RG : filtro.RG))
                 && (moto.MotoristaBrasil.CPF.Contains(string.IsNullOrEmpty(filtro.CPF) ? moto.MotoristaBrasil.CPF : filtro.CPF))
                 && (moto.MotoristaBrasil.CNH.Contains(string.IsNullOrEmpty(filtro.CNH) ? moto.MotoristaBrasil.CNH : filtro.CNH))
                 && (app.IsOk == filtro.IsOk || !filtro.IsOk.HasValue)
                 && (DbFunctions.TruncateTime(app.Data) >= filtro.DataInicio || !filtro.DataInicio.HasValue)
                 && (DbFunctions.TruncateTime(app.Data) <= filtro.DataFim || !filtro.DataFim.HasValue)
                 select new SincronizacaoMotoritasView
                 {
                     ID = app.ID,
                     RG = moto.MotoristaBrasil.RG,
                     CNH = moto.MotoristaBrasil.CNH,
                     CPF = moto.MotoristaBrasil.CPF,
                     IsOk = app.IsOk,
                     Mensagem = app.Mensagem,
                     Motorista = moto.Nome,
                     Operacao = moto.Operacao,
                     Data = app.Data
                 }
            );
            return query;
        }
    }
}

