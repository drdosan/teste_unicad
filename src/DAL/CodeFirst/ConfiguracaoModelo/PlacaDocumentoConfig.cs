
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class PlacaDocumentoConfig : EntityTypeConfiguration<PlacaDocumento>
    {
        public PlacaDocumentoConfig()
        {
            this.ToTable("PlacaDocumento");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDTipoDocumento).HasColumnName("IDTipoDocumento");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.DataVencimento).HasColumnName("DataVencimento");
            this.Property(t => t.Observacao).HasColumnName("Observacao");
            this.Property(t => t.Vencido).HasColumnName("Bloqueado");
            this.Property(t => t.Alerta1Enviado).HasColumnName("Alerta1Enviado");
            this.Property(t => t.Alerta2Enviado).HasColumnName("Alerta2Enviado");
            this.Property(t => t.Vencido).HasColumnName("Vencido");
            this.Property(t => t.Bloqueado).HasColumnName("Bloqueado");
            this.Property(t => t.TipoAcaoVencimento).HasColumnName("TipoAcaoVencimento");
            this.Property(t => t.UsuarioAlterouStatus).HasColumnName("UsuarioAlterouStatus");
            this.Property(t => t.Processado).HasColumnName("Processado");
        }
    }
}

