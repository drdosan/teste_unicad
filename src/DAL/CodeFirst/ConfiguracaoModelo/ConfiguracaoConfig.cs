
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class ConfiguracaoConfig : EntityTypeConfiguration<Configuracao>
    {
        public ConfiguracaoConfig()
        {
            this.ToTable("Configuracao");

            this.HasKey(chave => chave.ID);


            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.NmVariavel).HasColumnName("NmVariavel");
            this.Property(t => t.Valor).HasColumnName("Valor");
            this.Property(t => t.Descricao).HasColumnName("Descricao");
            this.Property(t => t.DtCriacao).HasColumnName("DtCriacao");
            this.Property(t => t.DtAtualizacao).HasColumnName("DtAtualizacao");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.IdPais).HasColumnName("IdPais");

            this.HasOptional(t => t.Pais)
               .WithMany(fk => fk.Configuracoes)
               .HasForeignKey(key => key.IdPais);
        }
    }
}

