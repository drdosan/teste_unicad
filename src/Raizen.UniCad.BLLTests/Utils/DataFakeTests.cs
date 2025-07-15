using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLLTests.Utils
{
    public class DataFakeTests
    {
        public static Placa GetPlacaBrasil01(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                Chassi = "12345678101213",
                CPFCNPJ = "14774281000144",
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,
                IDEstado = 1,
                Cidade = "Cidade teste",
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_BR1",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Renavam = "1234567890",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Brasil
            };
        }

        public static Placa GetPlacaBrasil02(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                Chassi = "12345678101213",
                CPFCNPJ = "14774281000144",
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,
                IDEstado = 1,
                Cidade = "Cidade teste",
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_BR2",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Renavam = "1234567890",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Brasil
            };
        }

        public static Placa GetPlacaArgentina01(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_ARG1",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Argentina,
                TransportitstaCUIT = "CUITTeste01",
                PBTC = 102.34
            };
        }

        public static Placa GetPlacaArgentina02(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_ARG2",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Argentina,
                TransportitstaCUIT = "CUITTeste02",
                PBTC = 102.34
            };
        }
    }
}
