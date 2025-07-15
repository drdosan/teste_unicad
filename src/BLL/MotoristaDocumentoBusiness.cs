using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.BLL.Log;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.DAL.Repositories;
using Raizen.UniCad.BLL.Interfaces;
using System.Resources;
using System.Reflection;
using Infraestructure.Utils;

namespace Raizen.UniCad.BLL
{
    public class MotoristaDocumentoBusiness : UniCadBusinessBase<MotoristaDocumento>
    {
        #region Variáveis privadas

        private Action<string, string> _logar;
        private readonly IMotoristaDocumentoRepository _motoristaDocumentoRepository;
        private readonly IMotoristaBusiness _motoristaBusiness;
        private readonly IMotoristaBusiness _motoristaArgentinaBusiness;
        private readonly IConfigBusiness _configBusiness;
        private readonly EnumPais _pais;

        public MotoristaDocumentoBusiness()
        {
            _motoristaDocumentoRepository = new MotoristaDocumentoRepository(Config.GetContext());
            _motoristaBusiness = new MotoristaBusiness();
            _motoristaArgentinaBusiness = new MotoristaBusiness(EnumPais.Argentina);
            _configBusiness = new ConfigBusiness();
            _pais = EnumPais.Brasil;
        }

        public MotoristaDocumentoBusiness(EnumPais pais)
        {
            _motoristaDocumentoRepository = new MotoristaDocumentoRepository(Config.GetContext());
            _motoristaBusiness = new MotoristaBusiness();
            _motoristaArgentinaBusiness = new MotoristaBusiness(EnumPais.Argentina);
            _configBusiness = new ConfigBusiness();
            _pais = pais;
        }

        public MotoristaDocumentoBusiness(IMotoristaDocumentoRepository motoristaDocumentoRepository,
                                          IMotoristaBusiness motoristaBusiness,
                                          IMotoristaBusiness motoristaArgentinaBusiness,
                                          IConfigBusiness configBusiness,
                                          EnumPais pais)
        {
            _motoristaDocumentoRepository = motoristaDocumentoRepository;
            _motoristaBusiness = motoristaBusiness;
            _motoristaArgentinaBusiness = motoristaArgentinaBusiness;
            _configBusiness = configBusiness;
            _pais = pais;
        }

        #endregion

        /// <summary>
        /// Definir um logger, que ser� utilizado para registrar passo a passo de execu��o de Job.
        /// </summary>
        /// <param name="logar">Action de Log</param>
        public void DefinirLogger(ILogExecucao logar)
        {
            _logar = new Action<string, string>((titulo, descricao) =>
            {
                logar.Log(titulo, descricao, CodigoExecucao.Descricao);
            });
        }

        public List<MotoristaDocumentoView> ListarDocumentos(int idMotorista)
        {
            return ListarDocumentos(idMotorista, EnumPais.Brasil);
        }
        public List<MotoristaDocumentoView> ListarDocumentos(int idMotorista, EnumPais pais)
        {
            using (UniCadDalRepositorio<MotoristaDocumento> repositorio = new UniCadDalRepositorio<MotoristaDocumento>())
            {
                var motoristaDocumentos = from motoristaDocumento in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking()
                                          join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on motoristaDocumento.IDTipoDocumento equals tipoDocumento.ID
                                          join motoristas in repositorio.ListComplex<MotoristaPesquisa>().AsNoTracking() on motoristaDocumento.IDMotorista equals motoristas.ID
                                          where (motoristaDocumento.IDMotorista == idMotorista)
                                          select new MotoristaDocumentoView
                                          {
                                              ID = motoristaDocumento.ID,
                                              IDMotorista = motoristaDocumento.IDMotorista,
                                              IDTipoDocumento = tipoDocumento.ID,
                                              Descricao = tipoDocumento.Descricao,
                                              Sigla = tipoDocumento.Sigla,
                                              Anexo = motoristaDocumento.Anexo,
                                              CPF = motoristas.IdPais == EnumPais.Brasil ? motoristas.CPF : motoristas.DNI,
                                              Obrigatorio = tipoDocumento.Obrigatorio,
                                              DataVencimento = motoristaDocumento.DataVencimento,
                                              Pendente = (motoristaDocumento.Bloqueado || motoristaDocumento.Vencido),
                                              Alerta1Enviado = motoristaDocumento.Alerta1Enviado,
                                              Alerta2Enviado = motoristaDocumento.Alerta2Enviado,
                                              Vencido = motoristaDocumento.Vencido,
                                              Bloqueado = motoristaDocumento.Bloqueado,
                                              DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento,
                                              Processado = motoristaDocumento.Processado,
                                              UsuarioAlterouStatus = motoristaDocumento.UsuarioAlterouStatus
                                          };
                return motoristaDocumentos.ToList();
            }
        }

        private void EnviarEmailDocumentosBloqueados(IEnumerable<MotoristaDocumentoView> documento, string ibm, string email, string razaosocial, int idPais)
        {
            _logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

            var assunto = Config.GetConfig(EnumConfig.TituloEmailBloqueioAutomatico, idPais);
            var corpo = Config.GetConfig(EnumConfig.CorpoEmailBloqueioAutomatico, idPais);

            var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
            var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
            var nomeText = resourceManager.GetString(EnumResource.CorpoNomeEmailBloqueioReprovaAutomaticaMotorista.ToString(), cultureInfo);
            var vencidoText = resourceManager.GetString(EnumResource.CorpoVencidoEmailBloqueioReprovaAutomaticaMotorista.ToString(), cultureInfo);
            var motoristaSingularText = resourceManager.GetString(EnumResource.MotoristaSingular.ToString(), cultureInfo);
            var motoristaPluralText = resourceManager.GetString(EnumResource.MotoristaPlural.ToString(), cultureInfo);

            var corpoEmail = new StringBuilder();
            var motoristaDocumentos = new StringBuilder();

            var motoristaDocumentoViews = documento as MotoristaDocumentoView[] ?? documento.ToArray();
            foreach (var motorista in motoristaDocumentoViews.Where(x => x.EmailEnviar == email).Select(x => new { x.IDMotorista, x.Nome, x.Email }).Distinct())
            {
                motoristaDocumentos.AppendFormat(String.Format(nomeText, motorista.Nome));
                foreach (var doc in motoristaDocumentoViews.Where(w => w.IDMotorista == motorista.IDMotorista && w.Email == motorista.Email).Select(x => x.Documento))
                {
                    motoristaDocumentos.AppendFormat(String.Format(vencidoText, doc));
                }
                motoristaDocumentos.Append("<br/>");
            }

            corpoEmail.AppendFormat(corpo, motoristaPluralText, motoristaDocumentos, motoristaSingularText);

            _logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
            Email.Enviar(email, assunto, corpoEmail.ToString(), null, true, _logar);

            //Se chegou até aqui significa que todos os documentos tiveram seu email enviado
            documento.ToList().ForEach(doc => doc.Enviado = true);
        }

        private void EnviarEmailDocumentosReprovados(IEnumerable<MotoristaDocumentoView> documento, string ibm, string email, string razaosocial, int idPais)
        {
            _logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

            var assunto = Config.GetConfig(EnumConfig.TituloEmailReprovaAutomatica, idPais);
            var corpo = Config.GetConfig(EnumConfig.CorpoEmailReprovaAutomatica, idPais);

            var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
            var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
            var nomeText = resourceManager.GetString(EnumResource.CorpoNomeEmailBloqueioReprovaAutomaticaMotorista.ToString(), cultureInfo);
            var vencidoText = resourceManager.GetString(EnumResource.CorpoVencidoEmailBloqueioReprovaAutomaticaMotorista.ToString(), cultureInfo);
            var motoristaSingularText = resourceManager.GetString(EnumResource.MotoristaSingular.ToString(), cultureInfo);
            var motoristaPluralText = resourceManager.GetString(EnumResource.MotoristaPlural.ToString(), cultureInfo);

            var corpoEmail = new StringBuilder();
            var motoristaDocumentos = new StringBuilder();

            var motoristaDocumentoViews = documento as MotoristaDocumentoView[] ?? documento.ToArray();
            foreach (var motorista in motoristaDocumentoViews.Where(x => x.EmailEnviar == email).Select(x => new { x.IDMotorista, x.Nome, x.Email }).Distinct())
            {
                motoristaDocumentos.AppendFormat(String.Format(nomeText, motorista.Nome));
                foreach (var doc in motoristaDocumentoViews.Where(w => w.IDMotorista == motorista.IDMotorista && w.Email == motorista.Email).Select(x => x.Documento))
                {
                    motoristaDocumentos.AppendFormat(String.Format(vencidoText, doc));
                }
                motoristaDocumentos.Append("<br/>");
            }

            corpoEmail.AppendFormat(corpo, motoristaPluralText, motoristaDocumentos, motoristaSingularText);

            _logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
            Email.Enviar(email, assunto, corpoEmail.ToString(), null, true, _logar);
            
            //Se chegou até aqui significa que todos os documentos tiveram seu email enviado
            documento.ToList().ForEach(doc => doc.Enviado = true);
        }

        private void EnviarEmailDeDocumentosVencidos(IEnumerable<MotoristaDocumentoView> documento, string email, int idPais)
        {
            _logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

            var assunto = Config.GetConfig(EnumConfig.TituloAlertaDocumentoMotorista, idPais);
            var corpoEmail = Config.GetConfig(EnumConfig.CorpoAlertaDocumentoMotorista, idPais);

            var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
            var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
            var motoristaText = resourceManager.GetString(EnumResource.CorpoMotoristaAlertaDocumentoMotorista.ToString(), cultureInfo);
            var documentoText = resourceManager.GetString(EnumResource.CorpoDocumentoAlertaDocumentoMotorista.ToString(), cultureInfo);
            var diasVencimentoText = resourceManager.GetString(EnumResource.CorpoDiasAcaoAlertaDocumentoMotorista.ToString(), cultureInfo);

            var corpoEmailDocs = new StringBuilder();
            var motoristaDocumentoViews = documento as MotoristaDocumentoView[] ?? documento.ToArray();
            foreach (var motorista in motoristaDocumentoViews.Select(x => new { x.IDMotorista, x.Nome, x.DocumentoIdentificacao }).OrderBy(x => x.Nome).Distinct())
            {
                corpoEmailDocs.AppendLine(String.Format(motoristaText, motorista.Nome, motorista.DocumentoIdentificacao));
                foreach (var doc in motoristaDocumentoViews.Where(w => w.IDMotorista == motorista.IDMotorista).Select(x => new { x.Documento, x.DiasVencimento }).Distinct())
                {
                    corpoEmailDocs.AppendLine(String.Format(documentoText, doc.Documento));
                    if (doc.DiasVencimento > 0)
                        corpoEmailDocs.AppendLine(String.Format(diasVencimentoText, doc.DiasVencimento));
                }
                corpoEmailDocs.AppendLine("<br/>");
            }

            _logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
            var body = String.Format(corpoEmail, corpoEmailDocs);
            Email.Enviar(email, assunto, body, null, true, _logar);

            //Se chegou até aqui significa que todos os documentos tiveram seu email enviado
            documento.ToList().ForEach(doc => doc.Enviado = true);
        }

        private void EnviarEmailAlerta(IEnumerable<MotoristaDocumentoView> documento, string email, int idPais, DateTime? dataExecucao = null)
        {
            _logar?.Invoke("Escrevendo e-mail", "Escrevendo e-mail");

            string assunto = Config.GetConfig(EnumConfig.TituloMotoristaAlertaVencimento, idPais);

            var resourceManager = new ResourceManager("Raizen.Unicad.BLL.Resources.Resource", Assembly.GetExecutingAssembly());
            var cultureInfo = CultureUtil.CultureInfoPorPais(idPais);
            var nomeDocumentoText = resourceManager.GetString(EnumResource.CorpoNomeDocumentoMotoristaAlertaVencimento.ToString(), cultureInfo);
            var diasVencimentoText = resourceManager.GetString(EnumResource.CorpoDiasVencimentoMotoristaAlertaVencimento.ToString(), cultureInfo);

            StringBuilder corpoEmail = new StringBuilder();
            StringBuilder motoristasEmail = new StringBuilder();
            int diasVencimento;

            var motoristaDocumentoViews = documento as MotoristaDocumentoView[] ?? documento.ToArray();
            foreach (var motorista in motoristaDocumentoViews.Select(x => new { x.IDMotorista, x.Nome, x.DocumentoIdentificacao }).OrderBy(x => x.Nome).Distinct())
            {
                motoristasEmail.AppendLine(String.Format(nomeDocumentoText, motorista.Nome, motorista.DocumentoIdentificacao));

                foreach (var doc in motoristaDocumentoViews.Where(w => w.IDMotorista == motorista.IDMotorista).Select(x => new { x.Documento, x.DiasVencimento, x.DiasVencimentoA2, x.DataVencimento }).OrderBy(o => o.DiasVencimento).Distinct())
                {
                    diasVencimento = (dataExecucao != null && doc.DiasVencimentoA2 > 0 && doc.DataVencimento == dataExecucao.Value.AddDays(doc.DiasVencimentoA2))
                                        ? doc.DiasVencimentoA2 : doc.DiasVencimento;

                    motoristasEmail.AppendLine(String.Format(diasVencimentoText, doc.Documento, diasVencimento));
                }
            }

            corpoEmail.AppendFormat(Config.GetConfig(EnumConfig.CorpoMotoristaAlertaVencimento, idPais), motoristasEmail);

            _logar?.Invoke("Enviando e-mail", "Chamada de Raizen.UniCad.BLL.Email.Enviar");
            Email.Enviar(email, assunto, corpoEmail.ToString(), null, true, _logar);

            //Se chegou até aqui significa que todos os documentos tiveram seu email enviado
            documento.ToList().ForEach(doc => doc.Enviado = true);
        }

        public bool VerificarDocumentosVencidosPorMotorista(int idMotorista)
        {
            var documentos = ListarMotoristaDocumentoPorMotorista(idMotorista);

            foreach(var documento in documentos)
            {
				if (documento.Vencido || (documento.DataVencimento.HasValue && documento.DataVencimento.Value < DateTime.Today))
				{
					return true;
				}
			}

            return false;
        }

        public List<MotoristaDocumentoView> ListarMotoristaDocumentoPorMotorista(int idMotorista)
        {
            return ListarMotoristaDocumentoPorMotorista(idMotorista, null, null, null);
        }
        public List<MotoristaDocumentoView> ListarMotoristaDocumentoPorMotorista(int idMotorista, string operacao)
        {
            return ListarMotoristaDocumentoPorMotorista(idMotorista, operacao, null, null);
        }
        public List<MotoristaDocumentoView> ListarMotoristaDocumentoPorMotorista(int idMotorista, string operacao, List<int> tipoProdutoList)
        {
            return ListarMotoristaDocumentoPorMotorista(idMotorista, operacao, tipoProdutoList, null);
        }

        public List<MotoristaDocumentoView> ListarMotoristaDocumentoPorMotorista(int idMotorista, string operacao, List<int> tipoProdutoList, List<int> tipoComposicaoList)
        {
            using (UniCadDalRepositorio<MotoristaDocumento> repositorio = new UniCadDalRepositorio<MotoristaDocumento>())
            {
                var query = GetQuery(repositorio, idMotorista, operacao, tipoProdutoList, tipoComposicaoList);

                return query.ToList();
            }
        }

        public List<MotoristaDocumentoServicoView> ListarMotoristaDocumentoPorMotoristaServico(int idMotorista)
        {
            using (var repositorio = new UniCadDalRepositorio<MotoristaDocumento>())
            {
                var query = GetQueryServico(repositorio, idMotorista);

                return query.ToList();
            }
        }
        public List<MotoristaDocumentoView> ListarMotoristaDocumento(int? idEmpresa, string operacao)
        {
            return ListarMotoristaDocumento(idEmpresa, operacao, null, null);
        }
        public List<MotoristaDocumentoView> ListarMotoristaDocumento(int? idEmpresa, string operacao, List<int> tipoProdutoList)
        {
            return ListarMotoristaDocumento(idEmpresa, operacao, tipoProdutoList, null);
        }
        public List<MotoristaDocumentoView> ListarMotoristaDocumento(int? idEmpresa, string operacao, List<int> tipoProdutoList, List<int> tipoComposicaoList)
        {
            using (var repositorio = new UniCadDalRepositorio<MotoristaDocumento>())
            {
                var query = GetQueryDocumentos(repositorio, idEmpresa, operacao, tipoProdutoList, tipoComposicaoList, (int)_pais);

                return query.ToList();
            }
        }

        private IQueryable<MotoristaDocumentoView> GetQueryDocumentos(UniCadDalRepositorio<MotoristaDocumento> repositorio, int? idEmpresa, string operacao, List<int> tipoProdutoList = null, List<int> tipoComposicaoList = null, int? IdPais = null)
        {
            IQueryable<MotoristaDocumentoView> motoristaDocumentos;

            if (tipoComposicaoList != null && tipoProdutoList != null)
                motoristaDocumentos = from tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking()
                                      join tipoDocumentoTipoComposicao in repositorio.ListComplex<TipoDocumentoTipoComposicao>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoComposicao.IDTipoDocumento
                                      join tipoDocumentoTipoProduto in repositorio.ListComplex<TipoDocumentoTipoProduto>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoProduto.IDTipoDocumento
                                      where (tipoDocumento.tipoCadastro == (int)EnumTipoCadastroDocumento.Motorista)
                                      && ((!idEmpresa.HasValue) || tipoDocumento.IDEmpresa == idEmpresa || idEmpresa == (int)EnumEmpresa.Ambos || tipoDocumento.IDEmpresa == (int)EnumEmpresa.Ambos)
                                      && (string.IsNullOrEmpty(operacao) || tipoDocumento.Operacao == operacao || (operacao == "Ambos") || tipoDocumento.Operacao == "Ambos")
                                      && (tipoDocumento.Status)
                                      && tipoComposicaoList.Contains(tipoDocumentoTipoComposicao.IDTipoComposicao)
                                      && tipoProdutoList.Contains(tipoDocumentoTipoProduto.IDTipoProduto)
                                      && (IdPais == null || (IdPais != null && (tipoDocumento.IDPais == IdPais)))
                                      select new MotoristaDocumentoView
                                      {
                                          Descricao = tipoDocumento.Descricao,
                                          Sigla = tipoDocumento.Sigla,
                                          IDTipoDocumento = tipoDocumento.ID,
                                          Obrigatorio = tipoDocumento.Obrigatorio,
                                          DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento
                                      };
            else
                motoristaDocumentos = from tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking()
                                      where (tipoDocumento.tipoCadastro == (int)EnumTipoCadastroDocumento.Motorista)
                                      && ((!idEmpresa.HasValue) || tipoDocumento.IDEmpresa == idEmpresa || idEmpresa == (int)EnumEmpresa.Ambos || tipoDocumento.IDEmpresa == (int)EnumEmpresa.Ambos)
                                      && (string.IsNullOrEmpty(operacao) || tipoDocumento.Operacao == operacao || (operacao == "Ambos") || tipoDocumento.Operacao == "Ambos")
                                      && (tipoDocumento.Status)
                                      && (IdPais == null || (IdPais != null && (tipoDocumento.IDPais == IdPais)))
                                      select new MotoristaDocumentoView
                                      {
                                          Descricao = tipoDocumento.Descricao,
                                          Sigla = tipoDocumento.Sigla,
                                          IDTipoDocumento = tipoDocumento.ID,
                                          Obrigatorio = tipoDocumento.Obrigatorio,
                                          DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento
                                      };

            return motoristaDocumentos;
        }

        private IQueryable<MotoristaDocumentoServicoView> GetQueryServico(UniCadDalRepositorio<MotoristaDocumento> repositorio, int idMotorista)
        {
            var motoristaDocumentos = from motoristaDocumento in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking()
                                      join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on motoristaDocumento.IDTipoDocumento equals tipoDocumento.ID
                                      join motoristas in repositorio.ListComplex<Motorista>().AsNoTracking() on motoristaDocumento.IDMotorista equals motoristas.ID
                                      where (motoristaDocumento.IDMotorista == idMotorista)
                                      select new MotoristaDocumentoServicoView
                                      {
                                          Descricao = tipoDocumento.Descricao,
                                          Sigla = tipoDocumento.Sigla,
                                          DataVencimento = motoristaDocumento.DataVencimento
                                      };
            return motoristaDocumentos;
        }

        private IQueryable<MotoristaDocumentoView> GetQuery(UniCadDalRepositorio<MotoristaDocumento> repositorio, int idMotorista, string operacao, List<int> tipoProdutoList = null, List<int> tipoComposicaoList = null, int? IdPais = null)
        {
            IQueryable<MotoristaDocumentoView> motoristaDocumentos;

            if (tipoComposicaoList != null && tipoProdutoList != null)
                motoristaDocumentos = from motoristaDocumento in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking()
                                      join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on motoristaDocumento.IDTipoDocumento equals tipoDocumento.ID
                                      join tipoDocumentoTipoComposicao in repositorio.ListComplex<TipoDocumentoTipoComposicao>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoComposicao.IDTipoDocumento
                                      join tipoDocumentoTipoProduto in repositorio.ListComplex<TipoDocumentoTipoProduto>().AsNoTracking() on tipoDocumento.ID equals tipoDocumentoTipoProduto.IDTipoProduto
                                      join motoristas in repositorio.ListComplex<Motorista>().AsNoTracking() on motoristaDocumento.IDMotorista equals motoristas.ID
                                      join motoristasBrasil in repositorio.ListComplex<MotoristaBrasil>().AsNoTracking() on motoristas.ID equals motoristasBrasil.IDMotorista into motoBR
                                      from subMotoristaBrasil in motoBR.DefaultIfEmpty()
                                      where (motoristaDocumento.IDMotorista == idMotorista)
                                      && (string.IsNullOrEmpty(operacao) || motoristas.Operacao == operacao || (operacao == "Ambos") || motoristas.Operacao == "Ambos")
                                      && tipoComposicaoList.Contains(tipoDocumentoTipoComposicao.IDTipoComposicao)
                                      && tipoProdutoList.Contains(tipoDocumentoTipoProduto.IDTipoProduto)
                                      && (IdPais == null || (IdPais != null && (tipoDocumento.IDPais == IdPais)))
                                      select new MotoristaDocumentoView
                                      {
                                          ID = motoristaDocumento.ID,
                                          IDMotorista = motoristaDocumento.IDMotorista,
                                          IDTipoDocumento = tipoDocumento.ID,
                                          Descricao = tipoDocumento.Descricao,
                                          Sigla = tipoDocumento.Sigla,
                                          Anexo = motoristaDocumento.Anexo,
                                          CPF = (subMotoristaBrasil != null ? subMotoristaBrasil.CPF : null),
                                          Obrigatorio = tipoDocumento.Obrigatorio,
                                          DataVencimento = motoristaDocumento.DataVencimento,
                                          Pendente = (motoristaDocumento.Bloqueado || motoristaDocumento.Vencido),
                                          DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento

                                      };
            else
                motoristaDocumentos = from motoristaDocumento in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking()
                                      join tipoDocumento in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on motoristaDocumento.IDTipoDocumento equals tipoDocumento.ID
                                      join motoristas in repositorio.ListComplex<Motorista>().AsNoTracking() on motoristaDocumento.IDMotorista equals motoristas.ID
                                      join motoristasBrasil in repositorio.ListComplex<MotoristaBrasil>().AsNoTracking() on motoristas.ID equals motoristasBrasil.IDMotorista into motoBR
                                      from subMotoristaBrasil in motoBR.DefaultIfEmpty()
                                      where (motoristaDocumento.IDMotorista == idMotorista)
                                      && (string.IsNullOrEmpty(operacao) || motoristas.Operacao == operacao || (operacao == "Ambos") || motoristas.Operacao == "Ambos")
                                      && (IdPais == null || (IdPais != null && (tipoDocumento.IDPais == IdPais)))
                                      select new MotoristaDocumentoView
                                      {
                                          ID = motoristaDocumento.ID,
                                          IDMotorista = motoristaDocumento.IDMotorista,
                                          IDTipoDocumento = tipoDocumento.ID,
                                          Descricao = tipoDocumento.Descricao,
                                          Sigla = tipoDocumento.Sigla,
                                          Anexo = motoristaDocumento.Anexo,
                                          CPF = (subMotoristaBrasil != null ? subMotoristaBrasil.CPF : null),
                                          Obrigatorio = tipoDocumento.Obrigatorio,
                                          DataVencimento = motoristaDocumento.DataVencimento,
                                          Pendente = (motoristaDocumento.Bloqueado || motoristaDocumento.Vencido),
                                          DocumentoPossuiVencimento = tipoDocumento.DocumentoPossuiVencimento
                                      };

            return motoristaDocumentos;
        }

        public void AtualizarDocumentos(List<MotoristaDocumentoView> documentos)
        {
            foreach (var documento in documentos)
            {
                var md = new MotoristaDocumento
                {
                    ID = documento.ID,
                    Anexo = documento.Anexo,
                    DataVencimento = documento.DataVencimento,
                    IDMotorista = documento.IDMotorista,
                    IDTipoDocumento = documento.IDTipoDocumento
                };
                Atualizar(md);
            }
        }

        public int ProcessarAlertaDocumentosMotorista(DateTime dataExecucao)
        {
            int erros = 0;
            var listaDocumentos = _motoristaDocumentoRepository.GetDocumentosAVencer(dataExecucao);

            _logar?.Invoke(string.Format("ProcessarAlertaDocumentosMotorista"),
               string.Format("Método recebe dados em GetDocumentosAVencer - count: {0}", listaDocumentos.Count()));

            foreach (var documentoEmail in listaDocumentos.Where(w => !String.IsNullOrEmpty(w.EmailEnviar) && w.QtdeAlertas > 0).Select(s => new { s.EmailEnviar, s.IdPais }).Distinct())
            {
                try
                {
                    _logar?.Invoke("Iterando lista Emails", string.Format("EMAIL:{0}, PAÍS:{1}", documentoEmail.EmailEnviar, documentoEmail.IdPais));

                    var documentos = listaDocumentos.Where(w => w.EmailEnviar == documentoEmail.EmailEnviar && w.IdPais == documentoEmail.IdPais);

                    _logar?.Invoke("Enviar e-mail de alerta", string.Format("EMAIL:{0} - PAÍS {1} - QTDE DOCS: {2}",
                                documentoEmail.EmailEnviar, documentoEmail.IdPais, documentos.Count()));

                    EnviarEmailAlerta(documentos, documentoEmail.EmailEnviar, (int)documentoEmail.IdPais, dataExecucao);

                    foreach (var documento in documentos)
                    {
                        var motorista = _motoristaDocumentoRepository.Selecionar(documento.ID);
                        _logar?.Invoke(string.Format("ID {0}", documento.ID), "Motorista selecionado.");

                        if (documento.Alerta1Enviado == false && documento.DiasVencimento > 0 && documento.DataVencimento == dataExecucao.AddDays(documento.DiasVencimento))
                        {
                            motorista.Alerta1Enviado = true;
                        }
                        else if (documento.Alerta2Enviado == false && documento.DiasVencimentoA2 > 0 && documento.DataVencimento == dataExecucao.AddDays(documento.DiasVencimentoA2))
                        {
                            motorista.Alerta2Enviado = true;
                        }

                        _motoristaDocumentoRepository.Atualizar(motorista);
                        _logar?.Invoke(string.Format("ID {0}", documento.ID), string.Format("Motorista marcado. Alerta1: {0} - Alerta2: {1}", motorista.Alerta1Enviado, motorista.Alerta2Enviado));
                    }
                }
                catch
                {
                    erros++;
                }
            }

            return erros;
        }

        public int ProcessarDocumentosVencidos(DateTime dtInicioExecucao)
        {
            _logar?.Invoke("MOTORISTA DOCUMENTOS - INICIO", "-----");

            _logar?.Invoke(
                "Consulta - Documentos vencidos/bloqueados",
                $"Listando documentos vencidos/bloqueados, data de busca: {dtInicioExecucao}");

            var listaDocumentosBloqueadosVencidos = _motoristaDocumentoRepository.GetDocumentosBloqueados(dtInicioExecucao);

            _logar?.Invoke(
                "Consulta - Documentos vencidos/bloqueados",
                $"Documentos vencidos/bloqueados consultados, count: {listaDocumentosBloqueadosVencidos.Count()}");

            if (listaDocumentosBloqueadosVencidos.Any())
            {
                listaDocumentosBloqueadosVencidos.ForEach(tipoDocumento =>
                {
                    tipoDocumento.TipoAlerta = 2;

                    if (tipoDocumento.TipoBloqueioImediato == EnumTipoBloqueioImediato.Sim)
                    {
                        tipoDocumento.TipoAlerta = 1;
                    }
                    else if (tipoDocumento.TipoBloqueioImediato == EnumTipoBloqueioImediato.Nao &&
                             tipoDocumento.TipoAcaoVencimento != EnumTipoAcaoVencimento.SemAcao &&
                             tipoDocumento.DataVencimento.Value.AddDays(tipoDocumento.QuantidadeDiasBloqueio.Value) <= dtInicioExecucao)
                    {
                        tipoDocumento.TipoAlerta = 1;
                    }
                });

                // ENVIAR OS E-MAILS AVISANDO QUE O DOCUMENTO SERÁ BLOQUEADO/REPROVADO.
                _logar?.Invoke("Processar documentos bloqueados/vencidos NV1 - Chamada", "Chamada de EnviarEmailDocumentosBloqueadosVencidos - Nível 1");
                EnviarEmailDocumentosBloqueadosVencidos(listaDocumentosBloqueadosVencidos, 1);

                // ENVIAR OS E-MAILS AVISANDO QUE O DOCUMENTO ESTÁ VENCIDO
                _logar?.Invoke("Processar documentos bloqueados/vencidos NV2 - Chamada", "Chamada de EnviarEmailDocumentosBloqueadosVencidos - Nível 2");
                EnviarEmailDocumentosBloqueadosVencidos(listaDocumentosBloqueadosVencidos, 2);

                // BLOQUEAR E/OU REPROVAR DOCUMENTOS
                _logar?.Invoke("Marcar documentos bloqueados e vencidos(sem ação)", "Após envio de e-mails, marcar documentos de motoristas bloqueados e vencidos(sem ação).");

                MarcarDocumentosBloqueadosReprovados(listaDocumentosBloqueadosVencidos, EnumTipoAcaoVencimento.Bloquear, EnumConfig.habilitarBloqueioDocMotorista, true);

                _logar?.Invoke("Marcar documentos reprovados", "Após envio de e-mails, marcar documentos de motoristas reprovados.");

                MarcarDocumentosBloqueadosReprovados(listaDocumentosBloqueadosVencidos, EnumTipoAcaoVencimento.Reprovar, EnumConfig.habilitarReprovaDocMotoristaAutomatica, false);

                // BLOQUEAR MOTORISTA
                MarcarMotoristaBloqueadosReprovados(
                    listaDocumentosBloqueadosVencidos,
                    EnumConfig.habilitarBloqueioMotorista,
                    EnumTipoAcaoVencimento.Bloquear,
                    EnumTipoIntegracaoSAP.Bloqueio,
                    EnumStatusMotorista.Bloqueado,
                    EnumConfig.JustificativaBloqueioAutomatico);

                // REPROVAR MOTORISTA
                MarcarMotoristaBloqueadosReprovados(
                    listaDocumentosBloqueadosVencidos,
                    EnumConfig.habilitarReprovaDocMotoristaAutomatica,
                    EnumTipoAcaoVencimento.Reprovar,
                    EnumTipoIntegracaoSAP.Nenhum,
                    EnumStatusMotorista.Reprovado,
                    EnumConfig.JustificativaReprovaAutomatica);
            }

            _logar?.Invoke("MOTORISTA DOCUMENTOS - FIM", "-----");

            return 0;
        }

        private void EnviarEmailDocumentosBloqueadosVencidos(IEnumerable<MotoristaDocumentoView> listaDocumentosBloqueadosVencidos, int nivelAlerta)
        {
            _logar?.Invoke(
                $"EnviarEmailDocumentosBloqueadosVencidos - NV {nivelAlerta}",
                $"Método recebe dados em listaDocumentosBloqueadosVencidos - count: {listaDocumentosBloqueadosVencidos.Count()}");

            var listaDocumentoEmail = listaDocumentosBloqueadosVencidos
                .Where(w => !String.IsNullOrEmpty(w.EmailEnviar) && w.TipoAlerta == nivelAlerta && w.QtdeAlertas > 0)
                .Select(s => new { s.EmailEnviar, s.IdPais, s.TipoAcaoVencimento })
                .Distinct()
                .ToList();

            foreach (var documentoEmail in listaDocumentoEmail)
            {
                var documentos = listaDocumentosBloqueadosVencidos
                    .Where(w => w.TipoAlerta == nivelAlerta
                                && w.EmailEnviar == documentoEmail.EmailEnviar
                                && w.IdPais == documentoEmail.IdPais
                                && w.TipoAcaoVencimento == documentoEmail.TipoAcaoVencimento
                                && w.QtdeAlertas > 0)
                    .ToList();

                var msgLog = $"EMAIL:{documentoEmail.EmailEnviar} - QTDE DOCS: {documentos.Count}";

                switch (nivelAlerta)
                {
                    case 2:
                        _logar?.Invoke("Enviar e-mail de alerta Vencido", msgLog);
                        EnviarEmailDeDocumentosVencidos(documentos, documentoEmail.EmailEnviar, (int)documentoEmail.IdPais);
                        break;
                    default:
                        switch (documentoEmail.TipoAcaoVencimento)
                        {
                            case EnumTipoAcaoVencimento.Bloquear:
                                _logar?.Invoke("Enviar e-mail de alerta Bloqueado", msgLog);
                                EnviarEmailDocumentosBloqueados(documentos, null, documentoEmail.EmailEnviar, null, (int)documentoEmail.IdPais);
                                break;
                            case EnumTipoAcaoVencimento.Reprovar:
                                _logar?.Invoke("Enviar e-mail de alerta Reprovação", msgLog);
                                EnviarEmailDocumentosReprovados(documentos, null, documentoEmail.EmailEnviar, null, (int)documentoEmail.IdPais);
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        private void MarcarDocumentosBloqueadosReprovados(List<MotoristaDocumentoView> listaDocumentosBloqueadosVencidos, EnumTipoAcaoVencimento tipoAcaoVencimento, EnumConfig habilitarBloqueio, bool incluirSemAcao)
        {
            const string usuario = "JOB";

            var documentos = listaDocumentosBloqueadosVencidos
                .Where(w => w.TipoAcaoVencimento == tipoAcaoVencimento || (incluirSemAcao && w.TipoAcaoVencimento == EnumTipoAcaoVencimento.SemAcao))
                .Select(x => new { x.ID, x.TipoAlerta, x.TipoAcaoVencimento })
                .Distinct()
                .ToList();

            foreach (var item in documentos)
            {
                var motorista = _motoristaDocumentoRepository.Selecionar(item.ID);

                if (_configBusiness.GetConfigInt(EnumConfig.habilitarVectoMotorista, (int)EnumPais.Padrao) != 0)
                {
                    motorista.Vencido = true;
                }

                if (item.TipoAcaoVencimento == (int)EnumTipoAcaoVencimento.SemAcao)
                {
                    motorista.Processado = true;
                }

                if (_configBusiness.GetConfigInt(habilitarBloqueio, (int)EnumPais.Padrao) != 0 && item.TipoAlerta == 1)
                {
                    motorista.Bloqueado = true;
                    motorista.TipoAcaoVencimento = (int)tipoAcaoVencimento;
                    motorista.UsuarioAlterouStatus = usuario;
                    motorista.Processado = true;
                }

                _motoristaDocumentoRepository.Atualizar(motorista);

                _logar?.Invoke(
                    string.Format("ID {0}", item),
                    $"Motorista marcado. Vencido: {motorista.Vencido} - {tipoAcaoVencimento}: {motorista.Bloqueado}");
            }
        }

        private void MarcarMotoristaBloqueadosReprovados(
            List<MotoristaDocumentoView> listaDocumentosBloqueadosVencidos,
            EnumConfig habilitar,
            EnumTipoAcaoVencimento tipoAcaoVencimento,
            EnumTipoIntegracaoSAP tipoIntegracaoSap,
            EnumStatusMotorista statusMotorista,
            EnumConfig justificativaAutomatica)
        {
            if (_configBusiness.GetConfigInt(habilitar, (int)EnumPais.Padrao) != 0)
            {
                const string usuario = "JOB";

                string textoLog1 = null;
                string textoLog2 = null;
                string textoLog3 = null;
                string textoLog4 = null;

                switch (tipoAcaoVencimento)
                {
                    case EnumTipoAcaoVencimento.Bloquear:
                        textoLog1 = "Bloquear";
                        textoLog2 = "bloqueada";
                        textoLog3 = "Bloqueio";
                        textoLog4 = "Bloqueando";
                        break;
                    case EnumTipoAcaoVencimento.Reprovar:
                        textoLog1 = "Reprovar";
                        textoLog2 = "reprovada";
                        textoLog3 = "Reprovação";
                        textoLog4 = "Reprovando";
                        break;
                    default:
                        break;
                }

                var lista = listaDocumentosBloqueadosVencidos
                    .Where(w => w.TipoAlerta == 1 && w.TipoAcaoVencimento == tipoAcaoVencimento)
                    .Select(x => x.IDMotorista)
                    .Distinct();

                _logar?.Invoke($"{textoLog1} motoristas", $"{textoLog1} motoristas - COUNT:{lista.Count()}");

                foreach (var item in lista)
                {
                    var mot = _motoristaBusiness.Selecionar(item);

                    _logar?.Invoke($"Marcar motoristas {textoLog2}s", $"Após envio de e-mails, marcar motorista {textoLog2}.");

                    if (mot != null)
                    {
                        _logar?.Invoke($"{textoLog3} - Motorista {mot.ID}", $"{textoLog4}...");

                        mot.Documentos = new MotoristaDocumentoBusiness().ListarDocumentos(mot.ID);
                        mot.Clientes = new MotoristaClienteBusiness().ListarClientes(mot.ID);

                        if (tipoIntegracaoSap != EnumTipoIntegracaoSAP.Nenhum)
                        {
                            mot.tipoIntegracao = tipoIntegracaoSap;
                        }

                        mot.IDStatus = (int)statusMotorista;
                        mot.UsuarioAlterouStatus = usuario;
                        mot.Justificativa = _configBusiness.GetConfig(justificativaAutomatica, (int)mot.IdPais);
                        mot.DataAtualizazao = DateTime.Now;

                        (mot.IdPais == EnumPais.Brasil ? _motoristaBusiness : _motoristaArgentinaBusiness)
                            .AtualizarMotorista(mot, false,statusMotorista == EnumStatusMotorista.Bloqueado);

                        _logar?.Invoke($"{textoLog3} - Motorista {mot.ID}", $"{textoLog2}.");

                    }
                }
            }
        }
    }
}
