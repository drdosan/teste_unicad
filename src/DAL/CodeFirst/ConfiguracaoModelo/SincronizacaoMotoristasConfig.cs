
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class SincronizacaoMotoristasConfig : EntityTypeConfiguration<SincronizacaoMotoristas>
    {
        public SincronizacaoMotoristasConfig()
        {
            this.ToTable("SincronizacaoMotoristas");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t=> t.Data).HasColumnName("Data");
            this.Property(t=> t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t=> t.IsOk).HasColumnName("IsOk");
            this.Property(t=> t.Mensagem).HasColumnName("Mensagem");            
        }
    }
}

