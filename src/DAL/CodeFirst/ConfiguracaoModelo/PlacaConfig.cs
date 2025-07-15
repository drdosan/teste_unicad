using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class PlacaConfig : EntityTypeConfiguration<Placa>
    {
        public PlacaConfig()
        {
            this.ToTable("Placa");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.PlacaVeiculo).HasColumnName("PlacaVeiculo");
            this.Property(t => t.Operacao).HasColumnName("Operacao");
            this.Property(t => t.IDTipoVeiculo).HasColumnName("IDTipoVeiculo");
            this.Property(t => t.Marca).HasColumnName("Marca");
            this.Property(t => t.Modelo).HasColumnName("Modelo");
            this.Property(t => t.Chassi).HasColumnName("Chassi");
            this.Property(t => t.AnoFabricacao).HasColumnName("AnoFabricacao");
            this.Property(t => t.AnoModelo).HasColumnName("AnoModelo");
            this.Property(t => t.Cor).HasColumnName("Cor");
            this.Property(t => t.TipoRastreador).HasColumnName("TipoRastreador");
            this.Property(t => t.NumeroAntena).HasColumnName("NumeroAntena");
            this.Property(t => t.Versao).HasColumnName("Versao");
            this.Property(t => t.CameraMonitoramento).HasColumnName("CameraMonitoramento");
            this.Property(t => t.BombaDescarga).HasColumnName("BombaDescarga");
            this.Property(t => t.Tara).HasColumnName("Tara");
            this.Property(t => t.NumeroEixos).HasColumnName("NumeroEixos");
            this.Property(t => t.EixosPneusDuplos).HasColumnName("EixosPneusDuplos");
            this.Property(t => t.NumeroEixosPneusDuplos).HasColumnName("NumeroEixosPneusDuplos");
            this.Property(t => t.EixosDistanciados).HasColumnName("EixosDistanciados");
            this.Property(t => t.NumeroEixosDistanciados).HasColumnName("NumeroEixosDistanciados");
            this.Property(t => t.IDTipoProduto).HasColumnName("IDTipoProduto");
            this.Property(t => t.MultiSeta).HasColumnName("MultiSeta");
            this.Property(t => t.IDTipoCarregamento).HasColumnName("IDTipoCarregamento");
            this.Property(t => t.DataNascimento).HasColumnName("DataNascimento");
            this.Property(t => t.RazaoSocial).HasColumnName("RazaoSocial");
            this.Property(t => t.IDCategoriaVeiculo).HasColumnName("IDCategoriaVeiculo");
            this.Property(t => t.IDTransportadora).HasColumnName("IDTransportadora");
            this.Property(t => t.IDTransportadora2).HasColumnName("IDTransportadora2");
            this.Property(t => t.IDPais).HasColumnName("IdPais");
        }
    }
}

