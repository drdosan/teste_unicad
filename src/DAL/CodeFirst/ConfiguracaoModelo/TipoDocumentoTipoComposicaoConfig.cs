using Raizen.UniCad.Model;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo
{
    public class TipoDocumentoTipoComposicaoConfig : EntityTypeConfiguration<TipoDocumentoTipoComposicao>
    {
        public TipoDocumentoTipoComposicaoConfig()
        {
            this.ToTable("TipoDocumentoTipoComposicao");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.IDTipoComposicao).HasColumnName("IDTipoComposicao");
            this.Property(t => t.IDTipoDocumento).HasColumnName("IDTipoDocumento");
            this.Property(t => t.Placa1).HasColumnName("Placa1");
            this.Property(t => t.Placa2).HasColumnName("Placa2");
            this.Property(t => t.Placa3).HasColumnName("Placa3");
            this.Property(t => t.Placa4).HasColumnName("Placa4");

            this.HasRequired(t => t.TipoComposicao).WithMany(t => t.TipoDocumentoTipoComposicao).HasForeignKey(t => t.IDTipoComposicao);
            this.HasRequired(t => t.TipoDocumento).WithMany(t => t.TipoDocumentoTipoComposicao).HasForeignKey(t => t.IDTipoDocumento);
        }
    }
}