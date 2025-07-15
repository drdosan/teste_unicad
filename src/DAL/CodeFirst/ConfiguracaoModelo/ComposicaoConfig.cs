using Raizen.UniCad.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class ComposicaoConfig : EntityTypeConfiguration<Composicao>
    {
        public ComposicaoConfig()
        {
            this.ToTable("Composicao");

            this.HasKey(chave => chave.ID);

            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDEmpresa).HasColumnName("IDEmpresa");
            this.Property(t => t.Operacao).HasColumnName("Operacao").HasMaxLength(3);
            this.Property(t => t.IDTipoComposicao).HasColumnName("IDTipoComposicao");
            this.Property(t => t.IDTipoComposicaoEixo).HasColumnName("IDTipoComposicaoEixo");
            this.Property(t => t.IDCategoriaVeiculo).HasColumnName("IDCategoriaVeiculo");
            this.Property(t => t.IDPlaca1).HasColumnName("IDPlaca1");
            this.Property(t => t.IDPlaca2).HasColumnName("IDPlaca2");
            this.Property(t => t.IDPlaca3).HasColumnName("IDPlaca3");
            this.Property(t => t.IDPlaca4).HasColumnName("IDPlaca4");
            this.Property(t => t.CPFCNPJ).HasColumnName("CPFCNPJ");
            this.Property(t => t.CPFCNPJArrendamento).HasColumnName("CPFCNPJArrendamento");
            this.Property(t => t.IDStatus).HasColumnName("IDStatus");
            this.Property(t => t.PBTC).HasColumnName("PBTC");
            this.Property(t => t.DataNascimento).HasColumnName("DataNascimento");
            this.Property(t => t.RazaoSocial).HasColumnName("RazaoSocial");
            this.Property(t => t.RazaoSocialArrendamento).HasColumnName("RazaoSocialArrendamento");
            this.Property(t => t.Justificativa).HasColumnName("Justificativa");
            this.Property(t => t.DataAtualizacao).HasColumnName("DataAtualizacao");
            this.Property(t => t.Observacao).HasColumnName("Observacao");
            this.Property(t => t.LoginUsuario).HasColumnName("LoginUsuario");
            this.Property(t => t.isUtilizaPlacaChave).HasColumnName("isUtilizaPlacaChave");
            this.Property(t => t.UsuarioAlterouStatus).HasColumnName("UsuarioAlterouStatus");
            this.Property(t => t.IdPais).HasColumnName("IdPais");
        }
    }
}

