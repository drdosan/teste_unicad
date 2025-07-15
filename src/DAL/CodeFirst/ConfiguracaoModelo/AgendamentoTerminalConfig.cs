
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Data.Entity.ModelConfiguration;
  using System.ComponentModel.DataAnnotations.Schema;

  using Raizen.UniCad.Model;
  
  namespace Raizen.UniCad.DAL.CodeFirst
  {
  public class AgendamentoTerminalConfig : EntityTypeConfiguration<AgendamentoTerminal>
    {
        public AgendamentoTerminalConfig()
        {
            this.ToTable("AgendamentoTerminal");
            this.HasKey(chave => chave.ID); 
            this.Property(t => t.ID).HasColumnName("ID"); 
            this.Property(t => t.IDTerminal).HasColumnName("IDTerminal");
            this.Property(t => t.IDTipoAgenda).HasColumnName("IDTipoAgenda");
            this.Property(t => t.Ativo).HasColumnName("Ativo");
            this.Property(t => t.Data).HasColumnName("Data");
        }
    }
}
  
