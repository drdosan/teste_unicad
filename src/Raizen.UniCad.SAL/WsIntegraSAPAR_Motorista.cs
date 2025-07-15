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

namespace Raizen.UniCad.SAL
{
    public class WsIntegraSAPAR_Motorista
    {
        private PersistirResponse AlterarMotorista(Motorista motorista, Motorista_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WsIntegracaoSAPMotorista.PersistirRequest();
            request.Ambiente = "FUELS";
			request.Idioma = "ES";
            if (tipoIntegracao == EnumTipoIntegracaoSAP.Excluir)
                request.MOTORISTA_AR = ExcluirMotorista(motorista);
            else
                request.MOTORISTA_AR = CriarMotorista(motorista);
            var retorno = client.Persistir_Sync(request);
            return retorno;
        }

        public string IntegrarMotorista(Motorista motorista, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            string UserName = ConfigurationManager.AppSettings["usuarioWSIntegracaoSAP"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSIntegracaoSAP"];
            string URL = ConfigurationManager.AppSettings["urlWSIntegracaoSAPMotorista"];
            StringBuilder retorno = new StringBuilder();
            string retornoCt = string.Empty;

            //CREDENCIAIS
            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            var client = new WsIntegracaoSAPMotorista.Motorista_OutService();
            client.Credentials = credentials;
            client.Url = URL;
            switch (tipoIntegracao)
            {
                case EnumTipoIntegracaoSAP.Inclusao:
                    var retornoInclusao = IncluirMotorista(motorista, client);
                    if (retornoInclusao.MOTORISTA_AR.Any(w => w.MSGTYP == "E"))
                    {
                        retornoCt = retornoInclusao.MOTORISTA_AR.First(w => w.MSGTYP == "E").MESSAGE;
                        if (!retornoInclusao.MOTORISTA_AR.First(w => w.MSGTYP == "E").MESSAGE.Contains("já existe"))
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
                case EnumTipoIntegracaoSAP.Excluir: //CSCUNI-648
                    var retornoAlteracao = AlterarMotorista(motorista, client, tipoIntegracao);
                    if (retornoAlteracao.MOTORISTA_AR != null && retornoAlteracao.MOTORISTA_AR.Any(w => w.MSGTYP == "E"))
                        retorno.Append(retornoAlteracao.MOTORISTA_AR.First(w => w.MSGTYP == "E").MESSAGE + "</br>");

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
			request.Idioma = "ES";
            request.MOTORISTA_AR = CriarMotorista(motorista);
            MarcarAlterado(motorista, request, EnumTipoIntegracaoSAP.Inclusao);

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
            request.Idioma = "ES";
            request.MOTORISTA_AR = CriarRequestConsultaMotorista(motorista);
            var retorno = client.Persistir_Sync(request);
            return retorno;
        }

        private static void MarcarAlterado(Motorista motorista, PersistirRequest request, EnumTipoIntegracaoSAP tipoOperacao)
        {
            request.MOTORISTAX_AR = new PersistirRequestItem5[1];
            request.MOTORISTAX_AR[0] = new PersistirRequestItem5()
            {
                APELLIDO = "X",
                COD_TRANSPORTISTA = "X",
                CONDUCTOR = motorista.MotoristaArgentina.DNI,
                LIC_NAC_COND = "X",
                LIC_NAC_HABIL = "X",
                MODO_ENTREGA = "X",
                NOMBRE = "X",
                STATUS = "X",
                TARJETA_ACCESO = "X",
                TIPO_NEGOCIO = "X",
                TPOPER = "I",
                VCTO_ART_CUOTA = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_ART_CUOTA) ? string.Empty : "X"),
                VCTO_ART_POLIZA = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_ART_POLIZA) ? string.Empty : "X"),
                VCTO_LIC_NAC_COND = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_LIC_NAC_COND) ? string.Empty : "X"),
                VCTO_LIC_NAC_HABIL = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_LIC_NAC_HABIL) ? string.Empty : "X"),
                VCTO_SEG_VIDA_CUO = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_SEG_VIDA_CUO) ? string.Empty : "X"),
                VCTO_SEG_VIDA_POL = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_SEG_VIDA_POL) ? string.Empty : "X"),
                VCTO_SSMA_MAN_DEF = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_SSMA_MAN_DEF) ? string.Empty : "X"),
                VCTO_SSMA_POLITICA = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_SSMA_POLITICA) ? string.Empty : "X"),
                VCTO_SSMA_RESPEME = (tipoOperacao == EnumTipoIntegracaoSAP.Inclusao && string.IsNullOrWhiteSpace(request.MOTORISTA_AR[0].VCTO_SSMA_RESPEME) ? string.Empty : "X"),
                VCTO_TRABAJO_ALTU = string.Empty  //Segundo André Araújo (no Teams), este campo será necessário apenas quando a Argentina tiver Lubricantes.
            };
        }
        private PersistirRequestItem4[] CriarRequestConsultaMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem4[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem4();
            //motoristas[0].RELCUROPDER = new DateTime(2020,2,17).ToString("yyyy-MM-dd");
            motoristas[0].CONDUCTOR = motorista.MotoristaArgentina.DNI;
            motoristas[0].TPOPER = "C";
            return motoristas;
        }

        private PersistirRequestItem4[] ExcluirMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem4[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem4();
            motoristas[0].CONDUCTOR = motorista.MotoristaArgentina.DNI;
            motoristas[0].TPOPER = "E";
            return motoristas;
        }

        private PersistirRequestItem4[] CriarMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem4[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem4();

            motoristas[0].APELLIDO = motorista.MotoristaArgentina.Apellido;
            motoristas[0].COD_TRANSPORTISTA = motorista.IBMTransportadora;
            motoristas[0].CONDUCTOR = motorista.MotoristaArgentina.DNI;
            motoristas[0].LIC_NAC_COND = motorista.MotoristaArgentina.LicenciaNacionalConducir;
            motoristas[0].LIC_NAC_HABIL = motorista.MotoristaArgentina.LicenciaNacionalHabilitante;
			motoristas[0].MODO_ENTREGA = motorista.Operacao;
            motoristas[0].NOMBRE = motorista.Nome;

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

            motoristas[0].TARJETA_ACCESO = motorista.MotoristaArgentina.Tarjeta;
            motoristas[0].TIPO_NEGOCIO = "COMB";
            motoristas[0].TPOPER = "I";

            if (motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
            {
                var siglaLNC = "LNC";

                var mesesLNC = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == siglaLNC) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == siglaLNC).MesesValidade : 0);
                var mesesLNH = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "LNH") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "LNH").MesesValidade : 0);
                var mesesARTPOL = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "ARTPOL") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "ARTPOL").MesesValidade : 0);
                var mesesARTCUO = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "ARTCUO") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "ARTCUO").MesesValidade : 0);
                var mesesSEGPOL = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SEGPOL") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SEGPOL").MesesValidade : 0);
                var mesesSEGCUO = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SEGCUO") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SEGCUO").MesesValidade : 0);
                var mesesSSMAMD = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SSMAMD") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SSMAMD").MesesValidade : 0);
                var mesesSSMARE = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SSMARE") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SSMARE").MesesValidade : 0);
                var mesesSSMAPOT = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SSMAPOT") ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SSMAPOT").MesesValidade : 0);

                //Licencia Nacional de Conducir
                motoristas[0].VCTO_LIC_NAC_COND = (motorista.Documentos != null
                                                      && motorista.Documentos.Any(w => w.Sigla == siglaLNC)
                                                      && motorista.Documentos.FirstOrDefault(w => w.Sigla == siglaLNC).DataVencimento.HasValue) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == siglaLNC).DataVencimento.Value.AddMonths(mesesLNC * -1).ToString("yyyy-MM-dd") : null;
                //Licencia Nacional Habilitante
                motoristas[0].VCTO_LIC_NAC_HABIL = (motorista.Documentos != null
                                                      && motorista.Documentos.Any(w => w.Sigla == "LNH")
                                                      && motorista.Documentos.FirstOrDefault(w => w.Sigla == "LNH").DataVencimento.HasValue) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "LNH").DataVencimento.Value.AddMonths(mesesLNH * -1).ToString("yyyy-MM-dd") : null;
                //ART - Poliza
                motoristas[0].VCTO_ART_POLIZA = (motorista.Documentos != null
                                                    && motorista.Documentos.Any(w => w.Sigla == "ARTPOL")
                                                    && motorista.Documentos.FirstOrDefault(w => w.Sigla == "ARTPOL").DataVencimento.HasValue) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "ARTPOL").DataVencimento.Value.AddMonths(mesesARTPOL * -1).ToString("yyyy-MM-dd") : null;
                //ART -Cuota
                motoristas[0].VCTO_ART_CUOTA = (motorista.Documentos != null
                                                    && motorista.Documentos.Any(w => w.Sigla == "ARTCUO")
                                                    && motorista.Documentos.FirstOrDefault(w => w.Sigla == "ARTCUO").DataVencimento.HasValue) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "ARTCUO").DataVencimento.Value.AddMonths(mesesARTCUO * -1).ToString("yyyy-MM-dd") : null;
                //Seguro - Poliza
                motoristas[0].VCTO_SEG_VIDA_POL = (motorista.Documentos != null
                                                    && motorista.Documentos.Any(w => w.Sigla == "SEGPOL")
                                                    && motorista.Documentos.FirstOrDefault(w => w.Sigla == "SEGPOL").DataVencimento.HasValue) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SEGPOL").DataVencimento.Value.AddMonths(mesesSEGPOL * -1).ToString("yyyy-MM-dd") : null;

                //Seguro - Cuota
                motoristas[0].VCTO_SEG_VIDA_CUO = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SEGCUO")) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SEGCUO").DataVencimento.Value.AddMonths(mesesSEGCUO * -1).ToString("yyyy-MM-dd") : null;

                //SSMA Manejo Defensivo
                motoristas[0].VCTO_SSMA_MAN_DEF = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SSMAMD")) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SSMAMD").DataVencimento.Value.AddMonths(mesesSSMAMD * -1).ToString("yyyy-MM-dd") : null;

                //CURSO DE MOVIMENTAÇÃO E OPERAÇÃO DE PRODUTOS PERIGOSOS
                motoristas[0].VCTO_SSMA_RESPEME = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SSMARE")) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SSMARE").DataVencimento.Value.AddMonths(mesesSSMARE * -1).ToString("yyyy-MM-dd") : null;

                //SSMA Políticas
                motoristas[0].VCTO_SSMA_POLITICA = (motorista.Documentos != null && motorista.Documentos.Any(w => w.Sigla == "SSMAPOT")) ? motorista.Documentos.FirstOrDefault(w => w.Sigla == "SSMAPOT").DataVencimento.Value.AddMonths(mesesSSMAPOT * -1).ToString("yyyy-MM-dd") : null;

                //motoristas[0].VCTO_TRABAJO_ALTU = ""; --Segundo André Araújo (no Teams), este campo será necessário apenas quando a Argentina tiver Lubricantes.
            }

            return motoristas;
        }
    }
}
