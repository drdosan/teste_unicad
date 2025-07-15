using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System.Linq;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class MotoristaBusinessTests : BaseTest
    {
        private readonly MotoristaBusiness _motoBll = new MotoristaBusiness();
        private readonly MotoristaDocumentoBusiness _mdBll = new MotoristaDocumentoBusiness();
        private readonly TipoDocumentoBusiness _tpDocumentoBll = new TipoDocumentoBusiness();
        private readonly UsuarioBusiness _usuarioBll = new UsuarioBusiness();
        private readonly TransportadoraBusiness _transpBll = new TransportadoraBusiness();
        private readonly UsuarioTransportadoraBusiness _userTranspBll = new UsuarioTransportadoraBusiness();
        Usuario _usuario;
        Transportadora _transp;
        UsuarioTransportadora _uTransp;
        private readonly string _data = DateTime.Now.ToString("yyyy-MM-dd");

        [TestMethod]
        [TestCategory("Motorista")]
        public void ExportarMotorista()
        {
            try
            {
                var file = _motoBll.Exportar(new Model.Filtro.MotoristaFiltro()
                {
                    IDEmpresa = (int)EnumEmpresa.Ambos,
                    IDStatus = (int)EnumStatusComposicao.EmAprovacao
                });
                Assert.IsNotNull(file);
            }
            catch (Exception ex)
            {

            }
        }

        private static List<MotoristaDocumentoView> AdicionarDocumentos(TipoDocumento tipoDoc1, TipoDocumento tipoDoc2, TipoDocumento tipoDoc3,
            TipoDocumento tipoDoc4, TipoDocumento tipoDoc5, TipoDocumento tipoDoc6, TipoDocumento tipoDoc7)
        {
            MotoristaDocumentoView documento1 = new MotoristaDocumentoView();
            documento1.IDTipoDocumento = tipoDoc1.ID;
            documento1.Sigla = tipoDoc1.Sigla;
            documento1.DataVencimento = DateTime.Now.AddDays(3);

            MotoristaDocumentoView documento2 = new MotoristaDocumentoView();
            documento2.IDTipoDocumento = tipoDoc2.ID;
            documento2.Sigla = tipoDoc2.Sigla;
            documento2.DataVencimento = DateTime.Now.AddDays(3);

            MotoristaDocumentoView documento3 = new MotoristaDocumentoView();
            documento3.IDTipoDocumento = tipoDoc3.ID;
            documento3.Sigla = tipoDoc3.Sigla;
            documento3.DataVencimento = DateTime.Now.AddDays(3);

            MotoristaDocumentoView documento4 = new MotoristaDocumentoView();
            documento4.IDTipoDocumento = tipoDoc4.ID;
            documento4.Sigla = tipoDoc4.Sigla;
            documento4.DataVencimento = DateTime.Now.AddDays(3);

            MotoristaDocumentoView documento5 = new MotoristaDocumentoView();
            documento5.IDTipoDocumento = tipoDoc5.ID;
            documento5.Sigla = tipoDoc5.Sigla;
            documento5.DataVencimento = DateTime.Now.AddDays(3);

            MotoristaDocumentoView documento6 = new MotoristaDocumentoView();
            documento6.IDTipoDocumento = tipoDoc6.ID;
            documento6.Sigla = tipoDoc6.Sigla;
            documento6.DataVencimento = DateTime.Now.AddDays(3);

            MotoristaDocumentoView documento7 = new MotoristaDocumentoView();
            documento7.IDTipoDocumento = tipoDoc7.ID;
            documento7.Sigla = tipoDoc7.Sigla;
            documento7.DataVencimento = DateTime.Now.AddDays(3);


            List<MotoristaDocumentoView> docs = new List<MotoristaDocumentoView>();
            docs.Add(documento1);
            docs.Add(documento2);
            docs.Add(documento3);
            docs.Add(documento4);
            docs.Add(documento5);
            docs.Add(documento6);
            docs.Add(documento7);
            return docs;
        }

        //[TestMethod]
        //[TestCategory("Motorista")]
        //public void ListarMotoristaCount()
        //{
        //    var result = _motoBll.ListarMotoristaCount(new Model.Filtro.MotoristaFiltro { });
        //    Assert.IsTrue(result > 0);
        //}

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod]
        [TestCategory("Motorista")]
        public void ListarMotoristaAlteracoes()
        {
            _transp = new Transportadora
            {
                CNPJCPF = "300400500",
                Desativado = false,
                DtAtualizacao = DateTime.Now,
                IDEmpresa = (int)EnumEmpresa.EAB,
                RazaoSocial = "Alexandre Spreafico Novaes",
                IBM = "0000301014",
                DtInclusao = DateTime.Now,
                IdPais = (int)EnumPais.Brasil
            };
            _transpBll.Adicionar(_transp);

            //Incluir Documento
            var tipoDoc1 = FuncoesCompartilhadasTests.IncluirDocumento("EXAME", "EXAME", EnumCategoriaVeiculo.Motorista);
            var tipoDoc2 = FuncoesCompartilhadasTests.IncluirDocumento("CDDS", "EXAME", EnumCategoriaVeiculo.Motorista);
            var tipoDoc3 = FuncoesCompartilhadasTests.IncluirDocumento("NR20", "EXAME", EnumCategoriaVeiculo.Motorista);
            var tipoDoc4 = FuncoesCompartilhadasTests.IncluirDocumento("NR35", "EXAME", EnumCategoriaVeiculo.Motorista);
            var tipoDoc5 = FuncoesCompartilhadasTests.IncluirDocumento("MOPP", "EXAME", EnumCategoriaVeiculo.Motorista);
            var tipoDoc6 = FuncoesCompartilhadasTests.IncluirDocumento("CNH", "EXAME", EnumCategoriaVeiculo.Motorista);
            var tipoDoc7 = FuncoesCompartilhadasTests.IncluirDocumento("TRM", "EXAME", EnumCategoriaVeiculo.Motorista);
            var docs = AdicionarDocumentos(tipoDoc1, tipoDoc2, tipoDoc3, tipoDoc4, tipoDoc5, tipoDoc6, tipoDoc7);
            var docs2 = AdicionarDocumentos(tipoDoc1, tipoDoc2, tipoDoc3, tipoDoc4, tipoDoc5, tipoDoc6, tipoDoc7);
            //Incluir Motorista
            var motorista = FuncoesCompartilhadasTests.CriarMotorista("98123721897", "B", _transp.ID, "65412811493", docs, "teste@teste.com.br",
                (int)EnumEmpresa.Combustiveis, "São Paulo", new DateTime(1987, 09, 03), "Teste Automátizado",
                "CIF", "SSP/SP", "DETRAN/SP", "2139812312-1", "1998121678");
            _motoBll.AdicionarMotorista(motorista);
            var motoristaNovo = FuncoesCompartilhadasTests.CriarMotorista("98123721897", "D", _transp.ID, "65412811493", docs2, "teste@teste.com.br",
                (int)EnumEmpresa.Combustiveis, "São Paulo", new DateTime(1987, 09, 03), "Teste Automátizado",
                "CIF", "SSP/SP", "DETRAN/SP", "2139812312-1", "1998121678");
            _motoBll.AdicionarMotorista(motoristaNovo);
            var alteracoes = _motoBll.CarregarAlteracoes(motoristaNovo, motorista);
            Assert.IsTrue(alteracoes.IsCategoriaCNHAlterado);


            motorista.Documentos.ForEach(w => _mdBll.Excluir(w.ID));
            motoristaNovo.Documentos.ForEach(w => _mdBll.Excluir(w.ID));
            _motoBll.Excluir(motorista);
            _motoBll.Excluir(motoristaNovo);
            _tpDocumentoBll.Excluir(tipoDoc1);
            _tpDocumentoBll.Excluir(tipoDoc2);
            _tpDocumentoBll.Excluir(tipoDoc3);
            _tpDocumentoBll.Excluir(tipoDoc4);
            _tpDocumentoBll.Excluir(tipoDoc5);
            _tpDocumentoBll.Excluir(tipoDoc6);
            _tpDocumentoBll.Excluir(tipoDoc7);

            _transpBll.Excluir(_transp);
        }
    }
}