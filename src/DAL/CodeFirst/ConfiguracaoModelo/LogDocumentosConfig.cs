
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class LogDocumentosConfig : EntityTypeConfiguration<LogDocumentos>
    {
        public LogDocumentosConfig()
        {
            this.ToTable("LogDocumentos");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID");
        }
    }
}

