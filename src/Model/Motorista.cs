using Raizen.UniCad.Model.Base;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model
{
    public class Motorista : MotoristaBase
    {
        [NotMapped]
        public bool jaExiste;
        [NotMapped]
        public int naoAprovado { get; set; }

        [NotMapped]
        public EnumTipoIntegracaoSAP tipoIntegracao { get; set; }

        [NotMapped]
        public List<MotoristaDocumentoView> Documentos { get; set; }
        [NotMapped]
        public List<MotoristaClienteView> Clientes { get; set; }
        [NotMapped]
        public string CPFEdicao { get; set; }
        [NotMapped]
        public string DNIEdicao { get; set; }
        [NotMapped]
        public string EmailSolicitante { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public virtual string IDCliente { get; set; }
        [NotMapped]
        public string IBMTransportadora { get; set; }
        [NotMapped]
        public DateTime? DataValidadeTreinamento { get; set; }
        [NotMapped]
        public string NomeTransportadora { get; set; }
        [NotMapped]
        public string OperacaoUsuario { get; set; }
        [NotMapped]
        public List<TerminalTreinamentoView> ListaTerminais { get; set; }
        [NotMapped]
        public TreinamentoView TreinamentoView { get; set; }
        [NotMapped]
        public List<HistoricoTreinamentoTeoricoMotorista> ListaTreinamento { get; set; }
        [NotMapped]
        public int MesesTreinamentoTeorico { get; set; }
        [NotMapped]
        public MotoristaBrasil MotoristaBrasil { get; set; }
        [NotMapped]
        public MotoristaArgentina MotoristaArgentina { get; set; }
    }
}
