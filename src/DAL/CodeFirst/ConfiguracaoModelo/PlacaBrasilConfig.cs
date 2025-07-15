using Raizen.UniCad.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo
{
    public class PlacaBrasilConfig : EntityTypeConfiguration<PlacaBrasil>
    {
        public PlacaBrasilConfig()
        {
            this.ToTable("PlacaBrasil");
            this.HasKey(chave => chave.IDPlaca);

            this.Property(t => t.IDPlaca).HasColumnName("IDPlaca").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.Renavam).HasColumnName("Renavam");
            this.Property(t => t.CPFCNPJ).HasColumnName("CPFCNPJ");
            this.Property(t => t.IDEstado).HasColumnName("IDEstado");
            this.Property(t => t.Cidade).HasColumnName("Cidade");
        }
    }
}
