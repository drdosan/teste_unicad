
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Data.Entity.ModelConfiguration;
  using System.ComponentModel.DataAnnotations.Schema;

  using Raizen.UniCad.Model;
  
  namespace Raizen.UniCad.DAL.CodeFirst
  {
  public class AgendamentoTerminalHorarioConfig : EntityTypeConfiguration<AgendamentoTerminalHorario>
    {
        public AgendamentoTerminalHorarioConfig()
        {
            this.ToTable("AgendamentoTerminalHorario");
            this.HasKey(chave => chave.ID); 
            this.Property(t => t.ID).HasColumnName("ID"); 
            this.Property(t => t.IDAgendamentoTerminal).HasColumnName("IDAgendamentoTerminal");
            this.Property(t => t.IDEmpresa).HasColumnName("IDEmpresa");
            this.Property(t => t.HoraInicio).HasColumnName("HoraInicio");
            this.Property(t => t.HoraFim).HasColumnName("HoraFim");
            this.Property(t => t.Operacao).HasColumnName("Operacao");
            this.Property(t => t.Vagas).HasColumnName("Vagas");            
          }
    }
}
  
