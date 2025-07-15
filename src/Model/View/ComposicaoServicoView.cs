using System;
using System.Runtime.Serialization;

namespace Raizen.UniCad.Model.View
{
    public class ComposicaoServicoView
    {
        [IgnoreDataMember]
        public int? IDPlaca1 {get;set; }
        [IgnoreDataMember]
        public int? IDPlaca2 {get;set; }
        [IgnoreDataMember]
        public int? IDPlaca3 {get;set; }
        [IgnoreDataMember]
        public int? IDPlaca4 {get;set; }
        public PlacaServicoView PlacaCavaloTruck { get; set; }
        public PlacaServicoView PlacaCarreta1 { get; set; }
        public PlacaServicoView PlacaDollyCarreta2 { get; set; }
        public PlacaServicoView PlacaCarreta2 { get; set; }
        public int LinhaNegocio {get;set; }
        public string Operacao { get; set; }
        public int TipoComposicao {get;set; }
        public string PlacaVeiculoCavaloTruck { get; set; }
        public string PlacaVeiculoCarreta1 { get; set; }
        public string PlacaVeiculoDollyCarreta2 { get; set; }
        public string PlacaVeiculoCarreta2 { get; set; }
        public bool Arrendamento { get; set; }
        public string CpfCnpjTransportadorArrendamento { get; set; }
        public string NomeRazaoSocialArrendamento { get; set; }
        public double TaraComposicao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public int Status {get;set; }
        public double Pbtc { get; set; }
    }
}


