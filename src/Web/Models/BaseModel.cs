using Raizen.Framework.Web.MVC.Bases;

namespace Raizen.UniCad.Web.Models
{
    public class BaseModel : ModelMVC
    {
        #region Modal
        //public ModalConfirmSPF ModalConfirm { get; private set; }
        #endregion

        #region Constructor
        public BaseModel()
        {
//            ModalConfirm = new ModalConfirmSPF();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Nome da apicação
        /// </summary>
        public string NomeAplicacaoSSO { get; set; }
        
        /// <summary>
        /// Indica se o nome da aplicação deve ser exibido no cabeçalho do site
        /// </summary>
        public bool ExibirNomeAppSSO { get; set; }
        #endregion
    }
}
