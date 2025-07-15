using System;

namespace Raizen.UniCad.Model.View
{
    public class MotoristaView
    {
        public virtual Int32 ID { get; set; }
        public virtual Int32 IDEmpresa { get; set; }
        public virtual Int32? IDTransportadora { get; set; }
        public virtual int IDStatus { get; set; }
        public virtual string Nome { get; set; }
        public virtual string CPF { get; set; }
        public virtual string Operacao { get; set; }
        public virtual string RG { get; set; }
        public virtual string OrgaoEmissor { get; set; }
        public virtual DateTime? Nascimento { get; set; }
        public virtual DateTime DataAtualizazao { get; set; }
        public virtual string LocalNascimento { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual string CNH { get; set; }
        public virtual string CategoriaCNH { get; set; }
        public virtual string OrgaoEmissorCNH { get; set; }
        public virtual string Telefone { get; set; }
        public virtual string Email { get; set; }
        public virtual string Justificativa { get; set; }
        public virtual string Anexo { get; set; }
        public virtual string CodigoEasyQuery { get; set; }
        public virtual string CodigoSalesForce { get;set; }
        public virtual int? IDMotorista { get; set; }
        public virtual string LoginUsuario { get; set; }
        public virtual string Clientes { get; set; }
        public virtual bool IsClonar { get; set; }
        public int Linhas { get; set; }
        public string UsuarioAlterouStatus { get; set; }
        public virtual string DNI { get; set; }
        public virtual string Apellido { get; set; }
        public virtual string NomeCliente { get; set; }
    }
}
