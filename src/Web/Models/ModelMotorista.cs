using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.BLL;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Utils;

namespace Raizen.UniCad.Web.Models
{
	public class ModelMotorista : BaseModel, IValidatableObject
	{
		#region Constantes
		public MotoristaFiltro Filtro { get; set; }
		public Motorista Motorista { get; set; }
		public List<TerminalTreinamentoView> ListaTerminais { get; set; }
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
		public List<string> Cnpjs { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();
			if (this.TreinamentoView != null)
			{
				if (!string.IsNullOrWhiteSpace(this.TreinamentoView.Anexo) && !this.TreinamentoView.dataValidade.HasValue)
				{
					results.Add(new ValidationResult("É necessário preencher a data de vencimento do treinamento teórico.", new string[] { "TreinamentoView_dataValidade" }));
					return results;
				}

				if (string.IsNullOrWhiteSpace(this.TreinamentoView.Anexo) && this.TreinamentoView.dataValidade.HasValue)
				{
					results.Add(new ValidationResult("É necessário anexar o documento do treinamento teórico.", new string[] { "TreinamentoView_Anexo" }));
					return results;
				}

				if (this.TreinamentoView.dataValidade.HasValue && !ValidacoesUtil.ValidarRangeData(this.TreinamentoView.dataValidade.Value.Year))
				{
					results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "TreinamentoView_dataValidade" }));
					return results;
				}

				//if (!string.IsNullOrWhiteSpace(this.TreinamentoView.Anexo) && this.TreinamentoView.dataValidade.HasValue && string.IsNullOrWhiteSpace(this.TreinamentoView.Justificativa))
				//{
				//    results.Add(new ValidationResult("Campo obrigatório.", new string[] { "TreinamentoView_Justificativa" }));
				//    return results;
				//}

				var treinamentoTeorico = new HistoricoTreinamentoTeoricoMotoristaBusiness().Existe(p => p.IDMotorista == this.ID);

				if (!treinamentoTeorico && string.IsNullOrWhiteSpace(this.TreinamentoView.Anexo) && !this.TreinamentoView.dataValidade.HasValue && this.ListaTerminais != null && this.ListaTerminais.Any())
				{
					results.Add(new ValidationResult("Não é permitido inserir um novo treinamento prático sem informações de treinamento teórico.", new string[] { "TreinamentoTeorico" }));
					return results;
				}

				if (this.ListaTerminais != null && this.ListaTerminais.Any(w => w.dataValidade == null))
				{
					results.Add(new ValidationResult("Todas as datas de validade do treinamento prático devem ser preenchidas.", new string[] { "TreinamentoPratico" }));
					return results;
				}

				if (this.ListaTerminais != null && this.ListaTerminais.Any(w => w.dataValidade.Value.Year <= 1800 || w.dataValidade.Value.Year >= 2900))
				{
					results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "TreinamentoPratico" }));
					return results;
				}
			}

			if (this.Motorista != null)
			{
				MotoristaBusiness appBll = new MotoristaBusiness();
				MotoristaPesquisaBusiness appPesquisaBll = new MotoristaPesquisaBusiness();
				var cpf = this.Motorista.MotoristaBrasil.CPF.RemoveCharacter();



                if (Motorista.MotoristaBrasil != null)
                {
                    bool telefoneEmailOk = true;
                    if (!string.IsNullOrEmpty(this.Motorista.Telefone))
                    {
                        var valido = ValidacoesUtil.ValidaTelefone(this.Motorista.Telefone);
                        if (!valido)
                        {
                            results.Add(new ValidationResult("Formato incorreto.", new string[] { "Motorista_Telefone" }));
                            telefoneEmailOk = false;
                        }
                    }
                    else
                    {
                        results.Add(new ValidationResult("Campo obrigatório.", new string[] { "Motorista_Telefone" }));
                        telefoneEmailOk = false;
                    }

                    if (!string.IsNullOrEmpty(this.Motorista.Email))
                    {
                        var valido = ValidacoesUtil.ValidaEmail(this.Motorista.Email);
                        if (!valido)
                        {
                            results.Add(new ValidationResult("Formato incorreto.", new string[] { "Motorista_Email" }));
                            telefoneEmailOk = false;
                        }
                    }
                    else
                    {
                        results.Add(new ValidationResult("Campo obrigatório.", new string[] { "Motorista_Email" }));
                        telefoneEmailOk = false;
                    }

                    if (!telefoneEmailOk)
                    {
                        return results;
                    }
                }


                if (Aprovar || Reprovar)
				{
					var idStatus = new MotoristaBusiness().Selecionar(Motorista.ID).IDStatus;
					if (idStatus == (int)EnumStatusMotorista.Aprovado || idStatus == (int)EnumStatusMotorista.Reprovado)
					{
						results.Add(new ValidationResult("Essa solicitação já foi aprovada/reprovada por outro usuário! A ação não poderá ser efetuada.", new string[] { "Motorista" }));
						return results;
					}
				}

				if (Reprovar == false)
				{

					if (Motorista.MotoristaBrasil.Nascimento.HasValue && !ValidacoesUtil.ValidarRangeData(Motorista.MotoristaBrasil.Nascimento.Value.Year))
					{
						results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900 .", new string[] { "Motorista_Nascimento" }));
						return results;
					}

					if (Motorista.Documentos != null && this.Aprovar)
					{
						var documentos = this.Motorista.Documentos.Any(w => w.Obrigatorio && !w.DataVencimento.HasValue && (w.DocumentoPossuiVencimento));
						if (this.Aprovar && documentos && !comRessalvas)
						{
							results.Add(new ValidationResult("É necessário preencher a data de vencimento de todos os documentos obrigatórios.", new string[] { "Documentos" }));
							return results;
						}

						if (Motorista.Documentos.Any(w => w.DataVencimento.HasValue ? w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900 : false))
						{
							results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900 .", new string[] { "Documentos" }));
							return results;
						}
					}

					if (this.Motorista.Operacao == "CIF" && !this.Motorista.IDTransportadora.HasValue)
					{
						results.Add(new ValidationResult("É necessário associar uma transportadora.", new string[] { "Motorista_Transportadora" }));
						return results;
					}
					if (this.Motorista.Operacao == "FOB" && this.Motorista.IDEmpresa != 0 && (this.Motorista.Clientes == null || !this.Motorista.Clientes.Any()))
					{
						results.Add(new ValidationResult("É necessário associar um cliente.", new string[] { "ClienteAuto" }));
						return results;
					}

					if (!string.IsNullOrEmpty(this.Motorista.MotoristaBrasil.CPF) && this.Motorista.IDEmpresa != 0)
					{
						var valido = ValidacoesUtil.ValidaCPF(cpf);

						if (!valido)
						{
							results.Add(new ValidationResult("O formato do CPF está inválido.", new string[] { "Motorista_CPF" }));
							return results;
						}


						if (Aprovar || Reprovar)
						{
							//verifica se o cpf mudou
							var numCpfAtual = appBll.Selecionar(moto => moto.ID == this.Motorista.ID).MotoristaBrasil.CPF;
							if (numCpfAtual != cpf)
							{
								var cpfExistente = appBll.Listar(w => w.MotoristaBrasil.CPF == cpf && w.ID != Motorista.ID && w.IDEmpresa == this.Motorista.IDEmpresa && (w.IDStatus == (int)EnumStatusMotorista.Aprovado || w.IDStatus == (int)EnumStatusMotorista.Bloqueado)).Any();
								if (cpfExistente)
								{
									results.Add(new ValidationResult("Esse motorista está aprovado/bloqueado, não é permitido utilizar esse CPF até que o mesmo seja excluído.", new string[] { "Motorista_CPF" }));
									return results;
								}
							}
						}

						var cpfJaUsada = appPesquisaBll.Listar(w =>
												w.ID != Motorista.ID &&
												w.ID != Motorista.IDMotorista &&
												w.CPF == cpf &&
												w.IDEmpresa == this.Motorista.IDEmpresa &&
												w.Operacao == this.Motorista.Operacao &&
												(w.IDStatus == (int)EnumStatusMotorista.EmAprovacao || w.IDStatus == (int)EnumStatusMotorista.Reprovado));
						if (cpfJaUsada != null && cpfJaUsada.Any())
						{
							if (cpfJaUsada.Any(w => w.IDStatus == (int)EnumStatusMotorista.Reprovado))
								results.Add(new ValidationResult("Não é possível realizar a edição deste motorista pois já existe motorista reprovado com esse CPF.", new string[] { "Motorista_CPF" }));
							else if (Motorista.IDStatus != (int)EnumStatusMotorista.Reprovado)
							{
								results.Add(new ValidationResult("Esse motorista está em aprovação, não é permitido incluir uma solicitação até que o mesmo seja aprovado/reprovado.", new string[] { "Motorista_CPF" }));
							}
							return results;
						}

						if ((string.IsNullOrEmpty(this.Motorista.Anexo) &&
							 this.Motorista.Documentos != null &&
							 this.Motorista.Documentos.Any() &&
							 this.Motorista.Documentos.Any(p => string.IsNullOrEmpty(p.Anexo) && p.Obrigatorio)) ||
							 (Motorista.Anexo == null && Motorista.Documentos == null))
						{
							results.Add(new ValidationResult("É necessário anexar os documentos obrigatórios ou o documento único do motorista.", new string[] { "Documentos" }));
							return results;
						}
					}
				}
				else if (Reprovar)
				{
					//verifica se o cpf mudou
					var numCpfAtual = appBll.Selecionar(moto => moto.ID == this.Motorista.ID).MotoristaBrasil.CPF;
					if (numCpfAtual != cpf)
					{
						var cpfExistente = appPesquisaBll.Listar(w => w.CPF == cpf && w.ID != Motorista.ID && w.IDEmpresa == this.Motorista.IDEmpresa && (w.IDStatus == (int)EnumStatusMotorista.Aprovado || w.IDStatus == (int)EnumStatusMotorista.Bloqueado)).Any();
						if (cpfExistente)
						{
							results.Add(new ValidationResult("Esse motorista está aprovado/bloqueado, não é permitido utilizar esse CPF até que o mesmo seja excluído.", new string[] { "Motorista_CPF" }));
							return results;
						}
					}
				}
			}

			return results;
		}
		#endregion

	}
}