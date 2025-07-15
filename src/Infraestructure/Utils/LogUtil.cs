using Raizen.Framework.Log.Client;
using Raizen.Framework.Log.Model;
using System;
using System.Configuration;

namespace Raizen.UniCad.Utils
{
    public static class LogUtil
    {
        public static void GravarLog(string origem, string descricao)
        {
            GravarLog(origem, descricao, string.Empty);
        }

        public static void GravarLog(string origem, string descricao, Exception ex)
        {
            GravarLog(origem, descricao, ExceptionUtil.ExceptionText(ex));
        }

        public static void GravarLog(string origem, string descricao, string detalhe)
        {
            LogErro log = new LogErro();

            log.ActionSource = origem;
            log.Message = descricao;
            log.StackTrace = detalhe;
            log.SiglaAplicacao = "UNICA";
            log.DateOccurrence = DateTime.Now;
            log.Username = "JOB";

            Logger.GerarLogErro(log);
        }

        public static void GravarLog(string origem, string descricao, string detalhe, string username)
        {
            LogErro log = new LogErro();

            log.ActionSource = origem;
            log.Message = descricao;
            log.StackTrace = detalhe;
            log.SiglaAplicacao = "UNICA";
            log.DateOccurrence = DateTime.Now;
            log.Username = username;

            Logger.GerarLogErro(log);
        }
    }
}
