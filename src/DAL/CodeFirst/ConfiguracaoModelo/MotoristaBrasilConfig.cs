using Raizen.UniCad.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaBrasilConfig : EntityTypeConfiguration<MotoristaBrasil>
    {
        public MotoristaBrasilConfig()
        {
            this.ToTable("MotoristaBrasil");
            this.HasKey(chave => chave.IDMotorista);
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CPF).HasColumnName("CPF");
            this.Property(t => t.RG).HasColumnName("RG");
            this.Property(t => t.OrgaoEmissor).HasColumnName("OrgaoEmissor");
            this.Property(t => t.Nascimento).HasColumnName("Nascimento");
            this.Property(t => t.LocalNascimento).HasColumnName("LocalNascimento");
            this.Property(t => t.CNH).HasColumnName("CNH");
            this.Property(t => t.CategoriaCNH).HasColumnName("CategoriaCNH");
            this.Property(t => t.OrgaoEmissorCNH).HasColumnName("OrgaoEmissorCNH");
		}
	}
}
