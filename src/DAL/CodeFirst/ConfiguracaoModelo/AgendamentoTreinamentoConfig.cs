
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class AgendamentoTreinamentoConfig : EntityTypeConfiguration<AgendamentoTreinamento>
    {
        public AgendamentoTreinamentoConfig()
        {
            this.ToTable("AgendamentoTreinamento");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t=> t.IDAgendamentoTerminalHorario).HasColumnName("IDAgendamentoTerminalHorario");
            this.Property(t=> t.IDMotorista).HasColumnName("IDMotorista");
            this.Property(t=> t.IDSituacao).HasColumnName("IDSituacao");
            this.Property(t=> t.Data).HasColumnName("Data");
            this.Property(t=> t.Usuario).HasColumnName("Usuario");
            this.Property(t=> t.CPFCongenere).HasColumnName("CPFCongenere");
            this.Property(t=> t.NomeMotorista).HasColumnName("NomeMotorista");
            this.Property(t=> t.IDEmpresaCongenere).HasColumnName("IDEmpresaCongenere");
        }
    }
}

