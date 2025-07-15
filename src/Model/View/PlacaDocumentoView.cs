using System;

namespace Raizen.UniCad.Model.View
{
    public class PlacaDocumentoView
    {
        public bool isAnexoAlterado { get; set; }
        public bool Pendente { get; set; }
        public virtual Int32 ID { get; set; }
        public virtual Int32 IDTipoDocumento { get; set; }
        public virtual Int32 IDPlaca { get; set; }
        public virtual string Operacao { get; set; }
        public virtual string Sigla { get; set; }
        public virtual string Placa { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Anexo { get; set; }
        public virtual int DiasVencimento { get; set; }
        public virtual int TipoAlerta { get; set; }
        public virtual string Email { get; set; }
        public virtual string IBM { get; set; }
        public virtual int IDUsuario { get; set; }
        public virtual string Documento { get; set; }
        public DateTime? DataVencimento { get; set; }
        public virtual bool Obrigatorio { get; set; }
		public virtual bool DocumentoPossuiVencimento { get; set; }
        public virtual bool Vencido { get; set; }
        public virtual bool isDataVencimentoAlterada { get; set; }
        public virtual bool Aprovar { get; set; }
        public int IDComposicao { get; set; }
        public bool Enviado { get; set; }
        public virtual string RazaoSocial { get; set; }
        public EnumTipoVeiculo TipoPlaca { get; set; }
        public int MesesValidade { get; set; }
        public EnumTipoAcaoVencimento TipoAcaoVencimento { get; set; }
        public EnumPais IdPais { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual int DiasVencimentoA2 { get; set; }
        public virtual bool Alerta1Enviado { get; set; }
        public virtual bool Alerta2Enviado { get; set; }
        public virtual string EmailCif { get; set; }
        public virtual string IbmTransportadora1 { get; set; }
        public virtual string RazaoSocialTransportadora1 { get; set; }
        public virtual string EmailEmpresaAmbos { get; set; }
        public virtual string IbmTransportadora2 { get; set; }
        public virtual string RazaoSocialTransportadora2 { get; set; }
        public virtual int? BloqueioImediato { get; set; }
        public virtual int? QuantidadeDiasBloqueio { get; set; }
        public virtual int IDUsuarioTransportadora1 { get; set; }
        public virtual int IDUsuarioTransportadora2 { get; set; }
        public EnumTipoBloqueioImediato TipoBloqueioImediato { get; set; }
        public virtual bool Processado { get; set; }

        public string EmailEnviar
        {
            get
            {
                return Operacao == "FOB" ? Email :
                       Operacao == "CIF" && IdEmpresa == (int)EnumEmpresa.Ambos ? EmailEmpresaAmbos :
                       EmailCif;
            }
        }

        public virtual int QtdeAlertas { get; set; }
    }   
}
