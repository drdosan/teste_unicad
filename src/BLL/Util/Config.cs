using Raizen.Framework.Utils.Cache;
using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL.Util
{
    public static class Config
    {
        #region [ Get ]

        public static int GetConfigInt(EnumConfig key, int idPais)
        {
            return GetConfigInt(key, false, idPais);
        }

        public static int GetConfigInt(EnumConfig key, bool useCache, int idPais)
        {
            int val;

            int.TryParse(GetConfig(key, useCache, idPais), out val);

            return val;
        }

        public static string GetConfig(EnumConfig key,int idPais)
        {
            return GetConfig(key, false,idPais);
        }

        public static string GetConfigAnexo(EnumConfig key)
        {
            ConfiguracaoBusiness configBll = new ConfiguracaoBusiness();

            var config = configBll.Selecionar(w => w.NmVariavel == key.ToString());

            if (config == null)
            {
                return string.Empty;
            }

            return config.Anexo;
        }

        public static string GetConfig(EnumConfig key, bool useCache, int idPais)
        {
            ConfiguracaoBusiness configBll = new ConfiguracaoBusiness();

            if (useCache)
            {
                var dado = CacheManager.Instance.Get(key.ToString());

                if (dado != null)
                {
                    return dado.ToString();
                }
            }

            var config = configBll.Selecionar(w => w.NmVariavel == key.ToString() && ( w.IdPais == idPais || w.IdPais == null ));

            if (config == null)
            {
                return string.Empty;
            }

            return config.Valor;
        }

		public static UniCadContexto GetContext()
		{
			var contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado());
#if DEBUG
			contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
			return contexto;
		}

		#endregion
	}
}
