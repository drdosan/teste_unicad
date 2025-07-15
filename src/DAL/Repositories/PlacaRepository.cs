using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.Repositories
{
    public class PlacaRepository : Repository<Placa>, IPlacaRepository
    {
        public PlacaRepository(UniCadContexto contexto) : base(contexto)
        {
        }
    }
}
