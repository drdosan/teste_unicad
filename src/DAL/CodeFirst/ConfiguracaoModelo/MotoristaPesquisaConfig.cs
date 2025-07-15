using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class MotoristaPesquisaConfig : EntityTypeConfiguration<MotoristaPesquisa>
    {
        public MotoristaPesquisaConfig()
        {
            this.ToTable("VW_Motorista");
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
            this.Property(t => t.Ativo).HasColumnName("Ativo");
            this.Property(t => t.Observacao).HasColumnName("Observacao");
            this.Property(t => t.PIS).HasColumnName("PIS");
            this.Property(t => t.UsuarioAlterouStatus).HasColumnName("UsuarioAlterouStatus");
            this.Property(t => t.IdPais).HasColumnName("IdPais");

            this.Property(t => t.CPF).HasColumnName("CPF");
            this.Property(t => t.RG).HasColumnName("RG");
            this.Property(t => t.OrgaoEmissor).HasColumnName("OrgaoEmissor");
            this.Property(t => t.Nascimento).HasColumnName("Nascimento");
            this.Property(t => t.LocalNascimento).HasColumnName("LocalNascimento");
            this.Property(t => t.CNH).HasColumnName("CNH");
            this.Property(t => t.CategoriaCNH).HasColumnName("CategoriaCNH");
            this.Property(t => t.OrgaoEmissorCNH).HasColumnName("OrgaoEmissorCNH");

            this.Property(t => t.Apellido).HasColumnName("Apellido");
            this.Property(t => t.CUITTransportista).HasColumnName("CUITTransportista");
            this.Property(t => t.DNI).HasColumnName("DNI");
            this.Property(t => t.LicenciaNacionalConducir).HasColumnName("LicenciaNacionalConducir");
            this.Property(t => t.LicenciaNacionalHabilitante).HasColumnName("LicenciaNacionalHabilitante");

        }
    }
}
