using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Utils
{
    public static class ArquivoUtil
    {
        #region CONSTANTES
        private const string ARQUIVO_TEMPORARIO = "{0:yyyyMMddHHmmss}_{1}";
        #endregion

        #region SALVAR ARQUIVO EXCEL
        public static string SalvarArquivo(HttpPostedFileBase ArquivoExcelInput, string caminhoSalvar)
        {
            try
            {  //Validação do formato do arquivo Excel
                if (ArquivoExcelInput.FileName.Trim().Length > 0 && ArquivoExcelInput.ContentLength > 0)
                {
                    string nomeArquivoOriginal = Path.GetFileName(ArquivoExcelInput.FileName);

                    if (!string.IsNullOrWhiteSpace(nomeArquivoOriginal))
                    {
                        return UploadArquivo(ArquivoExcelInput, caminhoSalvar, nomeArquivoOriginal);
                    }
                    else
                    {
                        return "O Arquivo selecionado está vazio!";
                    }
                }
                else
                {
                    return "O Arquivo selecionado está vazio!";
                }
            }
            catch (Exception ex)
            {
                return string.Format(GetTextoPorLingua("Ocorreu um erro ao realizar o upload: {0}.", "Se produjo un error al realizar el upload: {0}"), ex.Message);
            }
        }

        private static string UploadArquivo(HttpPostedFileBase ArquivoExcelInput, string caminhoSalvar, string nomeArquivoOriginal)
        {
            bool permitirSalvar = false;
            string[] extensoesValidas = new string[13];
            extensoesValidas[0] = "xls";
            extensoesValidas[1] = "xlsx";
            extensoesValidas[2] = "csv";
            extensoesValidas[3] = "jpg";
            extensoesValidas[4] = "jpeg";
            extensoesValidas[5] = "png";
            extensoesValidas[6] = "gif";
            extensoesValidas[7] = "doc";
            extensoesValidas[8] = "docx";
            extensoesValidas[9] = "pdf";
            extensoesValidas[10] = "ods";
            extensoesValidas[11] = "xlt";
            extensoesValidas[12] = "txt";

            //substituir espaço por _
            nomeArquivoOriginal = nomeArquivoOriginal.Replace(" ", "_");
            //remover caracteres especiais
            nomeArquivoOriginal =
                nomeArquivoOriginal.Substring(0, nomeArquivoOriginal.LastIndexOf('.')).RemoveSpecialCharacters()
                + "."
                + nomeArquivoOriginal.Substring(nomeArquivoOriginal.LastIndexOf('.') + 1).RemoveSpecialCharacters();

            string nomeArquivoTemporario = string.Format(ARQUIVO_TEMPORARIO, DateTime.Now, nomeArquivoOriginal);

            string extensao = Path.GetExtension(nomeArquivoTemporario).Replace(".", "");

            if (!string.IsNullOrWhiteSpace(extensao))
            {
                permitirSalvar = extensoesValidas.Any(i => i.ToLower(CultureInfo.InvariantCulture) == extensao.ToLower(CultureInfo.InvariantCulture));

            }
            else
            {
                permitirSalvar = false;
            }


            if (!permitirSalvar)
            {
                return string.Format("Formato de Arquivo Inválido. {0}", nomeArquivoOriginal);
            }

            var uploadPath = caminhoSalvar;
#if DEBUG
            uploadPath = "C:\\Raizen\\";
#endif

            string arquivoParaSalvar = Path.Combine(uploadPath, nomeArquivoTemporario);

            //Procura do caminho pra salvar Arquivo
            DirectoryInfo di = new DirectoryInfo(uploadPath);

            if (!di.Exists)
            {
                return string.Format("O caminho especificado para o upload não existe. {0}", uploadPath);
            }
            //Validação para salvar arquivo, identificado pelo formato do nome do arquivo.
            else
            {
                if (File.Exists(arquivoParaSalvar))
                {
                    return string.Format("Já existe um arquivo com esse mesmo nome salvo no diretorio. {0}", arquivoParaSalvar);
                }
            }

            //Salvando o arquivo na pasta do upload
            ArquivoExcelInput.SaveAs(arquivoParaSalvar);

            return nomeArquivoTemporario;
        }

        public static void ExcluirArquivo(string anexo, string caminhoArquivos)
        {
            try
            {
                if (!string.IsNullOrEmpty(anexo))
                    File.Delete(caminhoArquivos + anexo);
            }
            catch (Exception ex)
            {
                LogUtil.GravarLog("Arquivo", "Excluindo - " + anexo, ex);
            }
        }


        private static string GetTextoPorLingua(string msgPortugues, string msgEspanhol)
        {
            string SSO_IDIOMA = GetCookieIdioma();

            var pais = (SSO_IDIOMA.Equals("es-AR") ? EnumPais.Argentina : EnumPais.Brasil);

            switch (pais)
            {
                case EnumPais.Brasil:
                    return msgPortugues;
                case EnumPais.Argentina:
                    return msgEspanhol;
                default:
                    return msgPortugues;
            }
        }

        private static string GetCookieIdioma()
        {
            var cookie = "SSO_IDIOMA";

            if (HttpContext.Current != null && HttpContext.Current.Request.Cookies.Get(cookie) != null)
                return HttpContext.Current.Request.Cookies[cookie].Value;

            return "pt-BR";
        }
        #endregion


        //public static ArrayList ConverterPdfParaImagem(string path)
        //{
        //    string myPath;
        //    Guid myGuid;
        //    FrameDimension myDimension;
        //    ArrayList myImages = new ArrayList();
        //    int myPageCount;
        //    Bitmap myBMP;

        //    MemoryStream ms;
        //    System.Drawing.Image myImage;

        //    myPath = path;
        //    FileStream fs = new FileStream(myPath, FileMode.Open);
        //    myImage = System.Drawing.Image.FromStream(fs);
        //    myGuid = myImage.FrameDimensionsList[0];
        //    myDimension = new FrameDimension(myGuid);
        //    myPageCount = myImage.GetFrameCount(myDimension);
        //    for (int i = 0; i < myPageCount; i++)
        //    {
        //        ms = new MemoryStream();
        //        myImage.SelectActiveFrame(myDimension, i);
        //        myImage.Save(ms, ImageFormat.Bmp);
        //        myBMP = new Bitmap(ms);
        //        myImages.Add(myBMP);
        //        ms.Close();
        //    }
        //    fs.Close();

        //    return myImages;
        //}

        public static void LogText(string message)
        {
            string path = HttpContext.Current.Server.MapPath("~/Content/logtext.txt");
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(message);
                tw.Close();
            }
            else if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(message);
                tw.Close();
            }
        }
    }
}
