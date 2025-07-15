
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaDocumentoConfig : EntityTypeConfiguration<MotoristaDocumento>
    {
        public MotoristaDocumentoConfig()
        {
            this.ToTable("MotoristaDocumento");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.TipoAcaoVencimento).HasColumnName("TipoAcaoVencimento");
            this.Property(t => t.UsuarioAlterouStatus).HasColumnName("UsuarioAlterouStatus");
            this.Property(t => t.Processado).HasColumnName("Processado");
        }
    }
}

