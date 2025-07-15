using System;

namespace Raizen.UniCad.Model.View
{
    public class SincronizacaoMotoritasView
    {
        public int ID { get; set; }
        public string CPF { get; set; }
        public string Motorista { get; set; }
        public string Operacao { get; set; }
        public string Mensagem { get; set; }
        public bool IsOk { get; set; }
        public DateTime Data { get; set; }
        public string RG { get; set; }
        public string CNH { get; set; }
    }
}


