using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model.View
{
    public class ComposicaoView
    {
        [Column(TypeName = "int")]
        public int ID { get; set; }
        [Column(TypeName = "varchar")]
        public string Operacao { get; set; }
        [Column(TypeName = "int")]
        public int IDStatus { get; set; }
        [Column(TypeName = "varchar")]
        public string TipoComposicao { get; set; }
        [Column(TypeName = "varchar")]
        public string TipoComposicaoEixo { get; set; }
        [Column(TypeName = "varchar")]
        public string CategoriaVeiculo { get; set; }
        [Column(TypeName = "varchar")]
        public string CategoriaVeiculoEs { get; set; }
        [Column(TypeName = "int")]
        public int Linhas { get; set; }
        [Column(TypeName = "varchar")]
        public string Placa1 { get; set; }
        [Column(TypeName = "varchar")]
        public string Placa2 { get; set; }
        [Column(TypeName = "varchar")]
        public string Placa3 { get; set; }
        [Column(TypeName = "varchar")]
        public string Placa4 { get; set; }
        [Column(TypeName = "varchar")]
        public string CPFCNPJ { get; set; }
        [Column(TypeName = "datetime")]
        public Nullable<DateTime> DataNascimento { get; set; }
        [Column(TypeName = "varchar")]
        public string RazaoSocial { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataAtualizacao { get; set; }
        [Column(TypeName = "varchar")]
        public string Observacao { get; set; }
        [Column(TypeName = "varchar")]
        public string CodigoEasyQuery { get; set; }
        [Column(TypeName = "varchar")]
        public string CodigoSalesForce { get; set; }
        [Column(TypeName = "bit")]
        public bool MultiSeta { get; set; }
        [Column(TypeName = "bit")]
        public bool MultiCompartimento { get; set; }
        [Column(TypeName = "int")]
        public int IDEmpresa { get; set; }
        [Column(TypeName = "varchar")]
        public string EmpresaNome { get; set; }
        [Column(TypeName = "varchar")]
        public string CPFCNPJArrendamento { get; set; }
        [Column(TypeName = "varchar")]
        public string RazaoSocialArrendamento { get; set; }
        [Column(TypeName = "varchar")]
        public string LoginUsuario { get; set; }
        [Column(TypeName = "float")]
        public double PBTC { get; set; }
        [Column(TypeName = "int")]
        public int NumCompartimentos { get; set; }
        [Column(TypeName = "bit")]
        public bool Bloqueado { get; set; }
        [Column(TypeName = "datetime")]
        public Nullable<DateTime> CheckListData { get; set; }
        [Column(TypeName = "bit")]
        public Nullable<bool> CheckListAprovado { get; set; }
        [Column(TypeName = "decimal")]
        public decimal CapacidadeMinima { get; set; }
        [Column(TypeName = "decimal")]
        public decimal CapacidadeMaxima { get; set; }
        [Column(TypeName = "varchar")]
        public string UsuarioAlterouStatus { get; set; }
    }
}


