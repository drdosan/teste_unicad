
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

using Raizen.Framework.Entity.CodeFirst;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class UniCadContexto : BaseCodeFirstContext
    {

        public UniCadContexto(DbConnection connection, DbCompiledModel model) : base(connection, model)
        {
            Database.CommandTimeout = 3600;
            Database.SetInitializer<UniCadContexto>(null);
        }

        public UniCadContexto(DbConnection connection, DbCompiledModel model, int timeout) : base(connection, model)
        {
            Database.CommandTimeout = timeout;
            Database.SetInitializer<UniCadContexto>(null);
        }
    }
}

