
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class HistoricoTreinamentoTeoricoMotoristaConfig : EntityTypeConfiguration<HistoricoTreinamentoTeoricoMotorista>
    {
        public HistoricoTreinamentoTeoricoMotoristaConfig()
        {
            this.ToTable("HistoricoTreinamentoTeoricoMotorista");

            this.HasKey(chave => chave.ID);


            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t => t.Justificativa).HasColumnName("Justificativa");
            this.Property(t => t.Anexo).HasColumnName("Anexo");
            this.Property(t => t.Data).HasColumnName("Data");
            this.Property(t => t.DataCadastro).HasColumnName("DataCadastro");
            this.Property(t => t.Usuario).HasColumnName("Usuario");
            this.Property(t => t.CodigoUsuario).HasColumnName("CodigoUsuario");

        }
    }
}

