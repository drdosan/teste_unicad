using Raizen.UniCad.Model;
using System;
using System.Linq.Expressions;

namespace Raizen.UniCad.DAL.Interfaces
{
    public interface IPlacaRepository
    {
        Placa Selecionar(int id);

        Placa Selecionar(Expression<Func<Placa, bool>> where);
    }
}
