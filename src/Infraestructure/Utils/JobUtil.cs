using System;

namespace Raizen.UniCad.Utils
{
    public static class JobUtil
    {
        private const string MsgExecucaoInicio = "{0} - Iniciado.";
        private const string MsgExecucaoTermino = "{0} - Finalizado.";
        private const string MsgExecutadoSucesso = "{0} - Executado com sucesso!";
        private const string MsgExecutadoErro = "{0} - Executado com erro!";        

        public static void GravarLogInicio(string nome)
        {
            LogUtil.GravarLog(nome, string.Format(MsgExecucaoInicio, nome));
            EscreverConsole(MsgExecucaoInicio, nome);
        }

        public static void GravarLogTermino(string nome)
        {
            LogUtil.GravarLog(nome, string.Format(MsgExecucaoTermino, nome));
            EscreverConsole(MsgExecucaoTermino, nome);
        }

        public static void GravarLogSucesso(string nome)
        {
            LogUtil.GravarLog(nome, string.Format(MsgExecutadoSucesso, nome));
            EscreverConsole(MsgExecutadoSucesso, nome);
        }

        public static void GravarLogErro(string nome, Exception ex)
        {
            GravarLogErro(nome, ExceptionUtil.ExceptionText(ex));
        }

        public static void GravarLog(string nome, string mensagem, string detalhes)
        {
            LogUtil.GravarLog(nome, mensagem, detalhes);
            EscreverConsole(mensagem, nome);
            EscreverConsole("Mensagens: {0}", mensagem);
        }

        public static void GravarLogErro(string nome, string erro)
        {
            LogUtil.GravarLog(nome, string.Format(MsgExecutadoErro, nome), erro);
            EscreverConsole(MsgExecutadoErro, nome);
            EscreverConsole("Erro: {0}", erro);
        }

        public static void GravarLogErroRecursivo(string nome, Exception ex)
        {
            var detalhe = ex.ToString();
            var root = ex.InnerException;

            while (root != null)
            {
                detalhe += "\n\n\n\n-----------------------------------------------\n" + root.ToString();
                root = root.InnerException;
            }

            LogUtil.GravarLog(nome, string.Format(MsgExecutadoErro, nome), detalhe);
            EscreverConsole(MsgExecutadoErro, nome);
            EscreverConsole("Erro: {0}", detalhe);
        }

        public static void EscreverConsole(string texto, string valor)
        {
            EscreverConsole(string.Format(texto, valor));
        }

        public static void EscreverConsole(string texto)
        {
            Console.WriteLine(texto);
        }
    }
}
