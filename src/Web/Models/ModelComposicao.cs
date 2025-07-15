using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using System.Text.RegularExpressions;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.BLL.Util;

namespace Raizen.UniCad.Web.Models
{
    public class ModelComposicao : BaseModel, IValidatableObject
    {

        #region Constantes
        public ComposicaoFiltro Filtro { get; set; }
        public Composicao Composicao { get; set; }
        public List<ComposicaoView> ListaComposicao { get; set; }
        public bool Aprovar { get; set; }
        public bool Reprovar { get; set; }
        public bool comRessalvas { get; set; }
        public string UsuarioPerfil { get; set; }
        public string isArrendamento { get; set; }
        public int IdPais { get; set; }

        private EnumPais _pais { get; set; }

        #endregion


        #region Validação de Integridade

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (IdPais)
            {
                case (int)EnumPais.Argentina:
                    _pais = EnumPais.Argentina;
                    break;

                default:
                case (int)EnumPais.Brasil:
                    _pais = EnumPais.Brasil;
                    break;
            }

            return Validate();
        }

        private IEnumerable<ValidationResult> Validate()
        {
            var results = new List<ValidationResult>();
            int idCliente = 0;

            var composicaoBrasilBll = new ComposicaoBusiness();
            var composicaoArgentinaBll = new ComposicaoBusiness(EnumPais.Argentina);

            var placaBll = new PlacaBusiness();
            var placaArgentinaBll = new PlacaBusiness(EnumPais.Argentina);

            if (this.Composicao != null && (Aprovar || Reprovar))
            {
                var idStatus = composicaoBrasilBll.Selecionar(Composicao.ID).IDStatus;
                if (idStatus == (int)EnumStatusComposicao.Aprovado || idStatus == (int)EnumStatusComposicao.Reprovado)
                {
                    results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Essa solicitação já foi aprovada/reprovada por outro usuário! A ação não poderá ser efetuada.", "¡Esa solicitud ya fue aprobada/reprobada por otro usuario!. Esta acción no puede ser ejecutada", _pais), new string[] { "Composicao" }));
                    return results;
                }

                if (_pais == EnumPais.Argentina)
                {
                    string cnpjTransportadora;

                    var pplaca1 = placaArgentinaBll.Selecionar(p => p.ID == this.Composicao.IDPlaca1);

                    var pplaca2 = placaArgentinaBll.Selecionar(p => p.ID == this.Composicao.IDPlaca2);

                    if (this.Composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis || this.Composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
                    {
                        cnpjTransportadora = composicaoArgentinaBll.RecuperaCnpjCuit(pplaca2, pplaca1);
                    }
                    else
                    {
                        cnpjTransportadora = pplaca1?.PlacaArgentina.CUIT;
                    }

                    var ptransp = new TransportadoraBusiness(EnumPais.Argentina).Selecionar(w => w.CNPJCPF == cnpjTransportadora);

                    if (ptransp == null)
                    {
                        results.Add(new ValidationResult("¡Transportista no encontrado! Por favor regístralo en el SAP", new string[] { "Composicao_CPFCNPJ" }));
                        return results;
                    }
                }
            }

            if (this.Composicao != null && !Reprovar)
            {
                if (this.Composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
                    this.Composicao.Operacao = "CIF";

                if (_pais == EnumPais.Argentina && this.Composicao.IDEmpresa == 0)
                {
                    results.Add(new ValidationResult("Campo Requerido!", new string[] { "Composicao_IDEmpresa" }));
                    return results;
                }

                if (string.IsNullOrEmpty(this.Composicao.Operacao))
                {
                    results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Campo Obrigatório!", "Campo Requerido!", _pais), new string[] { "Composicao_Operacao" }));
                    return results;
                }

                Regex regex = new Regex(@"^\w+$");
                if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    this.Composicao.ID = int.Parse(this.ChavePrimaria);

                if (this.Composicao.Placa1 != null && !regex.Match(this.Composicao.Placa1).Success)
                {
                    results.Add(new ValidationResult(Traducao.GetTextoPorLingua("O formato da Placa está inválido.", "El formato de la tarjeta no es válido.", _pais), new string[] { "Composicao_Placa1" }));
                    return results;
                }
                else if (placaBll.Selecionar(p => p.PlacaVeiculo == this.Composicao.Placa1) == null)
                {
                    results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Placa não cadastrada no Sistema. Selecione + para incluir.", "Tarjeta no registrada en el sistema. Seleccione + para incluir.", _pais), new string[] { "Composicao_Placa1" }));
                    return results;
                }

                if (this.Composicao.Placa2 != null)
                {
                    if (!regex.Match(this.Composicao.Placa2).Success)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("O formato da Placa está inválido.", "El formato de la tarjeta no es válido.", _pais), new string[] { "Composicao_Placa2" }));
                        return results;
                    }
                    else if (placaBll.Selecionar(p => p.PlacaVeiculo == this.Composicao.Placa2) == null)
                    {

                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Placa não cadastrada no Sistema. Selecione + para incluir.", "Tarjeta no registrada en el sistema. Seleccione + para incluir.", _pais), new string[] { "Composicao_Placa2" }));
                        return results;
                    }
                }



                if (this.Composicao.Placa3 != null)
                {
                    if (!regex.Match(this.Composicao.Placa3).Success)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("O formato da Placa está inválido.", "El formato de la tarjeta no es válido.", _pais), new string[] { "Composicao_Placa3" }));
                        return results;
                    }
                    else if (placaBll.Selecionar(p => p.PlacaVeiculo == this.Composicao.Placa3) == null)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Placa não cadastrada no Sistema. Selecione + para incluir.", "Tarjeta no registrada en el sistema. Seleccione + para incluir.", _pais), new string[] { "Composicao_Placa3" }));
                        return results;
                    }
                }


                if (this.Composicao.Placa4 != null)
                {
                    if (!regex.Match(this.Composicao.Placa4).Success)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("O formato da Placa está inválido.", "El formato de la tarjeta no es válido."), new string[] { "Composicao_Placa4" }));
                        return results;
                    }
                    else if (placaBll.Selecionar(p => p.PlacaVeiculo == this.Composicao.Placa4) == null)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Placa não cadastrada no Sistema. Selecione + para incluir.", "Tarjeta no registrada en el sistema. Seleccione + para incluir.", _pais), new string[] { "Composicao_Placa4" }));
                        return results;
                    }
                }

                if (this.Composicao.IDPlaca1.HasValue)
                {
                    if (_pais == EnumPais.Brasil)
                    {
                        var composicaoAguardando = placaBll.ListarPorStatus(this.Composicao.IDPlaca1.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaBll.ListarPorStatus(this.Composicao.IDPlaca1.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if (composicaoAprovado || composicaoAguardando)
                        {
                            results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Esta placa está em aprovação, não é permitido incluir em uma composição até que a mesma seja aprovada/reprovada.", "Esta tarjeta está aprobada, no está permitida su inclusión en una composición hasta que se apruebe / desapruebe.", _pais), new string[] { "Composicao_Placa1" }));
                            return results;
                        }
                    }
                    else
                    {
                        var composicaoAguardando = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca1.Value, null, this.Composicao.Placa1, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca1.Value, null, this.Composicao.Placa1, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if ((composicaoAguardando || composicaoAprovado) && !Aprovar)
                        {
                            results.Add(new ValidationResult("Esta patente está en aprobación, no está permitido incluir una nueva solicitud hasta que la misma sea aprobada/reprobada.", new string[] { "Composicao_Placa1" }));
                            return results;
                        }
                    }

                    // Chama o método uma única vez e armazena o resultado
                    var documentos = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(Composicao.IDPlaca1.Value);

                    // Verifica se existem documentos obrigatórios sem data de vencimento
                    bool documentosObrigatoriosSemVencimento = documentos.Any(w => w.Obrigatorio && w.DataVencimento == null && w.DocumentoPossuiVencimento);

                    // Verifica se a data de vencimento é inválida
                    bool dataVencimentoInvalida = documentos.Any(w => w.DataVencimento.HasValue && (w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900));

                    // Juntando as duas validações em um único if
                    if (this.Aprovar && !comRessalvas && documentosObrigatoriosSemVencimento)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário preencher a data de vencimento de todos os documentos obrigatórios.", "Debe completar la fecha de vencimiento de todos los documentos obligatórios.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }

                    // Checando a segunda condição
                    if (Composicao.IDPlaca1.HasValue && dataVencimentoInvalida)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("A Data deve ser maior que 1800 e menor que 2900.", "La fecha debe ser mayor que 1800 y menor que 2900.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }

                    var placa1 = placaBll.Selecionar(this.Composicao.IDPlaca1.Value);
                    placa1.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placa1.ID);

                    if (this.Composicao.Operacao == "CIF" && (placa1.IDTransportadora == null))
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora a placa.", "Un portador debe estar asociado con la placa.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }

                    if (this.Composicao.Operacao == "CIF" && this.Composicao.IDEmpresa == (int)EnumEmpresa.Ambos && placa1.IDTransportadora2 == null)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora EAB a placa.", "Un proveedor de EAB debe estar asociado con la tarjeta.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }
                }

                if (this.Composicao.IDPlaca2.HasValue)
                {
                    if (_pais == EnumPais.Brasil)
                    {
                        var composicaoAguardando = placaBll.ListarPorStatus(this.Composicao.IDPlaca2.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaBll.ListarPorStatus(this.Composicao.IDPlaca2.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if (composicaoAprovado || composicaoAguardando)
                        {
                            results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Esta placa está em aprovação, não é permitido incluir em uma composição até que a mesma seja aprovada/reprovada.", "Esta tarjeta está aprobada, no está permitida su inclusión en una composición hasta que se apruebe / desapruebe.", _pais), new string[] { "Composicao_Placa2" }));
                            return results;
                        }
                    }
                    else
                    {
                        var composicaoAguardando = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca2.Value, null, this.Composicao.Placa2, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca2.Value, null, this.Composicao.Placa2, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if ((composicaoAguardando || composicaoAprovado) && !Aprovar)
                        {
                            results.Add(new ValidationResult("Esta patente está en aprobación, no está permitido incluir una nueva solicitud hasta que la misma sea aprobada/reprobada.", new string[] { "Composicao_Placa2" }));
                            return results;
                        }
                    }

                    var documentos = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(Composicao.IDPlaca2.Value);

                    bool documentosObrigatoriosSemVencimento = documentos.Any(w => w.Obrigatorio && w.DataVencimento == null && w.DocumentoPossuiVencimento);

                    bool dataVencimentoInvalida = documentos.Any(w => w.DataVencimento.HasValue && (w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900));

                    if (this.Aprovar && !comRessalvas && documentosObrigatoriosSemVencimento)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário preencher a data de vencimento de todos os documentos obrigatórios.", "Debe completar la fecha de vencimiento de todos los documentos obligatórios.", _pais), new string[] { "Composicao_Placa2" }));
                        return results;
                    }

                    if (Composicao.IDPlaca2.HasValue && dataVencimentoInvalida)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("A Data deve ser maior que 1800 e menor que 2900.", "La fecha debe ser mayor que 1800 y menor que 2900.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }

                    var placa2 = placaBll.Selecionar(this.Composicao.IDPlaca2.Value);
                    placa2.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placa2.ID);

                    if (this.Composicao.Operacao == "CIF" && (placa2.IDTransportadora == null))
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora a placa.", "Un portador debe estar asociado con la placa.", _pais), new string[] { "Composicao_Placa2" }));
                        return results;
                    }

                    if (this.Composicao.Operacao == "CIF" && this.Composicao.IDEmpresa == (int)EnumEmpresa.Ambos && placa2.IDTransportadora2 == null)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora EAB a placa.", "Un proveedor de EAB debe estar asociado con la tarjeta.", _pais), new string[] { "Composicao_Placa2" }));
                        return results;
                    }
                }

                if (this.Composicao.IDPlaca3.HasValue)
                {
                    if (_pais == EnumPais.Brasil)
                    {
                        var composicaoAguardando = placaBll.ListarPorStatus(this.Composicao.IDPlaca3.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaBll.ListarPorStatus(this.Composicao.IDPlaca3.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if (composicaoAprovado || composicaoAguardando)
                        {
                            results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Esta placa está em aprovação, não é permitido incluir em uma composição até que a mesma seja aprovada/reprovada.", "Esta tarjeta está aprobada, no está permitida su inclusión en una composición hasta que se apruebe / desapruebe.", _pais), new string[] { "Composicao_Placa3" }));
                            return results;
                        }
                    }
                    else
                    {
                        var composicaoAguardando = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca3.Value, null, this.Composicao.Placa3, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca3.Value, null, this.Composicao.Placa3, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if ((composicaoAguardando || composicaoAprovado) && !Aprovar)
                        {
                            results.Add(new ValidationResult("Esta patente está en aprobación, no está permitido incluir una nueva solicitud hasta que la misma sea aprobada/reprobada.", new string[] { "Composicao_Placa3" }));
                            return results;
                        }
                    }

                    var documentos = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(Composicao.IDPlaca3.Value);

                    bool documentosObrigatoriosSemVencimento = documentos.Any(w => w.Obrigatorio && w.DataVencimento == null && w.DocumentoPossuiVencimento);

                    bool dataVencimentoInvalida = documentos.Any(w => w.DataVencimento.HasValue && (w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900));

                    if (this.Aprovar && !comRessalvas && documentosObrigatoriosSemVencimento)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário preencher a data de vencimento de todos os documentos obrigatórios.", "Debe completar la fecha de vencimiento de todos los documentos obligatórios.", _pais), new string[] { "Composicao_Placa3" }));
                        return results;
                    }

                    if (Composicao.IDPlaca3.HasValue && dataVencimentoInvalida)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("A Data deve ser maior que 1800 e menor que 2900.", "La fecha debe ser mayor que 1800 y menor que 2900.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }

                    var Placa3 = placaBll.Selecionar(this.Composicao.IDPlaca3.Value);
                    Placa3.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(Placa3.ID);

                    if (this.Composicao.Operacao == "CIF" && (Placa3.IDTransportadora == null))
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora a placa.", "Un portador debe estar asociado con la placa.", _pais), new string[] { "Composicao_Placa3" }));
                        return results;
                    }

                    if (this.Composicao.Operacao == "CIF" && this.Composicao.IDEmpresa == (int)EnumEmpresa.Ambos && Placa3.IDTransportadora2 == null)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora EAB a placa.", "Un proveedor de EAB debe estar asociado con la tarjeta.", _pais), new string[] { "Composicao_Placa3" }));
                        return results;
                    }
                }

                if (this.Composicao.IDPlaca4.HasValue)
                {
                    if (_pais == EnumPais.Brasil)
                    {
                        var composicaoAguardando = placaBll.ListarPorStatus(this.Composicao.IDPlaca4.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaBll.ListarPorStatus(this.Composicao.IDPlaca4.Value, this.ChavePrimaria == "0" ? this.Composicao.IDComposicao?.ToString() : this.ChavePrimaria, null, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if (composicaoAprovado || composicaoAguardando)
                        {
                            results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Esta placa está em aprovação, não é permitido incluir em uma composição até que a mesma seja aprovada/reprovada.", "Esta tarjeta está aprobada, no está permitida su inclusión en una composición hasta que se apruebe / desapruebe.", _pais), new string[] { "Composicao_Placa4" }));
                            return results;
                        }
                    }
                    else
                    {
                        var composicaoAguardando = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca4.Value, null, this.Composicao.Placa3, EnumStatusComposicao.AguardandoAtualizacaoSAP, this.Composicao.Operacao);
                        var composicaoAprovado = placaArgentinaBll.ListarPorStatus(this.Composicao.IDPlaca4.Value, null, this.Composicao.Placa3, EnumStatusComposicao.EmAprovacao, this.Composicao.Operacao);

                        if ((composicaoAguardando || composicaoAprovado) && !Aprovar)
                        {
                            results.Add(new ValidationResult("Esta patente está en aprobación, no está permitido incluir una nueva solicitud hasta que la misma sea aprobada/reprobada.", new string[] { "Composicao_Placa4" }));
                            return results;
                        }
                    }

                    var documentos = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(Composicao.IDPlaca4.Value);

                    bool documentosObrigatoriosSemVencimento = documentos.Any(w => w.Obrigatorio && w.DataVencimento == null && w.DocumentoPossuiVencimento);

                    bool dataVencimentoInvalida = documentos.Any(w => w.DataVencimento.HasValue && (w.DataVencimento.Value.Year <= 1800 || w.DataVencimento.Value.Year >= 2900));

                    if (this.Aprovar && !comRessalvas && documentosObrigatoriosSemVencimento)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário preencher a data de vencimento de todos os documentos obrigatórios.", "Debe completar la fecha de vencimiento de todos los documentos obligatórios.", _pais), new string[] { "Composicao_Placa4" }));
                        return results;
                    }

                    if (Composicao.IDPlaca4.HasValue && dataVencimentoInvalida)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("A Data deve ser maior que 1800 e menor que 2900.", "La fecha debe ser mayor que 1800 y menor que 2900.", _pais), new string[] { "Composicao_Placa1" }));
                        return results;
                    }

                    var Placa4 = placaBll.Selecionar(this.Composicao.IDPlaca4.Value);
                    Placa4.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(Placa4.ID);

                    if (this.Composicao.Operacao == "CIF" && (Placa4.IDTransportadora == null))
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora a placa.", "Un portador debe estar asociado con la placa.", _pais), new string[] { "Composicao_Placa4" }));
                        return results;
                    }

                    if (this.Composicao.Operacao == "CIF" && this.Composicao.IDEmpresa == (int)EnumEmpresa.Ambos && Placa4.IDTransportadora2 == null)
                    {
                        results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário associar uma transportadora EAB a placa.", "Un proveedor de EAB debe estar asociado con la tarjeta.", _pais), new string[] { "Composicao_Placa4" }));
                        return results;
                    }
                }

                Composicao.CPFCNPJ = SetarTrasnportadoraComposicao(_pais, placaBll, placaArgentinaBll);

                if (this.Aprovar && !string.IsNullOrEmpty(this.Composicao.CPFCNPJ) && this.Composicao.Operacao == "FOB" && this.Composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                {
                    var transpBll = new TransportadoraBusiness();
                    Transportadora transp = null;
                    var cnpj = string.Empty;
                    if (!string.IsNullOrEmpty(this.Composicao.CPFCNPJArrendamento))
                    {
                        cnpj = this.Composicao.CPFCNPJArrendamento.RemoveCharacter();
                        transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB) && (_pais != EnumPais.Brasil || p.IBM.StartsWith("T")));

                        if (transp == null)
                        {
                            transpBll.Importar(DateTime.Now.AddDays(-1), (EnumEmpresa)this.Composicao.IDEmpresa, null, cnpj);
                            transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB && (_pais != EnumPais.Brasil || p.IBM.StartsWith("T"))));
                            if (transp == null)
                            {
                                results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Transportador do arrendamento não encontrado! Favor cadastrá-lo no SAP", "Transportista de alquiler no encontrado! Por favor regístralo con SAP", _pais), new string[] { "Composicao_CPFCNPJ" }));
                                return results;
                            }
                        }
                    }
                    else
                    {
                        cnpj = this.Composicao.CPFCNPJ.RemoveCharacter();
                        transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB && (_pais != EnumPais.Brasil || p.IBM.StartsWith("T"))));
                        if (transp == null)
                        {
                            transpBll.Importar(DateTime.Now.AddDays(-1), (EnumEmpresa)this.Composicao.IDEmpresa, null, cnpj);
                            transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB && (_pais != EnumPais.Brasil || p.IBM.StartsWith("T"))));
                            if (transp == null)
                            {
                                results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Transportador não encontrado! Favor cadastrá-lo no SAP", "Transportador no encontrado! Por favor regístralo con SAP", _pais), new string[] { "Composicao_CPFCNPJ" }));
                                return results;
                            }
                        }
                    }
                }

                if (this.Aprovar && !string.IsNullOrEmpty(this.Composicao.CPFCNPJ) && this.Composicao.Operacao == "FOB")
                {
                    var transpBll = new TransportadoraBusiness();
                    Transportadora transp = null;
                    var cnpj = string.Empty;

                    if (!string.IsNullOrEmpty(this.Composicao.CPFCNPJArrendamento))
                    {
                        cnpj = this.Composicao.CPFCNPJArrendamento.RemoveCharacter();
                        transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB));
                        if (transp == null)
                        {
                            transpBll.Importar(DateTime.Now.AddDays(-1), (EnumEmpresa)this.Composicao.IDEmpresa, null, cnpj);
                            transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB));
                            if (transp == null)
                            {
                                results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Transportador do arrendamento não encontrado! Favor cadastrá-lo no SAP", "Transportista de alquiler no encontrado! Por favor regístralo con SAP", _pais), new string[] { "Composicao_CPFCNPJ" }));
                                return results;
                            }
                        }
                    }
                    else
                    {
                        cnpj = this.Composicao.CPFCNPJ.RemoveCharacter();
                        transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB));
                        if (transp == null)
                        {
                            transpBll.Importar(DateTime.Now.AddDays(-1), (EnumEmpresa)this.Composicao.IDEmpresa, null, cnpj);
                            transp = transpBll.Selecionar(p => p.CNPJCPF == cnpj && p.IDEmpresa == this.Composicao.IDEmpresa && (p.Operacao == "FOB" || this.Composicao.IDEmpresa == (int)EnumEmpresa.EAB));
                            if (transp == null)
                            {
                                results.Add(new ValidationResult(Traducao.GetTextoPorLingua("Transportador não encontrado! Favor cadastrá-lo no SAP", "Transportador no encontrado! Por favor regístralo con SAP", _pais), new string[] { "Composicao_CPFCNPJ" }));
                                return results;
                            }
                        }
                    }
                }

                if (this.Aprovar && Convert.ToDecimal(this.Composicao.TaraComposicao) == 0)
                {
                    results.Add(new ValidationResult(Traducao.GetTextoPorLingua("É necessário informar a Tara.", "Es necesario informar a Tara.", _pais), new string[] { "Composicao_TaraComposicao" }));
                    return results;
                }

                if (this.Aprovar && _pais == EnumPais.Brasil && this.Composicao.PBTC.HasValue && this.Composicao.TaraComposicao > this.Composicao.PBTC)
                {
                    results.Add(new ValidationResult("A tara não pode ser maior que o PBTC.", new string[] { "Composicao_TaraComposicao" }));
                    return results;
                }

                var erroPlaca = _pais == EnumPais.Brasil ? "Placa não informada/inválida!" : "Tarjeta no informada / inválida!";


                if ((!this.Composicao.IDPlaca1.HasValue || this.Composicao.IDPlaca1 == 0) && this.Composicao.IDTipoComposicao != 10)
                    results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa1" }));

                if (_pais == EnumPais.Argentina)
                {
                    if ((this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.SemirremolqueChico ||
                        this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.SemirremolqueGrande ||
                        this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.Escalado) && (!this.Composicao.IDPlaca2.HasValue ||
                        this.Composicao.IDPlaca2 == 0))

                        results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa2" }));

                    if (this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.BitrenChico || this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.BitrenGrande)
                    {
                        if (!this.Composicao.IDPlaca2.HasValue || this.Composicao.IDPlaca2 == 0)
                            results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa2" }));

                        if (!this.Composicao.IDPlaca3.HasValue || this.Composicao.IDPlaca3 == 0)
                            results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa3" }));
                    }
                }

                else if (_pais == EnumPais.Brasil)
                {
                    if (this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.Carreta && (!this.Composicao.IDPlaca2.HasValue || this.Composicao.IDPlaca2 == 0))
                        results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa2" }));

                    if (this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.Bitrem && (!this.Composicao.IDPlaca3.HasValue || this.Composicao.IDPlaca3 == 0))
                        results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa3" }));

                    if (this.Composicao.IDTipoComposicao == (int)EnumTipoComposicao.BitremDolly && (!this.Composicao.IDPlaca4.HasValue || this.Composicao.IDPlaca1 == 0))
                        results.Add(new ValidationResult(erroPlaca, new string[] { "Composicao_Placa4" }));
                }

            }

            return results;

        }

        private string SetarTrasnportadoraComposicao(EnumPais pais, PlacaBusiness placaBrasilBll, PlacaBusiness placaArgentinaBll)
        {
            Placa placa1, placa2;
            var composicaoBll = new ComposicaoBusiness(pais);

            if (pais == EnumPais.Brasil)
            {
                placa1 = placaBrasilBll.ObtemPlaca1(Composicao);
                placa2 = placaBrasilBll.ObtemPlaca2(Composicao);

                if (Composicao.CPFCNPJ == null)
                {
                    if (Composicao.Operacao == "FOB")
                        return composicaoBll.RecuperaCnpjCuit(placa2, placa1);

                    return placa1.PlacaBrasil.CPFCNPJ;
                }

                return Composicao.CPFCNPJ;
            }

            //Se for Argentina
            placa1 = placaArgentinaBll.ObtemPlaca1(Composicao);
            placa2 = placaArgentinaBll.ObtemPlaca2(Composicao);

            if (Composicao.Operacao == "FOB")
                return composicaoBll.RecuperaCnpjCuit(placa2, placa1);

            return (placa1 != null ? placa1.PlacaArgentina.CUIT : placa2.PlacaArgentina.CUIT);
        }
    }

    #endregion
}
