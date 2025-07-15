/// Classe responsável em conter os programas java Scripts que customizan e interagem com bibliotecas internas do BootStrap v 3.0.3
/// Qualquer customização para CRUD's especificos deve ser tratado em um JS
/// JavaScript Design Patterns (Prototype Pattern) - ver Learning JavaScript Design Patterns A book by Addy Osmani Volume 1.5.2

function RaizenCustomControlsBootStrap() {

    this.idTarget = "";
    this.valueField = "";
    this.qtdeItensExibicao = "5";
}

RaizenCustomControlsBootStrap.prototype.ComboAutoComplete = function IniciarAutoComplete(hiddenID, valueID, 
                                                                                        tituloCampo, URLApi, 
                                                                                        isMultiple, funcaoRender, 
                                                                                        funcaoSelection, funcaoID, 
                                                                                        qtdCaracteres, changeEvent) {
            var sid = '#' + hiddenID;
            $(sid).select2({
                placeholder: tituloCampo,
                minimumInputLength: qtdCaracteres,
                allowClear: true,
                multiple: isMultiple,
                ajax: {
                    url: URLApi,
                    dataType: 'json',
                    data: function (term, page) {
                        return {
                            termoBusca: term // search term
                        };
                    },
                    results: function (data) {
                        return { results: data };
                    }
                },
                initSelection: function (element, callback) {
                    // the input tag has a value attribute preloaded that points to a preselected make's id
                    // this function resolves that id attribute to an object that select2 can render
                    // using its formatResult renderer - that way the make text is shown preselected
                    var id = $('#' + valueID).val();

                    if (id !== null && id.length > 0) {
                        $.ajax(Ihara.getURLApi(URLApi) + "/" + id, {
                            dataType: "json"
                        }).done(function (data) { callback(data); });
                    }
                },
                formatResult: funcaoRender,
                formatSelection: funcaoSelection,
                id: funcaoID
            }).on("change", function (e) {
                if (changeEvent != null && changeEvent != undefined)
                    changeEvent(e);
            });

            $(document.body).on("change", sid, function (ev) {
                var choice;
                var values = ev.val;
                // This is assuming the value will be an array of strings.
                // Convert to a comma-delimited string to set the value.
                if (values !== null && values.length > 0) {
                    for (var i = 0; i < values.length; i++) {
                        if (typeof choice !== 'undefined') {
                            choice += ",";
                            choice += values[i];
                        }
                        else {
                            choice = values[i];
                        }
                    }
                }

                // Set the value so that MVC will load the form values in the postback.
                $('#' + valueID).val(choice);
            });
        };

RaizenCustomControlsBootStrap.prototype.IniciarAutoComplete = function IniciarAutoComplete() {

    $('*[IsCompleteBootStrap="True"]').each(function () {

        this.valueField = $(this).attr('ValueField');
        this.idTarget = $(this).attr('TargetInput');
        this.qtdeItensExibicao = $(this).attr('QtdeItensExibicao');

        $(this).typeahead({
            name: $(this).attr('NameAutoComplete'),
            valueKey: $(this).attr('NameField'),
            nameKey: $(this).attr('ValueField'),
            scrollHeight: 500,
            limit: this.qtdeItensExibicao,
            remote: {
                url: $(this).attr('UrlData')        
            }
        });

        $(this).on('typeahead:selected', raizenCoreJs.raizenCustomControlsBootStrap.AtualizarItemSelecionado);

    });

};

RaizenCustomControlsBootStrap.prototype.AtualizarItemSelecionado = function AtualizarItemSelecionado(element, datum) {
    $('#' + this.idTarget).val($(datum).attr(this.valueField));
};


RaizenCustomControlsBootStrap.prototype.AtualizarSegundoTarget = function AtualizarSegundoTarget(obj) {
    $('#' + $(obj).attr('SegundoTarget')).val($(obj).val());
};

RaizenCoreJs.prototype.raizenCustomControlsBootStrap = new RaizenCustomControlsBootStrap();