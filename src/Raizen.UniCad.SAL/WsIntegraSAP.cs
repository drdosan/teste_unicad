using Raizen.UniCad.Model;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using Raizen.UniCad.SAL.WSIntegracaoSAP;
using Raizen.Framework.Utils.Extensions;
using Raizen.UniCad.Model.View;
using System.Globalization;
using Raizen.UniCad.SAL.WsIntegracaoSAPMotorista;
using Raizen.UniCad.SAL.TipoVeiculoComposicao;
using Raizen.UniCad.SAL.Interfaces;

namespace Raizen.UniCad.SAL
{
    public class WsIntegraSAP : IWsIntegraSAP
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
                        retornoCt = retornoInclusao.ComposicaoTransporte.Where(w => w.TpMsg == "E").First().Msg;
                        if (!retornoInclusao.ComposicaoTransporte.Where(w => w.TpMsg == "E").First().Msg.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }
                    if (retornoInclusao.UnidadeTransporte.Any(w => w.TpMsg == "E"))
                    {
                        retornoUt = retornoInclusao.UnidadeTransporte.Where(w => w.TpMsg == "E").First().Msg + "</br>";
                        if (!retornoInclusao.UnidadeTransporte.Where(w => w.TpMsg == "E").First().Msg.Contains("já existe"))
                        {
                            retorno.Append(retornoUt);
                            break;
                        }
                    }
                    if (retornoInclusao.Veiculo.Any(w => w.TpMsg == "E"))
                    {
                        retornoVc = retornoInclusao.Veiculo.Where(w => w.TpMsg == "E").First().Msg + "</br>";
                        if (!retornoInclusao.Veiculo.Where(w => w.TpMsg == "E").First().Msg.Contains("já existe"))
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
                        retorno.Append(retornoAlteracao.ComposicaoTransporte.Where(w => w.TpMsg == "E").First().Msg + "</br>");
                    if (retornoAlteracao.UnidadeTransporte != null && retornoAlteracao.UnidadeTransporte.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoAlteracao.UnidadeTransporte.Where(w => w.TpMsg == "E").First().Msg + "</br>");
                    if (retornoAlteracao.Veiculo != null && retornoAlteracao.Veiculo.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoAlteracao.Veiculo.Where(w => w.TpMsg == "E").First().Msg);
                    break;
                case EnumTipoIntegracaoSAP.Excluir:
                    var retornoExclusao = Excluir(composicao, client, tipoIntegracao);

                    if (retornoExclusao.Veiculo != null && retornoExclusao.Veiculo.Any(w => w.TpMsg == "E"))
                        retorno.Append(retornoExclusao.Veiculo.Where(w => w.TpMsg == "E").First().Msg);
                    break;
                default:
                    break;
            }

            return retorno.ToString();
        }

        private WSIntegracaoSAP.EliminarResponse Excluir(Composicao composicao, Veiculo_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WSIntegracaoSAP.EliminarRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "PT";
            request.Veiculo = CriarVeiculo(composicao, EnumTipoIntegracaoSAP.Excluir);
            var retorno = client.Eliminar_Sync(request);
            return retorno;
        }

        private WSIntegracaoSAP.CriarResponse Incluir(Composicao composicao, Veiculo_OutService client)
        {
            var request = new WSIntegracaoSAP.CriarRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "PT";
            request.ComposicaoTransporte = CriarComposicaoTransporte(composicao, EnumTipoIntegracaoSAP.Inclusao);
            request.UnidadeTransporte = CriarUnidadeTransporte(composicao, EnumTipoIntegracaoSAP.Inclusao);
            request.Veiculo = CriarVeiculo(composicao, EnumTipoIntegracaoSAP.Inclusao);

            var retorno = client.Criar_Sync(request);

            return retorno;
        }

        private PersistirResponse AlterarMotorista(Motorista motorista, Motorista_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WsIntegracaoSAPMotorista.PersistirRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "PT";
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Excluir)
                request.MOTORISTA = ExcluirMotorista(motorista);
            else
                request.MOTORISTA = CriarMotorista(motorista, tipoIntegracao);
            var retorno = client.Persistir_Sync(request);
            return retorno;
        }

        private WSIntegracaoSAP.ModificarResponse Alterar(Composicao composicao, Veiculo_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WSIntegracaoSAP.ModificarRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "PT";
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Alteracao)
            {
                request.ComposicaoTransporte = CriarComposicaoTransporte(composicao, tipoIntegracao);
                request.UnidadeTransporte = CriarUnidadeTransporte(composicao, tipoIntegracao);
            }
            request.Veiculo = CriarVeiculo(composicao, tipoIntegracao);
            var retorno = client.Modificar_Sync(request);
            return retorno;
        }

        //O4V1
        private WSIntegracaoSAP.Veiculo[] CriarVeiculo(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var veiculo = new WSIntegracaoSAP.Veiculo[1];
            veiculo[0] = new WSIntegracaoSAP.Veiculo();

            veiculo[0].Veiculo1 = composicao.p2 != null ? composicao.p2.PlacaVeiculo + (composicao.Operacao == "CIF" ? "*" : String.Empty) : (composicao.p1.PlacaVeiculo + (composicao.Operacao == "CIF" ? "*" : String.Empty));

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Excluir)
                return veiculo;

            PlacaDocumentoView cvv = composicao.p2 != null && composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault() : composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault() : null;
            PlacaDocumentoView cvvInmetro = composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault() : null;

            PlacaDocumentoView cvvImmetroDolly = null;
            PlacaDocumentoView cvvImmetroSegCarreta = null;
            PlacaDocumentoView civDolly = null;
            PlacaDocumentoView civCar2 = null;

            PlacaDocumentoView crlvDolly = null;
            PlacaDocumentoView crlvCar2 = null;

            PlacaDocumentoView cippDolly = null;
            PlacaDocumentoView cippCar2 = null;

            if (composicao.IDTipoComposicao == (int)EnumTipoComposicao.BitremDolly)
            {
                cvvImmetroDolly = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault() : null : null;
                cvvImmetroSegCarreta = composicao.p4 != null ? composicao.p4.Documentos != null ? composicao.p4.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault() : null : null;

                civDolly = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault() : null : null;
                civCar2 = composicao.p4 != null ? composicao.p4.Documentos != null ? composicao.p4.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault() : null : null;

                crlvDolly = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CRLV").FirstOrDefault() : null : null;
                crlvCar2 = composicao.p4 != null ? composicao.p4.Documentos != null ? composicao.p4.Documentos.Where(w => w.Sigla == "CRLV").FirstOrDefault() : null : null;

                cippDolly = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault() : null : null;
                cippCar2 = composicao.p4 != null ? composicao.p4.Documentos != null ? composicao.p4.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault() : null : null;
            }
            else if (composicao.IDTipoComposicao == (int)EnumTipoComposicao.Bitrem)
            {
                cvvImmetroSegCarreta = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault() : null : null;
                civCar2 = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault() : null : null;
                crlvCar2 = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CRLV").FirstOrDefault() : null : null;
                cippCar2 = composicao.p3 != null ? composicao.p3.Documentos != null ? composicao.p3.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault() : null : null;
            }

            PlacaDocumentoView civ = composicao.p2 != null ? composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault() : null : composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault() : null;
            PlacaDocumentoView civcv = composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault() : null;

            PlacaDocumentoView crlv = composicao.p2 != null ? composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "CRLV").FirstOrDefault() : null : composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CRLV").FirstOrDefault() : null;
            PlacaDocumentoView crlvCv = composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CRLV").FirstOrDefault() : null;

            PlacaDocumentoView cipp = composicao.p2 != null ? composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault() : null : composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault() : null;
            PlacaDocumentoView cippCv = composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault() : null;

            PlacaDocumentoView rntrc = composicao.p2 != null ?
                    composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "RNTRC").FirstOrDefault() : null :
                    composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "RNTRC").FirstOrDefault() : null;
            PlacaDocumentoView ctf = composicao.p2 != null ?
                    composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "CTF").FirstOrDefault() : null :
                    composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "CTF").FirstOrDefault() : null;
            PlacaDocumentoView licencaAmbiental = composicao.p2 != null ?
                    composicao.p2.Documentos != null ? composicao.p2.Documentos.Where(w => w.Sigla == "LO").FirstOrDefault() : null :
                    composicao.p1.Documentos != null ? composicao.p1.Documentos.Where(w => w.Sigla == "LO").FirstOrDefault() : null;
            bool isPendente = composicao.IsDocumentosPendentes;

            veiculo[0].RegCntry = "BR";
            veiculo[0].StVeiculo = "";
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Bloqueio)
                veiculo[0].StVeiculo = "1";
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.Desbloqueio)
                veiculo[0].StVeiculo = "";
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste)
                veiculo[0].Itinerario = "COMB";
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.AprovarCheckList && composicao.checkList != null)
            {
                veiculo[0].DtVencChLt = composicao.checkList.Data.ToString("yyyy-MM-dd");
                veiculo[0].DtRealChLt = composicao.checkList.DataCadastro.ToString("yyyy-MM-dd");
            }
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.ReprovarCheckList && composicao.checkList != null)
            {
                veiculo[0].DtVencChLt = composicao.checkList.Data.ToString("yyyy-MM-dd");
                veiculo[0].DtRealChLt = composicao.checkList.DataCadastro.ToString("yyyy-MM-dd");
                veiculo[0].StVeiculo = "1";
            }

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste
                || ((tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao
                    && composicao.p2 != null && composicao.p2.PlacaAlteracoes != null && composicao.p2.PlacaAlteracoes.IsPlacaVeiculoAlterado)
                    || composicao.p1 != null && composicao.p1.PlacaAlteracoes != null && composicao.p1.PlacaAlteracoes.IsPlacaVeiculoAlterado))
                veiculo[0].TxVeiculo = veiculo[0].UndTransp = veiculo[0].Veiculo1 = (composicao.p2 != null ? composicao.p2.PlacaVeiculo : composicao.p1.PlacaVeiculo) + (composicao.Operacao == "CIF" ? "*" : String.Empty);

            if (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && (composicao.p2 != null && composicao.p2.PlacaAlteracoes != null && composicao.p2.PlacaAlteracoes.IsUfAlterado))
                veiculo[0].UfPlacaCv = veiculo[0].CodDistrito = composicao.p2.Uf;
            else if (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && (composicao.p1 != null && composicao.p1.PlacaAlteracoes != null && composicao.p1.PlacaAlteracoes.IsUfAlterado))
                veiculo[0].UfPlacaCv = veiculo[0].CodDistrito = composicao.p1.Uf;
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste)
                veiculo[0].UfPlacaCv = veiculo[0].CodDistrito = composicao.p2 != null ? composicao.p2.Uf : composicao.p1.Uf;

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && (composicao.IsOperacaoAlterada || composicao.IsNumEixosAlterados)))
                veiculo[0].TpVeiculo = TipoVeiculoSAP.GetTpVeiculo(composicao);
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.IsIBMTransportadoraAlterada))
                veiculo[0].Transportadora = composicao.IBMTransportadora; //preciso verificar

            if (cvv != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cvv.isDataVencimentoAlterada)))
            {
                var mesesDtAfr = cvv != null ? cvv.MesesValidade : 0;
                veiculo[0].DtAfr = veiculo[0].DtVencCertCar1 = cvv != null ? cvv.DataVencimento.HasValue ? cvv.DataVencimento.Value.AddMonths(mesesDtAfr * -1).ToString("yyyy-MM-dd") : null : null;
            }
            if (cvvInmetro != null && (composicao.IDTipoComposicao != (int)EnumTipoComposicao.Truck) && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cvvInmetro.isDataVencimentoAlterada)))
            {
                var mesesCvvInmetro = cvvInmetro != null ? cvvInmetro.MesesValidade : 0;
                veiculo[0].DtVencCvvCv = cvvInmetro != null ? cvvInmetro.DataVencimento.HasValue ? cvvInmetro.DataVencimento.Value.AddMonths(mesesCvvInmetro * -1).ToString("yyyy-MM-dd") : null : null;
            }
            if (cvvImmetroDolly != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cvvImmetroDolly.isDataVencimentoAlterada)))
            {
                var mesesCvvDolly = cvvImmetroDolly != null ? cvvImmetroDolly.MesesValidade : 0;
                veiculo[0].DtVencCvvDolly = cvvImmetroDolly != null ? cvvImmetroDolly.DataVencimento.HasValue ? cvvImmetroDolly.DataVencimento.Value.AddMonths(mesesCvvDolly * -1).ToString("yyyy-MM-dd") : null : null;
            }
            if (cvvImmetroSegCarreta != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cvvImmetroSegCarreta.isDataVencimentoAlterada)))
            {
                var mesesVencCertCar2 = cvvImmetroSegCarreta != null ? cvvImmetroSegCarreta.MesesValidade : 0;
                veiculo[0].DtVencCvvCar2 = veiculo[0].DtVencCertCar2 = cvvImmetroSegCarreta != null ? cvvImmetroSegCarreta.DataVencimento.HasValue ? cvvImmetroSegCarreta.DataVencimento.Value.AddMonths(mesesVencCertCar2 * -1).ToString("yyyy-MM-dd") : null : null;
            }

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.isPlaca1Alterada)) //fazer IsQtdeCompartimentosAlterado

                veiculo[0].QtLacres = (composicao.QtdCompartimentos * 3).ToString();

            if (civ != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && civ.isDataVencimentoAlterada)))
            {
                var mesesVencDic = civ.MesesValidade;
                veiculo[0].DtVencCiv = civ.DataVencimento.HasValue ? civ.DataVencimento.Value.AddMonths(mesesVencDic * -1).ToString("yyyy-MM-dd") : null;
            }
            if (civcv != null && (composicao.IDTipoComposicao != (int)EnumTipoComposicao.Truck) && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && civcv.isDataVencimentoAlterada)))
            {
                var mesesVencCivCv = civcv.MesesValidade;
                veiculo[0].DtVencCivCv = civcv.DataVencimento.HasValue ? civcv.DataVencimento.Value.AddMonths(mesesVencCivCv * -1).ToString("yyyy-MM-dd") : null;
            }
            if (civDolly != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && civDolly.isDataVencimentoAlterada)))
            {
                var mesesCivDolly = civDolly.MesesValidade;
                veiculo[0].DtVencCivDolly = civDolly.DataVencimento.HasValue ? civDolly.DataVencimento.Value.AddMonths(mesesCivDolly * -1).ToString("yyyy-MM-dd") : null;
            }
            if (civCar2 != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && civCar2.isDataVencimentoAlterada)))
            {
                var mesesVencCivCar2 = civCar2.MesesValidade;
                veiculo[0].DtVencCivCar2 = civCar2.DataVencimento.HasValue ? civCar2.DataVencimento.Value.AddMonths(mesesVencCivCar2 * -1).ToString("yyyy-MM-dd") : null;
            }
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.p1 != null && composicao.p1.PlacaAlteracoes != null && composicao.p1.PlacaAlteracoes.IsPlacaVeiculoAlterado))
            {
                if (composicao.p1.IDTipoVeiculo != (int)EnumTipoVeiculo.Truck)
                    veiculo[0].PlacaCv = composicao.p1.PlacaVeiculo + (composicao.Operacao == "CIF" ? "*" : String.Empty);
            }

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.p3 != null && composicao.p3.PlacaAlteracoes != null && composicao.p3.PlacaAlteracoes.IsPlacaVeiculoAlterado))
            {
                if (composicao.IDTipoComposicao == (int)EnumTipoComposicao.BitremDolly)
                {
                    veiculo[0].PlacaDolly = composicao.p3 != null ? composicao.p3.PlacaVeiculo + (composicao.Operacao == "CIF" ? "*" : String.Empty) : string.Empty;
                    if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.p4 != null && composicao.p4.PlacaAlteracoes != null && composicao.p4.PlacaAlteracoes.IsPlacaVeiculoAlterado))
                    {
                        veiculo[0].PlacaCar2 = composicao.p4 != null ? composicao.p4.PlacaVeiculo + (composicao.Operacao == "CIF" ? "*" : String.Empty) : string.Empty;
                    }
                }
                else
                    veiculo[0].PlacaCar2 = composicao.p3 != null ? composicao.p3.PlacaVeiculo + (composicao.Operacao == "CIF" ? "*" : String.Empty) : string.Empty;
            }

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.IsCodEasyQyeryAlterado))
                veiculo[0].ChEasy = composicao.CodigoEasyQuery != null ? composicao.CodigoEasyQuery : string.Empty;
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste)
                veiculo[0].ChPend = isPendente ? "X" : string.Empty;
            if (crlv != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && crlv.isDataVencimentoAlterada)))
            {
                var meses = crlv.MesesValidade;
                veiculo[0].DtVencCrlvCar1 = crlv.DataVencimento.HasValue ? crlv.DataVencimento.Value.ToString("yyyy-MM-dd") : null;
            }
            if (crlvCv != null && (composicao.IDTipoComposicao != (int)EnumTipoComposicao.Truck) && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && crlvCv.isDataVencimentoAlterada)))
            {
                var meses = crlvCv.MesesValidade;
                veiculo[0].DtVencCrlvCv = crlvCv.DataVencimento.HasValue ? crlvCv.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (crlvDolly != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && crlvDolly.isDataVencimentoAlterada)))
            {
                var meses = crlvDolly.MesesValidade;
                veiculo[0].DtVencCrlvDolly = crlvDolly.DataVencimento.HasValue ? crlvDolly.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (crlvCar2 != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && crlvCar2.isDataVencimentoAlterada)))
            {
                var meses = crlvCar2.MesesValidade;
                veiculo[0].DtVencCrlvCar2 = crlvCar2.DataVencimento.HasValue ? crlvCar2.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (cipp != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cipp != null && cipp.isDataVencimentoAlterada)))
            {
                var meses = cipp.MesesValidade;
                veiculo[0].DtVencCippCar1 = cipp.DataVencimento.HasValue ? cipp.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (cippCv != null && (composicao.IDTipoComposicao != (int)EnumTipoComposicao.Truck) && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cippCv != null && cippCv.isDataVencimentoAlterada)))
            {
                var meses = cippCv.MesesValidade;
                veiculo[0].DtVencCippCv = cippCv.DataVencimento.HasValue ? cippCv.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (cippDolly != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cippDolly != null && cippDolly.isDataVencimentoAlterada)))
            {
                var meses = cippDolly.MesesValidade;
                veiculo[0].DtVencCippDolly = cippDolly.DataVencimento.HasValue ? cippDolly.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (cippCar2 != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && cippCar2 != null && cippCar2.isDataVencimentoAlterada)))
            {
                var meses = cippCar2.MesesValidade;
                veiculo[0].DtVencCippCar2 = cippCar2.DataVencimento.HasValue ? cippCar2.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (rntrc != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && rntrc != null && rntrc.isDataVencimentoAlterada)))
            {
                var meses = rntrc.MesesValidade;
                veiculo[0].DtVencRntrc = rntrc.DataVencimento.HasValue ? rntrc.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (ctf != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && ctf != null && ctf.isDataVencimentoAlterada)))
            {
                var meses = ctf.MesesValidade;
                veiculo[0].DtVencCtf = ctf.DataVencimento.HasValue ? ctf.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (licencaAmbiental != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && licencaAmbiental != null && licencaAmbiental.isDataVencimentoAlterada)))
            {
                var meses = licencaAmbiental.MesesValidade;
                veiculo[0].DtVencLic = licencaAmbiental.DataVencimento.HasValue ? licencaAmbiental.DataVencimento.Value.AddMonths(meses * -1).ToString("yyyy-MM-dd") : null;
            }
            if (composicao.p2 != null && (tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || composicao.jaExiste || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && composicao.p2.PlacaAlteracoes != null && composicao.p2.PlacaAlteracoes.IsIDTipoCarregamentoAlterado)))
            {
                switch (composicao.p2.IDTipoCarregamento)
                {
                    case (int)EnumTipoCarregamento.PorBaixo:
                        veiculo[0].CrgBaixo = veiculo[0].Protecao = "X";
                        veiculo[0].CrgCima = string.Empty;
                        break;
                    case (int)EnumTipoCarregamento.PorCima:
                        veiculo[0].CrgCima = "X";
                        veiculo[0].CrgBaixo = veiculo[0].Protecao = string.Empty;
                        break;
                    case (int)EnumTipoCarregamento.Ambos:
                        veiculo[0].CrgBaixo = veiculo[0].CrgCima = "X";
                        break;
                    default:
                        break;
                }
            }
            return veiculo;
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
            unidadeTransporte[0].UndTransp = unidadeTransporte[0].Texto = (composicao.p2 != null ? composicao.p2.PlacaVeiculo : composicao.p1.PlacaVeiculo) + (composicao.Operacao == "CIF" ? "*" : string.Empty);
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

        private WSIntegracaoSAP.ComposicaoTransporte[] CriarComposicaoTransporte(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            int total = composicao.p1 != null ? composicao.p1.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p2 != null ? composicao.p2.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p3 != null ? composicao.p3.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p4 != null ? composicao.p4.Compartimentos.Count(p => p.Operacao != "D") : 0;


            var composicaoTransporte = new WSIntegracaoSAP.ComposicaoTransporte[total];
            int i = 0;
            CarregarCompartimento(composicao.p1, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao);
            CarregarCompartimento(composicao.p2, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);
            CarregarCompartimento(composicao.p3, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);
            CarregarCompartimento(composicao.p4, tipoIntegracao, composicaoTransporte, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);

            return composicaoTransporte;
        }

        private static void CarregarCompartimento(Placa placa, EnumTipoIntegracaoSAP tipoIntegracao, ComposicaoTransporte[] composicaoTransporte, bool jaExiste, ref int i, Placa placa2 = null, string Operacao = null, Placa placaUnidTranp = null)
        {
            if (placa != null && placa.Compartimentos.Any())
            {
                foreach (var item in placa.Compartimentos.Where(p => p.Operacao != "D"))
                {
                    if (jaExiste || tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || (tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && item.setas.Any() && item.setas.Any(w => w.isVolumeAlterado)))
                    {

                        composicaoTransporte[i] = new WSIntegracaoSAP.ComposicaoTransporte();
                        composicaoTransporte[i].Seq = (i + 1).ToString();

                        composicaoTransporte[i].Texto = placa.PlacaVeiculo + (Operacao == "CIF" ? "*" : String.Empty);

                        if (placaUnidTranp != null)
                            composicaoTransporte[i].UndTransp = placaUnidTranp.PlacaVeiculo + (Operacao == "CIF" ? "*" : String.Empty);
                        else
                            composicaoTransporte[i].UndTransp = placa.PlacaVeiculo + (Operacao == "CIF" ? "*" : String.Empty);

                        if (!placa.MultiSeta && item.Operacao != "D")
                            composicaoTransporte[i].VolMax = item.setas[0].Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");

                        if (jaExiste && item.setas != null && item.setas.Any())
                            composicaoTransporte[i].Operacao = "U";
                        else
                            composicaoTransporte[i].Operacao = "D";

                        if ((tipoIntegracao != EnumTipoIntegracaoSAP.Inclusao && placa.PlacaAlteracoes != null && placa.PlacaAlteracoes.IsIDTipoProdutoAlterado) || tipoIntegracao == EnumTipoIntegracaoSAP.Inclusao || jaExiste)
                        {
                            composicaoTransporte[i].Grupo = placa2 != null ? (EnumExtensions.GetDescription((EnumTipoProdutoSAP)placa2.IDTipoProduto)) : (EnumExtensions.GetDescription((EnumTipoProdutoSAP)placa.IDTipoProduto));
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
                    i++;
                }
            }
        }

        public string IntegrarMotorista(Motorista motorista, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
            string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAPMotorista"];
            StringBuilder retorno = new StringBuilder();
            string retornoCt = string.Empty;
            string retornoUt = string.Empty;
            string retornoVc = string.Empty;
            //CREDENCIAIS
            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            var client = new WsIntegracaoSAPMotorista.Motorista_OutService();
            client.Credentials = credentials;
            client.Url = URL;
            switch (tipoIntegracao)
            {
                case EnumTipoIntegracaoSAP.Inclusao:
                    var retornoInclusao = IncluirMotorista(motorista, client);
                    if (retornoInclusao.MOTORISTA.Any(w => w.MSGTYP == "E"))
                    {
                        retornoCt = retornoInclusao.MOTORISTA.Where(w => w.MSGTYP == "E").First().MESSAGE;
                        if (!retornoInclusao.MOTORISTA.Where(w => w.MSGTYP == "E").First().MESSAGE.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }

                    retorno.Append(retornoCt);

                    if (retorno.ToString().ToLower(CultureInfo.InvariantCulture).Contains("já existe"))
                        motorista.jaExiste = true;
                    break;
                case EnumTipoIntegracaoSAP.Alteracao:
                case EnumTipoIntegracaoSAP.Bloqueio:
                case EnumTipoIntegracaoSAP.Desbloqueio:
                case EnumTipoIntegracaoSAP.AprovarCheckList:
                case EnumTipoIntegracaoSAP.ReprovarCheckList:
                //CSCUNI-648
                case EnumTipoIntegracaoSAP.Excluir:
                    var retornoAlteracao = AlterarMotorista(motorista, client, tipoIntegracao);
                    if (retornoAlteracao.MOTORISTA != null && retornoAlteracao.MOTORISTA.Any(w => w.MSGTYP == "E"))
                        retorno.Append(retornoAlteracao.MOTORISTA.Where(w => w.MSGTYP == "E").First().MESSAGE + "</br>");
                    break;
                default:
                    break;
            }

            return retorno.ToString();
        }

        private PersistirResponse IncluirMotorista(Motorista motorista, Motorista_OutService client)
        {
			var request = new WsIntegracaoSAPMotorista.PersistirRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "PT";
            request.MOTORISTA = CriarMotorista(motorista, EnumTipoIntegracaoSAP.Inclusao);
            MarcarAlterado(motorista, request);
            var retorno = client.Persistir_Sync(request);

            return retorno;
        }

        public PersistirResponse ConsultarMotorista(Motorista motorista)
        {
            string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
            string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAPMotorista"];
           
            //CREDENCIAIS
            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            var client = new WsIntegracaoSAPMotorista.Motorista_OutService();
            client.Credentials = credentials;
            client.Url = URL;

            var request = new WsIntegracaoSAPMotorista.PersistirRequest();
            request.Ambiente = "FUELS";
            request.Idioma = "PT";
            request.MOTORISTA = CriarRequestConsultaMotorista(motorista);
            var retorno = client.Persistir_Sync(request);
            return retorno;
        }

        private static void MarcarAlterado(Motorista motorista, PersistirRequest request)
        {
            request.MOTORISTAX = new PersistirRequestItem1[1];
            request.MOTORISTAX[0] = new PersistirRequestItem1()
            {
                TPOPER = "I",
                CPF = motorista.MotoristaBrasil.CPF,
                CATEGORIA = "X",
                CNH = "X",
                DATANASC = "X",
                EMAIL = "X",
                EXAMEMED = "X",
                EXPEDIDOR = "X",
                KUNNR = "X",
                LIFNR = "X",
                NOME = "X",
                RELCUROPDER = motorista.DataValidadeTreinamento.HasValue ? "X" : null,
                RG = "X",
                STATUS = "X",
                TELEFONE = "X",
                TIMTRM = "X",
                VALIDADE = "X",
                VALMOPE = "X",
                YANR20 = "X",
                YANR35 = "X",
                ZVALCDDS = "X",
                EXAMEPSI = "X",
                RELCURDIRDEF = "X"
            };
        }
        private PersistirRequestItem[] CriarRequestConsultaMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem();
            motoristas[0].CPF = motorista.MotoristaBrasil.CPF;
            motoristas[0].TPOPER = "C";
            return motoristas;
        }

        private PersistirRequestItem[] ExcluirMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem();
            motoristas[0].CPF = motorista.MotoristaBrasil.CPF;
            motoristas[0].TPOPER = "E";
            return motoristas;
        }

        private PersistirRequestItem[] CriarMotorista(Motorista motorista, EnumTipoIntegracaoSAP inclusao)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem();
            motoristas[0].CATEGORIA = motorista.MotoristaBrasil.CategoriaCNH;
            motoristas[0].CNH = motorista.MotoristaBrasil.CNH;
            motoristas[0].CPF = motorista.MotoristaBrasil.CPF;
            if (motorista.MotoristaBrasil.Nascimento.HasValue)
                motoristas[0].DATANASC = motorista.MotoristaBrasil.Nascimento.Value.ToString("yyyy-MM-dd");
            motoristas[0].EMAIL = motorista.Email;

            if (motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
            {
                var mesesODS = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "ODS").Any() ? motorista.Documentos.Where(w => w.Sigla == "ODS").FirstOrDefault().MesesValidade : 0);
                var examePsi = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "PSI").Any() ? motorista.Documentos.Where(w => w.Sigla == "PSI").FirstOrDefault().MesesValidade : 0);
                var mesesExameMed = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "EXAME").Any() ? motorista.Documentos.Where(w => w.Sigla == "EXAME").FirstOrDefault().MesesValidade : 0);
                var mesesZvalCdd = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "CDD").Any() ? motorista.Documentos.Where(w => w.Sigla == "CDD").FirstOrDefault().MesesValidade : 0);
                var mesesTimTrm = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "TRM").Any() ? motorista.Documentos.Where(w => w.Sigla == "TRM").FirstOrDefault().MesesValidade : 0);
                var mesesYanr20 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR20").Any() ? motorista.Documentos.Where(w => w.Sigla == "NR20").FirstOrDefault().MesesValidade : 0);
                var mesesYanr35 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR35").Any() ? motorista.Documentos.Where(w => w.Sigla == "NR35").FirstOrDefault().MesesValidade : 0);
                var mesesVALMOPE = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "MOPP").Any() ? motorista.Documentos.Where(w => w.Sigla == "MOPP").FirstOrDefault().MesesValidade : 0);
                var mesesVALIDADE = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "CNH").Any() ? motorista.Documentos.Where(w => w.Sigla == "CNH").FirstOrDefault().MesesValidade : 0);

                //ODS
                motoristas[0].ZVALCDDS = (motorista.Documentos != null
                                          && motorista.Documentos.Where(w => w.Sigla == "ODS").Any()
                                          && motorista.Documentos.Where(w => w.Sigla == "ODS").FirstOrDefault().DataVencimento.HasValue) ? motorista.Documentos.Where(w => w.Sigla == "ODS").FirstOrDefault().DataVencimento.Value.AddMonths(mesesODS * -1).ToString("yyyy-MM-dd") : null;
                //EXAME PSICOLOGICO
                motoristas[0].EXAMEPSI = (motorista.Documentos != null
                                          && motorista.Documentos.Where(w => w.Sigla == "PSI").Any()
                                          && motorista.Documentos.Where(w => w.Sigla == "PSI").FirstOrDefault().DataVencimento.HasValue) ? motorista.Documentos.Where(w => w.Sigla == "PSI").FirstOrDefault().DataVencimento.Value.AddMonths(examePsi * -1).ToString("yyyy-MM-dd") : null;
                //EXAME MEDICO
                motoristas[0].EXAMEMED = (motorista.Documentos != null
                    && motorista.Documentos.Where(w => w.Sigla == "EXAME").Any()
                    && motorista.Documentos.Where(w => w.Sigla == "EXAME").FirstOrDefault().DataVencimento.HasValue) ? motorista.Documentos.Where(w => w.Sigla == "EXAME").FirstOrDefault().DataVencimento.Value.AddMonths(mesesExameMed * -1).ToString("yyyy-MM-dd") : null;
                //CURSO DE DIREÇÃO DEFENSIVA
                motoristas[0].RELCURDIRDEF = (motorista.Documentos != null
                    && motorista.Documentos.Where(w => w.Sigla == "CDD").Any()
                    && motorista.Documentos.Where(w => w.Sigla == "CDD").FirstOrDefault().DataVencimento.HasValue) ? motorista.Documentos.Where(w => w.Sigla == "CDD").FirstOrDefault().DataVencimento.Value.AddMonths(mesesZvalCdd * -1).ToString("yyyy-MM-dd") : null;
                //TREINAMENTO DE INDUÇÃO
                motoristas[0].TIMTRM = (motorista.Documentos != null
                    && motorista.Documentos.Where(w => w.Sigla == "TRM").Any()
                    && motorista.Documentos.Where(w => w.Sigla == "TRM").FirstOrDefault().DataVencimento.HasValue) ? motorista.Documentos.Where(w => w.Sigla == "TRM").FirstOrDefault().DataVencimento.Value.AddMonths(mesesTimTrm * -1).ToString("yyyy-MM-dd") : null;

                //NR20
                motoristas[0].YANR20 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR20").Any()) ? motorista.Documentos.Where(w => w.Sigla == "NR20").FirstOrDefault().DataVencimento.Value.AddMonths(mesesYanr20 * -1).ToString("yyyy-MM-dd") : null;
                //NR35
                motoristas[0].YANR35 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR35").Any()) ? motorista.Documentos.Where(w => w.Sigla == "NR35").FirstOrDefault().DataVencimento.Value.AddMonths(mesesYanr35 * -1).ToString("yyyy-MM-dd") : null;
                //CURSO DE MOVIMENTAÇÃO E OPERAÇÃO DE PRODUTOS PERIGOSOS
                motoristas[0].VALMOPE = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "MOPP").Any()) ? motorista.Documentos.Where(w => w.Sigla == "MOPP").FirstOrDefault().DataVencimento.Value.AddMonths(mesesVALMOPE * -1).ToString("yyyy-MM-dd") : null;
                //CNH
                motoristas[0].VALIDADE = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "CNH").Any()) ? motorista.Documentos.Where(w => w.Sigla == "CNH").FirstOrDefault().DataVencimento.Value.AddMonths(mesesVALIDADE * -1).ToString("yyyy-MM-dd") : null;

            }

            motoristas[0].TPOPER = "I";

            motoristas[0].EXPEDIDOR = motorista.MotoristaBrasil.OrgaoEmissor;

            //provisório até arrumarem no SAP
            if (motorista.DataValidadeTreinamento.HasValue)
            {
                var mesesValidadeTreinamento = motorista.MesesTreinamentoTeorico;
                motoristas[0].RELCUROPDER = motorista.DataValidadeTreinamento.Value.AddMonths(mesesValidadeTreinamento * -1).ToString("yyyy-MM-dd");
            }

            //motoristas[0].MESSAGE -- campo não encontrado
            //motoristas[0].MSGTYP -- campo não encontrado

            motoristas[0].NOME = motorista.Nome;

            motoristas[0].RG = motorista.MotoristaBrasil.RG;
            switch (motorista.IDStatus)
            {
                case (int)EnumStatusMotorista.Aprovado:
                    motoristas[0].STATUS = ""; //em branco: apto
                    break;
                case (int)EnumStatusMotorista.Bloqueado: //I: Inapto
                    motoristas[0].STATUS = "1";
                    break;
                default:
                    break;
            }

            motoristas[0].TELEFONE = motorista.Telefone;

            //motoristas[0].KUNNR = não mandar;
            if (motorista.Operacao == "CIF")
            {
                motoristas[0].LIFNR = motorista.IBMTransportadora;
            }
            else if (motorista.Operacao == "FOB")
            {
                motoristas[0].LIFNR = "FOB";
            }
            return motoristas;
        }

        public string VincularVeiculoClienteSap(Placa placa)
        {
            string retorno = string.Empty;

            string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
            string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAP"];

            ICredentials credentials = new NetworkCredential(UserName, PassWord);

            var client = new Veiculo_OutService();
            client.Credentials = credentials;
            client.Url = URL;

            try
            {
                var request = new CriarRequest();

                request.VehicleCliente = new CriarRequestVehicleCliente[placa.Clientes.Count];
                request.Ambiente = "FUELS";
                request.Idioma = BuscaIdiomaPorPais(placa.IDPais);

                for (int i = 0; i < placa.Clientes.Count; i++)
                {
                    request.VehicleCliente[i] = new CriarRequestVehicleCliente()
                    {
                        Vehicle = placa.PlacaVeiculo,
                        Cliente = placa.Clientes[i].Ibm
                    };
                }

                client.Criar_Sync(request);
            }
            catch (Exception ex)
            {
                retorno = BuscaMensagemErroEnvioSap(placa.IDPais) + ex.Message;
            }

            return retorno;
        }

		public string VincularClienteSap(VincularClienteRequestClienteMotorista[] clienteMotorista)
		{
			string retorno = string.Empty;

			string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
			string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
			string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAPMotorista"];

			ICredentials credentials = new NetworkCredential(UserName, PassWord);

			var client = new Motorista_OutService();
			client.Credentials = credentials;
			client.Url = URL;

            var vincularCliente = new VincularClienteRequest();

			try
			{
				vincularCliente.Ambiente = "FUELS";
                vincularCliente.Idioma = "PT";
                vincularCliente.clienteMotorista = clienteMotorista;

				client.VincularCliente_Sync(vincularCliente);
			}
			catch (Exception ex)
			{
				retorno = BuscaMensagemErroEnvioSap(EnumPais.Brasil) + ex.Message;
			}

			return retorno;
		}

		public string ExcluirPlacaClienteSap(Placa placa)
        {
            string retorno = string.Empty;

            string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
            string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAP"];

            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            var client = new Veiculo_OutService
            {
                Credentials = credentials,
                Url = URL
            };

            try
            {
                var request = new ModificarRequest();

                request.Ambiente = "FUELS";
                request.Idioma = BuscaIdiomaPorPais(placa.IDPais);

                if (placa.Clientes.Count > 0)
                {
                    request.VehicleCliente = new ModificarRequestVehicleCliente[placa.Clientes.Count];
                    for (int i = 0; i < placa.Clientes.Count; i++)
                    {
                        request.VehicleCliente[i] = new ModificarRequestVehicleCliente()
                        {
                            Vehicle = placa.PlacaVeiculo,
                            Cliente = placa.Clientes[i].Ibm
                        };
                    }
                }
                else
                {
                    request.VehicleCliente = new ModificarRequestVehicleCliente[]
                    {
                        new ModificarRequestVehicleCliente
                        {
                            Vehicle = placa.PlacaVeiculo,
                        }
                    };
                }

                client.Modificar_Sync(request);
            }
            catch (Exception ex)
            {
                retorno = BuscaMensagemErroEnvioSap(placa.IDPais) + ex.Message;
            }

            return retorno;
        }

        private string BuscaIdiomaPorPais(EnumPais pais)
        {
            switch (pais)
            {
                case EnumPais.Argentina:
                    return "ES";
                default:
                    return "PT";
            }
        }

        private string BuscaMensagemErroEnvioSap(EnumPais pais)
        {
            switch (pais)
            {
                case EnumPais.Argentina:
                    return "Problema al enviar datos a la SAP. <br/>Error: ";
                default:
                    return "Problema no envio dos dados para o SAP. <br/>Erro: ";
            }
        }
    }
}
