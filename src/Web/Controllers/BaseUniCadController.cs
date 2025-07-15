using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Web.MVC.Bases;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Utils;
using Raizen.UserSystem.SAL.Model;

namespace Raizen.UniCad.Web.Controllers
{
    public class BaseUniCadController : BaseController
    {
        #region Construtores

        public BaseUniCadController()
        {

        }

        public BaseUniCadController(BaseControllerOptions options) : base(options)
        {

        }

        #endregion

        #region Constantes

        private const string clientNotfound = "Não é possível associar. Cliente não encontrado";
        private const string invalidNetwork = "Não é possível indicar este código, pois o mesmo não faz parte de sua rede cadastrada no CS Online";
        private const string clientNotfoundArg = "Incapaz de asociarse. Cliente no encontrado";
        private const string invalidNetworkArg = "No es posible indicar este código, ya que no forma parte de su red registrada CS Online";

        #endregion

        #region Métodos públicos

        public ActionResult ListarTerminais(string nome)
        {
            var searchTypeAheadEntity = new List<SearchTypeAheadEntityView>();

            if (!string.IsNullOrEmpty(nome) && nome.Length >= 3)
            {
                var lista = new TerminalBusiness().Listar(w => w.Nome.Contains(nome));
                if (lista != null)
                    foreach (var item in lista)
                    {
                        searchTypeAheadEntity.Add(new SearchTypeAheadEntityView { ID = item.ID, Name = item.Nome });
                    }

                if (lista == null || !lista.Any())
                {
                    searchTypeAheadEntity.Add(new SearchTypeAheadEntityView() { Name = "Ítem não encontrado" });
                }

            }
            return new JsonResult() { Data = searchTypeAheadEntity };

        }

        #region ListarClientes

        public ActionResult ListarClientes(string nome, string empresa, bool? isClienteAcs, int? idPais)
        {
            var pais = GetPaisEnum(idPais);
            var searchTypeAheadEntity = new List<SearchTypeAheadEntityView>();

            int? idEmpresa = null;
            if (!string.IsNullOrEmpty(empresa) && Convert.ToInt32(empresa) != (int)EnumEmpresa.Ambos)
                idEmpresa = Convert.ToInt32(empresa);

            if (!string.IsNullOrEmpty(nome) && nome.Length >= 3)
            {
                var idUsuario = 0;
                if (UsuarioCsOnline != null)
                    idUsuario = UsuarioCsOnline.ID;
                else
                {
                    var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
                    var user = new UsuarioBusiness().Selecionar(p => p.Login == login);
                    if (user != null && user.Externo && !user.Perfil.IsQuality())
                        idUsuario = user.ID;
                }
                var lista = new ClienteBusiness(pais).ListarClientes(new Model.Filtro.ClienteFiltro() { ID = idUsuario, Nome = nome, IDEmpresa = idEmpresa, Cnpjs = Session["cnpj"] as IList<string> });
                if (lista != null)
                    foreach (var item in lista)
                    {
                        searchTypeAheadEntity.Add(new SearchTypeAheadEntityView { ID = item.IDClienteTransportadora, Name = item.IBM + " - " + item.CPF_CNPJ + " - " + item.RazaoSocial });
                    }

                if (lista == null || !lista.Any())
                {
                    //R8) Verifcar se o autocomplete do cliente deve ou não levar em consideração a regra que diz que "Não é possível indicar este código, pois o mesmo não faz parte de sua rede cadastrada no CS Online"
                    if (isClienteAcs ?? false)
                        searchTypeAheadEntity.Add(new SearchTypeAheadEntityView() { Name = (pais == EnumPais.Brasil ? invalidNetwork : invalidNetworkArg) });
                    else
                        searchTypeAheadEntity.Add(new SearchTypeAheadEntityView() { Name = (pais == EnumPais.Brasil ? clientNotfound : clientNotfoundArg) });
                }
            }

            return new JsonResult() { Data = searchTypeAheadEntity };
        }

        #endregion
        public ActionResult ListarTransportadorasArrendamento(string nome, string empresa, string operacao, int? idPais)
        {
            var pais = GetPaisEnum(idPais);
            var searchTypeAheadEntity = new List<SearchTypeAheadEntityArredView>();
            nome = nome.RemoveCharacter();
            int? idEmpresa = null;
            if (!string.IsNullOrEmpty(empresa) && empresa != "3")
                idEmpresa = Convert.ToInt32(empresa);

            if (!string.IsNullOrEmpty(nome) && nome.Length >= 3)
            {
                var lista = new TransportadoraBusiness(pais).Listar(p => (p.CNPJCPF.Contains(nome)) && (!idEmpresa.HasValue || idEmpresa.Value == p.IDEmpresa) && (p.Operacao == operacao || string.IsNullOrEmpty(operacao)) && !p.Desativado);
                if (lista != null)
                    searchTypeAheadEntity.AddRange(lista.Select(item => new SearchTypeAheadEntityArredView { cnpj = item.CNPJCPF, Name = item.CNPJCPF + " - " + item.RazaoSocial, Valor = item.RazaoSocial }));

                if (lista == null || !lista.Any())
                {
                    searchTypeAheadEntity.Add(new SearchTypeAheadEntityArredView() { Name = "Não é possível associar. Transportadora não encontrada" });
                }
            }

            return new JsonResult() { Data = searchTypeAheadEntity };
        }

        public ActionResult ListarTransportadoras(string nome, string empresa, string operacao, int? idPais)
        {
            var pais = GetPaisEnum(idPais);
            var searchTypeAheadEntity = new List<SearchTypeAheadEntityView>();

            int? idEmpresa = null;
            if (!string.IsNullOrEmpty(empresa) && empresa != "3")
                idEmpresa = Convert.ToInt32(empresa);

            if (!string.IsNullOrEmpty(nome) && nome.Length >= 3)
            {
                var lista = new TransportadoraBusiness(pais).ListarTransportadoras(new Model.Filtro.TransportadoraFiltro() { Nome = nome, IDEmpresa = idEmpresa, Operacao = operacao });
                if (lista != null)
                    searchTypeAheadEntity.AddRange(lista.Select(item => new SearchTypeAheadEntityView
                    {
                        ID = item.ID,
                        Name = $"{item.IBM} - {item.RazaoSocial} - {item.CPF_CNPJ}"
                    }));

                if (lista == null || !lista.Any())
                {
                    searchTypeAheadEntity.Add(new SearchTypeAheadEntityView() { Name = "Não é possível associar. Transportadora não encontrada" });
                }
            }

            return new JsonResult() { Data = searchTypeAheadEntity };
        }

        #endregion

        #region Métodos privados

        private EnumPais GetPaisEnum(int? idPais)
        {
            switch (idPais)
            {
                case 1:
                    return EnumPais.Brasil;

                case 2:
                    return EnumPais.Argentina;

                default:
                    return EnumPais.Brasil;
            }
        }

        /// <summary>
        /// Devolve os bytes do arquivo em base64, ja concatenado com formatação pronta para exibição em componente "img" do HTML.
        /// Formato: data:image/gif;base64,{0}
        /// </summary>
        /// <param name="file">Nome do arquivo</param>
        /// <returns>String no formato data:image/gif;base64,{0}, onde no lugar do {0}, vai o byte array em base64.</returns>
        protected string ImageInByteArray(string file)
        {
            var uploadPath = Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao) + "/" + file;

#if DEBUG
            uploadPath = "C:\\Raizen\\" + file;
#endif
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(uploadPath);

                var base64 = Convert.ToBase64String(fileBytes);
                var imgSrc = String.Format("data:image/gif;base64,{0}", base64);

                return imgSrc;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        private string IbmUsuario => Session["ibmUsuario"]?.ToString();

        protected Usuario UsuarioCsOnline => (Usuario)Session["UsuarioUnicad"];

        protected Usuario UsuarioLogado
        {
            get
            {
                Usuario tipo = null;
                try
                {
                    if (UserSession.GetCurrentInfoUserSystem() != null)
                    {
                        var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
                        tipo = IbmUsuario != null ? new UsuarioBusiness().Selecionar(p => p.Login == IbmUsuario) : new UsuarioBusiness().Selecionar(p => p.Login == login);
                    }
                }
                catch (Exception ex)
                {
                    LogarExcecao(ex);
                }
                return tipo;
            }
        }

        public int? GetIdUsuarioTransportadora()
        {
            var login = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
            var user = new UsuarioBusiness().Selecionar(p => p.Login == login);
            int? idUsuario = null;
            if (user != null && user.Externo && !user.Perfil.IsQuality())
                idUsuario = user.ID;

            return idUsuario;
        }

        public string BuscarTransportadora(string cnpj, string frete, int idEmpresa, int? idPais)
        {
            string json = string.Empty;
            var pais = GetPaisEnum(idPais);

            Transportadora transp = new TransportadoraBusiness(pais).BuscarTranportadora(cnpj, frete, idEmpresa);

            if (transp != null)
            {
                json = transp.RazaoSocial;
            }

            return json;
        }
    }
}