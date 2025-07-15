#region Detalhes Gerador

// Assembly: Raizen.Gerador.UI
// Versão: 1.1.0.14463
// Data Geração: 15/03/2019 11:47:31

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;
  
namespace Raizen.UniCad.DAL.CodeFirst
{
    public class LogExecucaoJobConfig : EntityTypeConfiguration<LogExecucaoJob>
    {
        public LogExecucaoJobConfig()
        {
            this.ToTable("LogExecucaoJob","dbo");
            
            this.HasKey(chave => chave.Id); 

            
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			this.Property(t => t.Data).HasColumnName("Data");
			this.Property(t => t.Job).HasColumnName("Job");
			this.Property(t => t.Titulo).HasColumnName("Titulo");
			this.Property(t => t.Descricao).HasColumnName("Descricao");
			this.Property(t => t.Codigo).HasColumnName("Codigo");
			
        }
    }
}
