using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Utils;
using System;

namespace Raizen.UniCad.Web.Models
{
	public class ModelMotoristaArgentina : BaseModel, IValidatableObject
	{
		#region Constantes
		public MotoristaFiltro Filtro { get; set; }
		public Motorista Motorista { get; set; }
		public List<TerminalTreinamentoView> ListaTerminais { get; set; }
		public List<TipoProduto> ListaTipoProduto { get; set; }
		public List<TipoComposicao> ListaTipoComposicao { get; set; }
		public MotoristaAlteradoView Alteracoes { get; set; }
		public MotoristaTreinamentoTerminal MotoristaTreinamentoTerminal { get; set; }
		public List<HistorioBloqueioMotorista> ListaHistorico { get; set; }
		public List<HistorioAtivarMotorista> ListaHistoricoAtivar { get; set; }
		public List<MotoristaView> ListaMotorista { get; set; }
		public TreinamentoView TreinamentoView { get; set; }
		public List<HistoricoTreinamentoTeoricoMotorista> ListaTreinamento { get; set; }
		public int ID { get; set; }
		public int Acao { get; set; }
		public int LinhaNegocio { get; set; }
		public bool Aprovar { get; set; }
		public int isFromAgendamento { get; set; }
		public bool Reprovar { get; set; }
		public string Justificativa { get; set; }
		public bool comRessalvas { get; set; }
		public bool? TreinamentoAprovado { get; set; }
		public string UsuarioPerfil { get; set; }
		public Usuario Usuario { get; set; }
		public bool Novo { get; internal set; }
		#endregion

		#region Validação de Integridade
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			if (this.Motorista != null)
			{
				var dni = this.Motorista.MotoristaArgentina.DNI.RemoveCharacter();

				if (Aprovar || Reprovar)
				{
					var idStatus = new MotoristaBusiness().Selecionar(Motorista.ID).IDStatus;
					if (idStatus == (int)EnumStatusMotorista.Aprovado || idStatus == (int)EnumStatusMotorista.Reprovado)
					{
						results.Add(new ValidationResult("¡Esa solicitud ya fue aprobada/reprobada por otro usuario!. Esta acción no puede ser ejecutada", new string[] { "Motorista" }));
						return results;
					}

                    if (!Reprovar && !new MotoristaBusiness(EnumPais.Argentina).TransportadoraArgentinaValida(Motorista))
                    {
                        results.Add(new ValidationResult("¡Debe registrar el transportista de este conductor con SAP antes de aprobar su registro!", new string[] { "Motorista" }));
                        return results;
                    }

					if (string.IsNullOrWhiteSpace(this.Motorista.MotoristaArgentina.Tarjeta))
					{
						results.Add(new ValidationResult("¡Debe completar el campo de la tarjeta para la aprobación de este conductor!", new string[] { "Motorista" }));
						return results;
					}
				}

				if (!Reprovar)
				{
					return ReprovarInativo(dni);

				}
				else if (Reprovar)
				{
					//verifica se o dni mudou
					using (var appBll = new MotoristaBusiness(EnumPais.Argentina))
					{
						var numDniAtual = appBll.Selecionar(m => m.ID == this.Motorista.ID).MotoristaArgentina.DNI;
						return ValidaDniAtual(dni, numDniAtual);
					}
				}
			}

			return results;
		}        

        private List<ValidationResult> ValidaDniAtual(string dni, string numDniAtual)
		{
			var results = new List<ValidationResult>();

			if (numDniAtual != dni)
			{
				using (var appPesquisaBll = new MotoristaPesquisaBusiness())
				{
					var dniExistente = appPesquisaBll.Listar(w =>
						w.DNI == dni &&
						w.ID != Motorista.ID &&
						w.IDEmpresa == this.Motorista.IDEmpresa &&
						(w.IDStatus == (int)EnumStatusMotorista.Aprovado || w.IDStatus == (int)EnumStatusMotorista.Bloqueado)).Any();
					if (dniExistente)
					{
						results.Add(new ValidationResult("Este conductor está aprobado/bloqueado, no está permitido usar este DNI hasta que sea excluido.", new string[] { "Motorista_MotoristaArgentina_DNI" }));
						return results;
					}
				}
			}

			return results;
		}

		private List<ValidationResult> ReprovarInativo(string dni)
		{
			var results = new List<ValidationResult>();

			if (!string.IsNullOrEmpty(this.Motorista.Telefone))
			{
				var valido = ValidacoesUtil.ValidaTelefone(this.Motorista.Telefone);
				if (!valido)
				{
					results.Add(new ValidationResult("Formato incorrecto", new string[] { "Motorista_Telefone" }));
					return results;
				}
			}

			if (!string.IsNullOrEmpty(this.Motorista.Email))
			{
				var valido = ValidacoesUtil.ValidaEmail(this.Motorista.Email);
				if (!valido)
				{
					results.Add(new ValidationResult("Formato incorrecto", new string[] { "Motorista_Email" }));
					return results;
				}
			}

			if (Motorista.Documentos != null)
			{
				var documentos = this.Motorista.Documentos.Any(w => w.Obrigatorio && w.DataVencimento == null && (w.DocumentoPossuiVencimento));
				if (documentos && !comRessalvas)
				{
					results.Add(new ValidationResult("Debe completar la fecha de vencimiento de todos los documentos obligatorios.", new string[] { "Documentos" }));
					return results;
				}

				if (Motorista.Documentos.Any(w => w.DataVencimento.HasValue && (w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900)))
				{
					results.Add(new ValidationResult("La fecha debe ser mayor que 1800 y menor que 2900.", new string[] { "Documentos" }));
					return results;
				}
			}

			if (this.Motorista.Operacao == "CIF" && !this.Motorista.IDTransportadora.HasValue)
			{
				results.Add(new ValidationResult("Se necesita relacionar una transportadora", new string[] { "Motorista_Transportadora" }));
				return results;
			}
			if (this.Motorista.Operacao == "FOB" && this.Motorista.IDEmpresa != 0 && (this.Motorista.Clientes == null || !this.Motorista.Clientes.Any()))
			{
				results.Add(new ValidationResult("Se necesita relacionar un cliente", new string[] { "ClienteAuto" }));
				return results;
			}

			if (string.IsNullOrEmpty(this.Motorista.Operacao))
			{
				results.Add(new ValidationResult("Debes seleccionar una operación", new string[] { "Motorista_Operacao" }));
				return results;
			}

			if (!string.IsNullOrEmpty(this.Motorista.MotoristaArgentina.DNI) && this.Motorista.IDEmpresa != 0)
			{
				var valido = ValidacoesUtil.ValidaDNI(dni);

				if (!valido)
				{
					results.Add(new ValidationResult("El formato del DNI está incorrecto", new string[] { "Motorista_MotoristaArgentina_DNI" }));
					return results;
				}

				using (var appPesquisaBll = new MotoristaPesquisaBusiness())
				{
					if (Aprovar || Reprovar)
					{
						var numDniAtual = appPesquisaBll.Selecionar(this.Motorista.ID).DNI;
						results = ValidaDniAtual(dni, numDniAtual);
						if (results.Count > 0)
						{
							return results;
						}
					}

					var dniJaUsada = appPesquisaBll.Listar(w =>
									   w.ID != Motorista.ID &&
									   w.ID != Motorista.IDMotorista &&
									   w.DNI == dni &&
									   w.IDEmpresa == this.Motorista.IDEmpresa &&
									   w.Operacao == this.Motorista.Operacao &&
									   (w.IDStatus == (int)EnumStatusMotorista.EmAprovacao || w.IDStatus == (int)EnumStatusMotorista.Reprovado));

					if (dniJaUsada != null && dniJaUsada.Any())
					{
						if (dniJaUsada.Any(w => w.IDStatus == (int)EnumStatusMotorista.Reprovado))
							results.Add(new ValidationResult("No es posible editar este conductor pues existe un conductor reprobado y relacionado con este DNI", new string[] { "Motorista_MotoristaArgentina_DNI" }));
						else if (Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
						{
							results.Add(new ValidationResult("Este conductor está en aprobación, no está permitido incluir una solicitud hasta que el mismo sea aprobado/reprobado", new string[] { "Motorista_MotoristaArgentina_DNI" }));
						}
						return results;
					}
				}

				if ((string.IsNullOrEmpty(this.Motorista.Anexo) &&
					 this.Motorista.Documentos != null &&
					 this.Motorista.Documentos.Any() &&
					 this.Motorista.Documentos.Any(p => string.IsNullOrEmpty(p.Anexo) && p.Obrigatorio)) ||
					 (Motorista.Anexo == null && Motorista.Documentos == null))
				{
					results.Add(new ValidationResult("Se necesita adjuntar los documentos obligatorios o el documento único del conductor", new string[] { "Documentos" }));
					return results;
				}
			}

			return results;
		}
		#endregion

	}
}