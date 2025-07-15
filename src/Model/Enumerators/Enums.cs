using System.ComponentModel;
using System.Web;

namespace Raizen.UniCad.Model
{

    public class DescricaoPorLinguaAttribute : DescriptionAttribute
    {
        public DescricaoPorLinguaAttribute(string msgPortugues, string msgEspanhol)
        {
            string SSO_IDIOMA = GetCookieIdioma();

            var pais =
                (SSO_IDIOMA.Equals("es-AR") ? EnumPais.Argentina :
                 SSO_IDIOMA.Equals("pt-BR") ? EnumPais.Brasil :
                 EnumPais.Padrao);

            switch (pais)
            {
                case EnumPais.Brasil:
                    DescriptionValue = msgPortugues;
                    break;
                case EnumPais.Argentina:
                    DescriptionValue = msgEspanhol;
                    break;
                default:
                    DescriptionValue = msgPortugues;
                    break;
            }
        }

        private string GetCookieIdioma()
        {
            var cookie = "SSO_IDIOMA";

            if (HttpContext.Current != null &&
                HttpContext.Current.Request.Cookies.Get(cookie) != null &&
                HttpContext.Current.Request.Cookies[cookie].Value != null)
                return HttpContext.Current.Request.Cookies[cookie].Value;

            return "pt-BR";
        }
    }

    public enum EnumAcao
    {
        Inserir = 1,
        Editar = 2,
        Clonar = 3
    }

    public enum EnumSituacaoAgendamento
    {
        [DescricaoPorLingua("Apto", "Apto")]
        Apto = 1,
        [DescricaoPorLingua("Inapto", "No apto")]
        Inapto = 2,
        [DescricaoPorLingua("Não Compareceu", "No asistió")]
        NaoCompareceu = 3
    }


    public enum EnumTipoAgenda
    {
        [DescricaoPorLingua("Checklist", "Lista de verificación")]
        Checklist = 1,
        [DescricaoPorLingua("Treinamento Prático", "La formación práctica")]
        TreinamentoPratico = 2,
        [DescricaoPorLingua("Treinamento Teórico", "La formación teórica")]
        TreinamentoTeorico = 3,
    }

    /// <summary>
    /// Ação imediata
    /// </summary>
    public enum EnumTipoBloqueioImediato
    {
        Nao = 0,
        Sim = 1,
    }

    /// <summary>
    /// Ação vencimento
    /// </summary>
    public enum EnumTipoAcaoVencimento
    {
        SemAcao = 0,
        Bloquear = 1,
        Reprovar = 2
    }


    public enum EnumTipoCadastroDocumento
    {
        [DescricaoPorLingua("Veículo", "Vehículo")]
        Veiculo = 1,
        [DescricaoPorLingua("Motorista", "Conductor")]
        Motorista = 2
    }

    public enum EnumTipoContratacaoArgentina
    {

        [Description("Estructural")]
        Estructural = 1,
        [Description("En Transito")]
        EnTransito = 2,
        [Description("Spot")]
        Spot = 3
    }

    public enum EnumTipoCarregamento
    {
        [DescricaoPorLingua("Por baixo", "Por debajo")]
        PorBaixo = 1,
        [DescricaoPorLingua("Por Cima", "Por encima")]
        PorCima = 2,
        [Description("Ambos")]
        Ambos = 3
    }

    public enum EnumTipoCarregamentoArgentina
    {
        [Description("Por debajo")]
        PorBaixo = 1,
        [Description("Por encima")]
        PorCima = 2,
        [Description("Ambos")]
        Ambos = 3
    }

    public enum EnumTipoImportacao
    {
        [DescricaoPorLingua("Placa", "Patente")]
        Placa = 1,
        [Description("Composição")]
        Composicao = 2,
    }

    public enum EnumErro
    {
        [Description("Campo Inválido")]
        CampoInvalido = 1,
        [DescricaoPorLingua("Campo Obrigatório", "Campo obligatorio")]
        CampoObrigatorio = 2,
        [DescricaoPorLingua("Placa não Aprovada", "Patente no aprobada")]
        PlacaNaoAprovada = 3,
        [DescricaoPorLingua("Placa não Existe", "La patente no existe")]
        PlacaNaoExiste = 4,
        [DescricaoPorLingua("Placa do tipo Inválido", "Patente de tipo inválido")]
        PlacaTipoInvalido = 5,
        [DescricaoPorLingua("Transportadora não Existe", "La transportadora no existe")]
        TransportadoraNaoExiste = 6,
        [DescricaoPorLingua("Integração Sap", "Integración con el SAP")]
        IntegracaoSap = 7,
    }

    public enum EnumStatusImportacao
    {
        [DescricaoPorLingua("Processar", "Procesar")]
        Processar = 1,
        [DescricaoPorLingua("Processando", "Procesamiento")]
        Processando = 2,
        [DescricaoPorLingua("Processado", "Procesado")]
        Processado = 3,
        [DescricaoPorLingua("Processado com erro", "Procesado con error")]
        ProcessadoErro = 4,
    }

    public enum EnumTipoIntegracaoSAP
    {
        Inclusao,
        Alteracao,
        Bloqueio,
        Desbloqueio,
        AprovarCheckList,
        ReprovarCheckList,
        Excluir,
        Nenhum
    }

    public enum EnumTipoProdutoSAP
    {
        [Description("CLAROS")]
        CLAROS_BR = 1,
        [Description("ESCUROS")]
        ESCUROS_BR = 2,
        [Description("JETA1")]
        JET_BR = 3,
        [Description("AVGAS")]
        AVGas_BR = 4,
        [Description("CLAROS")]
        Arla_BR = 5,
        [Description("CLAROS")]
        CLAROS_AR = 6,
        [Description("JETA1")]
        JET_AR = 7,
        [Description("QUIMICOS")]
        QUIMICOS_AR = 8,
        [Description("ESCUROS")]
        ESCUROS_AR = 9,
        [Description("COKE")]
        COKE_AR = 10,
        [Description("LPG")]
        GLP_AR = 11,
        [Description("LUBRIFIC")]
        LUB_AR = 12
    }

    public enum EnumSituacaoPlacaLimite
    {
        [DescricaoPorLingua("Não Permitido", "No se permite")]
        NaoPermitido = 1,
        [DescricaoPorLingua("No Limite", "En el límite")]
        NoLimite = 2,
        [Description("Permitido")]
        Permitido = 3
    }

    public enum EnumStatusMotorista
    {
        [Description("Em Aprovação")]
        EmAprovacao = 1,
        [Description("Aprovado")]
        Aprovado = 2,
        [Description("Reprovado")]
        Reprovado = 4,
        [Description("Bloqueado")]
        Bloqueado = 5,
    }
    public enum EnumStatusMotoristaArg
    {
        [Description("En Aprobación")]
        EmAprovacao = 1,
        [Description("Aprobado")]
        Aprovado = 2,
        [Description("Rechazado")]
        Reprovado = 4,
        [Description("Bloqueado")]
        Bloqueado = 5,
    }

    public enum EnumStatusComposicao
    {
        [Description("Em Aprovação")]
        EmAprovacao = 1,
        [Description("Aprovado")]
        Aprovado = 2,
        [Description("Reprovado")]
        Reprovado = 4,
        [Description("Bloqueado")]
        Bloqueado = 5,
        [Description("Aguardando atualização SAP")]
        AguardandoAtualizacaoSAP = 6
    }
    public enum EnumStatusComposicaoArg
    {
        [Description("En Aprobación")]
        EmAprovacao = 1,
        [Description("Aprobado")]
        Aprovado = 2,
        [Description("Rechazado")]
        Reprovado = 4,
        [Description("Bloqueado")]
        Bloqueado = 5,
        [Description("En espera de actualización de SAP")]
        AguardandoAtualizacaoSAP = 6,
    }

    public enum EnumJob
    {
        Cliente = 1,
        Transportadora = 2,
        Documento = 3,
        Importacao = 4,
        IntegrarComposicaoPendenteSAP = 5,
        ExcluirArquivos = 6,
        SincronizarMotoritas = 7,
        EnviarEmailTeste = 8,
        ComposicaoDocumentoAlerta = 9,
        ComposicaoDocumento = 10,
        MotoristaDocumentoAlerta = 11,
        MotoristaDocumentoVencido = 12
    }


    public enum EnumPessoaTipo
    {
        Visitante = 1,
        Fornecedor = 2,
        EngenheiroSSMALicenciamento = 3,
        EngenheiroSSMARemediacao = 4,
        CoordenadorSSMARemediacao = 5,
        GerenteSSMA = 6,
        Acionista = 7
    }



    public enum ConfiguracaoNome
    {
        UltimaExecucaoUC4 = 1,
        ContadorErroCriticoJob = 2,
        ToleranciaErroCriticoJob = 3
    }

    public enum EnumExibicaoRelatorioAlertasEmitidos
    {
        Licencas = 1,
        Condicionantes = 2
    }

    public enum EnumTipoAgrupamento
    {
        Mensal = 1,
        Cumulativo = 2
    }

    public enum EnumExibicaoRelatorioProximosAlertas
    {
        Licencas = 1,
        Condicionantes = 2,
        Grafico = 3
    }

    public enum EnumTipoConsultaUnidade
    {
        Normal = 1,
        ComCaso = 2,
        SemCaso = 3
    }

    public enum EnumStatusItemTpu
    {
        Salvo = 1,
        EmAprovacao = 2,
        Aprovado = 3,
        Reprovado = 4
    }

    public enum EnumTipoComposicao
    {
        [Description("Truck")]
        Truck = 1,
        [Description("Carreta")]
        Carreta = 2,
        [Description("Bitrem")]
        Bitrem = 3,
        [Description("Bitrem + Dolly")]
        BitremDolly = 4,
        [Description("Semirremolque Chico (Mínimo 20 m³ /Máximo 29 m³)")]
        SemirremolqueChico = 5,
        [Description("Semirremolque Grande (Mínimo 30 m³ /Máximo 39 m³)")]
        SemirremolqueGrande = 6,
        [Description("Escalado (Mínimo 40 m³ /Máximo 49 m³)")]
        Escalado = 7,
        [Description("Bitren Chico  (Mínimo 45 m³ /Máximo 55 m³)")]
        BitrenChico = 8,
        [Description("Bitren Grande  (Mínimo60 m³ /Máximo 70 m³)")]
        BitrenGrande = 9,
        [Description("Truck")]
        Truck_ARG = 10,
    }

    public enum EnumTipoVeiculo
    {
        [Description("Cavalo")]
        Cavalo = 1,
        [Description("Carreta")]
        Carreta = 2,
        [Description("Dolly")]
        Dolly = 3,
        [Description("Truck")]
        Truck = 4,
        [Description("Tractor")]
        Tractor = 5,
        [Description("Semiremolque")]
        Semiremolque = 6,
        [Description("Truck")]
        Truck_Arg = 9
    }

    public enum EnumOrigemIntegracaoSAP
    {
        EAB = 1,
        Combustiveis = 2
    }

    public enum EnumStatusAtivoMotorista
    {
        [Description("Sim")]
        Sim = 1,
        [Description("Não")]
        Nao = 0
    }

    public enum EnumEmpresa
    {
        [Description("EAB")]
        EAB = 1,
        [Description("Combustíveis")]
        Combustiveis = 2,
        [Description("Ambos")]
        Ambos = 3,
    }

    public enum EnumEmpresaArg
    {
        [Description("EAB")]
        EAB = 1,
        [Description("Combustibles")]
        Combustibles = 2,
        [Description("Ambos")]
        Ambos = 3,
    }

    public enum EnumCategoriaVeiculo
    {
        [Description("Particular")]
        Particular = 1,
        [DescricaoPorLingua("Aluguel", "Alquiler")]
        Aluguel = 2,
        [Description("Todos")]
        Todos = 3,
        [Description("Motorista")]
        Motorista = 4,
        [Description("Treinamento")]
        Treinamento = 5
    }

    public enum EnumTipoProduto
    {
        [Description("Gasolina/ Diesel / Etanol ")]// claros
        Claros = 1,
        [Description("Óleo Combustível")]//pesado escuro
        Escuros = 2,
        [Description("JET")] // aviação
        JET = 3,
        [Description("AVGas")] //aviação
        AVGas = 4,
        [Description("ARLA")] //
        ARLA = 5,
        [Description("Diesel/Nafta/Fame/Etanol/Kerosen")] //levianos
        ClarosArg = 6,
        [Description("JET")]
        JETArg = 7,
        [Description("Químicos")]
        Quimicos = 8,
        [Description("Asfaltos/Fuel Oil")]
        Asfaltos = 9,
        [Description("Coque")]
        Coque = 10,
        [Description("GLP")] //lpg
        GLP = 11,
        [Description("Lubrific")]
        Lubrific = 12
    }

    public enum EnumTipoFrete
    {
        CIF,
        FOB
    }

    public enum EnumConfig
    {
        DataInicialConsultaCliente,
        DataInicialConsultaTransportador,
        DataInicialDocumento,
        DataInicialExcluirArquivo,
        SubCategoryId,
        ResolutionGroupId,
        UrlApiAAWeb,
        UrlSite,
        UrlSiteTransportadoras,
        UrlCsOnline,
        EmailAngelira,
        CaminhoExportacaoExcel,
        CaminhoAnexos,
        DataInicialConsultaClienteEAB,
        DataInicialConsultaTransportadorEAB,
        SubCategoryMotoristaEabId,
        SubCategoryMotoristaCombId,
        ResolutionGroupMotoristaId,
        EqCompEabCif,
        EqCompEabFob,
        EqCompCombCif,
        EqCompCombFob,
        EqCompAmbasCif,
        EqCompAmbasFob,
        EqMotoEabCif,
        EqMotoEabFob,
        EqMotoCombCif,
        EqMotoCombFob,
        SfCompEabCif,
        SfCompEabFob,
        SfCompCombCif,
        SfCompCombFob,
        SfCompAmbasCif,
        SfCompAmbasFob,
        SfMotoEabCif,
        SfMotoEabFob,
        SfMotoCombCif,
        SfMotoCombFob,
        IDSFMotoCombFOB,
        IDSFMotoCombCIF,
        IDSFCompCombFOB,
        IDSFCompCombCIF,
        IDSFMotoEABFOB,
        IDSFMotoEABCIF,
        IDSFCompEABFOB,
        IDSFCompEABCIF,
        EasyQuery,
        SalesForce,
        LinkIntegracaoSalesForce,
        PastaArquivoCarga,
        TituloChamadoEasyQueryMotorista,
        TituloChamadoEasyQueryComposicao,
        CaminhoPDF,
        usuarioWebService,
        senhaWebService,
        SomarDiasRessalvaDocumentos,
        QtdTentativasIntegracaoSAP,
        emailDebug,
        emailDebugJob,
        token,
        validadeTreinamentoTeorico,
        habilitarVectoPlaca,
        habilitarBloqueioDocPlaca,
        habilitarBloqueioPlaca,
        habilitarVectoMotorista,
        habilitarBloqueioDocMotorista,
        habilitarBloqueioMotorista,
        timeOutJobDoc,
        NumMinEixos,
        NumMaxEixos,
        ArquivoAjudaTara,
        ArquivoAjudaNumEixos,
        ArquivoAjudaCategoria,
        HabilitarLogDetalhadoJob,
        EmailComCopiaPara,
        JustificativaBloqueioAutomatico,
        JustificativaReprovaAutomatica,
        habilitarReprovaPlacaAutomatica,
        habilitarReprovaDocPlacaAutomatica,
        habilitarReprovaMotoristaAutomatica,
        habilitarReprovaDocMotoristaAutomatica,
        TituloEmailBloqueioAutomatico,
        CorpoEmailBloqueioAutomatico,
        TituloEmailReprovaAutomatica,
        CorpoEmailReprovaAutomatica,
        TituloVeiculoAprovado,
        CorpoVeiculoAprovado,
        TituloVeiculoReprovado,
        CorpoVeiculoReprovado,
        TituloMotoristaAprovado,
        CorpoMotoristaAprovado,
        TituloMotoristaReprovado,
        CorpoMotoristaReprovado,
        TituloVeiculoAlertaVencimento,
        CorpoVeiculoAlertaVencimento,
        TituloMotoristaAlertaVencimento,
        CorpoMotoristaAlertaVencimento,
        TituloChecklist,
        CorpoChecklist,
        EmailRutasky,
        TituloAlertaDocumentoComposicao,
        CorpoAlertaDocumentoComposicao,
        TituloAlertaDocumentoMotorista,
        CorpoAlertaDocumentoMotorista,
        ResolutionGroupMotoristaIdCIF,
        SubCategoryMotoristaCombIdCIF,
        habilitarEnvioQuickTasCIF,
        habilitarEnvioQuickTasFOB,
        AtivaIntegracaoCsonline,
    }

    public enum EnumResource
    {
        CorpoDiasAcaoAlertaDocumentoComposicao,
        CorpoDocumentoAlertaDocumentoComposicao,
        CorpoPlacaAlertaDocumentoComposicao,
        CorpoMotoristaAlertaDocumentoMotorista,
        CorpoDocumentoAlertaDocumentoMotorista,
        CorpoDiasAcaoAlertaDocumentoMotorista,
        CorpoNomeDocumentoMotoristaAlertaVencimento,
        CorpoDiasVencimentoMotoristaAlertaVencimento,
        CorpoPlacaDiasVencimentoVeiculoAlertaVencimento,
        CorpoVeiculoEmailBloqueioReprovaAutomaticaComposicao,
        CorpoNomeEmailBloqueioReprovaAutomaticaMotorista,
        CorpoVencidoEmailBloqueioReprovaAutomaticaMotorista,
        VeiculoSingular,
        VeiculoPlural,
        MotoristaSingular,
        MotoristaPlural
    }

    public enum EnumPais
    {
        Brasil = 1,
        Argentina = 2,
        Padrao = 3
    }

    public enum EnumMensagemPlaca
    {
        /* A criação deste enum se dá pela necessidade de remoção de tratativas encontradas em JS por meio de mensagens (strings), mas sim pelo Id identificador */
        SemMensagemTratadaNoJs = 0,
        [DescricaoPorLingua("Este veículo já está cadastrado! Caso queria conferir, clicar no ícone 'Lápis'", "¡Este vehículo ya está registrado! Si desea verlo, haga clic en el icono 'Lápiz'")]
        VeiculoJaCadastrado = 1
    }

    public enum EnumPlaca
    {
        Placa1 = 1,
        Placa2 = 2,
        Placa3 = 3,
        Placa4 = 4,
        Placa5 = 5,
        Placa6 = 6
    }

    /// <summary>
    /// Classe contendo os valores padrões dos perfis de usuário no sistema
    /// </summary>
    public static class EnumPerfil
    {
        /* Esta foi uma solução alternativa para criação de Enums com valores string */

        /// <summary>
        /// Usuários vindos do CSOnline Brasil
        /// </summary>
        public static string CLIENTE_ACS { get { return "Cliente ACS"; } }

        /// <summary>
        /// Usuários vindos do CSOnline Argentina
        /// </summary>
        public static string CLIENTE_ACS_ARGENTINA { get { return "Cliente ACS Argentina"; } }

        /// <summary>
        /// Usuários com perfil Quality
        /// </summary>
        public static string QUALITY { get { return "Quality"; } }

        /// <summary>
        /// Usuários com perfil Transportadora
        /// </summary>
        public static string TRANSPORTADORA { get { return "Transportadora"; } }

        /// <summary>
        /// Usuários com perfil TRANSPORTADORA_ARGENTINA
        /// </summary>
        public static string TRANSPORTADORA_ARGENTINA { get { return "Transportadora Argentina"; } }

        /// <summary>
        /// Usuários com perfil CLIENTE_EAB
        /// </summary>
        public static string CLIENTE_EAB { get { return "Cliente EAB"; } }
    }
}
