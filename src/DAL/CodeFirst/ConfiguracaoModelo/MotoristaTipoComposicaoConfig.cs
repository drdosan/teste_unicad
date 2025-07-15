using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo
{
    public class MotoristaTipoComposicaoConfig : EntityTypeConfiguration<MotoristaTipoComposicao>
    {
        public MotoristaTipoComposicaoConfig()
        {
            this.ToTable("MotoristaTipoComposicao");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t => t.IDTipoComposicao).HasColumnName("IDTipoComposicao");
        }

    }
}
