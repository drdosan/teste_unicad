using Raizen.UniCad.Model;

namespace Raizen.UniCad.SAL.TipoVeiculoComposicao
{
    public static class TipoVeiculoSAP
    {
        public const string RFC = "RFC";
        public const string RFE = "RFE";
        public const string RFA = "RFA";
        public const string RA1 = "RA1";
        public const string RA2 = "RA2";
        public const string RA3 = "RA3";
        public const string RA4 = "RA4";
        public const string RA5 = "RA5";
        public const string RC1 = "RC1";
        public const string RC2 = "RC2";
        public const string RC3 = "RC3";
        public const string RC4 = "RC4";
        public const string RC5 = "RC5";
        public const string RE1 = "RE1";
        public const string RE2 = "RE2";
        public const string RE3 = "RE3";
        public const string RC6 = "RC6";
        public const string RC8 = "RC8";
        public const string RC9 = "RC9";
        public const string RCS0 = "RCS0";
        public const string RCS1 = "RCS1";
        public const string RCS2 = "RCS2";
        public const string RFCS = "RFCS";
        public const string RAS0 = "RAS0";
        public const string RAS1 = "RAS1";
        public const string RAS2 = "RAS2";
        public const string RFAS = "RFAS";
        public const string RQS0 = "RQS0";
        public const string RQS1 = "RQS1";
        public const string RQS2 = "RQS2";
        public const string RFQS = "RFQS";
        public const string RES0 = "RES0";
        public const string RES1 = "RES1";
        public const string RES2 = "RES2";
        public const string RFES = "RFES";
        public const string ROS0 = "ROS0";
        public const string ROS1 = "ROS1";
        public const string ROS2 = "ROS2";
        public const string RFOS = "RFOS";
        public const string RCB0 = "RCB0";
        public const string RCB1 = "RCB1";
        public const string RFCB = "RFCB";
        public const string RAB0 = "RAB0";
        public const string RAB1 = "RAB1";
        public const string RFAB = "RFAB";
        public const string RQB0 = "RQB0";
        public const string RQB1 = "RQB1";
        public const string RFQB = "RFQB";
        public const string REB0 = "REB0";
        public const string REB1 = "REB1";
        public const string RFEB = "RFEB";
        public const string ROB0 = "ROB0";
        public const string ROB1 = "ROB1";
        public const string RFOB = "RFOB";
        public const string RFA0 = "RFA0";
        public const string RGA0 = "RGA0";
        public const string RCS3 = "RCS3";
        public const string RCS4 = "RCS4";
        public const string RCS5 = "RCS5";
        public const string RAS3 = "RAS3";
        public const string RLS1 = "RLS1";
        public const string RLS2 = "RLS2";
        public const string RFLB = "RFLB";
        public const string RFLS = "RFLS";
        public const string RFLU = "RFLU";


        public static string GetTpVeiculo(Model.Composicao composicao)
        {
            switch (composicao.Operacao)
            {
                case "FOB":
                    return PopularComposicaoFOB(composicao);
                case "CIF":
                    return PopularComposicaoCIF(composicao);
                default:
                    return string.Empty;
            }
        }

        private static string PopularComposicaoCIF(Composicao composicao)
        {
            var metros = composicao.Metros / 1000;
            int idTipoProdutoP1 = 0, idTipoProdutoP2 = 0;

            if (composicao.p1 != null)
                idTipoProdutoP1 = composicao.p1.IDTipoProduto.HasValue ? composicao.p1.IDTipoProduto.Value : 0;

            if (composicao.p2 != null)
                idTipoProdutoP2 = composicao.p2.IDTipoProduto.HasValue ? composicao.p2.IDTipoProduto.Value : 0;


            switch (composicao.IDTipoComposicao)
            {
                case (int)EnumTipoComposicao.Truck:
                    switch (idTipoProdutoP1)
                    {
                        case (int)EnumTipoProduto.JET:
                        case (int)EnumTipoProduto.AVGas:
                            if (composicao.EixosComposicao == 2 || composicao.EixosComposicao == 3 || composicao.EixosComposicao == 4)
                                return RA1;
                            else
                                return string.Empty;
                        case (int)EnumTipoProduto.ARLA:
                            {
                                if (metros <= 15)
                                    return RC6;
                                else if (metros > 15 && metros <= 25)
                                    return RC8;
                                else
                                    return RC9;
                            }
                        case (int)EnumTipoProduto.Claros:
                            if (composicao.EixosComposicao == 2 || composicao.EixosComposicao == 3 || composicao.EixosComposicao == 4)
                                return RC1;
                            else
                                return string.Empty;
                        case (int)EnumTipoProduto.Escuros:
                            if (composicao.EixosComposicao == 2 || composicao.EixosComposicao == 3 || composicao.EixosComposicao == 4)
                                return RE1;
                            return string.Empty;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.Carreta:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.JET:
                        case (int)EnumTipoProduto.AVGas:
                            if (composicao.EixosComposicao == 4)
                                return RA5;
                            else if (composicao.EixosComposicao == 5 || composicao.EixosComposicao == 6)
                                return RA2;
                            else
                                return string.Empty;
                        case (int)EnumTipoProduto.ARLA:
                            {
                                if (metros <= 15)
                                    return RC6;
                                else if (metros > 15 && metros <= 25)
                                    return RC8;
                                else
                                    return RC9;
                            }
                        case (int)EnumTipoProduto.Claros:
                            if (composicao.EixosComposicao == 4)
                                return RC5;
                            else if (composicao.EixosComposicao == 5 || composicao.EixosComposicao == 6)
                                return RC2;
                            else
                                return string.Empty;
                        case (int)EnumTipoProduto.Escuros:
                            if (composicao.EixosComposicao == 4 || composicao.EixosComposicao == 5 || composicao.EixosComposicao == 6)
                                return RE2;
                            else
                                return string.Empty;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.Bitrem:
                case (int)EnumTipoComposicao.BitremDolly:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.JET:
                        case (int)EnumTipoProduto.AVGas:
                            if (composicao.EixosComposicao == 7)
                                return RA3;
                            else if (composicao.EixosComposicao == 9)
                                return RA4;
                            else
                                return string.Empty;
                        case (int)EnumTipoProduto.ARLA:
                            {
                                if (metros <= 15)
                                    return RC6;
                                else if (metros > 15 && metros <= 25)
                                    return RC8;
                                else
                                    return RC9;
                            }
                        case (int)EnumTipoProduto.Claros:
                            if (composicao.EixosComposicao == 7)
                                return RC3;
                            else if (composicao.EixosComposicao == 9)
                                return RC4;
                            else
                                return string.Empty;
                        case (int)EnumTipoProduto.Escuros:
                            if (composicao.EixosComposicao == 7 || composicao.EixosComposicao == 9)
                                return RE3;
                            else
                                return string.Empty;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.SemirremolqueChico:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.Asfaltos:
                            return RES0;
                        case (int)EnumTipoProduto.Coque:
                            return ROS0;
                        case (int)EnumTipoProduto.ClarosArg:
                            return RCS0;
                        case (int)EnumTipoProduto.JETArg:
                            return RAS0;
                        case (int)EnumTipoProduto.Quimicos:
                            return RQS0;
                        case (int)EnumTipoProduto.GLP:
                            return RGA0;
                        case (int)EnumTipoProduto.Lubrific:
                            return RLS1;

                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.SemirremolqueGrande:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.Asfaltos:
                            return RES1;
                        case (int)EnumTipoProduto.Coque:
                            return ROS1;
                        case (int)EnumTipoProduto.ClarosArg:
                            if (composicao.TipoContratacao == (int)EnumTipoContratacaoArgentina.EnTransito)
                            {
                                return RCS3;
                            }
                            else
                                if (composicao.TipoContratacao == (int)EnumTipoContratacaoArgentina.Spot)
                            {
                                return RCS4;
                            }
                            return RCS1;
                        case (int)EnumTipoProduto.JETArg:
                            if (composicao.TipoContratacao == (int)EnumTipoContratacaoArgentina.Spot)
                            {
                                return RAS3;
                            }
                            return RAS1;
                        case (int)EnumTipoProduto.Quimicos:
                            return RQS1;
                        case (int)EnumTipoProduto.GLP:
                            return RGA0;

                        case (int)EnumTipoProduto.Lubrific:
                            return RLS2;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.Escalado:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.Asfaltos:
                            return RES2;
                        case (int)EnumTipoProduto.Coque:
                            return ROS2;
                        case (int)EnumTipoProduto.ClarosArg:
                            if (composicao.TipoContratacao == (int)EnumTipoContratacaoArgentina.Spot)
                            {
                                return RCS5;
                            }
                            return RCS2;
                        case (int)EnumTipoProduto.JETArg:
                            return RAS2;
                        case (int)EnumTipoProduto.Quimicos:
                            return RQS2;
                        case (int)EnumTipoProduto.GLP:
                            return RGA0;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.BitrenChico:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.Asfaltos:
                            return REB0;
                        case (int)EnumTipoProduto.Coque:
                            return ROB0;
                        case (int)EnumTipoProduto.ClarosArg:
                            return RCB0;
                        case (int)EnumTipoProduto.JETArg:
                            return RAB0;
                        case (int)EnumTipoProduto.Quimicos:
                            return RQB0;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoComposicao.BitrenGrande:
                    switch (idTipoProdutoP2)
                    {
                        case (int)EnumTipoProduto.Asfaltos:
                            return REB1;
                        case (int)EnumTipoProduto.Coque:
                            return ROB1;
                        case (int)EnumTipoProduto.ClarosArg:
                            return RCB1;
                        case (int)EnumTipoProduto.JETArg:
                            return RAB1;
                        case (int)EnumTipoProduto.Quimicos:
                            return RQB1;
                        default:
                            return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

        private static string PopularComposicaoFOB(Composicao composicao)
        {

            int idTipoProdutoP1 = 0, idTipoProdutoP2 = 0;

            if (composicao.p1 != null)
                idTipoProdutoP1 = composicao.p1.IDTipoProduto.HasValue ? composicao.p1.IDTipoProduto.Value : 0;

            if (composicao.p2 != null)
                idTipoProdutoP2 = composicao.p2.IDTipoProduto.HasValue ? composicao.p2.IDTipoProduto.Value : 0;


            switch (idTipoProdutoP2 != 0 ? idTipoProdutoP2 : idTipoProdutoP1)
            {
                case (int)EnumTipoProduto.Claros:
                case (int)EnumTipoProduto.ARLA:
                    return RFC;
                case (int)EnumTipoProduto.Escuros:
                    return RFE;
                case (int)EnumTipoProduto.JET:
                case (int)EnumTipoProduto.AVGas:
                    return RFA;
                case (int)EnumTipoProduto.Asfaltos:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                        case (int)EnumTipoComposicao.SemirremolqueGrande:
                        case (int)EnumTipoComposicao.Escalado:
                            return RFES;
                        case (int)EnumTipoComposicao.BitrenChico:
                        case (int)EnumTipoComposicao.BitrenGrande:
                            return RFEB;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoProduto.Coque:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                        case (int)EnumTipoComposicao.SemirremolqueGrande:
                        case (int)EnumTipoComposicao.Escalado:
                            return RFOS;
                        case (int)EnumTipoComposicao.BitrenChico:
                        case (int)EnumTipoComposicao.BitrenGrande:
                            return RFOB;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoProduto.ClarosArg:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                        case (int)EnumTipoComposicao.SemirremolqueGrande:
                        case (int)EnumTipoComposicao.Escalado:
                            return RFCS;
                        case (int)EnumTipoComposicao.BitrenChico:
                        case (int)EnumTipoComposicao.BitrenGrande:
                            return RFCB;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoProduto.JETArg:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                        case (int)EnumTipoComposicao.SemirremolqueGrande:
                        case (int)EnumTipoComposicao.Escalado:
                            return RFAS;
                        case (int)EnumTipoComposicao.BitrenChico:
                        case (int)EnumTipoComposicao.BitrenGrande:
                            return RFAB;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoProduto.Quimicos:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                        case (int)EnumTipoComposicao.SemirremolqueGrande:
                        case (int)EnumTipoComposicao.Escalado:
                            return RFQS;
                        case (int)EnumTipoComposicao.BitrenChico:
                        case (int)EnumTipoComposicao.BitrenGrande:
                            return RFQB;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoProduto.GLP:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                        case (int)EnumTipoComposicao.SemirremolqueGrande:
                        case (int)EnumTipoComposicao.Escalado:
                            return RFA0;
                        default:
                            return string.Empty;
                    }
                case (int)EnumTipoProduto.Lubrific:
                    switch (composicao.IDTipoComposicao)
                    {
                        case (int)EnumTipoComposicao.SemirremolqueChico:
                            return RFLS;
                        case (int)EnumTipoComposicao.BitrenChico:
                            return RFLB;
                        case (int)EnumTipoComposicao.Truck_ARG:
                            return RFLU;
                        default:
                            return string.Empty;
                    }

                default:
                    return string.Empty;
            }
        }
    }
}
