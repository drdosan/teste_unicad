using Raizen.Framework.Log.Bases;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Web.Models;
using Raizen.UniCad.Web.Util;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Routing;

namespace Raizen.UniCad.Web.Controllers
{
    [AllowAnonymous]
    public class AutenticarCsOnlineTradingController : Controller
    {
        private static readonly string SECRET = GenerateSecret();

        // GET: Autenticar
        [AllowAnonymous]
        public ActionResult Index(string token)
        {
            try
            {
                var claims = Jwt.GetClaims(token, SECRET);
                EnumPais pais = GetPais(claims["lan"]);
                string tokenSso;
                string mensagemErro = new ClienteAcsBusiness().AutenticarTrading(claims["nome"], pais, out tokenSso);

                if (!string.IsNullOrEmpty(mensagemErro))
                {
                    var parms = new RouteValueDictionary()
                    {
                        {"lan",claims["lan"] },
                        {"mensagem",mensagemErro },
                    };
                    return RedirectToAction("IndexMensagemErro", "AutenticarCsOnline", parms);
                }

                string cnpj = claims["cnpj"];
                Session["ibmUsuario"] = cnpj;
                var user = new UsuarioBusiness().Selecionar(p => p.Login == cnpj);
                Session["UsuarioUnicad"] = user;

                string tipo = claims["tipo"];
                if (user != null && string.IsNullOrEmpty(user.Email))
                {
                    Session["TOKEN_SSO_TEMP"] = tokenSso;

                    var parms = new RouteValueDictionary()
                {
                    {"userID",user.ID },
                    {"tipo",tipo },
                    {"pais",pais },
                };
                    return RedirectToAction("IndexModel", "AutenticarCsOnline", parms);
                }
                else
                {
                    Session["TOKEN_SSO"] = tokenSso;
                    if (Equals(pais, EnumPais.Brasil))
                    {
                        if (tipo == "Motorista")
                        {
                            return RedirectToAction("Index", "Motorista");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Composicao");
                        }
                    }
                    else
                    {
                        if (tipo == "Motorista")
                        {
                            return RedirectToAction("Index", "MotoristaArgentina");
                        }
                        else
                        {
                            return RedirectToAction("Index", "ComposicaoArgentina");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                new RaizenException("Problemas ao autenticar no CSOnline Trading", ex).LogarErro();
                var parms = new RouteValueDictionary()
                    {
                        {"lan","" },
                        {"mensagem",$"Problemas ao autenticar no CSOnline Trading - {ex.Message}" },
                    };
                return RedirectToAction("IndexMensagemErro", "AutenticarCsOnline", parms);
            }
        }

        // POST: Autenticar
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Index(AutenticarCsOnlineTradingModel request)
        {
            EnumPais pais = GetPais(request.Lan);
            string nome;
            string cnpj;
            string mensagemErro = new ClienteAcsBusiness().RegistrarTokenTrading(request.Token, pais, out cnpj, out nome);

            if (!string.IsNullOrEmpty(mensagemErro))
            {
                return new JsonHttpStatusResult(mensagemErro, System.Net.HttpStatusCode.BadRequest);
            }

            var claims = new Dictionary<string, string>
            {
                { "cnpj", cnpj },
                { "nome", nome },
                { "lan", request.Lan },
                { "tipo", request.Tipo },
                { "dv", request.Dv ?? String.Empty },
            };
            string tokenUnicad = Jwt.GenerateToken(claims, SECRET, TimeSpan.FromMinutes(45));
            return new JsonResult { Data = new { link = $"{Request.Url.AbsoluteUri}?token={tokenUnicad}" } };
        }

        private EnumPais GetPais(string lan)
        {
            switch (lan)
            {
                case "pt-BR":
                    return EnumPais.Brasil;

                case "es-AR":
                    return EnumPais.Argentina;
            }

            return 0;
        }

        private static string GenerateSecret()
        {
            var key = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return Convert.ToBase64String(key);
        }
    }
}