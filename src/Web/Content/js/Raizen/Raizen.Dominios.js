/// Classe responsável em representar os dominios de tipos para as classes javaScripts
/// Qualquer customização para CRUD's especificos deve ser tratado em um JS a parte
/// JavaScript Design Patterns (Prototype Pattern) - ver Learning JavaScript Design Patterns A book by Addy Osmani Volume 1.5.2
function RaizenDominios() {
    this.operacaoCrud = Object.create(OperacoesCRUD);
    this.estadoModalCrud = Object.create(EstadoCrudModal);
    this.tipoCrud = Object.create(TipoCrud);
    this.modoExibicaoTreeView = Object.create(ModoExibicaoTreeView);
    this.modoExibicaoFilhosTreeView = Object.create(ModoExibicaoFihosTreeView);
    this.estadoPaginacao = Object.create(EstadoPaginacao);
};

var OperacoesCRUD =
    {
         Insert : 0,
         Update : 1,
         Delete : 2,
         List : 3,
         Validando : 4,
         Editando : 5,
         Cancelar : 6
  };

var EstadoCrudModal =
  {
     EstadoModal : "0",
     Aberta : "1",
     Fechada : "0"
 };

 var TipoCrud =
 {
      Simples : 0,
      Modal : 1,
      ComplexoWizard : 2
  };

  var ModoExibicaoTreeView =
  {
     TotalmenteAberta : 1,
     TotalmenteFechada : 2,
     ParcialmenteAberta: 3,
     Estatica : 4
 };

 var ModoExibicaoFihosTreeView =
  {
      Aberto: 1,
      Fechado: 2
  };

var EstadoPaginacao =
{
    SemAcao: 0,
    Paginando: 1,
    RenovandoConsulta: 2
};

RaizenCoreJs.prototype.raizenDominios = new RaizenDominios();