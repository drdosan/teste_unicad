using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.Model;
using Raizen.UniCad.SAL.TipoVeiculoComposicao;
using System;

namespace Raizen.UniCad.BLLTests
{
    [TestClass]
    public class TipoVeiculoSAPTests
    {
        #region Brasil

        [DataTestMethod]
        [DataRow(1, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 1, "CIF", "")]
        [DataRow(2, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 2, "CIF", "RC1")]
        [DataRow(3, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 3, "CIF", "RC1")]
        [DataRow(4, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 4, "CIF", "RC1")]
        [DataRow(5, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 1, "CIF", "")]
        [DataRow(6, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 2, "CIF", "RE1")]
        [DataRow(7, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 3, "CIF", "RE1")]
        [DataRow(8, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 4, "CIF", "RE1")]
        [DataRow(9, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 1, "CIF", "")]
        [DataRow(10, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 2, "CIF", "RA1")]
        [DataRow(11, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 3, "CIF", "RA1")]
        [DataRow(12, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 4, "CIF", "RA1")]
        [DataRow(13, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 1, "CIF", "")]
        [DataRow(14, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 2, "CIF", "RA1")]
        [DataRow(15, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 3, "CIF", "RA1")]
        [DataRow(16, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 4, "CIF", "RA1")]
        [DataRow(17, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6")]
        [DataRow(18, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6")]
        [DataRow(19, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6")]
        [DataRow(20, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6")]        
        [DataRow(21, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC")]
        [DataRow(22, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC")]
        [DataRow(23, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC")]
        [DataRow(24, EnumTipoComposicao.Truck, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC")]
        [DataRow(25, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE")]
        [DataRow(26, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE")]
        [DataRow(27, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE")]
        [DataRow(28, EnumTipoComposicao.Truck, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE")]
        [DataRow(29, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 1, "FOB", "RFA")]
        [DataRow(30, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 2, "FOB", "RFA")]
        [DataRow(31, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 3, "FOB", "RFA")]
        [DataRow(32, EnumTipoComposicao.Truck, EnumTipoProduto.JET, "0", 4, "FOB", "RFA")]
        [DataRow(33, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA")]
        [DataRow(34, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA")]
        [DataRow(35, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA")]
        [DataRow(36, EnumTipoComposicao.Truck, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA")]
        [DataRow(37, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC")]
        [DataRow(38, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC")]
        [DataRow(39, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC")]
        [DataRow(40, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC")]

        [DataRow(41, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 1, "CIF", "")]
        [DataRow(42, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 2, "CIF", "")]
        [DataRow(43, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 3, "CIF", "")]
        [DataRow(44, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 4, "CIF", "RC5")]
        [DataRow(45, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 1, "CIF", "")]
        [DataRow(46, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 2, "CIF", "")]
        [DataRow(47, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 3, "CIF", "")]
        [DataRow(48, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 4, "CIF", "RE2")]
        [DataRow(49, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 1, "CIF", "")]
        [DataRow(50, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 2, "CIF", "")]
        [DataRow(51, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 3, "CIF", "")]
        [DataRow(52, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 4, "CIF", "RA5")]
        [DataRow(53, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 1, "CIF", "")]
        [DataRow(54, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 2, "CIF", "")]
        [DataRow(55, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 3, "CIF", "")]
        [DataRow(56, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 4, "CIF", "RA5")]
        [DataRow(57, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6")]
        [DataRow(58, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6")]
        [DataRow(59, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6")]
        [DataRow(60, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6")]
        [DataRow(61, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC")]
        [DataRow(62, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC")]
        [DataRow(63, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC")]
        [DataRow(64, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC")]
        [DataRow(65, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE")]
        [DataRow(66, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE")]
        [DataRow(67, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE")]
        [DataRow(68, EnumTipoComposicao.Carreta, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE")]
        [DataRow(69, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 1, "FOB", "RFA")]
        [DataRow(70, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 2, "FOB", "RFA")]
        [DataRow(71, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 3, "FOB", "RFA")]
        [DataRow(72, EnumTipoComposicao.Carreta, EnumTipoProduto.JET, "0", 4, "FOB", "RFA")]
        [DataRow(73, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA")]
        [DataRow(74, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA")]
        [DataRow(75, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA")]
        [DataRow(76, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA")]
        [DataRow(77, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC")]
        [DataRow(78, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC")]
        [DataRow(79, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC")]
        [DataRow(80, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC")]

        [DataRow(81, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 1, "CIF", "")]
        [DataRow(82, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 2, "CIF", "")]
        [DataRow(83, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 3, "CIF", "")]
        [DataRow(84, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 4, "CIF", "")]
        [DataRow(85, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 1, "CIF", "")]
        [DataRow(86, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 2, "CIF", "")]
        [DataRow(87, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 3, "CIF", "")]
        [DataRow(88, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 4, "CIF", "")]
        [DataRow(89, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 1, "CIF", "")]
        [DataRow(90, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 2, "CIF", "")]
        [DataRow(91, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 3, "CIF", "")]
        [DataRow(92, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 4, "CIF", "")]
        [DataRow(93, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 1, "CIF", "")]
        [DataRow(94, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 2, "CIF", "")]
        [DataRow(95, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 3, "CIF", "")]
        [DataRow(96, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 4, "CIF", "")]
        [DataRow(97, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6")]
        [DataRow(98, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6")]
        [DataRow(99, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6")]
        [DataRow(100, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6")]
        [DataRow(101, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC")]
        [DataRow(102, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC")]
        [DataRow(103, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC")]
        [DataRow(104, EnumTipoComposicao.Bitrem, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC")]
        [DataRow(105, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE")]
        [DataRow(106, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE")]
        [DataRow(107, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE")]
        [DataRow(108, EnumTipoComposicao.Bitrem, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE")]
        [DataRow(109, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 1, "FOB", "RFA")]
        [DataRow(110, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 2, "FOB", "RFA")]
        [DataRow(111, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 3, "FOB", "RFA")]
        [DataRow(112, EnumTipoComposicao.Bitrem, EnumTipoProduto.JET, "0", 4, "FOB", "RFA")]
        [DataRow(113, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA")]
        [DataRow(114, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA")]
        [DataRow(115, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA")]
        [DataRow(116, EnumTipoComposicao.Bitrem, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA")]
        [DataRow(111, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC")]
        [DataRow(118, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC")]
        [DataRow(119, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC")]
        [DataRow(120, EnumTipoComposicao.Bitrem, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC")]

        [DataRow(121, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 1, "CIF", "")]
        [DataRow(122, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 2, "CIF", "")]
        [DataRow(123, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 3, "CIF", "")]
        [DataRow(124, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 4, "CIF", "")]
        [DataRow(125, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 1, "CIF", "")]
        [DataRow(126, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 2, "CIF", "")]
        [DataRow(127, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 3, "CIF", "")]
        [DataRow(128, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 4, "CIF", "")]
        [DataRow(129, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 1, "CIF", "")]
        [DataRow(130, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 2, "CIF", "")]
        [DataRow(131, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 3, "CIF", "")]
        [DataRow(132, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 4, "CIF", "")]
        [DataRow(133, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 1, "CIF", "")]
        [DataRow(134, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 2, "CIF", "")]
        [DataRow(135, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 3, "CIF", "")]
        [DataRow(136, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 4, "CIF", "")]
        [DataRow(137, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 1, "CIF", "RC6")]
        [DataRow(138, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 2, "CIF", "RC6")]
        [DataRow(139, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 3, "CIF", "RC6")]
        [DataRow(140, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 4, "CIF", "RC6")]
        [DataRow(141, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 1, "FOB", "RFC")]
        [DataRow(142, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 2, "FOB", "RFC")]
        [DataRow(143, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 3, "FOB", "RFC")]
        [DataRow(144, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 4, "FOB", "RFC")]
        [DataRow(145, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 1, "FOB", "RFE")]
        [DataRow(146, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 2, "FOB", "RFE")]
        [DataRow(147, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 3, "FOB", "RFE")]
        [DataRow(148, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 4, "FOB", "RFE")]
        [DataRow(149, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 1, "FOB", "RFA")]
        [DataRow(150, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 2, "FOB", "RFA")]
        [DataRow(151, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 3, "FOB", "RFA")]
        [DataRow(152, EnumTipoComposicao.BitremDolly, EnumTipoProduto.JET, "0", 4, "FOB", "RFA")]
        [DataRow(153, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 1, "FOB", "RFA")]
        [DataRow(154, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 2, "FOB", "RFA")]
        [DataRow(155, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 3, "FOB", "RFA")]
        [DataRow(156, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 4, "FOB", "RFA")]
        [DataRow(151, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 1, "FOB", "RFC")]
        [DataRow(158, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 2, "FOB", "RFC")]
        [DataRow(159, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 3, "FOB", "RFC")]
        [DataRow(160, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "0", 4, "FOB", "RFC")]

        [DataRow(161, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "20000", 4, "CIF", "RC8")]
        [DataRow(162, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "25000", 4, "CIF", "RC8")]
        [DataRow(163, EnumTipoComposicao.Truck, EnumTipoProduto.ARLA, "30000", 4, "CIF", "RC9")]

        [DataRow(164, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 5, "CIF", "RA2")]
        [DataRow(165, EnumTipoComposicao.Carreta, EnumTipoProduto.AVGas, "0", 6, "CIF", "RA2")]
        [DataRow(161, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 5, "CIF", "RC2")]
        [DataRow(162, EnumTipoComposicao.Carreta, EnumTipoProduto.Claros, "0", 6, "CIF", "RC2")]
        [DataRow(163, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "20000", 4, "CIF", "RC8")]
        [DataRow(164, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "25000", 4, "CIF", "RC8")]
        [DataRow(165, EnumTipoComposicao.Carreta, EnumTipoProduto.ARLA, "30000", 4, "CIF", "RC9")]

        [DataRow(166, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "20000", 4, "CIF", "RC8")]
        [DataRow(167, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "25000", 4, "CIF", "RC8")]
        [DataRow(168, EnumTipoComposicao.BitremDolly, EnumTipoProduto.ARLA, "30000", 4, "CIF", "RC9")]
        [DataRow(169, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 7, "CIF", "RE3")]
        [DataRow(180, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Escuros, "0", 9, "CIF", "RE3")]
        [DataRow(181, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 7, "CIF", "RA3")]
        [DataRow(181, EnumTipoComposicao.BitremDolly, EnumTipoProduto.AVGas, "0", 9, "CIF", "RA4")]
        [DataRow(182, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 7, "CIF", "RC3")]
        [DataRow(183, EnumTipoComposicao.BitremDolly, EnumTipoProduto.Claros, "0", 9, "CIF", "RC4")]

        [DataRow(184, -1, EnumTipoProduto.Claros, "0", 9, "CIF", "")]
        [DataRow(185, -1, EnumTipoProduto.Escuros, "0", 9, "CIF", "")]
        [DataRow(186, -1, EnumTipoProduto.JET, "0", 9, "CIF", "")]
        [DataRow(187, -1, EnumTipoProduto.AVGas, "0", 9, "CIF", "")]
        [DataRow(188, -1, EnumTipoProduto.ARLA, "0", 9, "CIF", "")]
        [DataRow(189, -1, EnumTipoProduto.Claros, "0", 9, "FOB", "RFC")]
        [DataRow(190, -1, EnumTipoProduto.Escuros, "0", 9, "FOB", "RFE")]
        [DataRow(191, -1, EnumTipoProduto.JET, "0", 9, "FOB", "RFA")]
        [DataRow(192, -1, EnumTipoProduto.AVGas, "0", 9, "FOB", "RFA")]
        [DataRow(193, -1, EnumTipoProduto.ARLA, "0", 9, "FOB", "RFC")]

        [DataRow(194, EnumTipoComposicao.Truck, -1, "0", 9, "CIF", "")]
        [DataRow(195, EnumTipoComposicao.Carreta, -1, "0", 9, "CIF", "")]
        [DataRow(196, EnumTipoComposicao.Bitrem, -1, "0", 9, "CIF", "")]
        [DataRow(197, EnumTipoComposicao.BitremDolly, -1, "0", 9, "CIF", "")]
        [DataRow(198, EnumTipoComposicao.Truck, -1, "0", 9, "FOB", "")]
        [DataRow(199, EnumTipoComposicao.Carreta, -1, "0", 9, "FOB", "")]
        [DataRow(200, EnumTipoComposicao.Bitrem, -1, "0", 9, "FOB", "")]
        [DataRow(201, EnumTipoComposicao.BitremDolly, -1, "0", 9, "FOB", "")]

        [DataRow(202, -1, -1, "0", 9, "ZZZ", "")]

        public void GetTpVeiculo_Brasil_Tests(int dataRowID, EnumTipoComposicao tipoComposicao, EnumTipoProduto tipoProduto, string metros, int eixos, string operacao, string tipoEsperado)
        {
            //Arrange
            Composicao composicao = new Composicao()
            {
                Operacao = operacao,
                Metros = Convert.ToDecimal(metros),
                IDTipoComposicao = (int)tipoComposicao,
                EixosComposicao = eixos,
                p1 = new Placa()
                {
                    IDTipoProduto = (int)tipoProduto
                },
                p2 = new Placa()
                {
                    IDTipoProduto = (int)tipoProduto
                }
            };

            //Act
            var tipoCalculado = TipoVeiculoSAP.GetTpVeiculo(composicao);

            //Assert
            Assert.AreEqual(tipoEsperado, tipoCalculado, $"Erro no DataRow ID: {dataRowID}");
        }

        #endregion

        #region Argentina

        [DataTestMethod]
        [DataRow(1, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.ClarosArg, "CIF", "RCS0")]
        [DataRow(2, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.ClarosArg, "CIF", "RCS1")]
        [DataRow(3, EnumTipoComposicao.Escalado, EnumTipoProduto.ClarosArg, "CIF", "RCS2")]
        [DataRow(4, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.ClarosArg, "FOB", "RFCS")]
        [DataRow(5, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.ClarosArg, "FOB", "RFCS")]
        [DataRow(6, EnumTipoComposicao.Escalado, EnumTipoProduto.ClarosArg, "FOB", "RFCS")]
        [DataRow(7, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.JETArg, "CIF", "RAS0")]
        [DataRow(8, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.JETArg, "CIF", "RAS1")]
        [DataRow(9, EnumTipoComposicao.Escalado, EnumTipoProduto.JETArg, "CIF", "RAS2")]
        [DataRow(10, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.JETArg, "FOB", "RFAS")]
        [DataRow(11, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.JETArg, "FOB", "RFAS")]
        [DataRow(12, EnumTipoComposicao.Escalado, EnumTipoProduto.JETArg, "FOB", "RFAS")]
        [DataRow(13, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.Quimicos, "CIF", "RQS0")]
        [DataRow(14, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.Quimicos, "CIF", "RQS1")]
        [DataRow(15, EnumTipoComposicao.Escalado, EnumTipoProduto.Quimicos, "CIF", "RQS2")]
        [DataRow(16, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.Quimicos, "FOB", "RFQS")]
        [DataRow(17, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.Quimicos, "FOB", "RFQS")]
        [DataRow(18, EnumTipoComposicao.Escalado, EnumTipoProduto.Quimicos, "FOB", "RFQS")]
        [DataRow(19, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.Asfaltos, "CIF", "RES0")]
        [DataRow(20, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.Asfaltos, "CIF", "RES1")]
        [DataRow(21, EnumTipoComposicao.Escalado, EnumTipoProduto.Asfaltos, "CIF", "RES2")]
        [DataRow(22, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.Asfaltos, "FOB", "RFES")]
        [DataRow(23, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.Asfaltos, "FOB", "RFES")]
        [DataRow(24, EnumTipoComposicao.Escalado, EnumTipoProduto.Asfaltos, "FOB", "RFES")]
        [DataRow(25, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.Coque, "CIF", "ROS0")]
        [DataRow(26, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.Coque, "CIF", "ROS1")]
        [DataRow(27, EnumTipoComposicao.Escalado, EnumTipoProduto.Coque, "CIF", "ROS2")]
        [DataRow(28, EnumTipoComposicao.SemirremolqueChico, EnumTipoProduto.Coque, "FOB", "RFOS")]
        [DataRow(29, EnumTipoComposicao.SemirremolqueGrande, EnumTipoProduto.Coque, "FOB", "RFOS")]
        [DataRow(30, EnumTipoComposicao.Escalado, EnumTipoProduto.Coque, "FOB", "RFOS")]
        [DataRow(31, EnumTipoComposicao.BitrenChico, EnumTipoProduto.ClarosArg, "CIF", "RCB0")]
        [DataRow(32, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.ClarosArg, "CIF", "RCB1")]
        [DataRow(33, EnumTipoComposicao.BitrenChico, EnumTipoProduto.ClarosArg, "FOB", "RFCB")]
        [DataRow(34, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.ClarosArg, "FOB", "RFCB")]
        [DataRow(35, EnumTipoComposicao.BitrenChico, EnumTipoProduto.JETArg, "CIF", "RAB0")]
        [DataRow(36, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.JETArg, "CIF", "RAB1")]
        [DataRow(37, EnumTipoComposicao.BitrenChico, EnumTipoProduto.JETArg, "FOB", "RFAB")]
        [DataRow(38, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.JETArg, "FOB", "RFAB")]
        [DataRow(39, EnumTipoComposicao.BitrenChico, EnumTipoProduto.Quimicos, "CIF", "RQB0")]
        [DataRow(40, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.Quimicos, "CIF", "RQB1")]
        [DataRow(41, EnumTipoComposicao.BitrenChico, EnumTipoProduto.Quimicos, "FOB", "RFQB")]
        [DataRow(42, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.Quimicos, "FOB", "RFQB")]
        [DataRow(43, EnumTipoComposicao.BitrenChico, EnumTipoProduto.Asfaltos, "CIF", "REB0")]
        [DataRow(44, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.Asfaltos, "CIF", "REB1")]
        [DataRow(45, EnumTipoComposicao.BitrenChico, EnumTipoProduto.Asfaltos, "FOB", "RFEB")]
        [DataRow(46, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.Asfaltos, "FOB", "RFEB")]
        [DataRow(47, EnumTipoComposicao.BitrenChico, EnumTipoProduto.Coque, "CIF", "ROB0")]
        [DataRow(48, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.Coque, "CIF", "ROB1")]
        [DataRow(49, EnumTipoComposicao.BitrenChico, EnumTipoProduto.Coque, "FOB", "RFOB")]
        [DataRow(50, EnumTipoComposicao.BitrenGrande, EnumTipoProduto.Coque, "FOB", "RFOB")]

        [DataRow(51, EnumTipoComposicao.SemirremolqueChico, -1, "CIF", "")]
        [DataRow(52, EnumTipoComposicao.SemirremolqueGrande, -1, "CIF", "")]
        [DataRow(53, EnumTipoComposicao.Escalado, -1, "CIF", "")]
        [DataRow(54, EnumTipoComposicao.BitrenChico, -1, "CIF", "")]
        [DataRow(55, EnumTipoComposicao.BitrenGrande, -1, "CIF", "")]

        [DataRow(56, -1, EnumTipoProduto.ClarosArg, "FOB", "")]
        [DataRow(57, -1, EnumTipoProduto.JETArg, "FOB", "")]
        [DataRow(58, -1, EnumTipoProduto.Quimicos, "FOB", "")]
        [DataRow(59, -1, EnumTipoProduto.Asfaltos, "FOB", "")]
        [DataRow(60, -1, EnumTipoProduto.Coque, "FOB", "")]

        [DataRow(61, -1, -1, "ZZZ", "")]
        
        public void GetTpVeiculo_Argentina_Tests(int dataRowID, EnumTipoComposicao tipoComposicao, EnumTipoProduto tipoProduto, string operacao, string tipoEsperado)
        {
            //Arrange
            Composicao composicao = new Composicao()
            {
                Operacao = operacao,
                IDTipoComposicao = (int)tipoComposicao,
                p2 = new Placa()
                {
                    IDTipoProduto = (int)tipoProduto
                }
            };

            //Act
            var tipoCalculado = TipoVeiculoSAP.GetTpVeiculo(composicao);

            //Assert
            Assert.AreEqual(tipoEsperado, tipoCalculado, $"Erro no DataRow ID: {dataRowID}");
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
