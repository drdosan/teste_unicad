using Raizen.UniCad.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo
{
    public class PlacaArgentinaConfig : EntityTypeConfiguration<PlacaArgentina>
    {
        public PlacaArgentinaConfig()
        {
            this.ToTable("PlacaArgentina");
            this.HasKey(chave => chave.IDPlaca);
            
            this.Property(t => t.IDPlaca).HasColumnName("IDPlaca").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.Vencimento).HasColumnName("Vencimento");
            this.Property(t => t.CUIT).HasColumnName("CUIT");
            this.Property(t => t.PBTC).HasColumnName("PBTC");
            this.Property(t => t.NrMotor).HasColumnName("NrMotor");
            this.Property(t => t.SatelitalMarca).HasColumnName("SatelitalMarca");
            this.Property(t => t.SatelitalModelo).HasColumnName("SatelitalModelo");
            this.Property(t => t.SatelitalNrInterno).HasColumnName("SatelitalNrInterno");
            this.Property(t => t.SatelitalEmpresa).HasColumnName("SatelitalEmpresa");
            this.Property(t => t.Material).HasColumnName("Material");
            this.Property(t => t.Potencia).HasColumnName("Potencia");

        }
    }
}
