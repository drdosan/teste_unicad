
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Data.Entity.ModelConfiguration;
  using System.ComponentModel.DataAnnotations.Schema;

  using Raizen.UniCad.Model;
  
  namespace Raizen.UniCad.DAL.CodeFirst
  {
  public class TipoDocumentoTipoVeiculoConfig : EntityTypeConfiguration<TipoDocumentoTipoVeiculo>
    {
        public TipoDocumentoTipoVeiculoConfig()
        {
            this.ToTable("TipoDocumentoTipoVeiculo");
            
            this.HasKey(chave => chave.ID); 

            
            this.Property(t => t.ID).HasColumnName("ID"); 
this.Property(t => t.IDTipoDocumento).HasColumnName("IDTipoDocumento"); 
this.Property(t => t.IDTipoVeiculo).HasColumnName("IDTipoVeiculo"); 

          }
    }
}
  
