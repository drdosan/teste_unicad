
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class PlacaClienteConfig : EntityTypeConfiguration<PlacaCliente>
    {
        public PlacaClienteConfig()
        {
            this.ToTable("PlacaCliente");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDCliente).HasColumnName("IDCliente");
            this.Property(t => t.IDPlaca).HasColumnName("IDPlaca");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.DataAprovacao).HasColumnName("DataAprovacao");
        }
    }
}

