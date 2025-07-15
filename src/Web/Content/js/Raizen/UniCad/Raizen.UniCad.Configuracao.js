function Configuracao() {
    this.urlCarregarJobs = '';
    this.urlExecutarJob = '';
    this.urlSalvarJob = '';
    this.urlPesquisarLog = '';
}

var raizenCoreJs;
var RaizenCoreJs;
var ShowMessage;
var ShowErrorMenssage;
RaizenCoreJs.prototype.Configuracao = new Configuracao();

Configuracao.prototype.handleJqgrid = function handleJqgrid() {
    $("#tableJobGrid").jqGrid({
        url: this.urlCarregarJobs,
        mtype: 'Get',
        datatype: "json",
        colNames: ['ID', 'Nome', 'Periodicidade(Minutos)', 'Data Início', 'Ultima Execução', 'Em Execucao', 'Ativo', 'Operações'],
        colModel: [
            {
                label: "ID",
                name: 'ID',
                width: '10%',
            },
            {
                label: "Nome",
                name: 'Nome',
                width: '25%',
                editable: true,
                editoptions: { disabled: true }
            },
            {
                label: "NrPeriodicidadeMinutos",
                name: 'NrPeriodicidadeMinutos',
                width: '15%',
                editable: true
            },
            {
                label: "DtInicioJob",
                name: 'DtInicioJob',
                index: 'DtInicioJob',
                width: '10%',
                formatter: "date",
                formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y h:i ' },
                editable: true,
                editoptions: { disabled: true }
            },
            {
                label: "DtUltimaExecucao",
                name: 'DtUltimaExecucao',
                formatter: "date",
                formatoptions: { srcformat: 'ISO8601Long', newformat: 'd/m/Y h:i ' },
                width: '10%',
                editable: true
            },
            {
                label: "EmExecucao",
                name: 'EmExecucao',
                width: '10%',
                editable: true
            },
            {
                label: "StAtivo",
                name: 'StAtivo',
                width: '10%',
                editable: true
            },
            {   
                name: 'act',
                index: 'act',
                width: '10%',
                sortable: false,
                editable: false,
                editOptions: {},
            },

        ],
        gridComplete: function () {
            //var ids = jQuery("#tableJobGrid").getDataIDs();
            var ids = $("#tableJobGrid").jqGrid("getGridParam", "data"),
                id = $.map(ids, function (item) { return item.ID; });
            
            var executar;
            var editar;            
            for (var i = 0; i < id.length; i++) {
                console.log('i:' + i);
                var cl = id[i];
                console.log('cl:' +cl);
                executar = "<button class='btn btn-custom' type='button' title='Executar Job' onclick='raizenCoreJs.Configuracao.executarJob(" + cl + ");'>"
                                + "<i class='fa fa-play'></i>"
                            + "</button>",
                editar = "<button class='btn btn-custom' type='button' title='Editar Job' id='editar_" + (i+1) + "' onclick='editRow(" + (i+1) + ");'>"
                                + "<i class='fa fa-pencil'></i>"
                            + "</button>",
                jQuery("#tableJobGrid").setRowData(i+1, { act: executar + editar });
            }
        },
        editurl: this.urlSalvarJob,        
        width: 780,
        height: '100%',
        autowidth: true,
        loadonce: true,
        viewrecords: true,
        jsonReader: {
            root: "rows"
        },
    }

    )
}

var editRow = function editRow(id) {
    var lastSelection;
    if (id && id !== lastSelection) {
        var grid = $("#tableJobGrid");
        grid.jqGrid('restoreRow', lastSelection);
        grid.jqGrid('editRow', id, { keys: true });
        lastSelection = id;
    }    
}

Configuracao.prototype.executarJob = function executarJob(id) {
    console.log(this.urlExecutarJob);
    raizenCoreJs.raizenHelpers.AbrirLoading();
    $.ajax({
        url: this.urlExecutarJob,
        data: { id: id },
        type: 'POST',
        success: function (response) {
            if (response == '1')
                ShowMessage('Job Executado com Sucesso!')
            else
                ShowErrorMenssage('Erro ao executar o job: ' + response);
            raizenCoreJs.raizenHelpers.FecharLoading();
        }
    });
}

Configuracao.prototype.VerLogSincronizacao = function VerLogSincronizacao() {
    window.location.href = this.urlPesquisarLog;
}