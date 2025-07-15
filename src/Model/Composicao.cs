
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;

using Raizen.UniCad.Model.Base;
using Raizen.UniCad.Model.View;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model
  {
    public class Composicao : ComposicaoBase
    {
        [NotMapped]
        public EnumTipoIntegracaoSAP? tipoIntegracao;
        [NotMapped]
        public string chaveSAPEAB { get; set; }
        [NotMapped]
        public string chaveSAPCOMB { get; set; }

        [NotMapped]
        public ChecklistComposicao checkList { get; set; }
        [NotMapped]
        public bool jaExiste { get; set; }

        [NotMapped]
        public bool isPlaca1Alterada { get; set; }
        [NotMapped]
        public bool isPlaca2Alterada { get; set; }
        [NotMapped]
        public bool isPlaca3Alterada { get; set; }
        [NotMapped]
        public bool isPlaca4Alterada { get; set; }

        [NotMapped]
        public bool isPlaca1Pendente { get; set; }
        [NotMapped]
        public bool isPlaca2Pendente { get; set; }
        [NotMapped]
        public bool isPlaca3Pendente { get; set; }
        [NotMapped]
        public bool isPlaca4Pendente { get; set; }
        [NotMapped]
        public decimal VolumeComposicao { get; set; }
        [NotMapped]
        public int? ufCRLV { get; set; }
        [NotMapped]
        public Placa p1 { get; set; }
        [NotMapped]
        public Placa p2 { get; set; }
        [NotMapped]
        public Placa p3 { get; set; }
        [NotMapped]
        public Placa p4 { get; set; }
        [NotMapped]
        public bool IsDocumentosPendentes { get; set; }
        [NotMapped]
        public string IBMTransportadora { get; set; }
        [NotMapped]
        public string IBMTransportadora2 { get; set; }
        [NotMapped]
        public int PlacaOficial1 { get; set; }
        [NotMapped]
        public int PlacaOficial2 { get; set; }
        [NotMapped]
        public int PlacaOficial3 { get; set; }
        [NotMapped]
        public int PlacaOficial4 { get; set; }
        [NotMapped]

        public int Linha { get; set; }
        [NotMapped]
        public bool IsTaraAlterada { get; set; }
        [NotMapped]
        public bool IsNumEixosAlterados { get; set; }
        [NotMapped]
        public bool IsIBMTransportadoraAlterada { get; set; }
        [NotMapped]
        public bool IsOperacaoAlterada { get; set; }
        [NotMapped]
        public bool IsCodEasyQyeryAlterado { get; set; }
        [NotMapped]
        public bool IsUsuarioCsOnline { get; set; }
        [NotMapped]
        public bool IsPbtcAlterado { get; set; }
        public double? PBTC { get; set; }
        [NotMapped]
        public int QtdCompartimentos { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public string EmailSolicitante { get; set; }
        [NotMapped]
        public string CategoriaVeiculo { get; set; }
        [NotMapped]
        public virtual Int32 IDEmpresaUsuario { get; set; }
        [NotMapped]
        public bool? IgnorarLeci { get; set; }
        [NotMapped]
        public bool? IgnorarLeciAdm { get; set; }
        
        public int? TentativaIntegracao { get; set; }
        [NotMapped]
        public bool isArrendamento { get; set; }
        [NotMapped]
        public decimal Metros { get; set; }
        [NotMapped]

        public string OperacaoUsuario { get; set; }
        [NotMapped]
        public string LoginUsuarioCorrente { get; set; }
    }
}
  
