using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Extensions
{
    public static class MapperExtensions
    {
        #region MotoristaPesquisa -> Motorista
        public static Motorista Mapear(this MotoristaPesquisa motoristaPesquisa)
        {
            if (motoristaPesquisa == null)
                return null;

            var motorista = new Motorista
            {
                ID = motoristaPesquisa.ID,
                IDEmpresa = motoristaPesquisa.IDEmpresa,
                IDTransportadora = motoristaPesquisa.IDTransportadora,
                IDStatus = motoristaPesquisa.IDStatus,
                Operacao = motoristaPesquisa.Operacao,
                Nome = motoristaPesquisa.Nome,
                DataAtualizazao = motoristaPesquisa.DataAtualizazao,
                Telefone = motoristaPesquisa.Telefone,
                Email = motoristaPesquisa.Email,
                Anexo = motoristaPesquisa.Anexo,
                CodigoEasyQuery = motoristaPesquisa.CodigoEasyQuery,
                Ativo = motoristaPesquisa.Ativo,
                Observacao = motoristaPesquisa.Observacao,
                PIS = motoristaPesquisa.PIS,
                UsuarioAlterouStatus = motoristaPesquisa.UsuarioAlterouStatus,
                IdPais = motoristaPesquisa.IdPais,
				LoginUsuario = motoristaPesquisa.LoginUsuario,
                Justificativa = motoristaPesquisa.Justificativa
            };

            switch (motoristaPesquisa.IdPais)
            {
                case EnumPais.Brasil:
                    motorista.MotoristaBrasil = new MotoristaBrasil
                    {
                        IDMotorista = motoristaPesquisa.ID,
                        CPF = motoristaPesquisa.CPF,
                        RG = motoristaPesquisa.RG,
                        OrgaoEmissor = motoristaPesquisa.OrgaoEmissor,
                        Nascimento = motoristaPesquisa.Nascimento,
                        LocalNascimento = motoristaPesquisa.LocalNascimento,
                        CNH = motoristaPesquisa.CNH,
                        CategoriaCNH = motoristaPesquisa.CategoriaCNH,
                        OrgaoEmissorCNH = motoristaPesquisa.OrgaoEmissorCNH
                    };
                    break;

                case EnumPais.Argentina:
                    motorista.MotoristaArgentina = new MotoristaArgentina
                    {
                        IDMotorista = motoristaPesquisa.ID,
                        Apellido = motoristaPesquisa.Apellido,
                        CUITTransportista = motoristaPesquisa.CUITTransportista,
                        DNI = motoristaPesquisa.DNI,
                        LicenciaNacionalConducir = motoristaPesquisa.LicenciaNacionalConducir,
                        LicenciaNacionalHabilitante = motoristaPesquisa.LicenciaNacionalHabilitante,
                        Tarjeta = motoristaPesquisa.Tarjeta
                    };
                    break;
            }

            return motorista;
        }

        public static List<Motorista> Mapear(this List<MotoristaPesquisa> motoristaPesquisaList)
        {
            if (motoristaPesquisaList == null)
                return null;

            var motoristas = new List<Motorista>();

            foreach (var motoristaPesquisa in motoristaPesquisaList)
                motoristas.Add(motoristaPesquisa.Mapear());

            return motoristas;
        }
        
    }
    #endregion
}
