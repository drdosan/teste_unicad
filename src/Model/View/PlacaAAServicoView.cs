using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Raizen.UniCad.Model.View
{
    public class PlacaAAServicoView
    {
        [IgnoreDataMember]
        public int ID { get; set; }
        public virtual string Placa { get; set; }
        public virtual int LinhaNegocio { get; set; }
        public virtual string Operacao { get; set; }
        public virtual int Tipo {get;set;}
        public double Tara { get; set; }
        public int NumEixos { get; set; }
        public int? TipoProduto { get; set; }
        public bool EixosPneusDuplos { get; set; }
        public int? NumEixosPneusDuplos { get; set; }
        public bool EixosDistanciados { get; set; }
        public int? NumEixosPneusDistanciados { get; set; }
        public  int? Carregamento {get;set; }        
        public bool MultiSeta { get; set; }
        public string Renavam { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Chassi { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Cor { get; set; }
        public string TipoRastreador { get; set; }
        public string NumAntena { get; set; }
        public string Versao { get; set; }
        public bool CameraMonitoramento { get; set; }
        public bool BombaDescarga { get; set; }
        public bool PossuiAbs { get; set; }
        public string CnpjCpfTransportadorCrlv { get; set; }
        public string RazaoSocialNomeTransportadoraCrlv { get; set; }
        public DateTime? DataNascimentoTransportador { get; set; }
        public int Categoria { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Observacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public List<PlacaDocumentoAAServicoView> ListaDocumentos { get; set; }
        public List<PlacaPermissaoServicoView> ListaPermissoes { get; set; }
        public List<PlacaCompartimentoServicoView> ListaCompartimentos { get; set; }
    }
}


