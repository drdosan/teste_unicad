using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo
{
    public class PaisConfig : EntityTypeConfiguration<Pais>
    {
        public PaisConfig()
        {
            this.ToTable("Pais");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Sigla).HasColumnName("Sigla");
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.LinguagemPadrao).HasColumnName("LinguagemPadrao");
            this.Property(t => t.StAtivo).HasColumnName("StAtivo");
            this.Property(t => t.Bandeira).HasColumnName("Bandeira");
        }
    }
}
