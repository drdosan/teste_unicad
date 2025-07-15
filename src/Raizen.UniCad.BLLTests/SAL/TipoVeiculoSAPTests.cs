using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.Model;
using Raizen.UniCad.SAL.TipoVeiculoComposicao;
using System;
using System.Collections.Generic;

namespace Raizen.UniCad.BLLTests
{
    [TestClass]
    public class TipoVeiculoSAPTests
    {
        #region Brasil

        [TestMethod]
        public void GetTpVeiculo_Brasil_Tests()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
              visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<GetTpVeiculoBrasilTest_Cenario>()
            {
                new GetTpVeiculoBrasilTest_Cenario(1, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(2, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 2, "CIF", "RC1"),
                new GetTpVeiculoBrasilTest_Cenario(3, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 3, "CIF", "RC1"),
                new GetTpVeiculoBrasilTest_Cenario(4, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 4, "CIF", "RC1"),
                new GetTpVeiculoBrasilTest_Cenario(5, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(6, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 2, "CIF", "RE1"),
                new GetTpVeiculoBrasilTest_Cenario(7, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 3, "CIF", "RE1"),
                new GetTpVeiculoBrasilTest_Cenario(8, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 4, "CIF", "RE1"),
                new GetTpVeiculoBrasilTest_Cenario(9, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(10, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 2, "CIF", "RA1"),
                new GetTpVeiculoBrasilTest_Cenario(11, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 3, "CIF", "RA1"),
                new GetTpVeiculoBrasilTest_Cenario(12, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 4, "CIF", "RA1"),
                new GetTpVeiculoBrasilTest_Cenario(13, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(14, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 2, "CIF", "RA1"),
                new GetTpVeiculoBrasilTest_Cenario(15, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 3, "CIF", "RA1"),
                new GetTpVeiculoBrasilTest_Cenario(16, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 4, "CIF", "RA1"),
                new GetTpVeiculoBrasilTest_Cenario(17, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(18, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(19, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(20, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(21, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(22, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(23, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(24, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(25, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(26, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(27, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(28, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(29, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(30, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(31, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(32, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(33, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(34, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(35, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(36, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(37, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(38, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(39, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(40, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC"),

                new GetTpVeiculoBrasilTest_Cenario(41, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(42, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(43, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(44, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 4, "CIF", "RC5"),
                new GetTpVeiculoBrasilTest_Cenario(45, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(46, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(47, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(48, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 4, "CIF", "RE2"),
                new GetTpVeiculoBrasilTest_Cenario(49, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(50, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(51, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(52, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 4, "CIF", "RA5"),
                new GetTpVeiculoBrasilTest_Cenario(53, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(54, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(55, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(56, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 4, "CIF", "RA5"),
                new GetTpVeiculoBrasilTest_Cenario(57, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(58, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(59, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(60, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(61, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(62, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(63, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(64, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(65, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(66, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(67, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(68, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(69, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(70, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(71, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(72, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(73, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(74, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(75, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(76, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(77, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(78, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(79, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(80, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC"),

                new GetTpVeiculoBrasilTest_Cenario(81, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(82, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(83, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(84, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(85, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(86, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(87, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(88, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(89, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(90, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(91, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(92, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(93, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(94, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(95, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(96, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(97, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(98, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(99, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(100, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(101, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(102, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(103, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(104, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(105, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(106, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(107, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(108, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(109, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(110, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(111, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(112, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(113, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(114, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(115, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(116, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(111, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(118, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(119, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(120, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC"),

                new GetTpVeiculoBrasilTest_Cenario(121, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(122, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(123, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(124, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(125, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(126, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(127, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(128, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(129, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(130, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(131, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(132, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(133, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 1, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(134, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 2, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(135, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 3, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(136, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 4, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(137, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(138, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(139, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(140, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6"),
                new GetTpVeiculoBrasilTest_Cenario(141, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(142, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(143, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(144, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(145, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(146, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(147, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(148, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(149, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(150, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(151, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(152, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(153, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(154, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(155, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(156, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(151, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(158, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(159, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(160, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC"),

                new GetTpVeiculoBrasilTest_Cenario(161, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "20000", 4, "CIF", "RC8"),
                new GetTpVeiculoBrasilTest_Cenario(162, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "25000", 4, "CIF", "RC8"),
                new GetTpVeiculoBrasilTest_Cenario(163, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "30000", 4, "CIF", "RC9"),

                new GetTpVeiculoBrasilTest_Cenario(164, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 5, "CIF", "RA2"),
                new GetTpVeiculoBrasilTest_Cenario(165, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 6, "CIF", "RA2"),
                new GetTpVeiculoBrasilTest_Cenario(161, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 5, "CIF", "RC2"),
                new GetTpVeiculoBrasilTest_Cenario(162, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 6, "CIF", "RC2"),
                new GetTpVeiculoBrasilTest_Cenario(163, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "20000", 4, "CIF", "RC8"),
                new GetTpVeiculoBrasilTest_Cenario(164, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "25000", 4, "CIF", "RC8"),
                new GetTpVeiculoBrasilTest_Cenario(165, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "30000", 4, "CIF", "RC9"),

                new GetTpVeiculoBrasilTest_Cenario(166, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "20000", 4, "CIF", "RC8"),
                new GetTpVeiculoBrasilTest_Cenario(167, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "25000", 4, "CIF", "RC8"),
                new GetTpVeiculoBrasilTest_Cenario(168, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "30000", 4, "CIF", "RC9"),
                new GetTpVeiculoBrasilTest_Cenario(169, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 7, "CIF", "RE3"),
                new GetTpVeiculoBrasilTest_Cenario(180, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 9, "CIF", "RE3"),
                new GetTpVeiculoBrasilTest_Cenario(181, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 7, "CIF", "RA3"),
                new GetTpVeiculoBrasilTest_Cenario(181, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 9, "CIF", "RA4"),
                new GetTpVeiculoBrasilTest_Cenario(182, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 7, "CIF", "RC3"),
                new GetTpVeiculoBrasilTest_Cenario(183, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 9, "CIF", "RC4"),

                new GetTpVeiculoBrasilTest_Cenario(184, 0, EnumTipoProduto.Claros, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(185, 0, EnumTipoProduto.Escuros, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(186, 0, EnumTipoProduto.JET, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(187, 0, EnumTipoProduto.AVGas, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(188, 0, EnumTipoProduto.ARLA, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(189, 0, EnumTipoProduto.Claros, "0", 9, "FOB", "RFC"),
                new GetTpVeiculoBrasilTest_Cenario(190, 0, EnumTipoProduto.Escuros, "0", 9, "FOB", "RFE"),
                new GetTpVeiculoBrasilTest_Cenario(191, 0, EnumTipoProduto.JET, "0", 9, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(192, 0, EnumTipoProduto.AVGas, "0", 9, "FOB", "RFA"),
                new GetTpVeiculoBrasilTest_Cenario(193, 0, EnumTipoProduto.ARLA, "0", 9, "FOB", "RFC"),

                new GetTpVeiculoBrasilTest_Cenario(194, EnumTipoComposicao.Truck, 0, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(195, EnumTipoComposicao.Carreta, 0, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(196, EnumTipoComposicao.Bitrem, 0, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(197, EnumTipoComposicao.BitremDolly, 0, "0", 9, "CIF", ""),
                new GetTpVeiculoBrasilTest_Cenario(198, EnumTipoComposicao.Truck, 0, "0", 9, "FOB", ""),
                new GetTpVeiculoBrasilTest_Cenario(199, EnumTipoComposicao.Carreta, 0, "0", 9, "FOB", ""),
                new GetTpVeiculoBrasilTest_Cenario(200, EnumTipoComposicao.Bitrem, 0, "0", 9, "FOB", ""),
                new GetTpVeiculoBrasilTest_Cenario(201, EnumTipoComposicao.BitremDolly, 0, "0", 9, "FOB", ""),

                new GetTpVeiculoBrasilTest_Cenario(202, 0, 0, "0", 9, "ZZZ", "")
            };
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                Composicao composicao = new Composicao()
                {
                    Operacao = c.Operacao,
                    Metros = Convert.ToDecimal(c.Metros),
                    IDTipoComposicao = (int)c.TipoComposicao,
                    EixosComposicao = c.Eixos,
                    p1 = new Placa()
                    {
                        IDTipoProduto = (int)c.TipoProduto
                    },
                    p2 = new Placa()
                    {
                        IDTipoProduto = (int)c.TipoProduto
                    }
                };

                //Act
                var tipoCalculado = TipoVeiculoSAP.GetTpVeiculo(composicao);

                //Assert
                Assert.AreEqual(c.TipoEsperado, tipoCalculado, $"Erro no DataRow ID: {c.DataRowID}");
            }
        }

        private class GetTpVeiculoBrasilTest_Cenario
        {
            public GetTpVeiculoBrasilTest_Cenario(int dataRowID, EnumTipoComposicao tipoComposicao, EnumTipoProduto tipoProduto, string metros, int eixos, string operacao, string tipoEsperado)
            {
                DataRowID = dataRowID;
                TipoComposicao = tipoComposicao;
                TipoProduto = tipoProduto;
                Metros = metros;
                Eixos = eixos;
                Operacao = operacao;
                TipoEsperado = tipoEsperado;
            }

            public int DataRowID { get; set; }
            public EnumTipoComposicao TipoComposicao { get; set; }
            public EnumTipoProduto TipoProduto { get; set; }
            public string Metros { get; set; }
            public int Eixos { get; set; }
            public string Operacao { get; set; }
            public string TipoEsperado { get; set; }
        }

        #endregion

        #region Argentina

        [TestMethod]
        public void GetTpVeiculo_Argentina_Tests()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
              visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<GetTpVeiculoArgentinaTest_Cenario>()
            {
                new GetTpVeiculoArgentinaTest_Cenario(1, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.ClarosArg, "CIF", "RCS0"),
                new GetTpVeiculoArgentinaTest_Cenario(2, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.ClarosArg, "CIF", "RCS1"),
                
            };
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                Composicao composicao = new Composicao()
                {
                    Operacao = c.Operacao,
                    IDTipoComposicao = (int)c.TipoComposicao,
                    EixosComposicao = 2,
                    p2 = new Placa()
                    {
                        IDTipoProduto = (int)c.TipoProduto
                    }
                };

                //Act
                var tipoCalculado = TipoVeiculoSAP.GetTpVeiculo(composicao);

                //Assert
                Assert.AreEqual(c.TipoEsperado, tipoCalculado, $"Erro no DataRow ID: {c.DataRowID}");
            }
        }

        private class GetTpVeiculoArgentinaTest_Cenario
        {
            public GetTpVeiculoArgentinaTest_Cenario(int dataRowID, EnumTipoComposicao tipoComposicao, EnumTipoProduto tipoProduto, string operacao, string tipoEsperado)
            {
                DataRowID = dataRowID;
                TipoComposicao = tipoComposicao;
                TipoProduto = tipoProduto;
                Operacao = operacao;
                TipoEsperado = tipoEsperado;
            }

            public int DataRowID { get; set; }
            public EnumTipoComposicao TipoComposicao { get; set; }
            public EnumTipoProduto TipoProduto { get; set; }
            public string Operacao { get; set; }
            public string TipoEsperado { get; set; }
        }

        [TestMethod]
        public void GetTpVeiculo_SemPlaca02()
        {
            //Arrange
            Composicao composicao = new Composicao()
            {
                Operacao = "CIF",
                IDTipoComposicao = (int)EnumTipoComposicao.Truck,
                EixosComposicao = 2,
                p1 = new Placa()
                {
                    IDTipoProduto = (int)EnumTipoProduto.Claros
                }
            };

            //Act
            var tipoCalculado = TipoVeiculoSAP.GetTpVeiculo(composicao);

            //Assert
            Assert.AreEqual("RC1", tipoCalculado);
        }

        #endregion
    }
}
