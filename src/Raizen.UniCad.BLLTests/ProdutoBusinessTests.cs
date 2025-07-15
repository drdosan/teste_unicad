using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class ProdutoBusinessTests : BaseTest
    {
        ProdutoBusiness produtoBLL = new ProdutoBusiness();
        [TestMethod()]
        public void ManterProdutoTest()
        {

            string variavel = "ProdutoTeste";
            string retornoEsperado = "ProdutoTeste";
            string retorno = string.Empty;

            var produto = new Produto
            {
                Codigo = "0",
                Densidade = 0,
                IDTipoProduto = 1,
                Nome = variavel,
                Status = true
            };

            var resultadoAdicao = produtoBLL.Adicionar(produto);
            Assert.AreEqual(resultadoAdicao, true);

            retorno = produtoBLL.Selecionar(w => w.Nome == variavel).Nome;
            Assert.AreEqual(retornoEsperado, retorno);

            var resultadoExclusao = produtoBLL.Excluir(produto.ID);
            Assert.AreEqual(resultadoExclusao, true);
        }

        [TestMethod()]
        public void ListarProdutoTest()
        {
            var lista = produtoBLL.ListarProduto(new Model.Filtro.ProdutoFiltro(), new Framework.Models.PaginadorModel() { QtdeItensPagina = 10, PaginaAtual = 1});
            Assert.AreNotEqual(lista, null);
        }

        [TestMethod()]
        public void ListarProdutoCountTest()
        {
            var lista = produtoBLL.ListarProdutoCount(new Model.Filtro.ProdutoFiltro());
            Assert.AreNotEqual(lista, null);
        }

      
    }
}