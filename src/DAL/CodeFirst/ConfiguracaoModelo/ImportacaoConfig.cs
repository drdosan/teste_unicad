
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class ImportacaoConfig : EntityTypeConfiguration<Importacao>
    {
        public ImportacaoConfig()
        {
            this.ToTable("Importacao");

            this.HasKey(chave => chave.ID);
        }
    }
}

