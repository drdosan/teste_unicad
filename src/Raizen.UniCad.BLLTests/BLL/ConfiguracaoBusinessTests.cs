using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class ConfiguracaoBusinessTests : BaseTest
    {
        private readonly ConfiguracaoBusiness _configBll = new ConfiguracaoBusiness();
        
        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ManterConfiguracaoTest()
        {
            string variavel = "ConfiguracaoTeste";
            string retornoEsperado = "ConfiguracaoTeste";
            string retorno = string.Empty;

            var configuracao = new Configuracao
            {
                NmVariavel = variavel,
                Descricao = variavel,
                DtAtualizacao = DateTime.Now,
                DtCriacao = DateTime.Now,
                Valor = variavel
            };

            var resultadoAdicao = _configBll.Adicionar(configuracao);
            Assert.AreEqual(resultadoAdicao, true);

            retorno = _configBll.Selecionar(w => w.NmVariavel == variavel).NmVariavel;
            Assert.AreEqual(retornoEsperado, retorno);

            var resultadoExclusao = _configBll.Excluir(configuracao.ID);
            Assert.AreEqual(resultadoExclusao, true);

        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarConfiguracaoTest()
        {
            var teste = _configBll.ListarConfiguracao(new Model.Filtro.ConfiguracaoFiltro(), new Framework.Models.PaginadorModel() { PaginaAtual = 1, QtdeItensPagina = 10 });

            Assert.IsNotNull(teste);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarConfiguracaoCountTest()
        {
            Assert.IsNotNull(_configBll.ListarConfiguracaoCount(new Model.Filtro.ConfiguracaoFiltro()));
        }
    }
}