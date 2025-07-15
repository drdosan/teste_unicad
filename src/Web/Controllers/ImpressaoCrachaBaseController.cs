using Raizen.Framework.Web.MVC.Models;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
    public class ImpressaoCrachaBaseController : BaseUniCadController
    {
        

        #region Constantes
        private readonly ImpressaoCrachaBusiness _impressaoCrachaBll = new ImpressaoCrachaBusiness();
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelImpressaoCracha model = new ModelImpressaoCracha();
            model = ImpressaoCrachaLayout(model);
            return View("Index", model);
        }

        #endregion



        [HttpPost]
        public ActionResult DownloadCracha(ModelImpressaoCracha model)
        {
            if (model.Foto == null)
            {
                model = ImpressaoCrachaLayout(model);
                model.Resultado = new ResultadoOperacao();
                model.Resultado.CodigoResultado = Framework.Models.TipoResultado.Warning;
                model.Resultado.Mensagem = "Foto não carregada!";
                model.ContemErrosModel = "S";
                return RedirectToAction("Index", model);
            }

            ArquivoBusiness arquivoBLL = new ArquivoBusiness();
            bool download = true;

            try
            {
                if (String.IsNullOrWhiteSpace(model.Filtro.CPF) && String.IsNullOrWhiteSpace(model.Filtro.DNI))
                {
                    model = ImpressaoCrachaLayout(model);
                    model.Resultado = new ResultadoOperacao();
                    model.Resultado.CodigoResultado = Framework.Models.TipoResultado.Warning;
                    model.Resultado.Mensagem = "Digite um CPF!";
                    model.ContemErrosModel = "S";
                    return RedirectToAction("Index", model);
                }

                if (!String.IsNullOrWhiteSpace(model.Filtro.CPF))
                    model.MotoristaView = _impressaoCrachaBll.BuscarMotorista(model.Filtro);


                if (!String.IsNullOrWhiteSpace(model.Filtro.DNI))
                    model.MotoristaView = _impressaoCrachaBll.BuscarMotoristaArgentina(model.Filtro);


                if (model.MotoristaView != null)
                {
                    Motorista motorista = _motoristaBll.Selecionar(model.MotoristaView.ID);

                    if (!String.IsNullOrWhiteSpace(model.Filtro.CPF))
                    {
                        model.ImpressaoCrachaRetornoView = _impressaoCrachaBll.ValidarMotoristaImpressaoCracha(model.MotoristaView);
                        if (!model.ImpressaoCrachaRetornoView.AptoParaImpressaoDeCracha)
                        {
                            return View("Index", model);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(model.Filtro.DNI))
                    {
                        model.ImpressaoCrachaRetornoView = _impressaoCrachaBll.ValidarMotoristaArgentinoImpressaoCracha(model.MotoristaView);
                        if (!model.ImpressaoCrachaRetornoView.AptoParaImpressaoDeCracha)
                        {
                            return View("Index", model);
                        }
                    }

                   


                    motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(model.MotoristaView.ID);
                    var bytes = arquivoBLL.DownloadCrachaPDF(motorista, model.Foto, download);

                    var nomeArquivoDownload = "Cracha_" + model.MotoristaView.Nome.ToString() + ".pdf";


                    return File(bytes, "application/pdf", nomeArquivoDownload);


                }
                else
                {
                    model = ImpressaoCrachaLayout(model);
                    model.Resultado = new ResultadoOperacao();
                    model.Resultado.CodigoResultado = Framework.Models.TipoResultado.Warning;
                    model.Resultado.Mensagem = "Erro não Tratado!";
                    model.ContemErrosModel = "S";
                    return RedirectToAction("Index", model);
                }

            }
            catch (Exception ex)
            {
                model = ImpressaoCrachaLayout(model);
                return RedirectToAction("Index", model);
            }
        }




        [HttpPost]
        public ActionResult VisualizarCracha(ModelImpressaoCracha model)
        {

            var download = false;

            if (model.Foto == null)
            {
                return Json(new { success = false, responseText = "Foto não carregada!" }, JsonRequestBehavior.AllowGet);
            }


            try
            {

                ArquivoBusiness arquivoBLL = new ArquivoBusiness();



                if (String.IsNullOrWhiteSpace(model.Filtro.CPF) && String.IsNullOrWhiteSpace(model.Filtro.DNI))
                {
                    return Json(new { success = false, responseText = "Digite um CPF!" }, JsonRequestBehavior.AllowGet);

                }

                if (!String.IsNullOrWhiteSpace(model.Filtro.CPF))
                {
                    model.MotoristaView = _impressaoCrachaBll.BuscarMotorista(model.Filtro);
                }


                if (!String.IsNullOrWhiteSpace(model.Filtro.DNI))
                {
                    model.MotoristaView = _impressaoCrachaBll.BuscarMotoristaArgentina(model.Filtro);
                }

                if (model.MotoristaView != null)
                {

                    Motorista motorista = _motoristaBll.Selecionar(model.MotoristaView.ID);

                    if (!String.IsNullOrWhiteSpace(model.Filtro.CPF))
                    {
                        model.ImpressaoCrachaRetornoView = _impressaoCrachaBll.ValidarMotoristaImpressaoCracha(model.MotoristaView);
                        if (!model.ImpressaoCrachaRetornoView.AptoParaImpressaoDeCracha)
                        {
                            return Json(new { success = false, responseText = model.ImpressaoCrachaRetornoView.MensagemSituacao }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(model.Filtro.DNI))
                    {
                        model.ImpressaoCrachaRetornoView = _impressaoCrachaBll.ValidarMotoristaArgentinoImpressaoCracha(model.MotoristaView);
                        if (!model.ImpressaoCrachaRetornoView.AptoParaImpressaoDeCracha)
                        {
                            return Json(new { success = false, responseText = model.ImpressaoCrachaRetornoView.MensagemSituacao }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    



                    motorista.Documentos = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(model.MotoristaView.ID);
                    var bytes = arquivoBLL.DownloadCrachaPDF(motorista, model.Foto, download);


                    var json = Json(new { success = true, responseText = "ok!", bytes = Convert.ToBase64String(bytes, 0, bytes.Length) }, JsonRequestBehavior.AllowGet);
                    json.MaxJsonLength = Int32.MaxValue;

                    return json;
                }
                else
                {
                    return Json(new { success = false, responseText = "Erro não tratado!" }, JsonRequestBehavior.AllowGet);
                }



            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    responseText = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }




        #region ImpressaoCrachaLayout
        private ModelImpressaoCracha ImpressaoCrachaLayout(ModelImpressaoCracha model)
        {
            model.ConfiguracaoLayout.UtilizaComponenteBusca = true;
            model.ConfiguracaoLayout.UtilizaEmailUsuario = true;
            model.ConfiguracaoLayout.UtilizaListaAplicacoes = false;
            model.ConfiguracaoLayout.UtilizaMenuEsquerdo = true;
            model.ConfiguracaoLayout.UtilizaNotificacaoUsuario = true;
            model.ConfiguracaoLayout.UtilizaPerfilUsuario = true;
            model.ConfiguracaoLayout.UtilizaStatusTime = false;
            model.ConfiguracaoLayout.UtilizaTarefaUsuario = false;
            model.Motorista = new Model.Motorista();
            model.Filtro = new ImpressaoCrachaFiltro();
            model.ImpressaoCrachaRetornoView = new ImpressaoCrachaRetornoView();

            return model;
        }
        #endregion




    }
}