using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using Raizen.Framework.Utils.Cache;
using Raizen.Framework.Entity.Utils;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo
{
    public static class ConfigBuilder
    {

        public static SqlConnection GetConnection()
        {
            string connectionString = ConnectionUtils.GetConnectionString("UniCadContext");
            SqlConnection conn = new SqlConnection(connectionString);
            return conn;
        }

        private static DbModelBuilder GetBuilder()
        {
            var builder = new System.Data.Entity.DbModelBuilder();

            builder.Configurations.Add(new ProdutoConfig());
            builder.Configurations.Add(new AgendamentoChecklistConfig());
            builder.Configurations.Add(new TransportadoraConfig());
            builder.Configurations.Add(new ClienteConfig());
            builder.Configurations.Add(new EmpresaConfig());
            builder.Configurations.Add(new UsuarioConfig());
            builder.Configurations.Add(new UsuarioTransportadoraConfig());
            builder.Configurations.Add(new UsuarioClienteConfig());
            builder.Configurations.Add(new TipoProdutoConfig());
            builder.Configurations.Add(new TipoAgendaConfig());
            builder.Configurations.Add(new CategoriaVeiculoConfig());
            builder.Configurations.Add(new TipoDocumentoConfig());
            builder.Configurations.Add(new TipoDocumentoTipoProdutoConfig());
            builder.Configurations.Add(new TipoDocumentoTipoVeiculoConfig());
            builder.Configurations.Add(new TipoVeiculoConfig());
            builder.Configurations.Add(new LogDocumentosConfig());
            builder.Configurations.Add(new ConfiguracaoConfig());
            builder.Configurations.Add(new PlacaClienteConfig());
            builder.Configurations.Add(new PlacaSetaConfig());
            builder.Configurations.Add(new PlacaConfig());
            builder.Configurations.Add(new PlacaBrasilConfig());
            builder.Configurations.Add(new PlacaArgentinaConfig());
            builder.Configurations.Add(new PlacaDocumentoConfig());
            builder.Configurations.Add(new TipoCarregamentoConfig());
            builder.Configurations.Add(new JobConfig());
            builder.Configurations.Add(new EstadoConfig());
            builder.Configurations.Add(new TipoComposicaoConfig());
            builder.Configurations.Add(new ComposicaoConfig());
            builder.Configurations.Add(new ComposicaoEixoConfig());
            builder.Configurations.Add(new HistorioBloqueioComposicaoConfig());
            builder.Configurations.Add(new ChecklistComposicaoConfig());
            builder.Configurations.Add(new MotoristaConfig());
            builder.Configurations.Add(new MotoristaPesquisaConfig());
            builder.Configurations.Add(new MotoristaBrasilConfig());
            builder.Configurations.Add(new MotoristaArgentinaConfig());
            builder.Configurations.Add(new MotoristaClienteConfig());
            builder.Configurations.Add(new MotoristaDocumentoConfig());
            builder.Configurations.Add(new MotoristaTipoProdutoConfig());
            builder.Configurations.Add(new MotoristaTipoComposicaoConfig());
            builder.Configurations.Add(new HistorioAtivarMotoristaConfig());
            builder.Configurations.Add(new HistorioBloqueioMotoristaConfig());
            builder.Configurations.Add(new TerminalConfig());
            builder.Configurations.Add(new MotoristaTreinamentoTerminalConfig());
            builder.Configurations.Add(new HistoricoTreinamentoTeoricoMotoristaConfig());
            builder.Configurations.Add(new AgendamentoTerminalConfig());
            builder.Configurations.Add(new AgendamentoTerminalHorarioConfig());
            builder.Configurations.Add(new AgendamentoTreinamentoConfig());
            builder.Configurations.Add(new SincronizacaoMotoristasConfig());
            builder.Configurations.Add(new TerminalEmpresaConfig());
            builder.Configurations.Add(new LogExecucaoJobConfig());
            builder.Configurations.Add(new PaisConfig());
            builder.Configurations.Add(new TipoDocumentoTipoComposicaoConfig());
            IEnumerable<Type> modelosIgnorados = ConfigModels.ListaModelosIgnorados();

            if (modelosIgnorados != null && modelosIgnorados.Any())
            {
                builder.Ignore(modelosIgnorados);
            }

            return builder;
        }

        public static DbCompiledModel GetModeloCompilado()
        {
            DbCompiledModel ModeloCompiladoRetorno = GetModeloCompiladoCache();
            if (ModeloCompiladoRetorno != null)
            {
                return ModeloCompiladoRetorno;
            }
            else
            {
                DbProviderInfo DbInfo = new DbProviderInfo("System.Data.SqlClient", "2008");
                DbCompiledModel ModeloCompilado = GetBuilder().Build(DbInfo).Compile();

                SetModeloCompiladoCache(ModeloCompilado);

                return ModeloCompilado;
            }
        }

        #region GetModeloCompiladoCache

        /// <summary>
        /// Retorna o DbCompiledModel com as informações da confiuração e tabelas do Entity do Cache
        /// </summary>
        /// <returns></returns>
        private static DbCompiledModel GetModeloCompiladoCache()
        {
            if (HttpContext.Current != null && HttpContext.Current.Application != null && HttpContext.Current.Application["Contexto_UniCadContext"] != null)
            {
                return HttpContext.Current.Application["Contexto_UniCadContext"] as DbCompiledModel;
            }

            ICacheManager cache = CacheManager.Instance;
            object DbCompilado = cache.Get("Contexto_UniCadContext");

            if (DbCompilado != null)
            {
                return DbCompilado as DbCompiledModel;
            }

            return null;
        }

        #endregion

        #region SetModeloCompiladoCache

        /// <summary>
        /// Atualiza a área de cache com o DbCompiledModel
        /// </summary>
        /// <param name="modeloCompilado"></param>
        private static void SetModeloCompiladoCache(DbCompiledModel modeloCompilado)
        {
            if (HttpContext.Current != null && HttpContext.Current.Application != null)
            {
                HttpContext.Current.Application.Add("Contexto_UniCadContext",
                                                    modeloCompilado);
            }
            else
            {
                DateTimeOffset tempo = DateTime.Now;
                tempo = tempo.AddHours(24);

                ICacheManager cache = CacheManager.Instance;
                cache.Add("Contexto_UniCadContext", modeloCompilado, tempo);
            }
        }

        #endregion

    }
}

