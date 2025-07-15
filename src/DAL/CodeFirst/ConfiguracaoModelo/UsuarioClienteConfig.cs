
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class UsuarioClienteConfig : EntityTypeConfiguration<UsuarioCliente>
    {
        public UsuarioClienteConfig()
        {
            this.ToTable("UsuarioCliente");

            this.HasKey(chave => chave.ID);


            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.IDUsuario).HasColumnName("IDUsuario");
            this.Property(t => t.IDCliente).HasColumnName("IDCliente");
		}
    }
}

