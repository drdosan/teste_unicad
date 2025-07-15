using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease.Activities;

namespace Raizen.UniCad.Web.Controllers
{
    public class ArquivoController : Controller
    {
        public JsonResult AnexarArquivo()
        {
            var model = new JsonResult();
            try
            {
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    if (Request.Files[0].ContentLength > 20971520) //20mb
                    {
                        model.Data = "Tamanho Arquivo";
                        return model;
                    }
                    string fileName = null;

                    fileName = ArquivoUtil.SalvarArquivo(Request.Files[0], Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int) EnumPais.Padrao));

                    model.Data = fileName;
                }
            }
            catch (Exception ex)
            {
                model.Data = StringUtil.ExceptionText(ex);
            }
            return model;
        }

        public JsonResult UploadFotoCracha()
        {
            var model = new JsonResult();
            try
            {
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    if (Request.Files[0].ContentLength > 2097152) //2mb
                    {
                        model.Data = "Tamanho Arquivo";
                        return model;
                    }
                    string fileName = null;

                    fileName = ArquivoUtil.SalvarArquivo(Request.Files[0], Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao));

                    model.Data = fileName;
                }
            }
            catch (Exception ex)
            {
                model.Data = StringUtil.ExceptionText(ex);
            }
            return model;
        }

        //public JsonResult UploadFotoCrachaStream()
        //{
        //    var model = new JsonResult();
        //    try
        //    {
        //        if (Request.Files != null && Request.Files.Count > 0)
        //        {
        //            if (Request.Files[0].ContentLength > 2097152) //2mb
        //            {
        //                model.Data = "Tamanho Arquivo";
        //                return model;
        //            }


        //            //string fileName = null;

        //            //fileName = ArquivoUtil.SalvarArquivo(Request.Files[0], Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao));

        //            //model.Data = fileName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        model.Data = StringUtil.ExceptionText(ex);
        //    }
        //    return model;
        //}


        public FileResult Download(string file)
        {
            if (!file.Contains("..") || !file.Contains("/") || !file.Contains("\\"))
            {
                var uploadPath = Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao) + "/" + file;

#if DEBUG
                //uploadPath = "C:\\Raizen\\" + "/" + file;
                // Code scanning: Uncontrolled data used in path expression
                uploadPath = @"C:\Raizen";
                var userInput = file;
                string safePath = Path.Combine(uploadPath, Path.GetFileName(userInput));
#endif
                byte[] fileBytes = System.IO.File.ReadAllBytes(uploadPath);
                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = file;
                return response;
            } else
            {
                return new FileContentResult(null, "application/octet-stream"); ;
            }
        }
    }
}