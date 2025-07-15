namespace Raizen.UniCad.Model.Filtro
{
    public class TransportadoraFiltro
    {

        public TransportadoraFiltro()
        {
            Desativado = false;
        }

        public int ID { get; set; }

        public string Nome { get; set; }

        public int? IDEmpresa { get; set; }

        public string Operacao { get; set; }

        public bool Desativado { get; set; }

    }
}
