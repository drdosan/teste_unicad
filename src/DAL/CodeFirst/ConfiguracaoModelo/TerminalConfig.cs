
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class TerminalConfig : EntityTypeConfiguration<Terminal>
    {
        public TerminalConfig()
        {
            this.ToTable("Terminal");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.Sigla).HasColumnName("Sigla");
            this.Property(t => t.Cidade).HasColumnName("Cidade");
            this.Property(t => t.Endereco).HasColumnName("Endereco");
            this.Property(t => t.IDEstado).HasColumnName("IDEstado");
            this.Property(t => t.isPool).HasColumnName("isPool");
        }
    }
}

