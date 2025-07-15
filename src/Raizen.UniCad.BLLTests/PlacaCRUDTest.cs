using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Utils;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class PlacaCRUDTest
    {
        [TestMethod()]
        public void IncluirUmaPlacaBrasilTest()
        {
            var placaTeste = "BRA0001";
            var placa01 = FuncoesCompartilhadasTests.GetPlacaBrasil01(placaTeste, (int)EnumTipoVeiculo.Truck, false);

            //Limpa vestigios anteriores das placas de testes
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.DeleteList(c => c.PlacaVeiculo == placaTeste);
                    transactionScope.Complete();
                }
            }

            //Inclui uma placa de teste
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.Add(placa01);
                    transactionScope.Complete();
                }
            }

            //Seleciona uma placa de teste e verifica a placa incluida
            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var placaInserida = PlacaRepositorio.Get(placa01.ID);

                foreach (PropertyInfo prop in placa01.GetType().GetProperties())
                {
                    //Verifica o resultado esperado de todos os campos exceto o ID
                    if (prop.Name != "Id")
                    {
                        var valueExpected = prop.GetValue(placa01, null);
                        var actualValue = placaInserida.GetType().GetProperty(prop.Name).GetValue(placaInserida, null);

                        Assert.AreEqual(valueExpected, actualValue, $"Falha na asserção da propriedade [{prop.Name}]");
                    }
                }

            }

            //Limpa os registros de testes
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.DeleteList(c => c.PlacaVeiculo == placaTeste);
                    transactionScope.Complete();
                }
            }
        }

        [TestMethod()]
        public void IncluirUmaPlacaArgentinaTest()
        {
            var placaTeste = "ARG0001";
            var placa01 = FuncoesCompartilhadasTests.GetPlacaArgentina01(placaTeste, (int)EnumTipoVeiculo.Truck, false);

            //Limpa vestigios anteriores das placas de testes
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.DeleteList(c => c.PlacaVeiculo == placaTeste);
                    transactionScope.Complete();
                }
            }

            //Inclui uma placa de teste
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.Add(placa01);
                    transactionScope.Complete();
                }
            }

            //Seleciona uma placa de teste e verifica a placa incluida
            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var placaInserida = PlacaRepositorio.Get(placa01.ID);

                foreach (PropertyInfo prop in placa01.GetType().GetProperties())
                {
                    //Verifica o resultado esperado de todos os campos exceto o ID
                    if (prop.Name != "Id")
                    {
                        var valueExpected = prop.GetValue(placa01, null);
                        var actualValue = placaInserida.GetType().GetProperty(prop.Name).GetValue(placaInserida, null);

                        Assert.AreEqual(valueExpected, actualValue, $"Falha na asserção da propriedade [{prop.Name}]");
                    }
                }

            }

            //Limpa os registros de testes
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.DeleteList(c => c.PlacaVeiculo == placaTeste);
                    transactionScope.Complete();
                }
            }
        }

        [TestMethod()]
        public void IncluirDuasPlacasBrasilTest()
        {
            var placaTeste01 = "BRA0001";
            var placa01 = FuncoesCompartilhadasTests.GetPlacaBrasil01(placaTeste01, (int)EnumTipoVeiculo.Truck, false);

            var placaTeste02 = "BRA0002";
            var placa02 = FuncoesCompartilhadasTests.GetPlacaBrasil01(placaTeste02, (int)EnumTipoVeiculo.Truck, false);

            //Limpa vestigios anteriores das placas de testes
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.DeleteList(c => c.PlacaVeiculo == placaTeste01 || c.PlacaVeiculo == placaTeste02);
                    transactionScope.Complete();
                }
            }

            //Inclui uma placa de teste
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                    PlacaRepositorio.Add(placa01);
                
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                    PlacaRepositorio.Add(placa02);

                transactionScope.Complete();
            }

            //Seleciona uma placa de teste e verifica a placa incluida
            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var placaInserida01 = PlacaRepositorio.Get(placa01.ID);
                foreach (PropertyInfo prop in placa01.GetType().GetProperties())
                {
                    //Verifica o resultado esperado de todos os campos exceto o ID
                    if (prop.Name != "Id")
                    {
                        var valueExpected = prop.GetValue(placa01, null);
                        var actualValue = placaInserida01.GetType().GetProperty(prop.Name).GetValue(placaInserida01, null);

                        Assert.AreEqual(valueExpected, actualValue, $"Falha na asserção da propriedade [{prop.Name}]");
                    }
                }
            }

            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var placaInserida02 = PlacaRepositorio.Get(placa02.ID);
                foreach (PropertyInfo prop in placa02.GetType().GetProperties())
                {
                    //Verifica o resultado esperado de todos os campos exceto o ID
                    if (prop.Name != "Id")
                    {
                        var valueExpected = prop.GetValue(placa02, null);
                        var actualValue = placaInserida02.GetType().GetProperty(prop.Name).GetValue(placaInserida02, null);

                        Assert.AreEqual(valueExpected, actualValue, $"Falha na asserção da propriedade [{prop.Name}]");
                    }
                }
            }

            //Limpa os registros de testes
            using (TransactionScope transactionScope = Raizen.Framework.Utils.Transacao.Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    PlacaRepositorio.DeleteList(c => c.PlacaVeiculo == placaTeste01 || c.PlacaVeiculo == placaTeste02);
                    transactionScope.Complete();
                }
            }
        }



    }
}
