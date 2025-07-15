
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class ComposicaoEixoConfig : EntityTypeConfiguration<ComposicaoEixo>
    {
        public ComposicaoEixoConfig()
        {
            this.ToTable("ComposicaoEixo");

            this.HasKey(chave => chave.ID);


            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Codigo).HasColumnName("Codigo");
            this.Property(t => t.PesoMaximoPorEixo).HasColumnName("PesoMaximoPorEixo");
            this.Property(t => t.ComprimentoMaximo).HasColumnName("ComprimentoMaximo");
            this.Property(t => t.Imagem).HasColumnName("Imagem");
            this.Property(t => t.Ativo).HasColumnName("Ativo");

        }
    }
}

