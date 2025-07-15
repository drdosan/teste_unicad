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
using Raizen.UniCad.Utils;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model.View;
using System.Linq.Expressions;
using Raizen.Framework.Resources;
using Raizen.UniCad.Model.Base;

namespace Raizen.UniCad.Web.Models
{
	public class ModelPlaca : BaseModel, IValidatableObject
	{
			#region Constantes
			public PlacaFiltro Filtro { get; set; }
			public Placa Placa { get; set; }
			public PlacaView PlacaOficial { get; set; }
			public List<Placa> ListaPlaca { get; set; }
			public EnumMensagemPlaca MensagemId { get; set; }
			public string Mensagem { get; set; }
			public DateTime Data { get; set; }
			public string Anexo { get; set; }
			public string Justificativa { get; set; }
			public bool Aprovar { get; set; }
			public bool Novo { get; set; }
			public int IdPais { get; set; }
			public List<HistorioBloqueioComposicao> ListaHistorico { get; set; }

			public List<ChecklistComposicao> ListaHistoricoCheck { get; set; }
			public int IDCategoriaVeiculo { get; set; }
			public int? IDComposicao { get; set; }
			public Usuario Usuario { get; set; }
			public int IDEmpresa { get; internal set; }
			#endregion

		#region Validação de Integridade

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (int.Equals(this.IdPais, (int)EnumPais.Argentina))
				return ValidateArgentina();

			return ValidateBrasil();
		}
		private IEnumerable<ValidationResult> ValidateArgentina()
		{
			var results = new List<ValidationResult>();

			if (this.Placa != null)
			{
				if (UsuarioNaoCadastrado())
				{
					results.Add(new ValidationResult("Usuario no registrado en UNICAD, contacte a la persona responsablel", new string[] { "Usuario" }));
					return results;
				}

				if (this.Placa.Operacao == "CIF" && this.Placa.IDTransportadora == null)
				{
					results.Add(new ValidationResult("Es necesario vincular un transportista.", new string[] { "Placa_IDTransportadora" }));
					return results;
				}				

				if (string.IsNullOrEmpty(Placa.PlacaVeiculo))
				{
					results.Add(new ValidationResult("¡Campo obligatorio!", new string[] { "Placa_PlacaVeiculo" }));
					return results;
				}

				if (PlacaFormatoInvalido(Placa.PlacaVeiculo))
				{
					results.Add(new ValidationResult("El formato de la patente no es válido.", new string[] { "Placa_PlacaVeiculo" }));
					return results;
				}
				else
				{
					Placa placaExistente = new PlacaBusiness(EnumPais.Argentina).Selecionar(p => p.PlacaVeiculo == Placa.PlacaVeiculo);

					if (placaExistente != null && Placa.IDTipoVeiculo != placaExistente.IDTipoVeiculo)
					{
						results.Add(new ValidationResult("La tarjeta ya existe, pero no de tipo: " + EnumExtensions.GetDescription((EnumTipoVeiculo)Placa.IDTipoVeiculo), new string[] { "Placa_PlacaVeiculo" }));
						return results;
					}

					bool composicaoAguardando = new PlacaBusiness(EnumPais.Argentina).ListarPorStatus(this.ChavePrimaria, null, Placa.PlacaVeiculo, EnumStatusComposicao.AguardandoAtualizacaoSAP, Placa.Operacao);
					bool composicaoAprovado = new PlacaBusiness(EnumPais.Argentina).ListarPorStatus(this.ChavePrimaria, null, Placa.PlacaVeiculo, EnumStatusComposicao.EmAprovacao, Placa.Operacao);

					if ((composicaoAguardando || composicaoAprovado) && !Aprovar)
					{
						results.Add(new ValidationResult("Esta patente está en aprobación, no está permitido incluir una nueva solicitud hasta que la misma sea aprobada/reprobada.", new string[] { "Placa_PlacaVeiculo" }));
						return results;
					}
				}

				if (this.Placa.PlacaArgentina.CUIT != null)
				{
					this.Placa.PlacaArgentina.CUIT = this.Placa.PlacaArgentina.CUIT.RemoveCharacter();

					// TODO: Aguardando definição de regra para validação do campo "CUIT"

					if (ValidaRazaoSocial(Placa.RazaoSocial, Placa.PlacaArgentina.CUIT))
					{
						results.Add(new ValidationResult("!Campo obligatorio!", new string[] { "PlacaArgentina_CUIT" }));
						return results;
					}
				}

				if (int.Equals(Placa.IDCategoriaVeiculo, 0))
				{
					results.Add(new ValidationResult("!Campo obligatorio!", new string[] { "Placa_IDCategoriaVeiculo" }));
					return results;
				}

				if (ValidaDocumentosObrigatorios(Placa.Anexo, Placa.Documentos))
				{
					results.Add(new ValidationResult("Debe adjuntar los documentos obligatorios o el documento único de la patente", new string[] { "Documentos" }));
					return results;
				}

				if (ValidaDataDocumentos(Placa.Documentos))
				{
					results.Add(new ValidationResult("La fecha debe ser mayor que 1800 y menor que 2900.", new string[] { "Documentos" }));
					return results;
				}				

				if (this.Placa.Operacao == "FOB" && (this.Placa.Clientes == null || !this.Placa.Clientes.Any()) && GetNecessitaPermissao())
				{
					results.Add(new ValidationResult("Debe asociar un cliente.", new string[] { "ClienteAuto" }));
					return results;
				}

				if (this.Placa.Operacao != "FOB" || this.Placa.LinhaNegocio == (int)EnumEmpresa.EAB)
				{
					if (string.IsNullOrEmpty(this.Placa.PlacaArgentina.CUIT))
					{
						results.Add(new ValidationResult("!Campo obligatorio!", new List<string>() { "PlacaArgentina_CUIT" }));
						return results;
					}

					if (this.Placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Tractor && (!this.Placa.PlacaArgentina.PBTC.HasValue || double.Equals(this.Placa.PlacaArgentina.PBTC, 0)))
					{
						results.Add(new ValidationResult("!Campo obligatorio!", new string[] { "PlacaArgentina_PBTC" }));
						return results;
					}
				}

				if (this.ChavePrimaria != null)
				{
					Placa PlacaOld = new PlacaBusiness(EnumPais.Argentina).Selecionar(int.Parse(this.ChavePrimaria));

					if (PlacaOld != null && !PlacaOld.PlacaVeiculo.Equals(this.Placa.PlacaVeiculo) && new PlacaBusiness(EnumPais.Argentina).Selecionar(item => item.PlacaVeiculo.Equals(this.Placa.PlacaVeiculo)) != null)
					{
						results.Add(new ValidationResult("Esta patente ya existe.", new string[] { "Placa_PlacaVeiculo" }));
						return results;
					}
				}
			}

			return results;
		}
		private IEnumerable<ValidationResult> ValidateBrasil()
		{
			var results = new List<ValidationResult>();
			PlacaBusiness appBll = new PlacaBusiness();

			if (this.Placa != null)
			{
				PlacaBusiness placaBll = new PlacaBusiness();

				if (UsuarioNaoCadastrado())
				{
					results.Add(new ValidationResult("Usuário não cadastrado no UNICAD, entre em contato com o responsável", new string[] { "Usuario" }));
					return results;
				}

				if (string.IsNullOrEmpty(Placa.PlacaVeiculo))
				{
					results.Add(new ValidationResult(MensagensPadrao.CampoObrigatorio, new string[] { "Placa_PlacaVeiculo" }));
					return results;
				}


				if (PlacaFormatoInvalido(Placa.PlacaVeiculo))
				{
					results.Add(new ValidationResult("O formato da Placa está inválido.", new string[] { "Placa_PlacaVeiculo" }));
					return results;
				}
				else
				{
					Placa placaExistente = new PlacaBusiness(EnumPais.Argentina).Selecionar(p => p.PlacaVeiculo == Placa.PlacaVeiculo);

					if (placaExistente != null)
					{
						if (Placa.IDTipoVeiculo != placaExistente.IDTipoVeiculo)
						{
							results.Add(new ValidationResult("Placa já existe, porém não é do tipo: " + EnumExtensions.GetDescription((EnumTipoVeiculo)Placa.IDTipoVeiculo), new string[] { "Placa_PlacaVeiculo" }));
							return results;
						}
					}

					bool composicaoAguardando = appBll.ListarPorStatus(this.ChavePrimaria, null, Placa.PlacaVeiculo, EnumStatusComposicao.AguardandoAtualizacaoSAP, Placa.Operacao);
					bool composicaoAprovado = appBll.ListarPorStatus(this.ChavePrimaria, null, Placa.PlacaVeiculo, EnumStatusComposicao.EmAprovacao, Placa.Operacao);

					if ((composicaoAguardando || composicaoAprovado) && !Aprovar)
					{
						results.Add(new ValidationResult("Esta placa está em aprovação, não é permitido incluir uma solicitação até que a mesma seja aprovada/reprovada.", new string[] { "Placa_PlacaVeiculo" }));
						return results;
					}

					//retirada validação de placa bloqueada CSCUNI-1335
					//bool composicaoBloqueada;
					//if (Placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck
					//    || (Placa.LinhaNegocio == (int)EnumEmpresa.EAB && Placa.Numero == 1)
					//    || (Placa.LinhaNegocio == (int)EnumEmpresa.Combustiveis && Placa.Numero == 2)
					//    || (Placa.LinhaNegocio == (int)EnumEmpresa.Ambos && (Placa.Numero == 1 || Placa.Numero == 2)))
					//    composicaoBloqueada = false;
					//else
					//    composicaoBloqueada = appBll.ListarPorStatus(this.ChavePrimaria, null, Placa.PlacaVeiculo, EnumStatusComposicao.Bloqueado, Placa.Operacao);
					//if (composicaoBloqueada && !Aprovar)
					//{

					//    results.Add(new ValidationResult("Esta placa está bloqueada, não é permitido incluir uma solicitação até que a mesma seja desbloqueada.", new string[] { "Placa_PlacaVeiculo" }));
					//    return results;
					//}
				}

				bool valido = false;
				bool placaCnpj = false;

				if (this.Placa.PlacaBrasil.CPFCNPJ != null)
				{
					this.Placa.PlacaBrasil.CPFCNPJ = this.Placa.PlacaBrasil.CPFCNPJ.RemoveCharacter();

					placaCnpj = this.Placa.PlacaBrasil.CPFCNPJ.Length > 11;
					valido = placaCnpj ? ValidacoesUtil.ValidaCNPJ(Placa.PlacaBrasil.CPFCNPJ) : ValidacoesUtil.ValidaCPF(Placa.PlacaBrasil.CPFCNPJ);

					if (valido && !placaCnpj && !Placa.DataNascimento.HasValue)
					{
						results.Add(new ValidationResult("A Data de Nascimento é obrigatória.", new string[] { "Placa_DataNascimento" }));
						return results;
					}

					if (valido && !placaCnpj && !ValidacoesUtil.ValidarRangeData(Placa.DataNascimento.Value.Year))
					{
						results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "Placa_DataNascimento" }));
						return results;
					}

					if (!valido)
					{
						results.Add(new ValidationResult("O formato do CPF/CNPJ está inválido.", new string[] { "Placa_CPFCNPJ" }));
						return results;
					}

					if (ValidaRazaoSocial(Placa.RazaoSocial, Placa.PlacaBrasil.CPFCNPJ))
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_RazaoSocial" }));
						return results;
					}
				}

				if (int.Equals(Placa.IDCategoriaVeiculo, null) || int.Equals(Placa.IDCategoriaVeiculo, 0))
				{
					results.Add(new ValidationResult(MensagensPadrao.CampoObrigatorio, new string[] { "Placa_IDCategoriaVeiculo" }));
					return results;
				}


				if ((this.Placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Carreta || this.Placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck) && this.Placa.Setas != null)
				{
					if (this.Placa.LinhaNegocio == (int)EnumEmpresa.EAB &&
						(Placa.Setas.Any(w => w.LacreCompartimento1.HasValue && w.LacreCompartimento1.Value <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento2.HasValue && w.LacreCompartimento2 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento3.HasValue && w.LacreCompartimento3 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento4.HasValue && w.LacreCompartimento4 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento5.HasValue && w.LacreCompartimento5 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento6.HasValue && w.LacreCompartimento6 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento7.HasValue && w.LacreCompartimento7 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento8.HasValue && w.LacreCompartimento8 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento9.HasValue && w.LacreCompartimento9 <= 0.00m)
						|| Placa.Setas.Any(w => w.LacreCompartimento10.HasValue && w.LacreCompartimento10 <= 0.00m)))
					{
						results.Add(new ValidationResult("Informe a quantidade de lacre de cada compartimento.", new string[] { "Compartimento" }));
						return results;
					}


					if (!this.Placa.MultiSeta)
					{
						if ((Placa.Setas.Any(w => w.VolumeCompartimento1.HasValue && w.VolumeCompartimento1.Value <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento2.HasValue && w.VolumeCompartimento2 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento3.HasValue && w.VolumeCompartimento3 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento4.HasValue && w.VolumeCompartimento4 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento5.HasValue && w.VolumeCompartimento5 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento6.HasValue && w.VolumeCompartimento6 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento7.HasValue && w.VolumeCompartimento7 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento8.HasValue && w.VolumeCompartimento8 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento9.HasValue && w.VolumeCompartimento9 <= 0.00m)
								|| Placa.Setas.Any(w => w.VolumeCompartimento10.HasValue && w.VolumeCompartimento10 <= 0.00m)))
						{
							results.Add(new ValidationResult("Todos os campos deverão conter volume.", new string[] { "Compartimento" }));
							return results;
						}
					}
					else
					{
						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento1.HasValue && p.VolumeCompartimento1.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal1.HasValue && p.CompartimentoPrincipal1.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 1 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento2.HasValue && p.VolumeCompartimento2.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal2.HasValue && p.CompartimentoPrincipal2.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 2 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento3.HasValue && p.VolumeCompartimento3.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal3.HasValue && p.CompartimentoPrincipal3.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 3 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento4.HasValue && p.VolumeCompartimento4.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal4.HasValue && p.CompartimentoPrincipal4.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 4 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento5.HasValue && p.VolumeCompartimento5.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal5.HasValue && p.CompartimentoPrincipal5.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 5 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento6.HasValue && p.VolumeCompartimento6.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal6.HasValue && p.CompartimentoPrincipal6.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 6 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento7.HasValue && p.VolumeCompartimento7.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal7.HasValue && p.CompartimentoPrincipal7.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 7 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento8.HasValue && p.VolumeCompartimento8.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal8.HasValue && p.CompartimentoPrincipal8.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 8 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento9.HasValue && p.VolumeCompartimento9.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal9.HasValue && p.CompartimentoPrincipal9.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 9 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						if (this.Placa.Setas.Any(p => (p.VolumeCompartimento10.HasValue && p.VolumeCompartimento10.Value > 0)))
							if (!this.Placa.Setas.Any(p => p.CompartimentoPrincipal10.HasValue && p.CompartimentoPrincipal10.Value))
							{
								results.Add(new ValidationResult("Seta principal do compartimento 10 é obrigatória.", new string[] { "Compartimento" }));
								return results;
							}

						foreach (var item in this.Placa.Setas)
						{
							if (this.Placa.MultiSeta && item.VolumeCompartimento1.HasValue && item.VolumeCompartimento1.Value == 0 && item.CompartimentoPrincipal1.HasValue && item.CompartimentoPrincipal1.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 1 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento2.HasValue && item.VolumeCompartimento2.Value == 0 && item.CompartimentoPrincipal2.HasValue && item.CompartimentoPrincipal2.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 2 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento3.HasValue && item.VolumeCompartimento3.Value == 0 && item.CompartimentoPrincipal3.HasValue && item.CompartimentoPrincipal3.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 3 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento4.HasValue && item.VolumeCompartimento4.Value == 0 && item.CompartimentoPrincipal4.HasValue && item.CompartimentoPrincipal4.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 4 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento5.HasValue && item.VolumeCompartimento5.Value == 0 && item.CompartimentoPrincipal5.HasValue && item.CompartimentoPrincipal5.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 5 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento6.HasValue && item.VolumeCompartimento6.Value == 0 && item.CompartimentoPrincipal6.HasValue && item.CompartimentoPrincipal6.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 6 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento7.HasValue && item.VolumeCompartimento7.Value == 0 && item.CompartimentoPrincipal7.HasValue && item.CompartimentoPrincipal7.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 7 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento8.HasValue && item.VolumeCompartimento8.Value == 0 && item.CompartimentoPrincipal8.HasValue && item.CompartimentoPrincipal8.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 8 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento9.HasValue && item.VolumeCompartimento9.Value == 0 && item.CompartimentoPrincipal9.HasValue && item.CompartimentoPrincipal9.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 9 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
							if (this.Placa.MultiSeta && item.VolumeCompartimento10.HasValue && item.VolumeCompartimento10.Value == 0 && item.CompartimentoPrincipal10.HasValue && item.CompartimentoPrincipal10.Value)
							{
								results.Add(new ValidationResult("Seta principal do compartimento 10 é obrigatória e deve conter volume.", new string[] { "Compartimento" }));
								return results;
							}
						}
					}
				}

				if (ValidaDocumentosObrigatorios(Placa.Anexo, Placa.Documentos))
				{
					results.Add(new ValidationResult("É necessário anexar os documentos obrigatórios ou o documento único da placa.", new string[] { "Documentos" }));
					return results;
				}

				if (ValidaDataDocumentos(Placa.Documentos))
				{
					results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "Documentos" }));
					return results;
				}

				if (this.Placa.Operacao == "FOB" && (this.Placa.Clientes == null || !this.Placa.Clientes.Any()) && GetNecessitaPermissao())
				{
					results.Add(new ValidationResult("É necessário associar um cliente.", new string[] { "ClienteAuto" }));
					return results;
				}


				if (this.Placa.Operacao == "CIF" && !this.Placa.IDTransportadora.HasValue)
				{
					results.Add(new ValidationResult("É necessário associar uma transportadora.", new string[] { "Placa_IDTransportadora" }));
					return results;
				}

				//if (this.Placa.Documentos != null && this.Placa.Documentos.Any())
				//{
				//    foreach(var doc in this.Placa.Documentos)
				//    {
				//        if (doc.DataVencimento < DateTime.Now.Date)
				//        {
				//            results.Add(new ValidationResult("Data de vencimento anterior a data atual.", new string[] { "Documento_DataVencimento" }));
				//            return results;
				//        }
				//    }
				//}

				if (this.Placa.Operacao != "FOB" || this.Placa.LinhaNegocio == (int)EnumEmpresa.EAB)
				{

					if (string.IsNullOrEmpty(this.Placa.Marca))
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_Marca" }));
						return results;
					}
					if (string.IsNullOrEmpty(this.Placa.Modelo))
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_Modelo" }));
						return results;
					}
					if (string.IsNullOrEmpty(this.Placa.Chassi))
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_Chassi" }));
						return results;
					}
					if (this.Placa.AnoFabricacao == 0)
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_AnoFabricacao" }));
						return results;
					}

					if (!ValidacoesUtil.ValidarRangeData(this.Placa.AnoFabricacao))
					{
						results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "Placa_AnoFabricacao" }));
						return results;
					}

					if (this.Placa.AnoModelo == 0)
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_AnoModelo" }));
						return results;
					}

					if (!ValidacoesUtil.ValidarRangeData(this.Placa.AnoModelo))
					{
						results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "Placa_AnoModelo" }));
						return results;
					}

					if (string.IsNullOrEmpty(this.Placa.Cor))
					{
						results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_Cor" }));
						return results;
					}

					if (this.Placa.LinhaNegocio != (int)EnumEmpresa.EAB)
					{
						if (string.IsNullOrEmpty(this.Placa.TipoRastreador))
						{
							results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_TipoRastreador" }));
							return results;
						}
						if (string.IsNullOrEmpty(this.Placa.NumeroAntena))
						{
							results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_NumeroAntena" }));
							return results;
						}
						if (string.IsNullOrEmpty(this.Placa.Versao))
						{
							results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_Versao" }));
							return results;
						}
						//if (string.IsNullOrEmpty(this.Placa.CameraMonitoramento))
						//{
						//    results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_Versao" }));
						//    return results;
						//}
					}
				}


				if (this.Placa.AnoModelo < this.Placa.AnoFabricacao)
				{
					results.Add(new ValidationResult("O Ano de Modelo não pode ser menor que o Ano de Fabricação.", new string[] { "Placa_AnoModelo" }));
					return results;
				}

				if (!this.Placa.IDTipoProduto.HasValue && (this.Placa.IDTipoVeiculo == 2 || this.Placa.IDTipoVeiculo == 4))
				{
					results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_IDTipoProduto" }));
					return results;
				}

				if (!this.Placa.IDTipoCarregamento.HasValue && (this.Placa.IDTipoVeiculo == 2 || this.Placa.IDTipoVeiculo == 4))
				{
					results.Add(new ValidationResult("Preenchimento obrigatório.", new string[] { "Placa_IDTipoCarregamento" }));
					return results;
				}

				if (this.Placa.NumeroEixosDistanciados > 3)
				{
					results.Add(new ValidationResult("Limite máximo de 3 eixos distânciados.", new string[] { "Placa_EixosDistanciados" }));
					return results;
				}

				if (this.Placa.NumeroEixosPneusDuplos > 10)
				{
					results.Add(new ValidationResult("Limite máximo de 10 eixos com pneus duplos.", new string[] { "Placa_NumeroEixosPneusDuplos" }));
					return results;
				}

				if (this.ChavePrimaria != null)
				{
					Placa PlacaOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

					if (PlacaOld != null && !PlacaOld.PlacaVeiculo.Equals(this.Placa.PlacaVeiculo))
					{
						Placa placa = appBll.Selecionar(item => item.PlacaVeiculo.Equals(this.Placa.PlacaVeiculo));

						if (placa != null)
						{
							results.Add(new ValidationResult("Está Placa já existe.", new string[] { "Placa_PlacaVeiculo" }));
							return results;
						}

					}
				}
			}

			return results;
		}

		#endregion

		#region Private methods

		private bool ValidaDataDocumentos(List<PlacaDocumentoView> documentos)
		{
			return documentos != null && documentos.Any(w => w.DataVencimento.HasValue && (w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900));
		}
		private bool GetNecessitaPermissao()
		{
			return this.Placa.idTipoComposicao == (int)EnumTipoComposicao.Truck ||
												(this.Placa.LinhaNegocio == 1 && this.Placa.idTipoParteVeiculo == 1) ||
												(this.Placa.LinhaNegocio == 2 && this.Placa.idTipoParteVeiculo == 2) ||
												(this.Placa.LinhaNegocio == 3 && this.Placa.idTipoParteVeiculo == 1);
		}
		private bool ValidaDocumentosObrigatorios(string anexo, List<PlacaDocumentoView> documentos)
		{
			return string.IsNullOrEmpty(anexo) && (documentos != null && documentos.Any() && documentos.Any(p => string.IsNullOrEmpty(p.Anexo) && p.Obrigatorio));
		}
		private bool ValidaRazaoSocial(string razaoSocial, string CPFCNPJCUIT)
		{
			return string.IsNullOrEmpty(razaoSocial) && CPFCNPJCUIT.Length > 11;
		}
		private bool UsuarioNaoCadastrado()
		{
			return Placa.idUsuario == 0;
		}
		private bool PlacaFormatoInvalido(string placa)
		{
			Regex regex = new Regex(@"^\w+$");
			return placa != null && !regex.Match(placa).Success;
		}

		#endregion
	}
}