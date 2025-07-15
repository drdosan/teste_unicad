
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Data.Entity.ModelConfiguration;
  using System.ComponentModel.DataAnnotations.Schema;

  using Raizen.UniCad.Model;
  
  namespace Raizen.UniCad.DAL.CodeFirst
  {
  public class TipoVeiculoConfig : EntityTypeConfiguration<TipoVeiculo>
    {
        public TipoVeiculoConfig()
        {
            this.ToTable("TipoVeiculo");
            this.HasKey(chave => chave.ID); 

            
            this.Property(t => t.ID).HasColumnName("ID"); 
            this.Property(t => t.Nome).HasColumnName("Nome");
            this.Property(t => t.Pais).HasColumnName("IdPais");
        }
    }
}
  
