
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class ChecklistComposicaoConfig : EntityTypeConfiguration<ChecklistComposicao>
    {
        public ChecklistComposicaoConfig()
        {
            this.ToTable("ChecklistComposicao");

            this.HasKey(chave => chave.ID);


            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDComposicao).HasColumnName("IDComposicao");
            this.Property(t => t.Justificativa).HasColumnName("Justificativa");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.Data).HasColumnName("Data");
            this.Property(t => t.DataCadastro).HasColumnName("DataCadastro");
            this.Property(t => t.Usuario).HasColumnName("Usuario");
            this.Property(t => t.CodigoUsuario).HasColumnName("CodigoUsuario");

        }
    }
}

