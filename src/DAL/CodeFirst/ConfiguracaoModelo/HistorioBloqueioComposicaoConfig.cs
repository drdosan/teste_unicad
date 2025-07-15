
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class HistorioBloqueioComposicaoConfig : EntityTypeConfiguration<HistorioBloqueioComposicao>
    {
        public HistorioBloqueioComposicaoConfig()
        {
            this.ToTable("HistorioBloqueioComposicao");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDComposicao).HasColumnName("IDComposicao");
            this.Property(t => t.Justificativa).HasColumnName("Justificativa");
            this.Property(t => t.Bloqueado).HasColumnName("Bloqueado");

        }
    }
}

