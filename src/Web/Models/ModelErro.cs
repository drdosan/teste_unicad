namespace Raizen.UniCad.Web.Models
{
    public class ModelErro : BaseModel
    {
        public string Titulo { get; set; }
        public string MensagemAmigavel { get; set; }
        public string DetalheTecnico { get; set; }
        public string DescricaoAcaoUsuario { get; set; }
        public AcaoUsuarioErro AcaoUsuario { get; set; }
    }

    public enum AcaoUsuarioErro
    {
        ContacteHelpDesk = 1,
        SoliciteAcesso = 2,
        TenteNovamente = 3
    }
}