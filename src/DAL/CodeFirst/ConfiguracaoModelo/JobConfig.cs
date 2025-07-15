
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class JobConfig : EntityTypeConfiguration<Job>
    {
        public JobConfig()
        {
            this.ToTable("Job");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.NrPeriodicidadeMinutos).HasColumnName("NrPeriodicidadeMinutos");
            this.Property(t => t.DtUltimaExecucao).HasColumnName("DtUltimaExecucao");
            this.Property(t => t.EmExecucao).HasColumnName("EmExecucao");
            this.Property(t => t.DtInicioJob).HasColumnName("DtInicioJob");
            this.Property(t => t.StAtivo).HasColumnName("StAtivo");
        }
    }
}

