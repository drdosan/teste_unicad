
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaClienteConfig : EntityTypeConfiguration<MotoristaCliente>
    {
        public MotoristaClienteConfig()
        {
            this.ToTable("MotoristaCliente");

            this.HasKey(chave => chave.ID);

            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDCliente).HasColumnName("IDCliente");
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t => t.DataAprovacao).HasColumnName("DataAprovacao");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
        }
    }
}

