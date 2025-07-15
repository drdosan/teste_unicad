using Raizen.UniCad.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaConfig : EntityTypeConfiguration<Motorista>
    {
        public MotoristaConfig()
        {
            this.ToTable("Motorista");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDEmpresa).HasColumnName("IDEmpresa");
            this.Property(t => t.IDTransportadora).HasColumnName("IDTransportadora");
            this.Property(t => t.IDStatus).HasColumnName("IDStatus");
            this.Property(t => t.Operacao).HasColumnName("Operacao");
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.DataAtualizazao).HasColumnName("DataAtualizazao");
            this.Property(t => t.Telefone).HasColumnName("Telefone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.CodigoEasyQuery).HasColumnName("CodigoEasyQuery");
            this.Property(t => t.CodigoSalesForce).HasColumnName("CodigoSalesForce");
            this.Property(t => t.Ativo).HasColumnName("Ativo");
            this.Property(t => t.Observacao).HasColumnName("Observacao");
            this.Property(t => t.PIS).HasColumnName("PIS");
            this.Property(t => t.UsuarioAlterouStatus).HasColumnName("UsuarioAlterouStatus");  
            this.Property(t => t.IdPais).HasColumnName("IdPais");
		}
	}
}
