using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Raizen.Framework.Models;
using Raizen.UniCad.BLL.Log;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Utils;

namespace Raizen.UniCad.BLL
{
    public class JobBusiness : UniCadBusinessBase<Job>
    {
        const string JOB_NOME = "JOB_UNICAD";
        const string JOB_NOME_CLIENTE = "CLIENTE";
        const string JOB_NOME_TRANSPORTADORA = "TRANSPORTADORA";
        const string JOB_NOME_DOCUMENTO = "DOCUMENTO";
        const string JOB_NOME_IMPORTACAO = "IMPORTACAO";
        const string JOB_NOME_PENDENTE = "PENDENTE_SAP";
        const string JOB_NOME_ARQUIVO = "EXCLUIR_ARQUIVOS";

        const string JOB_NOME_SINCRONIZAR_MOTORITAS_SAP = "SINCRONIZAR_MOTORITAS_SAP";
        const string JOB_NOME_ENVIAR_EMAIL_TESTE = "ENVIAR_EMAIL_TESTE";
        const string JOB_NOME_COMPOSICAO_DOCUMENTO_ALERTA = "COMPOSICAO_DOCUMENTO_ALERTA";
        const string JOB_NOME_COMPOSICAO_DOCUMENTO_VENCIDO = "COMPOSICAO_DOCUMENTO_VENCIDO";
        const string JOB_NOME_MOTORISTA_DOCUMENTO_ALERTA = "MOTORISTA_DOCUMENTO_ALERTA";
        const string JOB_NOME_MOTORISTA_DOCUMENTO_VENCIDO = "MOTORISTA_DOCUMENTO_VENCIDO";

        public int ExecutarJob(string jobNome)
        {
            ConfiguracaoBusiness configBLL = new ConfiguracaoBusiness();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
            DateTime dtInicioExecucao = DateTime.Now;
            int erros = 0;

            if (string.IsNullOrWhiteSpace(jobNome))
            {
                erros += ProcessarPendenteAtualizacaoSAP(dtInicioExecucao);
                erros += ProcessarClientes(dtInicioExecucao);
                erros += ProcessarTransportadoras(dtInicioExecucao);
                erros += ExcluirArquivos(dtInicioExecucao);
            }
            else
            {
                MethodInfo method = this.GetType().GetMethod($"Processar{jobNome}");
                object result = method.Invoke(this, new object[] { dtInicioExecucao });
                erros = (int)result;
            }

            return erros;
        }

        public int ProcessarAlertaDocumentosComposicao(DateTime dtInicioExecucao)
        {
            ILogExecucao logger = new LogExecucao(nameof(ProcessarAlertaDocumentosComposicao), true);

            logger.Log("Início", "Buscando Job na tabela de Jobs", CodigoExecucao.Descricao);
            var jobDocumento = this.Selecionar(w => w.Nome == JOB_NOME_COMPOSICAO_DOCUMENTO_ALERTA);

            try
            {
                logger.Log("Verificações iniciais", "Verificando periodicidade, se job está ativo e se está em execução ", CodigoExecucao.Descricao);
                if (dtInicioExecucao > Convert.ToDateTime(jobDocumento.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobDocumento.NrPeriodicidadeMinutos))
                        && jobDocumento.StAtivo
                        && !VerificarJobEmExecucao(jobDocumento))
                {
                    DateTime dtExecucaoJob = Convert.ToDateTime(jobDocumento.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumento.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumento.NrPeriodicidadeMinutos));
                    }

                    logger.Log("Atualizando Job", "Atualizando Job p/ \"Em execução\" e informado data de início.", CodigoExecucao.Descricao);
                    jobDocumento.EmExecucao = true;
                    jobDocumento.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobDocumento);

                    logger.Log("Chamada de Business (Placa)", "Chamada de ProcessarAlertaDocumentosComposicao (PlacaDocumentoBusiness)", CodigoExecucao.Descricao);

                    string habilitarLogDetalhadoJob = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                    var documentoBLL = new PlacaDocumentoBusiness();

                    bool definirLog = false;
                    if (bool.TryParse(habilitarLogDetalhadoJob, out definirLog) && definirLog)
                    {
                        documentoBLL.DefinirLogger(logger);
                    }

                    var retorno = documentoBLL.ProcessarAlertaDocumentosComposicao(dtInicioExecucao.Date);

                    jobDocumento.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("AlertaDocumentosComposicao");
                }
                else
                {
                    JobUtil.GravarLog("AlertaDocumentosComposicao", "AlertaDocumentosComposicao - Ação não efetuada", "O JOB ainda não está no periodo de execução ou já possui uma execução em andamento");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo($"{JOB_NOME}: AlertaDocumentosComposicao", ex);
                return 1;
            }

            finally
            {
                jobDocumento.EmExecucao = false;
                Atualizar(jobDocumento);
            }
        }

        public int ProcessarDocumentosVencidosComposicao(DateTime dtInicioExecucao)
        {
            ILogExecucao logger = new LogExecucao(nameof(ProcessarDocumentosVencidosComposicao), true);

            logger.Log("Início", "Buscando Job na tabela de Jobs", CodigoExecucao.Descricao);
            var jobDocumentoVencido = this.Selecionar(w => w.Nome == JOB_NOME_COMPOSICAO_DOCUMENTO_VENCIDO);

            try
            {
                logger.Log("Verificações iniciais", "Verificando periodicidade, se job está ativo e se está em execução ", CodigoExecucao.Descricao);
                if (dtInicioExecucao > Convert.ToDateTime(jobDocumentoVencido.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobDocumentoVencido.NrPeriodicidadeMinutos))
                        && jobDocumentoVencido.StAtivo
                        && !VerificarJobEmExecucao(jobDocumentoVencido))
                {
                    DateTime dtExecucaoJob = Convert.ToDateTime(jobDocumentoVencido.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumentoVencido.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumentoVencido.NrPeriodicidadeMinutos));
                    }

                    logger.Log("Atualizando Job", "Atualizando Job p/ \"Em execução\" e informado data de início.", CodigoExecucao.Descricao);
                    jobDocumentoVencido.EmExecucao = true;
                    jobDocumentoVencido.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobDocumentoVencido);

                    logger.Log("Chamada de Business (Placa)", "Chamada de ProcessarDocumentosVencidos (PlacaDocumentoBusiness)", CodigoExecucao.Descricao);

                    string habilitarLogDetalhadoJob = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                    var documentoBLL = new PlacaDocumentoBusiness();

                    bool definirLog = false;
                    if (bool.TryParse(habilitarLogDetalhadoJob, out definirLog) && definirLog)
                    {
                        documentoBLL.DefinirLogger(logger);
                    }

                    var retorno = documentoBLL.ProcessarDocumentosVencidos(dtInicioExecucao.Date);

                    jobDocumentoVencido.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("DocumentosVencidosComposicao");
                }
                else
                {
                    JobUtil.GravarLog("DocumentosVencidosComposicao", "DocumentosVencidosComposicao - Ação não efetuada", "O JOB ainda não está no periodo de execução ou já possui uma execução em andamento");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo($"{JOB_NOME}: DocumentosVencidosComposicao", ex);
                return 1;
            }

            finally
            {
                jobDocumentoVencido.EmExecucao = false;
                Atualizar(jobDocumentoVencido);
            }
        }

        public int ProcessarAlertaDocumentosMotorista(DateTime dtInicioExecucao)
        {
            ILogExecucao logger = new LogExecucao(nameof(ProcessarAlertaDocumentosMotorista), true);

            logger.Log("Início", "Buscando Job na tabela de Jobs", CodigoExecucao.Descricao);
            var jobDocumento = this.Selecionar(w => w.Nome == JOB_NOME_MOTORISTA_DOCUMENTO_ALERTA);

            try
            {
                logger.Log("Verificações iniciais", "Verificando periodicidade, se job está ativo e se está em execução ", CodigoExecucao.Descricao);
                if (dtInicioExecucao > Convert.ToDateTime(jobDocumento.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobDocumento.NrPeriodicidadeMinutos))
                        && jobDocumento.StAtivo
                        && !VerificarJobEmExecucao(jobDocumento))
                {
                    DateTime dtExecucaoJob = Convert.ToDateTime(jobDocumento.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumento.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumento.NrPeriodicidadeMinutos));
                    }

                    logger.Log("Atualizando Job", "Atualizando Job p/ \"Em execução\" e informado data de início.", CodigoExecucao.Descricao);
                    jobDocumento.EmExecucao = true;
                    jobDocumento.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobDocumento);

                    logger.Log("Chamada de Business (Motorista)", "Chamada de ProcessarAlertaDocumentosMotorista (MotoristaDocumentoBusiness)", CodigoExecucao.Descricao);

                    string habilitarLogDetalhadoJob = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                    var motoristaDocumentoBLL = new MotoristaDocumentoBusiness();

                    bool definirLog = false;
                    if (bool.TryParse(habilitarLogDetalhadoJob, out definirLog) && definirLog)
                    {
                        motoristaDocumentoBLL.DefinirLogger(logger);
                    }

                    var retorno = motoristaDocumentoBLL.ProcessarAlertaDocumentosMotorista(dtInicioExecucao.Date);

                    jobDocumento.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("AlertaDocumentosMotorista");
                }
                else
                {
                    JobUtil.GravarLog("AlertaDocumentosMotorista", "AlertaDocumentosMotorista - Ação não efetuada", "O JOB ainda não está no periodo de execução ou já possui uma execução em andamento");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo($"{JOB_NOME}: AlertaDocumentosMotorista", ex);
                return 1;
            }

            finally
            {
                jobDocumento.EmExecucao = false;
                Atualizar(jobDocumento);
            }
        }

        /// <summary>
        /// Método alternativo de correção do bug causado pela ausência do Job ID=6 na base de dados, 
        /// o que acabou bagunçando a ordem dos IDs no momento atualização de dados.
        /// Ticket relacionado: https://jira.minhati.com.br/browse/UNICAD-92
        /// </summary>
        /// <param name="job"></param>
        public void CorrigirID(Job job)
        {
            switch (job.Nome)
            {
                case JOB_NOME_CLIENTE:
                    job.ID = (int)EnumJob.Cliente;
                    break;

                case JOB_NOME_TRANSPORTADORA:
                    job.ID = (int)EnumJob.Transportadora;
                    break;

                case JOB_NOME_DOCUMENTO:
                    job.ID = (int)EnumJob.Documento;
                    break;

                case JOB_NOME_IMPORTACAO:
                    job.ID = (int)EnumJob.Importacao;
                    break;

                case JOB_NOME_PENDENTE:
                    job.ID = (int)EnumJob.IntegrarComposicaoPendenteSAP;
                    break;

                case JOB_NOME_ARQUIVO:
                    job.ID = (int)EnumJob.ExcluirArquivos;
                    break;

                case JOB_NOME_SINCRONIZAR_MOTORITAS_SAP:
                    job.ID = (int)EnumJob.SincronizarMotoritas;
                    break;

                case JOB_NOME_ENVIAR_EMAIL_TESTE:
                    job.ID = (int)EnumJob.EnviarEmailTeste;
                    break;

                case JOB_NOME_COMPOSICAO_DOCUMENTO_ALERTA:
                    job.ID = (int)EnumJob.ComposicaoDocumentoAlerta;
                    break;

                case JOB_NOME_COMPOSICAO_DOCUMENTO_VENCIDO:
                    job.ID = (int)EnumJob.ComposicaoDocumento;
                    break;

                case JOB_NOME_MOTORISTA_DOCUMENTO_ALERTA:
                    job.ID = (int)EnumJob.MotoristaDocumentoAlerta;
                    break;

                case JOB_NOME_MOTORISTA_DOCUMENTO_VENCIDO:
                    job.ID = (int)EnumJob.MotoristaDocumentoVencido;
                    break;

                default:
                    job.ID = -1;
                    break;
            }
        }

        public int ProcessarDocumentosVencidosMotorista(DateTime dtInicioExecucao)
        {
            ILogExecucao logger = new LogExecucao(nameof(ProcessarDocumentosVencidosMotorista), true);

            logger.Log("Início", "Buscando Job na tabela de Jobs", CodigoExecucao.Descricao);
            var jobDocumentoVencido = this.Selecionar(w => w.Nome == JOB_NOME_MOTORISTA_DOCUMENTO_VENCIDO);

            try
            {
                logger.Log("Verificações iniciais", "Verificando periodicidade, se job está ativo e se está em execução ", CodigoExecucao.Descricao);
                if (dtInicioExecucao > Convert.ToDateTime(jobDocumentoVencido.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobDocumentoVencido.NrPeriodicidadeMinutos))
                        && jobDocumentoVencido.StAtivo
                        && !VerificarJobEmExecucao(jobDocumentoVencido))
                {
                    DateTime dtExecucaoJob = Convert.ToDateTime(jobDocumentoVencido.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumentoVencido.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobDocumentoVencido.NrPeriodicidadeMinutos));
                    }

                    logger.Log("Atualizando Job", "Atualizando Job p/ \"Em execução\" e informado data de início.", CodigoExecucao.Descricao);
                    jobDocumentoVencido.EmExecucao = true;
                    jobDocumentoVencido.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobDocumentoVencido);

                    logger.Log("Chamada de Business (Motorista)", "Chamada de ProcessarDocumentosVencidos (PlacaDocumentoBusiness)", CodigoExecucao.Descricao);

                    string habilitarLogDetalhadoJob = Config.GetConfig(EnumConfig.HabilitarLogDetalhadoJob, (int)EnumPais.Padrao);

                    var documentoBLL = new MotoristaDocumentoBusiness();

                    bool definirLog = false;
                    if (bool.TryParse(habilitarLogDetalhadoJob, out definirLog) && definirLog)
                    {
                        documentoBLL.DefinirLogger(logger);
                    }

                    var retorno = documentoBLL.ProcessarDocumentosVencidos(dtInicioExecucao.Date);

                    jobDocumentoVencido.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("DocumentosVencidosMotorista");
                }
                else
                {
                    JobUtil.GravarLog("DocumentosVencidosMotorista", "DocumentosVencidosMotorista - Ação não efetuada", "O JOB ainda não está no periodo de execução ou já possui uma execução em andamento");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo($"{JOB_NOME}: DocumentosVencidosMotorista", ex);
                return 1;
            }

            finally
            {
                jobDocumentoVencido.EmExecucao = false;
                Atualizar(jobDocumentoVencido);
            }
        }

        public List<Job> ListarJob(JobFiltro filtro, PaginadorModel paginador)
        {
            using (UniCadDalRepositorio<Job> repositorio = new UniCadDalRepositorio<Job>())
            {
                IQueryable<Job> query = GetQueryJob(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.ID)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }
        }

        public int ListarJobCount(JobFiltro filtro)
        {
            using (UniCadDalRepositorio<Job> repositorio = new UniCadDalRepositorio<Job>())
            {
                IQueryable<Job> query = GetQueryJob(filtro, repositorio);
                return query.Count();
            }
        }

        private IQueryable<Job> GetQueryJob(JobFiltro filtro, IUniCadDalRepositorio<Job> repositorio)
        {
            IQueryable<Job> query = (from app in repositorio.ListComplex<Job>().AsNoTracking().OrderBy(i => i.ID)
                                     where (app.Nome.Contains(string.IsNullOrEmpty(filtro.Nome) ? app.Nome : filtro.Nome))
                                     select app);
            return query;
        }

        private int ExcluirArquivos(DateTime dtInicioExecucao)
        {
            var jobArquivo = Selecionar(w => w.Nome == JOB_NOME_ARQUIVO);
            if (jobArquivo == null)
            {
                JobUtil.GravarLogErro(JOB_NOME + ": Excluir Arquivos", "Job não encontrado");
                return 0;
            }
            try
            {
                if (dtInicioExecucao > Convert.ToDateTime(jobArquivo.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobArquivo.NrPeriodicidadeMinutos))
                        && jobArquivo.StAtivo
                        && !VerificarJobEmExecucao(jobArquivo))
                {

                    DateTime dtExecucaoJob = Convert.ToDateTime(jobArquivo.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobArquivo.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobArquivo.NrPeriodicidadeMinutos));
                    }

                    jobArquivo.EmExecucao = true;
                    jobArquivo.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobArquivo);

                    ArquivoBusiness arquivoBLL = new ArquivoBusiness();
                    //arquivoBLL.ProcessarArquivos(DateTime.Now.ToString("yyyy-MM-dd"));

                    jobArquivo.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("Excluir Arquivos");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErro(JOB_NOME + ": Excluir Arquivos", ex.Message);
                return 1;
            }

            finally
            {
                jobArquivo.EmExecucao = false;
                Atualizar(jobArquivo);
            }
        }

        public int ProcessarPendenteAtualizacaoSAP(DateTime dtInicioExecucao)
        {
            var jobPendente = Selecionar(w => w.Nome == JOB_NOME_PENDENTE);
            try
            {
                if (dtInicioExecucao > Convert.ToDateTime(jobPendente.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobPendente.NrPeriodicidadeMinutos))
                        && jobPendente.StAtivo
                        && !VerificarJobEmExecucao(jobPendente))
                {

                    DateTime dtExecucaoJob = Convert.ToDateTime(jobPendente.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobPendente.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobPendente.NrPeriodicidadeMinutos));
                    }

                    jobPendente.EmExecucao = true;
                    jobPendente.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobPendente);

                    ComposicaoBusiness compBLL = new ComposicaoBusiness();
                    compBLL.IntegrarComposicaoPendenteSap();

                    jobPendente.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("Pendente_SAP");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo(JOB_NOME + ": Pendente_SAP", ex);
                return 1;
            }

            finally
            {
                jobPendente.EmExecucao = false;
                Atualizar(jobPendente);
            }
        }

        public int ProcessarTransportadoras(DateTime dtInicioExecucao)
        {
            ConfiguracaoBusiness configBLL = new ConfiguracaoBusiness();
            var jobTransportadoras = Selecionar(w => w.Nome == JOB_NOME_TRANSPORTADORA);

            try
            {
                if (dtInicioExecucao > Convert.ToDateTime(jobTransportadoras.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobTransportadoras.NrPeriodicidadeMinutos))
                        && jobTransportadoras.StAtivo
                        && !VerificarJobEmExecucao(jobTransportadoras))
                {

                    DateTime dtExecucaoJob = Convert.ToDateTime(jobTransportadoras.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobTransportadoras.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobTransportadoras.NrPeriodicidadeMinutos));
                    }

                    jobTransportadoras.EmExecucao = true;
                    jobTransportadoras.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobTransportadoras);

                    Configuracao dataInicialTransportadora = configBLL.Selecionar(w => w.NmVariavel == "dataInicialConsultaTransportador");
                    DateTime? dtInicialTransportadora = null;
                    dtInicialTransportadora = DateTime.ParseExact(dataInicialTransportadora.Valor, "yyyy-MM-dd HH:mm",
                                           System.Globalization.CultureInfo.InvariantCulture);
                    TransportadoraBusiness tranportadoraBLL = new TransportadoraBusiness();
                    tranportadoraBLL.Importar(dtInicialTransportadora, EnumEmpresa.Combustiveis);

                    //Tirar 10 minutos que é o tempo médio que o job roda (inicio - fim)
                    dataInicialTransportadora.Valor = DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm");
                    configBLL.Atualizar(dataInicialTransportadora);


                    jobTransportadoras.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("Transportadora");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo(JOB_NOME + ": Transportadora", ex);
                return 1;
            }

            finally
            {
                jobTransportadoras.EmExecucao = false;
                Atualizar(jobTransportadoras);
            }
        }

        public int ProcessarClientes(DateTime dtInicioExecucao)
        {
            ConfiguracaoBusiness configBLL = new ConfiguracaoBusiness();
            var jobClientes = Selecionar(w => w.Nome == JOB_NOME_CLIENTE);

            try
            {
                if (dtInicioExecucao > Convert.ToDateTime(jobClientes.DtUltimaExecucao).AddMinutes(Convert.ToInt32(jobClientes.NrPeriodicidadeMinutos))
                        && jobClientes.StAtivo
                        && !VerificarJobEmExecucao(jobClientes))
                {

                    DateTime dtExecucaoJob = Convert.ToDateTime(jobClientes.DtUltimaExecucao);

                    while (dtExecucaoJob.AddMinutes(Convert.ToInt32(jobClientes.NrPeriodicidadeMinutos)) <= dtInicioExecucao)
                    {
                        dtExecucaoJob = dtExecucaoJob.AddMinutes(Convert.ToInt32(jobClientes.NrPeriodicidadeMinutos));
                    }

                    jobClientes.EmExecucao = true;
                    jobClientes.DtInicioJob = dtInicioExecucao;
                    Atualizar(jobClientes);

                    Configuracao dataInicialCliente = configBLL.Selecionar(w => w.NmVariavel == "dataInicialConsultaCliente");
                    DateTime? dtInicialCliente = null;
                    dtInicialCliente = DateTime.ParseExact(dataInicialCliente.Valor, "yyyy-MM-dd HH:mm",
                                           System.Globalization.CultureInfo.InvariantCulture);
                    ClienteBusiness clienteBLL = new ClienteBusiness();
                    clienteBLL.Importar(dtInicialCliente, EnumEmpresa.Combustiveis);

                    //Tirar 10 minutos que é o tempo médio que o job roda (inicio - fim)
                    dataInicialCliente.Valor = DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm");
                    configBLL.Atualizar(dataInicialCliente);
                    jobClientes.DtUltimaExecucao = dtExecucaoJob;

                    JobUtil.GravarLogSucesso("Cliente");
                }
                return 0;
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErroRecursivo(JOB_NOME + ": Cliente", ex);
                return 1;
            }

            finally
            {
                jobClientes.EmExecucao = false;
                Atualizar(jobClientes);
            }
        }

        public bool VerificarJobEmExecucao(Job job)
        {
            bool jobEmExecucao = job.EmExecucao;

            //verifica se o job está em execução ou se está em execução porém a ultima execução é maior que a tolerância
            if ((jobEmExecucao && (DateTime.Now - job.DtUltimaExecucao).TotalMinutes <= 60))
                return true;
            else
                return false;
        }
    }
}

