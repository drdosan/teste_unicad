using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.UniCad.Model.Base;

namespace Raizen.UniCad.Model.View
{
    public class PlacaView
    {
        public virtual Int32 ID { get; set; }
        public virtual string PlacaVeiculo { get; set; }
        public virtual string Operacao { get; set; }
        public virtual string TipoVeiculo { get; set; }
        public virtual Int32 IDTipoVeiculo { get; set; }
        public virtual string Renavam { get; set; }
        public virtual string Marca { get; set; }
        public virtual string Modelo { get; set; }
        public virtual string Material { get; set; }
        public virtual string Potencia { get; set; }
        public virtual string Chassi { get; set; }
        public virtual string NrMotor { get; set; }
        public virtual string SatelitalMarca { get; set; }
        public virtual string SatelitalModelo { get; set; }
        public virtual string SatelitalNrInterno { get; set; }
        public virtual string SatelitalEmpresa { get; set; }
        public virtual Int32 AnoFabricacao { get; set; }
        public virtual Int32 AnoModelo { get; set; }
        public virtual string Cor { get; set; }
        public virtual string TipoRastreador { get; set; }
        public virtual string NumeroAntena { get; set; }
        public virtual string Versao { get; set; }
        public virtual bool CameraMonitoramento { get; set; }
        public virtual bool BombaDescarga { get; set; }
        public virtual double Tara { get; set; }
        public virtual Int32 NumeroEixos { get; set; }
        public virtual bool EixosPneusDuplos { get; set; }
        public virtual Nullable<Int32> NumeroEixosPneusDuplos { get; set; }
        public virtual bool EixosDistanciados { get; set; }
        public virtual Nullable<Int32> NumeroEixosDistanciados { get; set; }
        public virtual Int32? IDTipoProduto { get; set; }
        public virtual bool MultiSeta { get; set; }
        public virtual Int32? IDTipoCarregamento { get; set; }
        public virtual string CPFCNPJ { get; set; }
        public virtual Nullable<DateTime> DataNascimento { get; set; }
        public virtual string Datas { get; set; }
        public virtual string RazaoSocial { get; set; }
        public virtual string Vencimento { get; set; }
        public virtual string Cuit { get; set; }
        public virtual Int32 IDCategoriaVeiculo { get; set; }
        public virtual string Observacao { get; set; }
        public virtual bool Existe { get; set; }        
        public virtual decimal Volume { get; set; }
        public int? IDEstado { get; set; }
        public int? IdTransportadora { get; set; }
        public string Transportadora { get; set; }
        public string TipoProduto { get; set; }
        public string TipoCarregamento { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int IDStatus { get; set; }
        public int? Principal { get; set; }
        public bool PossuiAbs { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Status { get; set; }
        public string CategoriaVeiculo { get; set; }
        public string Transportadora2 { get; set; }
        public int? IdTransportadora2 { get; set; }
        public string Clientes { get; set; }
        public string VolumeCompartimento1 { get; set; }
        public string VolumeCompartimento2 { get; set; }
        public string VolumeCompartimento3 { get; set; }
        public string VolumeCompartimento4 { get; set; }
        public string VolumeCompartimento5 { get; set; }
        public string VolumeCompartimento6 { get; set; }
        public string VolumeCompartimento7 { get; set; }
        public string VolumeCompartimento8 { get; set; }
        public string VolumeCompartimento9 { get; set; }
        public string VolumeCompartimento10 { get; set; }
        public int IdPais { get; set; }
        public double? PBTC { get; set; }

        #region Constructors

        public PlacaView()
        {
            this.IdPais = (int)EnumPais.Brasil;
        }

        public PlacaView(int numeroEixos, int numeroEixosDistanciados, int numeroEixosPneusDuplos, EnumCategoriaVeiculo categoriaVeiculo, EnumPais pais)
        {
            this.NumeroEixos = numeroEixos;
            this.NumeroEixosDistanciados = numeroEixosDistanciados;
            this.NumeroEixosPneusDuplos = numeroEixosPneusDuplos;
            this.IDCategoriaVeiculo = (int)categoriaVeiculo;
            this.IdPais = (int)pais;
            this.PBTC = 0;
        }

        #endregion

    }
}
  

