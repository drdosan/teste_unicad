using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL.Interfaces
{
    public interface IConfigBusiness
    {
        int GetConfigInt(EnumConfig key, int idPais);
        int GetConfigInt(EnumConfig key, bool useCache, int idPais);
        string GetConfig(EnumConfig key, int idPais);
    }
}
