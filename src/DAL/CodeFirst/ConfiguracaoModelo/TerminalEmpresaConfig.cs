
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class TerminalEmpresaConfig : EntityTypeConfiguration<TerminalEmpresa>
    {
        public TerminalEmpresaConfig()
        {
            this.ToTable("TerminalEmpresa");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t=> t.IDTerminal).HasColumnName("IDTerminal");
            this.Property(t=> t.Nome).HasColumnName("Nome");
        }
    }
}

