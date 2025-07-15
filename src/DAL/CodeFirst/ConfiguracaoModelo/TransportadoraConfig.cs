
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class TransportadoraConfig : EntityTypeConfiguration<Transportadora>
    {
        public TransportadoraConfig()
        {
            this.ToTable("Transportadora");

            this.HasKey(chave => chave.ID);
        }
    }
}

