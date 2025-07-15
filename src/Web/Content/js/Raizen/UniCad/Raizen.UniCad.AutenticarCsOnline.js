function AutenticarCsOnline() {
	// Inicializador de variáveis
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var RaizenHelpers;
var ShowMessageSucess;
var ShowErrorMenssage;

AutenticarCsOnline.prototype.ValidarEmail = function ValidarEmail(idPais) {
	var regex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	if ($('#Email').val() == '') {
		idPais === 1 ? ShowMessage('O Email é obrigatório!') : ShowMessage('Se requiere correo electrónico!');

		return false;
	}
	if (!regex.test($('#Email').val())) {
		idPais === 1 ? ShowErrorMenssage('E-mail inválido!') : ShowErrorMenssage('Email inválido!');

		return false;
	}

	return true;
}

RaizenCoreJs.prototype.AutenticarCsOnline = new AutenticarCsOnline();

