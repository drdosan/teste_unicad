
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Data.Entity.ModelConfiguration;
  using System.ComponentModel.DataAnnotations.Schema;

  using Raizen.UniCad.Model;
  
  namespace Raizen.UniCad.DAL.CodeFirst
  {
  public class TipoCarregamentoConfig : EntityTypeConfiguration<TipoCarregamento>
    {
        public TipoCarregamentoConfig()
        {
            this.ToTable("TipoCarregamento");
            
            this.HasKey(chave => chave.ID); 

            
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.NomeEs).HasColumnName("NomeEs");
        }
    }
}
  
