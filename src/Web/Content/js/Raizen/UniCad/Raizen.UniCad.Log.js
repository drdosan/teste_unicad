function Log() {
    this.urlDetalharLog = '';
}

var raizenCoreJs;
var RaizenCoreJs;
Log.prototype.Detalhar = function Detalhar(id) {
    $.get(this.urlDetalharLog,
    {
        "IdLog": id
    },
    function (data) {
        $("#containerLog").html(data);
        $("#ModalLog").modal('show');
    });
};

RaizenCoreJs.prototype.Log = new Log();

function keypress(obj, event) {
    if (event.keyCode == 13) {
        raizenCoreJs.raizenCRUD.RealizarPesquisa();
        event.preventDefault();
        return false;
    }
}