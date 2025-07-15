using Raizen.Framework.Utils.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL.TipoVeiculoComposicao;
using Raizen.UniCad.SAL.WSIntegracaoSAP;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Raizen.UniCad.SAL
{
    public class WsIntegraSAPAR_Veiculo
    {
        public string IntegrarComposicao(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
            string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAP"];
            StringBuilder retorno = new StringBuilder();
            string retornoCt = string.Empty;
            string retornoUt = string.Empty;
            string retornoVc = string.Empty;
            //CREDENCIAIS
            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            var client = new WSIntegracaoSAP.Veiculo_OutService();
            client.Credentials = credentials;
            client.Url = URL;
            switch (tipoIntegracao)
            {
                case EnumTipoIntegracaoSAP.Inclusao:
                    var retornoInclusao = Incluir(composicao, client);
                    if (retornoInclusao.ComposicaoTransporte.Any(w => w.TpMsg == "E"))
                    {
                        retornoCt = retornoInclusao.ComposicaoTransporte.First(w => w.TpMsg == "E").Msg;
                        if (!retornoInclusao.ComposicaoTransporte.First(w => w.TpMsg == "E").Msg.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }
                    if (retornoInclusao.UnidadeTransporte.Any(w => w.TpMsg == "E"))
                    {
                        retornoUt = retornoInclusao.UnidadeTransporte.First(w => w.TpMsg == "E").Msg + "</br>";
                        if (!retornoInclusao.UnidadeTransporte.First(w => w.TpMsg == "E").Msg.Contains("já existe"))
                        {
                            retorno.Append(retornoUt);
                            break;
                        }
                    }
                    if (retornoInclusao.VeiculoAr.Any(w => w.TpMsg == "E"))
                    {
                        retornoVc = retornoInclusao.VeiculoAr.First(w => w.TpMsg == "E").Msg + "</br>";
                        if (!retornoInclusao.VeiculoAr.First(w => w.TpMsg == "E").Msg.Contains("já existe"))
                        {
                            retorno.Append(retornoVc);
                            break;
                        }
                    }

                    retorno.Append(retornoCt);
                    retorno.Append(retornoUt);
                    retorno.Append(retornoVc);

                    if (retorno.ToString().ToLower(CultureInfo.InvariantCulture).Contains("já existe"))
                        composicao.jaExiste = true;
                    break;
                case EnumTipoIntegracaoSAP.Alteracao:
                case EnumTipoIntegracaoSAP.Bloqueio:
                case EnumTipoIntegracaoSAP.Desbloqueio:
                case EnumTipoIntegracaoSAP.AprovarCheckList:
                case EnumTipoIntegracaoSAP.ReprovarCheckList:
                    var retornoAlteracao = Alterar(composicao, client, tipoIntegracao);
                    if (retornoAlteracao.ComposicaoTransporte != null && retornoAlteracao.ComposicaoTransporte.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoAlteracao.ComposicaoTransporte.First(w => w.TpMsg == "E").Msg + "</br>");
                    if (retornoAlteracao.UnidadeTransporte != null && retornoAlteracao.UnidadeTransporte.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoAlteracao.UnidadeTransporte.First(w => w.TpMsg == "E").Msg + "</br>");
                    if (retornoAlteracao.VeiculoAr != null && retornoAlteracao.VeiculoAr.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoAlteracao.VeiculoAr.First(w => w.TpMsg == "E").Msg);
                    break;
                case EnumTipoIntegracaoSAP.Excluir:
                    var retornoExclusao = Excluir(composicao, client);

                    if (retornoExclusao.VeiculoAr != null && retornoExclusao.VeiculoAr.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoExclusao.VeiculoAr.First(w => w.TpMsg == "E").Msg);
                    break;
                default:
                    break;
            }

            return retorno.ToString();
        }

        private WSIntegracaoSAP.EliminarResponse Excluir(Composicao composicao, Veiculo_OutService client)
        {
            var request = new WSIntegracaoSAP.EliminarRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "ES";
            request.VeiculoAr = CriarVeiculoAr(composicao, EnumTipoIntegracaoSAP.Excluir);
            var retorno = client.Eliminar_Sync(request);
            return retorno;
        }

        private WSIntegracaoSAP.CriarResponse Incluir(Composicao composicao, Veiculo_OutService client)
        {
            var request = new WSIntegracaoSAP.CriarRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "ES";
            request.ComposicaoTransporte = CriarComposicaoTransporte(composicao, EnumTipoIntegracaoSAP.Inclusao);
            request.UnidadeTransporte = CriarUnidadeTransporte(composicao, EnumTipoIntegracaoSAP.Inclusao);
            request.VeiculoAr = CriarVeiculoAr(composicao, EnumTipoIntegracaoSAP.Inclusao);

            var retorno = client.Criar_Sync(request);

            return retorno;
        }

        private VeiculoAr[] CriarVeiculoAr(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            VeiculoAr[] veiculoAr = new VeiculoAr[1];
            veiculoAr[0] = new VeiculoAr();

            #region Campos comuns

            veiculoAr[0].RegCntry = "AR";
            veiculoAr[0].Multiflecha = VerificaMultiFlecha(composicao); // Dever ser o campo multiseta de qual placa: p1, p2, p3 ou p4 ?
            veiculoAr[0].Route = null; // Edu (Raizen) via whatsapp: "Route itinerário o unicad não envia"
            veiculoAr[0].TuNumber = ObtemValorTuNumber(composicao, tipoIntegracao);
            veiculoAr[0].Vehicle = ObtemPlacaOperacaoO4V1SAP(composicao);
            veiculoAr[0].TraPatente = ObtemPlacaVeiculo(composicao, 1);
            veiculoAr[0].Sem2Patente = ObtemPlacaVeiculo(composicao, 3);

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste
              || ((tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                  && composicao.p2 != null && composicao.p2.PlacaAlteracoes != null && composicao.p2.PlacaAlteracoes.IsPlacaVeiculoAlterado)
                  || composicao.p1 != null && composicao.p1.PlacaAlteracoes != null && composicao.p1.PlacaAlteracoes.IsPlacaVeiculoAlterado))
                veiculoAr[0].VehText = ObtemPlacaOperacaoO4V1SAP(composicao);

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.IsIBMTransportadoraAlterada))
                veiculoAr[0].Carrier = composicao.IBMTransportadora; //preciso verificar

            if (composicao.p2 != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.p2.PlacaAlteracoes != null && composicao.p2.PlacaAlteracoes.IsIDTipoCarregamentoAlterado)))
            {
                switch (composicao.p2.IDTipoCarregamento)
                {
                    case (int)EnumTipoCarregamento.PorBaixo:
                        veiculoAr[0].CrgBot = "X";
                        veiculoAr[0].CrgUp = string.Empty;
                        break;
                    case (int)EnumTipoCarregamento.PorCima:
                        veiculoAr[0].CrgUp = "X";
                        veiculoAr[0].CrgBot = string.Empty;
                        break;
                    case (int)EnumTipoCarregamento.Ambos:
                        veiculoAr[0].CrgBot = veiculoAr[0].CrgUp = "X";
                        break;
                    default:
                        break;
                }
            }

            veiculoAr[0].VehStatus = "";
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Bloqueio)
                veiculoAr[0].VehStatus = "1";
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.Desbloqueio)
                veiculoAr[0].VehStatus = "";

            veiculoAr[0].VehType = TipoVeiculoSAP.GetTpVeiculo(composicao);

            #endregion

            #region Documentos

            #region SEGPOL (Seguro - Poliza - SEGPOL)

            var siglaSEGPOLTra = "SEGPOL";

            var mesesSEGPOLTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaSEGPOLTra) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).MesesValidade : 0);
            if (mesesSEGPOLTra != null)
                veiculoAr[0].TraSegPol = (composicao.p1.Documentos != null
                                            && composicao.p1.Documentos.Any(w => w.Sigla == siglaSEGPOLTra)
                                            && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).DataVencimento.Value.AddMonths((int)mesesSEGPOLTra * -1).ToString("yyyy-MM-dd") : null;

            var mesesSEGPOLSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaSEGPOLTra) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).MesesValidade : 0);
            if (mesesSEGPOLSem != null)
                veiculoAr[0].SemSegPol = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaSEGPOLTra)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).DataVencimento.Value.AddMonths((int)mesesSEGPOLSem * -1).ToString("yyyy-MM-dd") : null;

            var mesesSEGPOLSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaSEGPOLTra) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).MesesValidade : 0);
            if (mesesSEGPOLSem2 != null)
                veiculoAr[0].Sem2SegPol = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaSEGPOLTra)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGPOLTra).DataVencimento.Value.AddMonths((int)mesesSEGPOLSem2 * -1).ToString("yyyy-MM-dd") : null;
            #endregion

            #region SEGCUO (Seguro - Poliza - SEGCUO)

            var siglaSEGCUOTra = "SEGCUO";

            var mesesSEGCUOTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaSEGCUOTra) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).MesesValidade : 0);
            if (mesesSEGCUOTra != null)
                veiculoAr[0].TraSegCuo = (composicao.p1.Documentos != null
                                            && composicao.p1.Documentos.Any(w => w.Sigla == siglaSEGCUOTra)
                                            && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).DataVencimento.Value.AddMonths((int)mesesSEGCUOTra * -1).ToString("yyyy-MM-dd") : null;

            var mesesSEGCUOSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaSEGCUOTra) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).MesesValidade : 0);
            if (mesesSEGCUOSem != null)
                veiculoAr[0].SemSegCuo = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaSEGCUOTra)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).DataVencimento.Value.AddMonths((int)mesesSEGCUOSem * -1).ToString("yyyy-MM-dd") : null;

            var mesesSEGCUOSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaSEGCUOTra) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).MesesValidade : 0);
            if (mesesSEGCUOSem2 != null)
                veiculoAr[0].Sem2SegCuo = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaSEGCUOTra)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSEGCUOTra).DataVencimento.Value.AddMonths((int)mesesSEGCUOSem2 * -1).ToString("yyyy-MM-dd") : null;
            #endregion

            #region RTV (Revision Técniva Vehicular)

            var siglaRTV = "RTV";

            var mesesRTVTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaRTV) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).MesesValidade : 0);
            if (mesesRTVTra != null)
                veiculoAr[0].TraVtv = (composicao.p1.Documentos != null
                                            && composicao.p1.Documentos.Any(w => w.Sigla == siglaRTV)
                                            && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).DataVencimento.Value.AddMonths((int)mesesRTVTra * -1).ToString("yyyy-MM-dd") : null;

            var mesesRTVSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaRTV) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).MesesValidade : 0);
            if (mesesRTVSem != null)
                veiculoAr[0].SemVtv = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaRTV)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).DataVencimento.Value.AddMonths((int)mesesRTVSem * -1).ToString("yyyy-MM-dd") : null;

            var mesesRTVSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaRTV) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).MesesValidade : 0);
            if (mesesRTVSem2 != null)
                veiculoAr[0].Sem2Vtv = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaRTV)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaRTV).DataVencimento.Value.AddMonths((int)mesesRTVSem2 * -1).ToString("yyyy-MM-dd") : null;
            #endregion

            #region RUTA (R.U.T.A)

            var siglaRUTA = "RUTA";

            var mesesRUTATra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaRUTA) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).MesesValidade : 0);
            if (mesesRUTATra != null)
                veiculoAr[0].TraRuta = (composicao.p1.Documentos != null
                                            && composicao.p1.Documentos.Any(w => w.Sigla == siglaRUTA)
                                            && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).DataVencimento.Value.AddMonths((int)mesesRUTATra * -1).ToString("yyyy-MM-dd") : null;

            var mesesRUTASem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaRUTA) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).MesesValidade : 0);
            if (mesesRUTASem != null)
                veiculoAr[0].SemRuta = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaRUTA)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).DataVencimento.Value.AddMonths((int)mesesRUTASem * -1).ToString("yyyy-MM-dd") : null;

            var mesesRUTASem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaRUTA) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).MesesValidade : 0);
            if (mesesRUTASem2 != null)
                veiculoAr[0].Sem2Ruta = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaRUTA)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaRUTA).DataVencimento.Value.AddMonths((int)mesesRUTASem2 * -1).ToString("yyyy-MM-dd") : null;
            #endregion

            #region CCPC (Certificado de Control Puntos Criticos)

            var siglaCCPC = "CCPC";

            var mesesCCPCTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaCCPC) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaCCPC).MesesValidade : 0);
            if (mesesCCPCTra != null)
                veiculoAr[0].TraCertpcrit = (composicao.p1.Documentos != null
                                            && composicao.p1.Documentos.Any(w => w.Sigla == siglaCCPC)
                                            && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaCCPC).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaCCPC).DataVencimento.Value.AddMonths((int)mesesCCPCTra * -1).ToString("yyyy-MM-dd") : null;

            //TODO: Descomentar estas linhas abaixo após o problema no SAP for corrigido.
            var mesesCCPCSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaCCPC) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCCPC).MesesValidade : 0);
            if (mesesCCPCSem != null)
                veiculoAr[0].SemCertpcrit = (composicao.p2.Documentos != null
                                   && composicao.p2.Documentos.Any(w => w.Sigla == siglaCCPC)
                                   && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCCPC).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCCPC).DataVencimento.Value.AddMonths((int)mesesCCPCSem * -1).ToString("yyyy-MM-dd") : null;

            #endregion

            #region CCT (Certificado de Control de Tapas - CCT)

            var siglaCCT = "CCT";

            var mesesCCTSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaCCT) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCCT).MesesValidade : 0);
            if (mesesCCTSem != null)
                veiculoAr[0].SemCtrltapas = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaCCT)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCCT).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCCT).DataVencimento.Value.AddMonths((int)mesesCCTSem * -1).ToString("yyyy-MM-dd") : null;

            var mesesCCTSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaCCT) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaCCT).MesesValidade : 0);
            if (mesesCCTSem2 != null)
                veiculoAr[0].Sem2Ctrltapas = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaCCT)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaCCT).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaCCT).DataVencimento.Value.AddMonths((int)mesesCCTSem2 * -1).ToString("yyyy-MM-dd") : null;

            #endregion

            #region CC (Calibracion de Cisterna - CC)

            var siglaCC = "CC";

            var mesesCCSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaCC) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCC).MesesValidade : 0);
            if (mesesCCSem != null)
                veiculoAr[0].SemCalibciste = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaCC)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCC).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaCC).DataVencimento.Value.AddMonths((int)mesesCCSem * -1).ToString("yyyy-MM-dd") : null;

            var mesesCCSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaCC) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaCC).MesesValidade : 0);
            if (mesesCCSem2 != null)
                veiculoAr[0].Sem2Calibciste = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaCC)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaCC).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaCC).DataVencimento.Value.AddMonths((int)mesesCCSem2 * -1).ToString("yyyy-MM-dd") : null;

            #endregion

            #region VI (Verificacion Interna - VI)

            var siglaVI = "VI";

            var mesesVISem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaVI) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVI).MesesValidade : 0);
            if (mesesVISem != null)
                veiculoAr[0].SemVerifinter = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaVI)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVI).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVI).DataVencimento.Value.AddMonths((int)mesesVISem * -1).ToString("yyyy-MM-dd") : null;

            var mesesVISem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaVI) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVI).MesesValidade : 0);
            if (mesesVISem2 != null)
                veiculoAr[0].Sem2Verifinter = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaVI)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVI).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVI).DataVencimento.Value.AddMonths((int)mesesVISem2 * -1).ToString("yyyy-MM-dd") : null;
            #endregion

            #region VE (Verificacion Externa - VE)

            var siglaVE = "VE";

            var mesesVESem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaVE) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVE).MesesValidade : 0);
            if (mesesVESem != null)
                veiculoAr[0].SemVerifexter = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaVE)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVE).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVE).DataVencimento.Value.AddMonths((int)mesesVESem * -1).ToString("yyyy-MM-dd") : null;

            var mesesVESem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaVE) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVE).MesesValidade : 0);
            if (mesesVESem2 != null)
                veiculoAr[0].Sem2Verifexter = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaVE)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVE).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVE).DataVencimento.Value.AddMonths((int)mesesVESem2 * -1).ToString("yyyy-MM-dd") : null;
            #endregion

            #region VH (Verificacion Externa - VH)

            var siglaVH = "VH";

            var mesesVHSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaVH) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVH).MesesValidade : 0);
            if (mesesVHSem != null)
                veiculoAr[0].SemVerifhermet = (composicao.p2.Documentos != null
                                            && composicao.p2.Documentos.Any(w => w.Sigla == siglaVH)
                                            && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVH).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaVH).DataVencimento.Value.AddMonths((int)mesesVHSem * -1).ToString("yyyy-MM-dd") : null;

            var mesesVHSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaVH) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVH).MesesValidade : 0);
            if (mesesVHSem2 != null)
                veiculoAr[0].Sem2Verifhermet = (composicao.p3.Documentos != null
                                            && composicao.p3.Documentos.Any(w => w.Sigla == siglaVH)
                                            && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVH).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaVH).DataVencimento.Value.AddMonths((int)mesesVHSem2 * -1).ToString("yyyy-MM-dd") : null;

            #endregion

            if (composicao.Operacao == "CIF")
            {
                #region SPOTCT (Spot Check Trimestral)

                var siglaSPOTCT = "SPOTCT";

                var mesesSPOTCTTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCT) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).MesesValidade : 0);
                if (mesesSPOTCTTra != null)
                    veiculoAr[0].TraSpotrim = (composicao.p1.Documentos != null
                                                && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCT)
                                                && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.Value.AddMonths((int)mesesSPOTCTTra * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCTSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCT) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).MesesValidade : 0);
                if (mesesSPOTCTSem != null)
                    veiculoAr[0].SemSpotrim = (composicao.p2.Documentos != null
                                                && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCT)
                                                && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.Value.AddMonths((int)mesesSPOTCTSem * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCTSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCT) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).MesesValidade : 0);
                if (mesesSPOTCTSem2 != null)
                    veiculoAr[0].Sem2Spotrim = (composicao.p3.Documentos != null
                                                && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCT)
                                                && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.Value.AddMonths((int)mesesSPOTCTSem2 * -1).ToString("yyyy-MM-dd") : null;

                #endregion

                #region SPOTCM (Spot Check Mensual)

                var siglaSPOTCM = "SPOTCM";

                var mesesSPOTCMTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCM) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).MesesValidade : 0);
                if (mesesSPOTCMTra != null)
                    veiculoAr[0].TraSpotmen = (composicao.p1.Documentos != null
                                                && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCM)
                                                && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).DataVencimento.Value.AddMonths((int)mesesSPOTCMTra * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCMSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCM) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).MesesValidade : 0);
                if (mesesSPOTCMSem != null)
                    veiculoAr[0].SemSpotmen = (composicao.p2.Documentos != null
                                                && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCM)
                                                && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).DataVencimento.Value.AddMonths((int)mesesSPOTCMSem * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCMSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCM) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).MesesValidade : 0);
                if (mesesSPOTCMSem2 != null)
                    veiculoAr[0].Sem2Spotmen = (composicao.p3.Documentos != null
                                                && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCM)
                                                && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCM).DataVencimento.Value.AddMonths((int)mesesSPOTCMSem2 * -1).ToString("yyyy-MM-dd") : null;
                #endregion
            }
            else if (composicao.Operacao == "FOB")
            {
                #region SPOTCS (Spot Check Semestral)

                var siglaSPOTCS = "SPOTCS";

                var mesesSPOTCSTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCS) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).MesesValidade : 0);
                if (mesesSPOTCSTra != null)
                    veiculoAr[0].TraSpotsem = (composicao.p1.Documentos != null
                                                && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCS)
                                                && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).DataVencimento.Value.AddMonths((int)mesesSPOTCSTra * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCSSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCS) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).MesesValidade : 0);
                if (mesesSPOTCSSem != null)
                    veiculoAr[0].SemSpotsem = (composicao.p2.Documentos != null
                                                && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCS)
                                                && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).DataVencimento.Value.AddMonths((int)mesesSPOTCSSem * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCSSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCS) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).MesesValidade : 0);
                if (mesesSPOTCSSem2 != null)
                    veiculoAr[0].Sem2Spotsem = (composicao.p3.Documentos != null
                                                && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCS)
                                                && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCS).DataVencimento.Value.AddMonths((int)mesesSPOTCSSem2 * -1).ToString("yyyy-MM-dd") : null;
                #endregion

                #region SPOTCT (Spot Check Trimestral)

                var siglaSPOTCT = "SPOTCT";

                var mesesSPOTCTTra = (composicao.p1 == null ? (int?)null : composicao.p1.Documentos != null && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCT) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).MesesValidade : 0);
                if (mesesSPOTCTTra != null)
                    veiculoAr[0].TraSpotrim = (composicao.p1.Documentos != null
                                                && composicao.p1.Documentos.Any(w => w.Sigla == siglaSPOTCT)
                                                && composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.HasValue) ? composicao.p1.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.Value.AddMonths((int)mesesSPOTCTTra * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCTSem = (composicao.p2 == null ? (int?)null : composicao.p2.Documentos != null && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCT) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).MesesValidade : 0);
                if (mesesSPOTCTSem != null)
                    veiculoAr[0].SemSpotrim = (composicao.p2.Documentos != null
                                                && composicao.p2.Documentos.Any(w => w.Sigla == siglaSPOTCT)
                                                && composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.HasValue) ? composicao.p2.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.Value.AddMonths((int)mesesSPOTCTSem * -1).ToString("yyyy-MM-dd") : null;

                var mesesSPOTCTSem2 = (composicao.p3 == null ? (int?)null : composicao.p3.Documentos != null && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCT) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).MesesValidade : 0);
                if (mesesSPOTCTSem2 != null)
                    veiculoAr[0].Sem2Spotrim = (composicao.p3.Documentos != null
                                                && composicao.p3.Documentos.Any(w => w.Sigla == siglaSPOTCT)
                                                && composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.HasValue) ? composicao.p3.Documentos.FirstOrDefault(w => w.Sigla == siglaSPOTCT).DataVencimento.Value.AddMonths((int)mesesSPOTCTSem2 * -1).ToString("yyyy-MM-dd") : null;

                #endregion
            }

            #endregion

            return veiculoAr;
        }

        private string ObtemValorTuNumber(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Desbloqueio)
                return null;

            return ObtemPlacaOperacaoO4V1SAP(composicao);
        }

        private string ObtemPlacaVeiculo(Composicao composicao, int sequenciaPlaca)
        {
            if (composicao == null)
                return null;

            switch (sequenciaPlaca)
            {
                case 1:
                    return (composicao.p1?.PlacaVeiculo);
                case 2:
                    return (composicao.p2?.PlacaVeiculo);
                case 3:
                    return (composicao.p3?.PlacaVeiculo);
                case 4:
                    return (composicao.p4?.PlacaVeiculo);
            }

            return null;
        }

        private string VerificaMultiFlecha(Composicao composicao)
        {
            if (composicao == null)
                return null;

            var p1MultiSeta = ((composicao.p1 != null) && composicao.p1.MultiSeta);
            var p2MultiSeta = ((composicao.p2 != null) && composicao.p2.MultiSeta);
            var p3MultiSeta = ((composicao.p3 != null) && composicao.p3.MultiSeta);
            var p4MultiSeta = ((composicao.p4 != null) && composicao.p4.MultiSeta);

            if (p1MultiSeta || p2MultiSeta || p3MultiSeta || p4MultiSeta)
                return "X";

            return string.Empty;
        }

        private WSIntegracaoSAP.ModificarResponse Alterar(Composicao composicao, Veiculo_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WSIntegracaoSAP.ModificarRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "ES";
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Alteracao)
            {
                request.ComposicaoTransporte = CriarComposicaoTransporte(composicao, tipoIntegracao);
                request.UnidadeTransporte = CriarUnidadeTransporte(composicao, tipoIntegracao);
            }
            request.VeiculoAr = CriarVeiculoAr(composicao, tipoIntegracao);
            var retorno = client.Modificar_Sync(request);
            return retorno;
        }

        //O4C1
        private UnidadeTransporte[] CriarUnidadeTransporte(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var unidadeTransporte = new WSIntegracaoSAP.UnidadeTransporte[1];
            unidadeTransporte[0] = new WSIntegracaoSAP.UnidadeTransporte();
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao)
            {
                unidadeTransporte[0].Tipo = "RD"; //FIXO
                unidadeTransporte[0].UnMedPeso = "KG";
                unidadeTransporte[0].UnMedVol = "L";
            }
            unidadeTransporte[0].UndTransp = unidadeTransporte[0].Texto = ObtemPlacaOperacaoO4V1SAP(composicao);
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.IsTaraAlterada))
            {
                unidadeTransporte[0].Tara = (composicao.TaraComposicao * 1000).ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
                unidadeTransporte[0].PesoMax = (composicao.PBTC * 1000).Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
            }
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.IsNumEixosAlterados))
                unidadeTransporte[0].NmEixos = composicao.EixosComposicao.ToString();
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.IsIBMTransportadoraAlterada))
                unidadeTransporte[0].Transportadora = composicao.IBMTransportadora;
            return unidadeTransporte;
        }

        private string ObtemPlacaOperacaoO4V1SAP(Composicao composicao)
        {
            return (composicao.p2 != null ? composicao.p2.PlacaVeiculo : composicao.p1.PlacaVeiculo) + RegraCIF_FOB(composicao.Operacao);
        }

        private WSIntegracaoSAP.ComposicaoTransporte[] CriarComposicaoTransporte(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            int total = composicao.p1 != null ? composicao.p1.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p2 != null ? composicao.p2.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p3 != null ? composicao.p3.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p4 != null ? composicao.p4.Compartimentos.Count(p => p.Operacao != "D") : 0;

            var composicaoTransporte = new WSIntegracaoSAP.ComposicaoTransporte[total];
            int i = 0;
            //Foi removido o carregamento da Placa1, pois na Argentina a P1 é um truck e portanto, não possui compartimentos
            //ST - 24/02 - Foi INCLUÍDO o carregamento da Placa1, pois na Argentina a P1 é um truck e teve uma demanda para a inclusão de truck
            CarregarCompartimento(composicao.p1, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p1, composicao.Operacao, composicao.p1);
            CarregarCompartimento(composicao.p2, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);
            CarregarCompartimento(composicao.p3, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);
            CarregarCompartimento(composicao.p4, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);

            return composicaoTransporte;
        }

        private void CarregarCompartimento(Placa placa, EnumTipoIntegracaoSAP tipoIntegracao, ComposicaoTransporte[] composicaoTransporte, bool jaExiste, ref int i, Placa placa2 = null, string Operacao = null, Placa placaUnidTranp = null)
        {
            if (placa != null && placa.Compartimentos.Any())
            {
                foreach (var item in placa.Compartimentos.Where(p => p.Operacao != "D"))
                {
                    if (jaExiste || tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && item.setas.Any() && item.setas.Any(w => w.isVolumeAlterado)))
                        PopularComposicaoTransporte(placa, tipoIntegracao, composicaoTransporte, jaExiste, i, placa2, Operacao, placaUnidTranp, item);

                    i++;
                }
            }
        }

        private void PopularComposicaoTransporte(Placa placa, EnumTipoIntegracaoSAP tipoIntegracao, ComposicaoTransporte[] composicaoTransporte, bool jaExiste, int i, Placa placa2, string Operacao, Placa placaUnidTranp, CompartimentoView item)
        {
            composicaoTransporte[i] = new WSIntegracaoSAP.ComposicaoTransporte();
            composicaoTransporte[i].Seq = (i + 1).ToString();

            composicaoTransporte[i].Texto = placa.PlacaVeiculo + RegraCIF_FOB(Operacao);

            composicaoTransporte[i].UndTransp = placaUnidTranp != null ? placaUnidTranp.PlacaVeiculo + RegraCIF_FOB(Operacao) : placa.PlacaVeiculo + RegraCIF_FOB(Operacao);

            if (!placa.MultiSeta && item.Operacao != "D")
                composicaoTransporte[i].VolMax = item.setas[0].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

            if (jaExiste && item.setas != null && item.setas.Any())
                composicaoTransporte[i].Operacao = "U";
            else
                composicaoTransporte[i].Operacao = "D";

            if ((tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && placa.PlacaAlteracoes != null && placa.PlacaAlteracoes.IsIDTipoProdutoAlterado) || tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)
            {
                if (placa2 != null && placa2.IDTipoProduto != null)
                    composicaoTransporte[i].Grupo = EnumExtensions.GetDescription((EnumTipoProdutoSAP)placa2.IDTipoProduto);
                else if (placa != null && placa.IDTipoProduto != null)
                    composicaoTransporte[i].Grupo = EnumExtensions.GetDescription((EnumTipoProdutoSAP)placa.IDTipoProduto);
            }
            //verifica se tem aquele compartimento e se a operação não de de inclusão e se teve volume alterado ou a operação foi preenchida
            //ou se apenas é operação de inclusão
            if (item.setas.Count > 0
                && (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                    && (item.setas[0].isVolumeAlterado
                    || !string.IsNullOrEmpty(item.setas[0].Operacao))
                || (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)))
            {
                composicaoTransporte[i].Seta01 = item.setas[0].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (item.setas[0].Principal && item.setas[0].Volume.HasValue)
                    composicaoTransporte[i].VolMax = item.setas[0].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (!string.IsNullOrEmpty(item.setas[0].Operacao))
                    composicaoTransporte[i].Operacao = item.setas[0].Operacao;

            }

            //verifica se tem aquele compartimento e se a operação não de de inclusão e se teve volume alterado ou a operação foi preenchida
            //ou se apenas é operação de inclusão
            if (item.setas.Count > 1
                && (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                    && (item.setas[1].isVolumeAlterado
                    || !string.IsNullOrEmpty(item.setas[1].Operacao))
                || (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)))
            {
                composicaoTransporte[i].Seta02 = item.setas[1].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
                if (item.setas[1].Principal && item.setas[1].Volume.HasValue)
                    composicaoTransporte[i].VolMax = item.setas[1].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (!string.IsNullOrEmpty(item.setas[1].Operacao))
                    composicaoTransporte[i].Operacao = item.setas[1].Operacao;
            }

            //verifica se tem aquele compartimento e se a operação não de de inclusão e se teve volume alterado ou a operação foi preenchida
            //ou se apenas é operação de inclusão
            if (item.setas.Count > 2
                && (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                    && (item.setas[2].isVolumeAlterado
                    || !string.IsNullOrEmpty(item.setas[2].Operacao))
                || (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)))
            {
                composicaoTransporte[i].Seta03 = item.setas[2].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
                if (item.setas[2].Principal && item.setas[2].Volume.HasValue)
                    composicaoTransporte[i].VolMax = item.setas[2].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (!string.IsNullOrEmpty(item.setas[2].Operacao))
                    composicaoTransporte[i].Operacao = item.setas[2].Operacao;

            }

            //verifica se tem aquele compartimento e se a operação não de de inclusão e se teve volume alterado ou a operação foi preenchida
            //ou se apenas é operação de inclusão
            if (item.setas.Count > 3
                && (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                    && (item.setas[3].isVolumeAlterado
                    || !string.IsNullOrEmpty(item.setas[3].Operacao))
                || (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)))
            {
                composicaoTransporte[i].Seta04 = item.setas[3].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (item.setas[3].Principal && item.setas[3].Volume.HasValue)
                    composicaoTransporte[i].VolMax = item.setas[3].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (!string.IsNullOrEmpty(item.setas[3].Operacao))
                    composicaoTransporte[i].Operacao = item.setas[3].Operacao;

            }

            //verifica se tem aquele compartimento e se a operação não de de inclusão e se teve volume alterado ou a operação foi preenchida
            //ou se apenas é operação de inclusão
            if (item.setas.Count > 4
                && (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                    && (item.setas[4].isVolumeAlterado
                    || !string.IsNullOrEmpty(item.setas[4].Operacao))
                || (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)))
            {
                composicaoTransporte[i].Seta05 = item.setas[4].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (item.setas[4].Principal && item.setas[4].Volume.HasValue)
                    composicaoTransporte[i].VolMax = item.setas[4].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                if (!string.IsNullOrEmpty(item.setas[4].Operacao))
                    composicaoTransporte[i].Operacao = item.setas[4].Operacao;
            }
        }

        private string RegraCIF_FOB(string Operacao)
        {
            return (Operacao == "CIF" ? "*" : String.Empty);
        }
    }
}
