
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
	public class UsuarioConfig : EntityTypeConfiguration<Usuario>
	{
		public UsuarioConfig()
		{
			this.ToTable("Usuario");

			this.HasKey(chave => chave.ID);

			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Nome).HasColumnName("Nome");

			this.Property(t => t.Login).HasColumnName("Login");
			this.Property(t => t.Email).HasColumnName("Email");
			this.Property(t => t.IDEmpresa).HasColumnName("IDEmpresa");
			this.Property(t => t.Operacao).HasColumnName("Operacao");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.Externo).HasColumnName("Externo");
			this.Property(t => t.IDPais).HasColumnName("IDPais");

		}
	}
}

