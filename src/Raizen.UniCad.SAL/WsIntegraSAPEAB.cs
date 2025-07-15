using Raizen.UniCad.Model;
using Raizen.UniCad.SAL.WSIntegracaoSAP;
using Raizen.UniCad.SAL.WsIntegracaoSAPMotorista;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Raizen.UniCad.SAL
{
    public class WsIntegraSAPEAB
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

                    if (retornoInclusao.VeiculoEab.Any(w => w.MsgTpVeh == "E"))
                    {
                        retornoCt = retornoInclusao.VeiculoEab.Where(w => w.MsgTpVeh == "E").First().MsgVeh;
                        if (!retornoInclusao.VeiculoEab.Where(w => w.MsgTpVeh == "E").First().MsgVeh.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }
                    if (retornoInclusao.VeiculoEab.Any(w => w.MsgTpComp == "E"))
                    {
                        retornoCt = retornoInclusao.VeiculoEab.Where(w => w.MsgTpComp == "E").First().MsgComp;
                        if (!retornoInclusao.VeiculoEab.Where(w => w.MsgTpComp == "E").First().MsgComp.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }
                    if (retornoInclusao.VeiculoEab.Any(w => w.MsgTpCharac == "E"))
                    {
                        retornoCt = retornoInclusao.VeiculoEab.Where(w => w.MsgTpCharac == "E").First().MsgCharac;
                        if (!retornoInclusao.VeiculoEab.Where(w => w.MsgTpCharac == "E").First().MsgCharac.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }

                    retorno.Append(retornoCt);

                    break;
                case EnumTipoIntegracaoSAP.Alteracao:
                case EnumTipoIntegracaoSAP.Bloqueio:
                case EnumTipoIntegracaoSAP.Desbloqueio:
                case EnumTipoIntegracaoSAP.AprovarCheckList:
                case EnumTipoIntegracaoSAP.ReprovarCheckList:
                    var retornoAlteracao = Alterar(composicao, client, tipoIntegracao);
                    if (retornoAlteracao.VeiculoEab != null && retornoAlteracao.VeiculoEab.Any(w => w.MsgTpVeh == "E"))
                        retorno.Append(retornoAlteracao.VeiculoEab.Where(w => w.MsgTpVeh == "E").First().MsgVeh);
                    if (retornoAlteracao.VeiculoEab != null && retornoAlteracao.VeiculoEab.Any(w => w.MsgTpComp == "E"))
                        retorno.Append(retornoAlteracao.VeiculoEab.Where(w => w.MsgTpComp == "E").First().MsgComp);
                    if (retornoAlteracao.VeiculoEab != null && retornoAlteracao.VeiculoEab.Any(w => w.MsgTpCharac == "E"))
                        retorno.Append(retornoAlteracao.VeiculoEab.Where(w => w.MsgTpCharac == "E").First().MsgCharac);
                    break;
                case EnumTipoIntegracaoSAP.Excluir:
                    var retornoExclusao = Excluir(composicao, client, tipoIntegracao);
                    if (retornoExclusao.VeiculoEab != null && retornoExclusao.VeiculoEab.Any(w => w.MsgTpVeh == "E"))
                        retorno.Append(retornoExclusao.VeiculoEab.Where(w => w.MsgTpVeh == "E").First().MsgVeh);
                    if (retornoExclusao.VeiculoEab != null && retornoExclusao.VeiculoEab.Any(w => w.MsgTpComp == "E"))
                        retorno.Append(retornoExclusao.VeiculoEab.Where(w => w.MsgTpComp == "E").First().MsgComp);
                    if (retornoExclusao.VeiculoEab != null && retornoExclusao.VeiculoEab.Any(w => w.MsgTpCharac == "E"))
                        retorno.Append(retornoExclusao.VeiculoEab.Where(w => w.MsgTpCharac == "E").First().MsgCharac);
                    break;
                default:
                    break;
            }

            return retorno.ToString();
        }

        private WSIntegracaoSAP.CriarResponse Excluir(Composicao composicao, Veiculo_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WSIntegracaoSAP.CriarRequest();
            request.Ambiente = "EAB";
            request.ComposicaoTransporteEab = CriarComposicaoTransporte(composicao, EnumTipoIntegracaoSAP.Inclusao);
            request.LacreEab = CriarLacres(composicao, EnumTipoIntegracaoSAP.Inclusao);

            switch (composicao.IDTipoComposicao)
            {
                case (int)EnumTipoComposicao.Truck:
                    request.VeiculoEab = new VeiculoEab[1];
                    break;
                case (int)EnumTipoComposicao.Carreta:
                    request.VeiculoEab = new VeiculoEab[2];
                    break;
                case (int)EnumTipoComposicao.Bitrem:
                    request.VeiculoEab = new VeiculoEab[3];
                    break;
                case (int)EnumTipoComposicao.BitremDolly:
                    request.VeiculoEab = new VeiculoEab[3];
                    break;
                default:
                    break;
            }

            request.VeiculoEab[0] = CriarVeiculo(composicao, EnumTipoIntegracaoSAP.Excluir);

            string noCheckLeci = string.Empty;

            if (composicao.IgnorarLeciAdm.HasValue)
                noCheckLeci = composicao.IgnorarLeciAdm.Value ? "X" : "";

            request.VeiculoEab[0].NoCheckLeci = noCheckLeci;

            if (composicao.p2 != null)
                request.VeiculoEab[1] = new VeiculoEab() { Placa = composicao.p2.PlacaVeiculo, Operacao = "S", NoCheckLeci = noCheckLeci };
            if (composicao.p3 != null && composicao.IDTipoComposicao != (int)EnumTipoComposicao.BitremDolly)
                request.VeiculoEab[2] = new VeiculoEab() { Placa = composicao.p3.PlacaVeiculo, Operacao = "S", NoCheckLeci = noCheckLeci };
            if (composicao.p4 != null)
                request.VeiculoEab[2] = new VeiculoEab() { Placa = composicao.p4.PlacaVeiculo, Operacao = "S", NoCheckLeci = noCheckLeci };

            var retorno = client.Criar_Sync(request);

            return retorno;
        }

        private WSIntegracaoSAP.CriarResponse Alterar(Composicao composicao, Veiculo_OutService client, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var request = new WSIntegracaoSAP.CriarRequest();
            request.Ambiente = "EAB";
            request.ComposicaoTransporteEab = CriarComposicaoTransporte(composicao, tipoIntegracao);
            request.LacreEab = CriarLacres(composicao, tipoIntegracao);

            switch (composicao.IDTipoComposicao)
            {
                case (int)EnumTipoComposicao.Truck:
                    request.VeiculoEab = new VeiculoEab[1];
                    break;
                case (int)EnumTipoComposicao.Carreta:
                    request.VeiculoEab = new VeiculoEab[2];
                    break;
                case (int)EnumTipoComposicao.Bitrem:
                    request.VeiculoEab = new VeiculoEab[3];
                    break;
                case (int)EnumTipoComposicao.BitremDolly:
                    request.VeiculoEab = new VeiculoEab[3];
                    break;
                default:
                    break;
            }

            request.VeiculoEab[0] = CriarVeiculo(composicao, tipoIntegracao);

            if (composicao.p2 != null)
                request.VeiculoEab[1] = new VeiculoEab() { Placa = composicao.p2.PlacaVeiculo, Operacao = "S" };
            if (composicao.p3 != null && composicao.IDTipoComposicao != (int)EnumTipoComposicao.BitremDolly)
                request.VeiculoEab[2] = new VeiculoEab() { Placa = composicao.p3.PlacaVeiculo, Operacao = "S" };
            if (composicao.p4 != null)
                request.VeiculoEab[2] = new VeiculoEab() { Placa = composicao.p4.PlacaVeiculo, Operacao = "S" };

            if (tipoIntegracao == EnumTipoIntegracaoSAP.ReprovarCheckList)
            {
                request.VeiculoEab[0].Operacao = "I";
                var retornoAlt = client.Criar_Sync(request);

                if (retornoAlt != null && retornoAlt.VeiculoEab != null && !retornoAlt.VeiculoEab.Any(p => p.MsgTpVeh == "E"))
                {
                    request.VeiculoEab[0].Operacao = "B";
                    var retorno = client.Criar_Sync(request);

                    return retorno;
                }
                else
                    return retornoAlt;
            }
            else
            {
                var retorno = client.Criar_Sync(request);

                return retorno;
            }
        }

        private WSIntegracaoSAP.CriarResponse Incluir(Composicao composicao, Veiculo_OutService client)
        {
            var request = new WSIntegracaoSAP.CriarRequest();
            request.Ambiente = "EAB";
            request.ComposicaoTransporteEab = CriarComposicaoTransporte(composicao, EnumTipoIntegracaoSAP.Inclusao);
            request.LacreEab = CriarLacres(composicao, EnumTipoIntegracaoSAP.Inclusao);

            switch (composicao.IDTipoComposicao)
            {
                case (int)EnumTipoComposicao.Truck:
                    request.VeiculoEab = new VeiculoEab[1];
                    break;
                case (int)EnumTipoComposicao.Carreta:
                    request.VeiculoEab = new VeiculoEab[2];
                    break;
                case (int)EnumTipoComposicao.Bitrem:
                    request.VeiculoEab = new VeiculoEab[3];
                    break;
                case (int)EnumTipoComposicao.BitremDolly:
                    request.VeiculoEab = new VeiculoEab[3];
                    break;
                default:
                    break;
            }

            request.VeiculoEab[0] = CriarVeiculo(composicao, EnumTipoIntegracaoSAP.Inclusao);

            string noCheckLeci = string.Empty;

            if (composicao.IgnorarLeciAdm.HasValue)
                noCheckLeci = composicao.IgnorarLeciAdm.Value ? "X" : "";

            request.VeiculoEab[0].NoCheckLeci = noCheckLeci;

            if (composicao.p2 != null)
                request.VeiculoEab[1] = new VeiculoEab() { Placa = composicao.p2.PlacaVeiculo, Operacao = "S", NoCheckLeci = noCheckLeci };
            if (composicao.p3 != null && composicao.IDTipoComposicao != (int)EnumTipoComposicao.BitremDolly)
                request.VeiculoEab[2] = new VeiculoEab() { Placa = composicao.p3.PlacaVeiculo, Operacao = "S", NoCheckLeci = noCheckLeci };
            if (composicao.p4 != null)
                request.VeiculoEab[2] = new VeiculoEab() { Placa = composicao.p4.PlacaVeiculo, Operacao = "S", NoCheckLeci = noCheckLeci };

            var retorno = client.Criar_Sync(request);

            return retorno;
        }

        private LacreEab[] CriarLacres(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            int total = composicao.p1 != null ? composicao.p1.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p2 != null ? composicao.p2.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p3 != null ? composicao.p3.Compartimentos.Count(p => p.Operacao != "D") : 0;
            total += composicao.p4 != null ? composicao.p4.Compartimentos.Count(p => p.Operacao != "D") : 0;

            var lacres = new LacreEab[total];

            int i = 0;
            CarregarLacres(composicao.p1, tipoIntegracao, lacres, composicao.IDTipoComposicao, ref i);
            CarregarLacres(composicao.p2, tipoIntegracao, lacres, composicao.IDTipoComposicao, ref i);
            if (composicao.p3 != null && composicao.p3.IDTipoVeiculo != (int)EnumTipoVeiculo.Dolly)
                CarregarLacres(composicao.p3, tipoIntegracao, lacres, composicao.IDTipoComposicao, ref i);
            CarregarLacres(composicao.p4, tipoIntegracao, lacres, composicao.IDTipoComposicao, ref i);

            return lacres;
        }

        private void CarregarLacres(Placa p1, EnumTipoIntegracaoSAP tipoIntegracao, LacreEab[] lacres, int idTipo, ref int i)
        {
            int h = 1;
            if (p1 != null && p1.Compartimentos != null && p1.Compartimentos.Any())
                foreach (var item in p1.Compartimentos.Where(p => p.Operacao != "D"))
                {
                    lacres[i] = new WSIntegracaoSAP.LacreEab();
                    lacres[i].Compart = h.ToString();

                    if (idTipo == (int)EnumTipoComposicao.Truck)
                        lacres[i].Operacao = "I";
                    else
                        lacres[i].Operacao = "S";

                    lacres[i].Placa = p1.PlacaVeiculo;
                    lacres[i].QtdLacres = (byte)item.setas.Sum(p => p.Lacres);
                    lacres[i].QtdLacresSpecified = true;
                    i++;
                    h++;
                }
        }

        //O4V1
        private WSIntegracaoSAP.VeiculoEab CriarVeiculo(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var veiculo = new WSIntegracaoSAP.VeiculoEab();

            if (tipoIntegracao == EnumTipoIntegracaoSAP.Bloqueio)
                veiculo.Operacao = "B";
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.Desbloqueio)
                veiculo.Operacao = "D";
            else
                veiculo.Operacao = "I";

            if (tipoIntegracao == EnumTipoIntegracaoSAP.AprovarCheckList && composicao.checkList != null)
            {
                veiculo.VctoChecklist = composicao.checkList.Data.ToString("yyyy-MM-dd");
            }
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.ReprovarCheckList && composicao.checkList != null)
            {
                veiculo.VctoChecklist = DateTime.Now.ToString("yyyy-MM-dd");
                veiculo.Operacao = "B";
            }
            else if (tipoIntegracao == EnumTipoIntegracaoSAP.Excluir)
                veiculo.Operacao = "E";

            veiculo.Placa = composicao.p1.PlacaVeiculo;

            StringBuilder material = new StringBuilder();

            material.Append(composicao.p1.Uf);

            if (composicao.p2 != null)
                material.Append(string.Format(" {0}", composicao.p2.PlacaVeiculo));

            if (composicao.p3 != null && composicao.p3.IDTipoVeiculo != (int)EnumTipoVeiculo.Dolly)
                material.Append(string.Format(" {0}", composicao.p3.PlacaVeiculo));

            if (composicao.p4 != null)
                material.Append(string.Format(" {0}", composicao.p4.PlacaVeiculo));

            veiculo.TxMaterial = material.ToString();

            veiculo.PsBruto = (composicao.PBTC * 1000).Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
            veiculo.PsLiquido = (composicao.TaraComposicao * 1000).ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
            veiculo.Volume = (composicao.VolumeComposicao).ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
            veiculo.UnVol = "L";

            veiculo.Mfrnr = string.IsNullOrEmpty(composicao.IBMTransportadora2) ? composicao.IBMTransportadora : composicao.IBMTransportadora2;
            veiculo.Ano = composicao.p1.AnoFabricacao.ToString();
            veiculo.Eixo = composicao.EixosComposicao.ToString();
            veiculo.Marca = composicao.p1.Marca;
            veiculo.Modelo = composicao.p1.Modelo;
            veiculo.Cor = composicao.p1.Cor;
            veiculo.CidVei = composicao.p1.PlacaBrasil.Cidade;
            veiculo.Renavam = composicao.p1.PlacaBrasil.Renavam;
            veiculo.PropVei = string.IsNullOrEmpty(composicao.CPFCNPJArrendamento) ? "PROPRIO" : "TERCEIRO";
            veiculo.Tipo = "Tanque"; // fixo

            //aguardando desenvolvimento do SAP
            //veiculo.IgnorarLeci = composicao.IgnorarLeci;


            if (composicao.p1.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck)
                veiculo.TipProd = "Truck"; // fixo
            else
                veiculo.TipProd = "Cavalo Mecânico"; // fixo

            veiculo.TipCar = "Não aplicável"; // fixo
            veiculo.TipFin = "Tração"; // fixo
            //veiculo[0].TpMateiral = GetTpVeiculo(composicao);  // Fixo não mandar
            //veiculo[0].StInd // Fixo não mandar
            //veiculo[0].BsUndMed // Fixo não mandar
            //veiculo[0].GpMat // Fixo não mandar
            //veiculo[0].Vhart = "";
            veiculo.Uf = composicao.p1.Uf;

            var mesesAe = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "AE").Any() ? composicao.p1.Documentos.Where(w => w.Sigla == "AE").FirstOrDefault().MesesValidade : 0);
            veiculo.VctoAeAcimaPbtc = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "AE").Any()
                && composicao.p1.Documentos.Where(w => w.Sigla == "AE").FirstOrDefault().DataVencimento != null) ? composicao.p1.Documentos.Where(w => w.Sigla == "AE").FirstOrDefault().DataVencimento.Value.AddMonths(mesesAe * -1).ToString("yyyy-MM-dd") : null;

            var mesesAet = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "AET").Any() ? composicao.p1.Documentos.Where(w => w.Sigla == "AET").FirstOrDefault().MesesValidade : 0);
            veiculo.VctoAetComprto = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "AET").Any()
                && composicao.p1.Documentos.Where(w => w.Sigla == "AET").FirstOrDefault().DataVencimento != null) ? composicao.p1.Documentos.Where(w => w.Sigla == "AET").FirstOrDefault().DataVencimento.Value.AddMonths(mesesAet * -1).ToString("yyyy-MM-dd") : null;

            var mesesCvv = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "CVV").Any() ? composicao.p1.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault().MesesValidade : 0);
            veiculo.VctoAfericao = (composicao.p1.Documentos != null && composicao.p1.Documentos.Where(w => w.Sigla == "CVV").Any()
                && composicao.p1.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault().DataVencimento != null) ?
                composicao.p1.Documentos.Where(w => w.Sigla == "CVV").FirstOrDefault().DataVencimento.Value.AddMonths(mesesCvv * -1).ToString("yyyy-MM-dd") : null;

            var mesesCipp = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").Any() ? composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault().MesesValidade : 0);
            veiculo.VctoCipp = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").Any()
                && composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault().DataVencimento != null) ? composicao.p1.Documentos.Where(w => w.Sigla == "CIPP").FirstOrDefault().DataVencimento.Value.AddMonths(mesesCvv * -1).ToString("yyyy-MM-dd") : null;

            var mesesCiv = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "CIV").Any() ? composicao.p1.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault().MesesValidade : 0);
            veiculo.VctoCiv = (composicao.p1.Documentos != null
                && composicao.p1.Documentos.Where(w => w.Sigla == "CIV").Any()
                && composicao.p1.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault().DataVencimento != null) ? composicao.p1.Documentos.Where(w => w.Sigla == "CIV").FirstOrDefault().DataVencimento.Value.AddMonths(mesesCiv * -1).ToString("yyyy-MM-dd") : null;

            return veiculo;
        }

        private WSIntegracaoSAP.ComposicaoTransporteEab[] CriarComposicaoTransporte(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            int total = composicao.p1 != null ? composicao.p1.Compartimentos.Where(p => p.Operacao != "D").Sum(p => p.setas.Count) : 0;
            total += composicao.p2 != null ? composicao.p2.Compartimentos.Where(p => p.Operacao != "D").Sum(p => p.setas.Count) : 0;
            total += composicao.p3 != null ? composicao.p3.Compartimentos.Where(p => p.Operacao != "D").Sum(p => p.setas.Count) : 0;
            total += composicao.p4 != null ? composicao.p4.Compartimentos.Where(p => p.Operacao != "D").Sum(p => p.setas.Count) : 0;

            var composicaoTransporte = new WSIntegracaoSAP.ComposicaoTransporteEab[total];
            int i = 0;

            CarregarCompartimento(composicao.p1, tipoIntegracao, composicaoTransporte, composicao.IDTipoComposicao, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao);
            CarregarCompartimento(composicao.p2, tipoIntegracao, composicaoTransporte, composicao.IDTipoComposicao, composicao.jaExiste, ref i, null, composicao.Operacao, composicao.p2);
            CarregarCompartimento(composicao.p3, tipoIntegracao, composicaoTransporte, composicao.IDTipoComposicao, composicao.jaExiste, ref i, composicao.p2, composicao.Operacao, composicao.p2);
            CarregarCompartimento(composicao.p4, tipoIntegracao, composicaoTransporte, composicao.IDTipoComposicao, composicao.jaExiste, ref i, null, composicao.Operacao, composicao.p2);

            return composicaoTransporte;
        }

        private static void CarregarCompartimento(Placa placa, EnumTipoIntegracaoSAP tipoIntegracao, ComposicaoTransporteEab[] composicaoTransporte, int IDTipo, bool jaExiste, ref int i, Placa placa2 = null, string Operacao = null, Placa placaUnidTranp = null)
        {
            if (placa != null && placa.Compartimentos.Any())
            {
                int h = 1;
                foreach (var item in placa.Compartimentos.Where(p => p.Operacao != "D"))
                {
                    if (item.setas.Any())
                    {
                        int j = 1;
                        foreach (var seta in item.setas)
                        {
                            composicaoTransporte[i] = new WSIntegracaoSAP.ComposicaoTransporteEab();
                            composicaoTransporte[i].Compart = h.ToString();
                            composicaoTransporte[i].Placa = placa.PlacaVeiculo;
                            composicaoTransporte[i].NoSeta = j.ToString();
                            composicaoTransporte[i].Seta = seta.Volume.Value.ToString("N2", CultureInfo.InvariantCulture).Replace(",", "");
                            composicaoTransporte[i].IndSetaPrinc = seta.Principal ? "X" : null;

                            if (IDTipo == (int)EnumTipoComposicao.Truck)
                                composicaoTransporte[i].Operacao = "I";
                            else
                                composicaoTransporte[i].Operacao = "S";

                            i++;
                            j++;
                        }

                    }
                    h++;
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
                    if (retornoInclusao.MOTORISTA_EAB.Any(w => w.MSGTYP_MOT == "E"))
                    {
                        retornoCt = retornoInclusao.MOTORISTA_EAB.Where(w => w.MSGTYP_MOT == "E").First().MESSAGE_MOT;
                        if (!retornoInclusao.MOTORISTA_EAB.Where(w => w.MSGTYP_MOT == "E").First().MESSAGE_MOT.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }

                    if (retornoInclusao.MOTORISTA_EAB.Any(w => w.MSGTYP_CHARAC == "E"))
                    {
                        retornoCt = retornoInclusao.MOTORISTA_EAB.Where(w => w.MSGTYP_CHARAC == "E").First().MESSAGE_CHARAC;
                        if (!retornoInclusao.MOTORISTA_EAB.Where(w => w.MSGTYP_CHARAC == "E").First().MESSAGE_CHARAC.Contains("já existe"))
                        {
                            retorno.Append(retornoCt);
                            break;
                        }
                    }

                    retorno.Append(retornoCt);

                    if (retorno.ToString().ToLower(CultureInfo.InvariantCulture).Contains("já existe"))
                        motorista.jaExiste = true;
                    break;
                case EnumTipoIntegracaoSAP.Excluir:
                    var retornoExclusao = ExcluirMotorista(motorista, client);
                    if (retornoExclusao.MOTORISTA_EAB.Any(w => w.MSGTYP_MOT == "E"))
                    {
                        retorno.Append(retornoCt);
                    }
                    break;
                default:
                    break;
            }

            return retorno.ToString();
        }

        private PersistirResponse ExcluirMotorista(Motorista motorista, Motorista_OutService client)
        {
            var request = new WsIntegracaoSAPMotorista.PersistirRequest();
            request.Ambiente = "EAB";
            request.MOTORISTA_EAB = ExcluirMotorista(motorista);
            MarcarAlterado(motorista, request);
            var retorno = client.Persistir_Sync(request);

            return retorno;
        }

        private PersistirResponse IncluirMotorista(Motorista motorista, Motorista_OutService client)
        {
            var request = new WsIntegracaoSAPMotorista.PersistirRequest();
            request.Ambiente = "EAB";
            request.MOTORISTA_EAB = CriarMotorista(motorista, EnumTipoIntegracaoSAP.Inclusao);
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
            request.Ambiente = "EAB";
            request.MOTORISTA_EAB = CriarRequestConsultaMotorista(motorista);
            var retorno = client.Persistir_Sync(request);
            return retorno;
        }

        private static void MarcarAlterado(Motorista motorista, PersistirRequest request)
        {
            request.MOTORISTAX_EAB = new PersistirRequestItem3[1];
            request.MOTORISTAX_EAB[0] = new PersistirRequestItem3()
            {
                TPOPER = "I",
                STCD2 = motorista.MotoristaBrasil.CPF,
                DATA_NASCIMENTO = "X",
                NAME = "X",
                NAME3 = "X",
                NAME4 = "X",
                SMTP_ADDR = "X",
                SORT1 = "X",
                STCD3 = "X",
                STCD4 = "X",
                TELF1 = "X",
                STKZU = "X",
                VCTO_CARREG_MOTORISTA = "X",
                VCTO_CNH = "X",
                VCTO_MOPE = "X",
                VCTO_NR20 = "X",
                VCTO_NR35 = "X",
                VCTO_TREIN_MOTORISTA = "X"
            };
        }

        private PersistirRequestItem2[] CriarRequestConsultaMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem2[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem2();
            motoristas[0].STCD2 = motorista.MotoristaBrasil.CPF;
            motoristas[0].TPOPER = "C";
            return motoristas;
        }

        private PersistirRequestItem2[] ExcluirMotorista(Motorista motorista)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem2[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem2();

            motoristas[0].TPOPER = "E";
            motoristas[0].STCD2 = motorista.MotoristaBrasil.CPF;
            return motoristas;
        }

        private PersistirRequestItem2[] CriarMotorista(Motorista motorista, EnumTipoIntegracaoSAP inclusao)
        {
            var motoristas = new WsIntegracaoSAPMotorista.PersistirRequestItem2[1];
            motoristas[0] = new WsIntegracaoSAPMotorista.PersistirRequestItem2();

            if (motorista.MotoristaBrasil != null && motorista.MotoristaBrasil.Nascimento.HasValue)
                motoristas[0].DATA_NASCIMENTO = motorista.MotoristaBrasil.Nascimento.Value.ToString("yyyy-MM-dd"); ;

            motoristas[0].SMTP_ADDR = motorista.Email;

            if (motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
            {
                var mesesVctoNR20 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR20").Any() ? motorista.Documentos.Where(w => w.Sigla == "NR20").FirstOrDefault().MesesValidade : 0);
                var mesesVctoNR35 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR35").Any() ? motorista.Documentos.Where(w => w.Sigla == "NR35").FirstOrDefault().MesesValidade : 0);
                var mesesVctoMOPE = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "MOPP").Any() ? motorista.Documentos.Where(w => w.Sigla == "MOPP").FirstOrDefault().MesesValidade : 0);
                var mesesVctoCNH = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "CNH").Any() ? motorista.Documentos.Where(w => w.Sigla == "CNH").FirstOrDefault().MesesValidade : 0);

                motoristas[0].VCTO_NR20 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR20").Any()) ? motorista.Documentos.Where(w => w.Sigla == "NR20").FirstOrDefault().DataVencimento.Value.AddMonths(mesesVctoNR20 * -1).ToString("yyyy-MM-dd") : null;
                motoristas[0].VCTO_NR35 = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "NR35").Any()) ? motorista.Documentos.Where(w => w.Sigla == "NR35").FirstOrDefault().DataVencimento.Value.AddMonths(mesesVctoNR35 * -1).ToString("yyyy-MM-dd") : null;
                motoristas[0].VCTO_MOPE = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "MOPP").Any()) ? motorista.Documentos.Where(w => w.Sigla == "MOPP").FirstOrDefault().DataVencimento.Value.AddMonths(mesesVctoMOPE * -1).ToString("yyyy-MM-dd") : null;
                //motoristas[0].VCTO_CARREG_MOTORISTA = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "MOPP").Any()) ? motorista.Documentos.Where(w => w.Sigla == "EXAME").FirstOrDefault().DataVencimento.Value.ToString("yyyy-MM-dd") : null;
                motoristas[0].VCTO_CNH = (motorista.Documentos != null && motorista.Documentos.Where(w => w.Sigla == "CNH").Any()) ? motorista.Documentos.Where(w => w.Sigla == "CNH").FirstOrDefault().DataVencimento.Value.AddMonths(mesesVctoCNH * -1).ToString("yyyy-MM-dd") : null;
            }

            motoristas[0].TPOPER = "I";

            motoristas[0].KTOKK = "9500";

            motoristas[0].NAME = motorista.Nome;
            motoristas[0].NAME3 = motorista.MotoristaBrasil.RG;
            motoristas[0].NAME4 = motorista.MotoristaBrasil.OrgaoEmissor;

            if (motorista.Operacao == "FOB")
            {
                motoristas[0].SORT1 = motorista.Nome;
            }
            else
            {
                motoristas[0].SORT1 = motorista.NomeTransportadora;
            }
            motoristas[0].STCD2 = motorista.MotoristaBrasil.CPF;
            motoristas[0].STCD3 = motorista.MotoristaBrasil.CNH;
            motoristas[0].STCD4 = motorista.MotoristaBrasil.OrgaoEmissorCNH;

            switch (motorista.IDStatus)
            {
                case (int)EnumStatusMotorista.Aprovado:
                    motoristas[0].STKZU = ""; //em branco: apto
                    break;
                case (int)EnumStatusMotorista.Bloqueado: //I: Inapto
                    motoristas[0].STKZU = "X";
                    break;
                default:
                    break;
            }

            //bloqueado ou desbloqueado
            if (motorista.DataValidadeTreinamento.HasValue)
            {
                motoristas[0].VCTO_TREIN_MOTORISTA = motorista.DataValidadeTreinamento.Value.ToString("yyyy-MM-dd");// treinamento teórico
            }

            motoristas[0].TELF1 = motorista.Telefone;

            return motoristas;
        }
    }
}
