using Raizen.Framework.Utils.Cache;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class ConfigBusiness : UniCadBusinessBase<Configuracao>, IConfigBusiness
    {
        public ConfigBusiness()
        {

        }

        public int GetConfigInt(EnumConfig key, int idPais)
        {
            return GetConfigInt(key, false, idPais);
        }

        public int GetConfigInt(EnumConfig key, bool useCache, int idPais)
        {
            int val;

            int.TryParse(GetConfig(key, useCache, idPais), out val);

            return val;
        }

        public string GetConfig(EnumConfig key, int idPais)
        {
            return GetConfig(key, false, idPais);
        }

        private string GetConfig(EnumConfig key, bool useCache, int idPais)
        {
            if (useCache)
            {
                var dado = CacheManager.Instance.Get(key.ToString());

                if (dado != null)
                {
                    return dado.ToString();
                }
            }

            var config = Selecionar(w => w.NmVariavel == key.ToString() && (w.IdPais == idPais || w.IdPais == null));

            if (config == null)
            {
                return string.Empty;
            }

            return config.Valor;
        }
    }
}
