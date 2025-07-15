using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class MotoristaAlteradoView
    {
        public virtual bool IsEmpresaAlterado { get; set; }
        public virtual string EmpresaAlterado { get; set; }
        public virtual bool IsTransportadoraAlterado { get; set; }
        public virtual string TransportadoraAlterado { get; set; }
        public virtual bool IsStatusAlterado { get; set; }
        public virtual string StatusAlterado { get; set; }
        public virtual bool IsNomeAlterado { get; set; }
        public virtual string NomeAlterado { get; set; }
        public virtual bool IsCPFAlterado { get; set; }
		public virtual string CPFAlterado { get; set; }
		
        public virtual bool IsOperacaoAlterado { get; set; }
        public virtual string OperacaoAlterado { get; set; }
        public virtual bool IsRGAlterado { get; set; }
        public virtual string RGAlterado { get; set; }
        public virtual bool IsOrgaoEmissorAlterado { get; set; }
        public virtual string OrgaoEmissorAlterado { get; set; }
        public virtual bool IsNascimentoAlterado { get; set; }
        public virtual string NascimentoAlterado { get; set; }
        public virtual bool IsDataAtualizazaoAlterado { get; set; }
        public virtual string DataAtualizazaoAlterado { get; set; }
        public virtual bool IsLocalNascimentoAlterado { get; set; }
        public virtual string LocalNascimentoAlterado { get; set; }
        public virtual bool IsAtivoAlterado { get; set; }
        public virtual string AtivoAlterado { get; set; }
        public virtual bool IsCNHAlterado { get; set; }
        public virtual string CNHAlterado { get; set; }
        public virtual bool IsCategoriaCNHAlterado { get; set; }
        public virtual string CategoriaCNHAlterado { get; set; }
        public virtual bool IsOrgaoEmissorCNHAlterado { get; set; }
        public virtual string OrgaoEmissorCNHAlterado { get; set; }
        public virtual bool IsTelefoneAlterado { get; set; }
        public virtual string TelefoneAlterado { get; set; }
        public virtual bool IsEmailAlterado { get; set; }
        public virtual bool IsPisAlterado { get; set; }
        public virtual string EmailAlterado { get; set; }
        public virtual bool IsJustificativaAlterado { get; set; }
        public virtual string JustificativaAlterado { get; set; }
        public virtual bool IsAnexoAlterado { get; set; }
        public virtual string AnexoAlterado { get; set; }
        public string PisAlterado { get; set; }
        public virtual bool IsClientesAlterado { get; set; }

        #region Campos Argentina

        public virtual bool IsDNIAlterado { get; set; }
        public virtual string DNIAlterado { get; set; }

        public virtual bool IsLNCAlterado { get; set; }
        public virtual string LNCAlterado { get; set; }

        public virtual bool IsLNHAlterado { get; set; }
        public virtual string LNHAlterado { get; set; }

        public virtual bool IsCUITAlterado { get; set; }
        public virtual string CUITAlterado { get; set; }

        public virtual bool IsTarjetalterado { get; set; }
        public virtual string TarjetaAlterado { get; set; }

        #endregion

    }
}
