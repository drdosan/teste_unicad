using Raizen.UniCad.Model;
using System;
using System.Linq.Expressions;

namespace Raizen.UniCad.DAL.Interfaces
{
    public interface IComposicaoRepository
    {
        Composicao Selecionar(int id);

        Composicao Selecionar(Expression<Func<Composicao, bool>> where);
    }
}
