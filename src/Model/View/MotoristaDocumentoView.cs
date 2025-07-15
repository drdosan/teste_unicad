using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.UniCad.Model.Base;
using Raizen.UniCad.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.View
{
    public class MotoristaDocumentoView
    {
        public bool naoAprovado { get; set; }

        public bool Pendente { get; set; }
        public virtual Int32 ID { get; set; }
        public virtual Int32 IDTipoDocumento { get; set; }
        public virtual Int32 IDMotorista { get; set; }
        public virtual string Operacao { get; set; }
        public virtual string Sigla { get; set; }
        public virtual string CPF { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Anexo { get; set; }
        public virtual int DiasVencimento { get; set; }
        public virtual int TipoAlerta { get; set; }
        public virtual string Email { get; set; }
        public virtual string IBM { get; set; }
        public virtual int IDUsuario { get; set; }
        public virtual string Documento { get; set; }
        public virtual string RazaoSocial { get; set; }
        public DateTime? DataVencimento { get; set; }
        public virtual bool Obrigatorio { get; set; }
        public virtual bool Vencido { get; set; }
        public virtual bool isDataVencimentoAlterada { get; set; }
        public virtual bool Processado { get; set; }
        public virtual string UsuarioAlterouStatus { get; set; }
        public EnumTipoAcaoVencimento TipoAcaoVencimento { get; set; }
        [NotMapped]
        public virtual bool Aprovar { get; set; }
        [NotMapped]
        public virtual bool VisualizarDoc { get; set; }
        public string Nome { get; set; }
        [NotMapped]
        public virtual bool Alerta1Enviado { get; set; }
        [NotMapped]
        public virtual bool Alerta2Enviado { get; set; }
        [NotMapped]
        public virtual bool Bloqueado { get; set; }
        [NotMapped]
        public int MesesValidade { get; set; }

        [NotMapped]
        public string TipoProduto { get; set; }

        public virtual bool DocumentoPossuiVencimento { get; set; }


        [NotMapped]
        public virtual bool DataVencimentoEditavel { get; set; }

        public EnumPais IdPais { get; set; }


        public virtual int DiasVencimentoA2 { get; set; }
        public virtual string IbmTransportadora { get; set; }
        public virtual string EmailTransportadora { get; set; }
        public virtual string RazaoSocialTransportadora { get; set; }
        public virtual int? BloqueioImediato { get; set; }
        public virtual int? QuantidadeDiasBloqueio { get; set; }
        public EnumTipoBloqueioImediato TipoBloqueioImediato { get; set; }
        public virtual string DNI { get; set; }

        public string EmailEnviar
        {
            get
            {
                return Operacao == "FOB" ? Email : EmailTransportadora;
            }

        }

        public string DocumentoIdentificacao
        {
            get
            {
                return IdPais == EnumPais.Argentina ? DNI : CPF;
            }
        }

        public virtual bool Enviado { get; set; }

        public virtual int QtdeAlertas { get; set; }
    }
}


