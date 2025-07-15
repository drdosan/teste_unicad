using Raizen.UniCad.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaArgentinaConfig : EntityTypeConfiguration<MotoristaArgentina>
    {
        public MotoristaArgentinaConfig()
        {
            this.ToTable("MotoristaArgentina");
            this.HasKey(chave => chave.IDMotorista);
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			this.Property(t => t.Apellido).HasColumnName("Apellido");
			this.Property(t => t.CUITTransportista).HasColumnName("CUITTransportista");
			this.Property(t => t.DNI).HasColumnName("DNI");
            this.Property(t => t.LicenciaNacionalConducir).HasColumnName("LicenciaNacionalConducir");
            this.Property(t => t.LicenciaNacionalHabilitante).HasColumnName("LicenciaNacionalHabilitante");
            this.Property(t => t.Tarjeta).HasColumnName("Tarjeta");
        }
    }
}
