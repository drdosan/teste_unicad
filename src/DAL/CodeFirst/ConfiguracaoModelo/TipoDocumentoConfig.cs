
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class TipoDocumentoConfig : EntityTypeConfiguration<TipoDocumento>
    {
        public TipoDocumentoConfig()
        {
            this.ToTable("TipoDocumento");

            this.HasKey(chave => chave.ID);


            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Sigla).HasColumnName("Sigla");
            this.Property(t => t.Descricao).HasColumnName("Descricao");
            this.Property(t => t.Operacao).HasColumnName("Operacao");
            this.Property(t => t.IDCategoriaVeiculo).HasColumnName("IDCategoriaVeiculo");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Obrigatorio).HasColumnName("Obrigatorio");
            this.Property(t => t.Observacao).HasColumnName("Observacao");
            this.Property(t => t.Alerta1).HasColumnName("Alerta1");
            this.Property(t => t.Alerta2).HasColumnName("Alerta2");
            this.Property(t => t.qtdeAlertas).HasColumnName("qtdeAlertas");
            this.Property(t => t.tipoCadastro).HasColumnName("tipoCadastro");
            this.Property(t => t.MesesValidade).HasColumnName("MesesValidade");
            this.Property(t => t.TipoAcaoVencimento).HasColumnName("TipoAcaoVencimento");
            this.Property(t => t.IDPais).HasColumnName("IDPais");
            this.Property(t => t.DocumentoPossuiVencimento).HasColumnName("DocumentoPossuiVencimento");
        }
    }
}

