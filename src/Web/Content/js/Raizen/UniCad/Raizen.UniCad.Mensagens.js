$(document).ready(function () {
	//CONFIGURA OS MENUS SUSPENSOS PARA O PADRÃO
	$('.manydata').selectize();

	//monta datepicker
	$("input[data-type='date']:not(#Placa_DataNascimento):not(.dataValidade)").datepicker({
		dateFormat: 'dd/mm/yy',
		dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'],
		dayNamesMin: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S', 'D'],
		dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
		monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
		monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
		nextText: 'Próximo',
		prevText: 'Anterior'
	});
});

var raizenCoreJs;
var RaizenCoreJs;

function RaizenMensagens() {

	this.MensagemOperacao = "";
	this.ContemErros = "";
	this.ValidacoesModel = "";




	//$('input[data-ibm]')
	//        .mask('0000000000')
	//        .focusout(function () { formatIBM($(this)); });


	//$('select[data-select2-element]').select2();

	//$.validator.addMethod('date', function (value, element) {
	//    if (this.optional(element)) {
	//        return true;
	//    }
	//    var valid = true;
	//    try {
	//        $.datepicker.parseDate('dd/mm/yy', value);
	//    }
	//    catch (err) {
	//        valid = false;
	//    }
	//    return valid;
	//});

	////monta datepicker
	//$("input[data-type='date']").datepicker({
	//    dateFormat: 'dd/mm/yy',
	//    dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'],
	//    dayNamesMin: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S', 'D'],
	//    dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
	//    monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
	//    monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
	//    nextText: 'Próximo',
	//    prevText: 'Anterior'
	//});

	//$("select.select2").select2({
	//    minimumResultsForSearch: 5
	//});

	//$("input[data-currency-field]").maskMoney({
	//    showSymbol: false,
	//    symbol: "R$",
	//    decimal: ",",
	//    thousands: ".",
	//    allowNegative: true,
	//    allowZero: true,
	//    precision: $("input[data-currency-field]").data('currency-precision') || 2
	//});
}

var formatIBM = function ($campo) {
	var _valor = $campo.val();
	if (_valor == '' || _valor == 0)
		return;
	while (_valor.length < 10) {
		_valor = '0' + _valor;
	}
	$campo.val(_valor);
}

RaizenMensagens.prototype.ExibirMensagemOperacao = function ExibirMensagemOperacao() {
	if (this.MensagemOperacao.length > 0) {
		raizenCoreJs.raizenHelpers.MensagemOperacao = this.MensagemOperacao;
		raizenCoreJs.raizenHelpers.ContemErros = this.ContemErros;

		raizenCoreJs.raizenHelpers.ExibirMensagemResultadoOperacao();
	}

	if (this.ValidacoesModel.length > 0) {
		$.each(this.ValidacoesModel, function (idx, obj) {
			var controleLabel = $("#" + obj.IdControle);
			var mensagemErro = $(controleLabel).html() + ': ' + obj.MensagemValidacao;

			raizenCoreJs.raizenMensagens.EncontrarControleErro(controleLabel);

			raizenCoreJs.raizenHelpers.MensagemOperacao = mensagemErro;
			raizenCoreJs.raizenHelpers.ContemErros = "S";
			raizenCoreJs.raizenHelpers.ExibirMensagemResultadoOperacao();
		});
	}
};

RaizenMensagens.prototype.EncontrarControleErro = function EncontrarControleErro(idControle) {
	$(idControle).parents('div').each(function () {
		if ($(this).hasClass('form-group1')) {
			$(this).removeClass('form-group1');
			$(this).addClass('form-group1 has-error');
			return;
		}
	});

	$(idControle).parents('div').each(function () {
		if ($(this).hasClass('form-group')) {
			$(this).removeClass('form-group');
			$(this).addClass('form-group  has-error');
			return;
		}
	});
};

RaizenCoreJs.prototype.raizenMensagens = new RaizenMensagens();