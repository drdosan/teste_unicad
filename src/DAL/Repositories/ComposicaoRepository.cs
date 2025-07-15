using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.Repositories
{
    public class ComposicaoRepository : Repository<Composicao>, IComposicaoRepository
    {
        public ComposicaoRepository(UniCadContexto contexto) : base(contexto)
        {
        }
    }
}
