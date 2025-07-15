using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.Framework.Models;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class TipoDocumentoBusinessTests : BaseTest
    {
        private readonly TipoDocumentoBusiness _tipoDocumentoBll = new TipoDocumentoBusiness();
        private readonly TipoDocumentoTipoVeiculoBusiness _tipoDocumentoTipoVeiculoBll = new TipoDocumentoTipoVeiculoBusiness();
        private readonly TipoDocumentoTipoProdutoBusiness _tipoDocumentoTipoProdutoBll = new TipoDocumentoTipoProdutoBusiness();

        [TestMethod()]
        [TestCategory("Tipo Documento")]
        public void ListarTipoDocumentoTest()
        {
            var retorno = _tipoDocumentoBll.ListarTipoDocumento(new TipoDocumentoFiltro(), new PaginadorModel { QtdeItensPagina = 10, PaginaAtual = 1 });
            Assert.IsNotNull(retorno);
        }
        [TestMethod]
        [TestCategory("Tipo Documento")]
        public void ListarTipoDocumentoCountTest()
        {
            var retorno = _tipoDocumentoBll.ListarTipoDocumentoCount(new TipoDocumentoFiltro());
            Assert.IsNotNull(retorno);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod]
        [TestCategory("Tipo Documento")]
        public void AdicionarTipoDocumento()
        {
            var listTiposProduto = new List<TipoDocumentoTipoProdutoView>
            {
                (new TipoDocumentoTipoProdutoView {IDTipoProduto = (int) EnumTipoProduto.ARLA}),
                (new TipoDocumentoTipoProdutoView {IDTipoProduto = (int) EnumTipoProduto.AVGas}),
                (new TipoDocumentoTipoProdutoView {IDTipoProduto = (int) EnumTipoProduto.Claros}),
                (new TipoDocumentoTipoProdutoView {IDTipoProduto = (int) EnumTipoProduto.Escuros}),
                (new TipoDocumentoTipoProdutoView {IDTipoProduto = (int) EnumTipoProduto.JET})
            };

            var listTipoVeiculo = new List<TipoDocumentoTipoVeiculoView>
            {
                (new TipoDocumentoTipoVeiculoView {IDTipoVeiculo = (int) EnumTipoVeiculo.Carreta}),
                (new TipoDocumentoTipoVeiculoView {IDTipoVeiculo = (int) EnumTipoVeiculo.Cavalo}),
                (new TipoDocumentoTipoVeiculoView {IDTipoVeiculo = (int) EnumTipoVeiculo.Dolly}),
                (new TipoDocumentoTipoVeiculoView {IDTipoVeiculo = (int) EnumTipoVeiculo.Truck}),
            };


            var tipoDocumento = new TipoDocumento
            {
                Alerta1 = 60,
                Alerta2 = 30,
                BloqueioImediato = (int)EnumTipoBloqueioImediato.Nao,
                DataAtualizacao = DateTime.Now,
                Descricao = "Teste Automatizado - Tipo Documento",
                IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Particular,
                IDEmpresa = (int)EnumEmpresa.Combustiveis,
                Obrigatorio = true,
                Observacao = "Teste",
                Sigla = "TA",
                qtdeAlertas = 2,
                tipoCadastro = (int)EnumTipoCadastroDocumento.Veiculo,
                Operacao = "CIF",
                TiposProduto = listTiposProduto,
                TiposVeiculo = listTipoVeiculo,
                MesesValidade = 24,
                QtdDiasBloqueio = 10,
                Status = true,
                IDPais = 1
            };
            var retorno = _tipoDocumentoBll.AdicionarTipoDocumento(tipoDocumento);
            Assert.IsTrue(retorno);

            tipoDocumento.Operacao = "FOB";
            var retornoAtualizcao = _tipoDocumentoBll.AtualizarTipoDocumento(tipoDocumento);
            Assert.IsTrue(retornoAtualizcao);


            foreach (var item in listTipoVeiculo)
            {
                _tipoDocumentoTipoVeiculoBll.Excluir(item.ID);
            }

            foreach (var item in listTiposProduto)
            {
                _tipoDocumentoTipoProdutoBll.Excluir(item.ID);
            }
            _tipoDocumentoBll.Excluir(tipoDocumento);
        }

    }
}
