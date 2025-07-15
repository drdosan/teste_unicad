
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  using Raizen.UniCad.Model;
  using Raizen.UniCad.DAL.CodeFirst;

  namespace Raizen.UniCad.DAL
  {
  public interface IUniCadDalRepositorio<T>
    : IUniCadDalCodeFirst<T>
    {
    }
}    

