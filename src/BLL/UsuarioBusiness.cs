using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.UserSystem.Proxy;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UserSystem.SAL.Model;
using Raizen.UserSystem.SAL.Model.Filtro;

namespace Raizen.UniCad.BLL
{
    public class UsuarioBusiness : UniCadBusinessBase<Usuario>
    {
        public List<Usuario> ListarUsuario(UsuarioFiltro filtro, PaginadorModel paginador)
        {

            using (UniCadDalRepositorio<Usuario> repositorio = new UniCadDalRepositorio<Usuario>())
            {
                IQueryable<Usuario> query = GetQueryUsuario(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.Nome)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }

        }

        public int ListarUsuarioCount(UsuarioFiltro filtro)
        {

            using (UniCadDalRepositorio<Usuario> repositorio = new UniCadDalRepositorio<Usuario>())
            {
                IQueryable<Usuario> query = GetQueryUsuario(filtro, repositorio);
                return query.Count();
            }

        }

        private IQueryable<Usuario> GetQueryUsuario(UsuarioFiltro filtro, IUniCadDalRepositorio<Usuario> repositorio)
        {
            IQueryable<Usuario> query = (from app in repositorio.ListComplex<Usuario>().AsNoTracking().OrderBy(i => i.Nome)
                                         where (app.Nome.Contains(string.IsNullOrEmpty(filtro.Nome) ? app.Nome : filtro.Nome))
                                         && (app.Login.Contains(string.IsNullOrEmpty(filtro.Login) ? app.Login : filtro.Login))
                                         && (app.Perfil == (string.IsNullOrEmpty(filtro.Perfil) ? app.Perfil : filtro.Perfil))
                                         && (app.Status == filtro.Status || !filtro.Status.HasValue)
										 && (app.IDPais == filtro.IDPais || !filtro.IDPais.HasValue)
                                         select app);
            return query;
        }

        public string AdicionarUsuario(Usuario usuario, bool integrarUserSystem = true)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Usuario> UsuarioRepositorio = new UniCadDalRepositorio<Usuario>())
                {
                    if (usuario.Externo && integrarUserSystem)
                        usuario.Login = usuario.Email;

                    UsuarioRepositorio.Add(usuario);

                    var clienteBusiness = new UsuarioClienteBusiness();

                    if (usuario?.Clientes != null)
                        foreach (var cliente in usuario.Clientes)
                        {
                            cliente.IDUsuario = usuario.ID;
                            clienteBusiness.Adicionar(new UsuarioCliente { IDCliente = cliente.IDCliente, IDUsuario = usuario.ID });
                        }

                    var transportadoraBusiness = new UsuarioTransportadoraBusiness();

                    if (usuario?.Transportadoras != null)
                        foreach (var Transportadora in usuario.Transportadoras)
                        {
                            Transportadora.IDUsuario = usuario.ID;
                            transportadoraBusiness.Adicionar(new UsuarioTransportadora { IDTransportadora = Transportadora.IDTransportadora, IDUsuario = usuario.ID });
                        }

                    string mensagem = null;
                    if (integrarUserSystem)
                        mensagem = IntegrarUserSystem(usuario, null);

                    if (string.IsNullOrEmpty(mensagem))
                    {
                        transactionScope.Complete();
                        return null;
                    }

                    new RaizenException(mensagem).LogarErro();
                    return mensagem;
                }
            }
        }

        public string AtualizarUsuario(Usuario usuario, bool integrarUserSystem = true)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                //R3) Verificar se deverá rodar a rotina de integrar com UserSystem na atualização do usuário
                if (usuario.Perfil == EnumPerfil.CLIENTE_ACS || usuario.Perfil == EnumPerfil.CLIENTE_ACS_ARGENTINA)
                    integrarUserSystem = false;
                if (usuario.Externo && integrarUserSystem)
                    usuario.Login = usuario.Email;

                var perfilOld = Selecionar(usuario.ID).Perfil;

                Atualizar(usuario);

                var usuarioClienteBusiness = new UsuarioClienteBusiness();
                var usuarioClientes = usuarioClienteBusiness.Listar(p => p.IDUsuario == usuario.ID);

                foreach (var item in usuarioClientes)
                {
                    if (usuario.Clientes == null || !usuario.Clientes.Any(p => p.ID == item.ID))
                    {
                        usuarioClienteBusiness.Excluir(item.ID);
                    }
                }

                if (usuario.Clientes != null)
                    foreach (var item in usuario.Clientes)
                    {
                        item.IDUsuario = usuario.ID;

                        if (item.ID == 0)
                        {
                            usuarioClienteBusiness.Adicionar(new UsuarioCliente { IDCliente = item.IDCliente, IDUsuario = usuario.ID });
                        }
                        else
                        {
                            usuarioClienteBusiness.Atualizar(new UsuarioCliente { IDCliente = item.IDCliente, IDUsuario = usuario.ID, ID = item.ID });
                        }
                    }

                var usuarioTransportadoraBusiness = new UsuarioTransportadoraBusiness();
                var usuarioTransportadoras = usuarioTransportadoraBusiness.Listar(p => p.IDUsuario == usuario.ID);

                foreach (var item in usuarioTransportadoras)
                {
                    if (usuario.Transportadoras == null || !usuario.Transportadoras.Any(p => p.ID == item.ID))
                    {

                        usuarioTransportadoraBusiness.Excluir(item.ID);
                    }
                }

                if (usuario.Transportadoras != null)
                    foreach (var item in usuario.Transportadoras)
                    {
                        item.IDUsuario = usuario.ID;

                        if (item.ID == 0)
                        {
                            usuarioTransportadoraBusiness.Adicionar(new UsuarioTransportadora { IDTransportadora = item.IDTransportadora, IDUsuario = usuario.ID });
                        }
                        else
                        {
                            usuarioTransportadoraBusiness.Atualizar(new UsuarioTransportadora { IDTransportadora = item.IDTransportadora, IDUsuario = usuario.ID, ID = item.ID });
                        }
                    }

                string mensagem = null;
                if (integrarUserSystem)
                    mensagem = IntegrarUserSystem(usuario, perfilOld);

                if (string.IsNullOrEmpty(mensagem))
                {
                    transactionScope.Complete();
                    return null;
                }

                new RaizenException(mensagem).LogarErro();
                return mensagem;
            }
        }

        public string ResetarSenha(int id)
        {
            try
            {
                var usuario = Selecionar(id);

                var proxy = new ClientService(ConfigurationManager.AppSettings["ControleUsuarioUnicadServico"], ConfigurationManager.AppSettings["SenhaUsuarioUnicadServico"], ConfigurationManager.AppSettings["SIGLA_APP"]);


                var filtro = new InfoUsuarioFiltro();
                filtro.Campo = CampoBuscaUserSystem.Usuario;
                filtro.Valor = usuario.Login;
                filtro.Operacao = OperacaoBuscaUserSystem.Igual;
                var users = proxy.BuscarUsuariosUserSystem(filtro);
                if (users != null && users.Data != null && users.Data.Any())
                {
                    var user = users.Data.First();

					//var response = proxy.EsqueciMinhaSenha(user.Login);
					var response = proxy.EsqueciMinhaSenha(new UsuarioAppFiltro { Idioma = usuario.IDPais == EnumPais.Brasil ? "PT-BR" : "ES-AR", Login = user.Login, SiglaApp = "UNICA" });

					if (response != null)
                        return response.Message;
                }
                return "Usuário não encontrado!";
            }
            catch (Exception ex)
            {
                new RaizenException().LogarErro();
                return "Problemas ao resetar a senha! - " + ex.Message;
            }
        }

		private string IntegrarUserSystem(Usuario usuario, string perfilOld)
        {
            //trocar credenciais por usuário de serviço
            var proxy = new ClientService(ConfigurationManager.AppSettings["ControleUsuarioUnicadServico"], ConfigurationManager.AppSettings["SenhaUsuarioUnicadServico"], ConfigurationManager.AppSettings["SIGLA_APP"]);

            var filtro = new InfoUsuarioFiltro();

            filtro.Campo = CampoBuscaUserSystem.Usuario;
            filtro.Valor = usuario.Login;
            filtro.Operacao = OperacaoBuscaUserSystem.Igual;

            var app = UserSession.GetCurrentInfoUserSystem().InformacoesApp;

            var dominio = proxy.ListarDominiosADAtivos();
            bool comunicarUsuario = true;
            if (app != null)
            {
                var perfis = app.Perfis;

                if (perfis != null)
                {
                    var perfil = perfis.FirstOrDefault(p => p.Nome == usuario.Perfil);
                    if (perfil != null)
                    {
                        filtro.MaxResultados = 1;

                        var users = proxy.BuscarUsuariosUserSystem(filtro);
                        var userAd = proxy.ConsultarInformacaoUsuarioAD(filtro.Valor);

                        var info = proxy.ConsultarInformacaoUsuarioUserSystem(filtro.Valor);

                        if ((users == null || users.Data == null) && (info == null || info.Data == null) && (usuario.Externo || (userAd != null && userAd.Data != null)))
                        {
                            comunicarUsuario = false;
                            var senha = SenhaRandomica();
                            var response = proxy.CadastrarUsuario(new InfoUsuario
                            {
                                Email = usuario.Email,
                                ApagarSenha = true,
                                DataExpiracao = DateTime.Now.AddDays(60),
                                Login = usuario.Login,
                                Nome = usuario.Nome,
                                Senha = senha,
                                Status = (usuario.Status ? "A" : "I"),
                                DominioAD = dominio.Data.First(p => p.Name == "Cosan"),
                                Tipo = (byte)(usuario.Externo ? 0 : 1)
                            });
                            if (response != null && response.Data != null)
                            {
                                InfoUsuario user = response.Data;
                                user.Senha = senha;
                                ComunicarUsuarioNovaSenha(user, usuario.IDPais);
                            }
                            else
                            {
                                if (response != null)
                                {
                                    return response.Message;
                                }

                                return "Problemas ao obter dados do UserSystem";
                            }
                        }
                        else
                        {
                            if (users?.Data != null && users.Data.Any())
                            {
                                var user = users.Data.First();
                                user.Status = (usuario.Status ? "A" : "I");
                                user.Nome = usuario.Nome;
                                proxy.AlterarUsuario(user);
                            }
                        }

                        var associado = proxy.ListarPerfilPorUsuario(new PerfilAppUsuarioFiltro { Login = usuario.Login, Sigla = "UNICA" });
                        if (perfilOld != usuario.Perfil && associado != null && associado.Data != null)
                        {
                            var perfilsOld = perfis.FirstOrDefault(p => p.Nome == perfilOld);
                            var remover = associado.Data.Any(p => p.Nome == perfilOld);
                            if (remover)
                                proxy.RemoverAssociacaoUsuarioPerfilApp(new InfoAssociarUsuarioPerfil { IdProfileApp = perfilsOld.IdProfileApp, Login = usuario.Login });
                        }

                        if (associado?.Data == null || !associado.Data.Any(p => p.Nome == usuario.Perfil) && usuario.Status)
                        {
                            var retPerf = proxy.AssociarUsuarioPerfilApp(new InfoAssociarUsuarioPerfil { IdProfileApp = perfil.IdProfileApp, Login = usuario.Login });
                            if (!retPerf.Success)
                                return retPerf.Message;
                            if (comunicarUsuario)
                            {
                                ComunicarUsuarioNovoAcesso(usuario);
                            }
                        }
                        else if (!usuario.Status)
                        {
                            proxy.RemoverAssociacaoUsuarioPerfilApp(new InfoAssociarUsuarioPerfil { IdProfileApp = perfil.IdProfileApp, Login = usuario.Login });
                        }
                    }
                }
                else
                {
                    new RaizenException().LogarErro();
                    return "Não existe perfil associado com a aplicação!";
                }
            }
            else
            {
                new RaizenException().LogarErro();
                return "Aplicação não encontrada no UserSystem!";
            }
            return null;
        }

        private void ComunicarUsuarioNovoAcesso(Usuario usuario)
        {
            var site = Config.GetConfig(EnumConfig.UrlSite, (int)EnumPais.Padrao);

            if (usuario.Externo)
                site = Config.GetConfig(EnumConfig.UrlSiteTransportadoras, (int)EnumPais.Padrao);

            var subject = usuario.IDPais == EnumPais.Argentina ? "Raízen – Acceso Concedido" : "Raízen - Acesso concedido";

            var configBll = new ConfiguracaoBusiness();

            var config = configBll.Selecionar(w => w.NmVariavel == (usuario.IDPais == EnumPais.Brasil ? "CadastroUsuariosBrasil" : "CadastroUsuariosArgentina"));

            if (config == null)
                return;

            var msg = string.Format(config.Valor, site);

            var enviado = Email.Enviar(usuario.Email, subject, msg);
        }

        private static void ComunicarUsuarioNovaSenha(InfoUsuario user, EnumPais IDPais)
        {
            var site = Config.GetConfig(EnumConfig.UrlSite, (int)EnumPais.Padrao);

            var userBll = new UsuarioBusiness();
            var configBll = new ConfiguracaoBusiness();

            var usuario = userBll.Selecionar(user.Id);

            if (user.Tipo == 0)
                site = Config.GetConfig(EnumConfig.UrlSiteTransportadoras, (int)EnumPais.Padrao);

            var idPais = EnumPais.Brasil;

			if (usuario == null)
				idPais = IDPais;
			else
				idPais = usuario.IDPais;

            var subject = idPais == EnumPais.Argentina ? "Raízen – Acceso Concedido" : "Raízen - Acesso concedido";

            var config = configBll.Selecionar(w => w.NmVariavel == (idPais == EnumPais.Brasil ? "CadastroUsuariosSenhaBrasil" : "CadastroUsuariosSenhaArgentina"));

			if (config == null)
                return;

            var msg = string.Format(config.Valor, user.Login, user.Senha, site);

            var enviado = Email.Enviar(user.Email, subject, msg);
        }

        private string SenhaRandomica()
        {
            return string.Format("{0}{1}{2}", alfanumericoAleatorio(6), "@", new Random().Next(99));
        }

        public static string alfanumericoAleatorio(int tamanho)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}

