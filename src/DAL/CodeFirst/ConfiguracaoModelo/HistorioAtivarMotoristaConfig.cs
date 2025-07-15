
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class HistorioAtivarMotoristaConfig : EntityTypeConfiguration<HistorioAtivarMotorista>
    {
        public HistorioAtivarMotoristaConfig()
        {
            this.ToTable("HistorioAtivarMotorista");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t => t.Justificativa).HasColumnName("Justificativa");
            this.Property(t => t.Ativo).HasColumnName("Ativo");
            this.Property(t => t.Data).HasColumnName("Data");

        }
    }
}

