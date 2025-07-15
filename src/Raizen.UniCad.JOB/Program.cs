using Raizen.UniCad.BLL;
using Raizen.UniCad.Utils;
using System;
using System.Linq;

namespace Raizen.UniCad.JOB
{
    public class Program
    {
        const string JOB_NOME = "JOB_UNICAD";

        public static int Main(string[] args)
        {
            JobBusiness jobBLL = new JobBusiness();
            string argJobNome = args.Any() ? args[0] : null;
            string logJobNome = string.IsNullOrEmpty(argJobNome) ? $"{JOB_NOME}: Geral" : $"{JOB_NOME}: {argJobNome}";
            
            JobUtil.GravarLogInicio(logJobNome);

            var erros = jobBLL.ExecutarJob(argJobNome);

            try
            {
                if (erros == 0)
                    JobUtil.GravarLogSucesso(logJobNome);
                else
                    //TODO: SALVAR LOGS EM BANCOS TAMBEM
                    JobUtil.GravarLogErro(logJobNome, "JOB FINALIZADO COM ERROS! VERIFIQUE O LOG PARA MAIORES DETALHES");
            }
            catch (Exception ex)
            {
                JobUtil.GravarLogErro(logJobNome, ex.Message);
                erros = 1;
            }

            return erros;
        }
    }
}
