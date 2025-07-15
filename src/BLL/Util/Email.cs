using Raizen.Framework.Log.Bases;
using Raizen.Framework.Utils.Mail;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Raizen.UniCad.BLL
{
    /// <summary>
    /// Classe de envio de email
    /// </summary>
    public static class Email
    {
        /// <summary>
        /// Enviar email
        /// </summary>
        /// <param name="para">Destinatário</param>
        /// <param name="assunto">Assunto do email</param>
        /// <param name="mensagem">Mensagem do email</param>
        /// <param name="anexos"></param>
        /// <param name="isOrigemJob">Origem job utiliza outro email teste</param>
        public static bool Enviar(string para, string assunto, string mensagem, IList<string> anexos = null, bool isOrigemJob = false, Action<string, string> logar = null)
        {
#if DEBUG2
            return true;
#endif
            var logDocumentoBusiness = new LogDocumentosBusiness();

            try
            {
                logar?.Invoke("Inicio envio email", "Inicio envio email");

                //USAR ANTIX SMTP IMPOSTOR NESSE IP E PORTA
                var emailTeste = isOrigemJob ? Config.GetConfig(EnumConfig.emailDebugJob, (int)EnumPais.Padrao) : Config.GetConfig(EnumConfig.emailDebug, (int)EnumPais.Padrao);
                if (!string.IsNullOrWhiteSpace(emailTeste))
                {
                    para = emailTeste;
                }

                var emailComCopiaPara = Config.GetConfig(EnumConfig.EmailComCopiaPara, (int)EnumPais.Padrao);

                var emails = para.Split(';');
                foreach (var to in emails)
                { 
                    logar?.Invoke("Enviando e-mail", string.Format("Enviando e-mail para: {0}", to));
                    var mail = new SmtpModel
                    {
                        From = ConfigurationManager.AppSettings["EmailNaoResponda"] ?? "naoresponda@raizen.com",
                        Body = mensagem,
                        IsBodyHtml = true,
                        Subject = assunto
                    };
                    if (anexos != null)
                    {
                        mail.AttachmentFilePaths = anexos;
                    }

                    mail.To = to;

                    if (!string.IsNullOrEmpty(emailComCopiaPara))
                    {
                        mail.BCC = emailComCopiaPara.Replace(";", ",");
                    }
                    SmtpService.Send(mail);

                    logar?.Invoke("Enviado e-mail", "E-mail enviado.");

                    try
                    {
                        logar?.Invoke("Inserindo LogDocumentos", "Inserindo LogDocumentos do envio de e-mail.");

                        var log = new LogDocumentos();

                        log.Email = string.IsNullOrEmpty(mail.BCC) ? to : string.Format("{0};{1}", to, mail.BCC);
                        log.Mensagem = mensagem;
                        log.Nome = assunto;
                        log.Data = DateTime.Now;

                        logDocumentoBusiness.Adicionar(log);
                        logar?.Invoke("Inserido LogDocumentos", "Inserido LogDocumentos.");
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            logar?.Invoke("Fim envio email", string.Format("Fim envio email - ERRO {0}", ex.InnerException.ToString()));
                        }
                        else
                        {
                            logar?.Invoke("Fim envio email", string.Format("Fim envio email - ERRO {0}", ex.ToString()));
                        }

                        throw new RaizenException(ex.Message, ex);
                    }
                }
                logar?.Invoke("Fim envio email", "Fim envio email - SUCESSO");

                return true;
            }
            catch (Exception ex)
            {
                logar?.Invoke("Fim envio email", string.Format("Fim envio email - ERRO {0}", ex.ToString()));

                new RaizenException(ex.Message, ex).LogarErro();

                return false;
            }
        }
    }
}
