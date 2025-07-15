
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raizen.Framework.Entity.CodeFirst;

namespace Raizen.UniCad.DAL.CodeFirst
{
    internal class UniCadDalCodeFirst<T>
      : CodeFirstRepository<T> where T : class
    {
        public UniCadDalCodeFirst(UniCadContexto context)
            : base(context)
        {

        }

    }
}
  
