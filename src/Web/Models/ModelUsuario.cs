using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.BLL;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Raizen.UniCad.Web.Models
{
    public class ModelUsuario : BaseModel, IValidatableObject
    {
        #region Constantes
        public UsuarioFiltro Filtro { get; set; }
        public Usuario Usuario { get; set; }
        public List<Usuario> ListaUsuario { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.Usuario != null)
            {
                UsuarioBusiness appBll = new UsuarioBusiness();

                if (!string.IsNullOrEmpty(this.ChavePrimaria))
                {
                    Usuario UsuarioOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                    if ((!UsuarioOld.Login.Equals(this.Usuario.Login)) && appBll.Existe(item => item.Login.Equals(this.Usuario.Login)))
                    {
                        results.Add(new ValidationResult("Já existe Usuário com esse Login.", new string[] { "Usuario_Login" }));
                        return results;
                    }

                    if (!string.IsNullOrEmpty(UsuarioOld.Email) && !string.IsNullOrEmpty(Usuario.Email))
                    {
                        //R6) Verificar a validação de email pré-existente no cadastro do usuário
                        if ((!UsuarioOld.Email.Equals(this.Usuario.Email)) && appBll.Existe(item => item.Email.Equals(this.Usuario.Email)) && Usuario.Perfil != EnumPerfil.CLIENTE_ACS && Usuario.Perfil != EnumPerfil.CLIENTE_ACS_ARGENTINA)
                        {
                            results.Add(new ValidationResult("Já existe Usuário com esse Email.", new string[] { "Usuario_Email" }));
                            return results;
                        }
                    }
                }
                else
                {
                    if (appBll.Existe(item => item.Login.Equals(this.Usuario.Login)))
                    {
                        results.Add(new ValidationResult("Já existe um Usuário com esse Login.", new string[] { "Usuario_Login" }));
                        return results;
                    }

                    if (!string.IsNullOrEmpty(this.Usuario.Email))
                        if (appBll.Existe(item => item.Email.Equals(this.Usuario.Email)) && Usuario.Externo)
                        {
                            results.Add(new ValidationResult("Já existe um Usuário com esse Email.", new string[] { "Usuario_Email" }));
                            return results;
                        }
                }



                if (!this.Usuario.Externo)
                {
                    if (this.Usuario.Perfil == "Cliente EAB")
                    {
                        results.Add(new ValidationResult("Perfil de Cliente EAB não é permitido para Usuário interno.", new string[] { "Usuario_Perfil" }));
                        return results;
                    }

                    if (this.Usuario.Perfil == "Transportadora")
                    {
                        results.Add(new ValidationResult("Perfil de Transportadora não é permitido para Usuário interno.", new string[] { "Usuario_Perfil" }));
                        return results;
                    }
                    if (this.Usuario.Perfil == "Transportadora Argentina")
                    {
                        results.Add(new ValidationResult("Perfil de Transportadora Argentina não é permitido para Usuário interno.", new string[] { "Usuario_Perfil" }));
                        return results;
                    }

                    if (string.IsNullOrEmpty(this.Usuario.Login))
                    {
                        results.Add(new ValidationResult("O Campo Login é Obrigatório.", new string[] { "Usuario_Login" }));
                        return results;
                    }

                    Regex regexCsTr = new Regex("(CS|TR)[0-9]{6}");
                    var str = Usuario.Login.ToUpper(CultureInfo.InvariantCulture);
                    
                    if ((str.StartsWith("CS") || str.StartsWith("TR")) && !regexCsTr.Match(str).Success)
                    {
                        results.Add(new ValidationResult("O formato do Login está inválido.", new string[] { "Usuario_Login" }));
                        return results;
                    }
                }
                else
                {
                    //R7) Verificar a validação do Usuário Externo que deve ser do Perfil Transportadora ou Cliente EAB
                    if (!PerfilExternoAutorizado())
                    {
                        results.Add(new ValidationResult("O perfil selecionado não permite cadastrar Usuário Externo.", new string[] { "Usuario_Perfil" }));
                        return results;
                    }

                }
                if (string.IsNullOrEmpty(this.Usuario.Email))
                {
                    results.Add(new ValidationResult("O Campo Email é Obrigatório.", new string[] { "Usuario_Email" }));
                    return results;
                }

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (Usuario.Email != null && !regex.Match(Usuario.Email).Success)
                {
                    results.Add(new ValidationResult("O formato do Email está inválido.", new string[] { "Usuario_Email" }));
                    return results;
                }

                if (Usuario.Externo && (Usuario.Perfil == "Transportadora" || Usuario.Perfil == "Transportadora Argentina") && (Usuario.Transportadoras == null || !Usuario.Transportadoras.Any()))
                {
                    results.Add(new ValidationResult("Informe ao menos uma transportadora.", new string[] { "TransportadoraAuto" }));
                    return results;
                }

                if (Usuario.Externo && Usuario.Perfil == "Cliente EAB" && (Usuario.Clientes == null || !Usuario.Clientes.Any()))
                {
                    results.Add(new ValidationResult("Informe ao menos um Cliente.", new string[] { "ClienteAuto" }));
                    return results;
                }
            }

            return results;
        }

        private bool PerfilExternoAutorizado()
        {
            return
                this.Usuario.Perfil == EnumPerfil.TRANSPORTADORA ||
                this.Usuario.Perfil.Trim() == EnumPerfil.TRANSPORTADORA_ARGENTINA ||
                this.Usuario.Perfil == EnumPerfil.CLIENTE_EAB ||
                this.Usuario.Perfil == EnumPerfil.CLIENTE_ACS ||
                this.Usuario.Perfil == EnumPerfil.CLIENTE_ACS_ARGENTINA ||
                this.Usuario.Perfil == EnumPerfil.CLIENTE_ACS_ARGENTINA ||
                this.Usuario.Perfil == EnumPerfil.QUALITY;
        }

        #endregion

    }
}