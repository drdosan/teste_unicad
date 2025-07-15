
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaTreinamentoConfig : EntityTypeConfiguration<MotoristaTreinamento>
    {
        public MotoristaTreinamentoConfig()
        {
            this.ToTable("MotoristaTreinamento");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.DataValidadeTreinamentoPratico).HasColumnName("DataValidadeTreinamentoPratico");
            this.Property(t => t.DataValidadeTreinamentoTeorico).HasColumnName("DataValidadeTreinamentoTeorico");
        }
    }
}

