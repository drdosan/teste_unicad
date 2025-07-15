using System;
using System.Collections.Generic;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLLTests
{    
    public static class FuncoesCompartilhadasTests
    {
        public static Motorista CriarMotorista(string cnh, string categoriaCnh, int idTransportadora, string cpf,
            List<MotoristaDocumentoView> docs, string email, int idEmpresa, string localNascimento,
            DateTime dataNascimento, string nome, string operacao, string orgaoEmissor, string orgaoEmissorCnh, string rg, string telefone)
        {
            var moto = new Motorista
            {
                CNH = cnh,
                CategoriaCNH = categoriaCnh,
                IDTransportadora = idTransportadora,
                CPF = cpf,
                Documentos = docs,
                Email = email,
                IDEmpresa = idEmpresa,
                LocalNascimento = localNascimento,
                Nascimento = dataNascimento,
                DataAtualizazao = DateTime.Now,
                Nome = nome,
                Operacao = operacao,
                OrgaoEmissor = orgaoEmissor,
                OrgaoEmissorCNH = orgaoEmissorCnh,
                RG = rg,
                Telefone = telefone,
                EmailSolicitante = email,
                LoginUsuario = "tr009592"
            };
            
            return moto;
        }
        public static Transportadora CriarTransportadora(string cnpjCpf, int idEmpresa, string razaoSocial, string ibm)
        {
            var transp = new Transportadora
            {
                CNPJCPF = cnpjCpf,
                Desativado = false,
                DtAtualizacao = DateTime.Now,
                IDEmpresa = idEmpresa,
                RazaoSocial = razaoSocial,
                IBM = ibm,
                DtInclusao = DateTime.Now,
                Operacao = "CIF"
            };
            return transp;
        }
        public static Cliente CriarCliente(string cnpjCpf, int idEmpresa, string razaoSocial, string ibm)
        {
            var cliente = new Cliente
            {
                CNPJCPF = cnpjCpf,
                Desativado = false,
                DtAtualizacao = DateTime.Now,
                IDEmpresa = idEmpresa,
                RazaoSocial = razaoSocial,
                IBM = ibm,
                DtInclusao = DateTime.Now                
            };            
            return cliente;
        }
        public static Usuario CriarUsuario(string nome, string login, string email, string Perfil, string Operacao, bool isExterno, EnumEmpresa empresa)
        {
            var usuario = new Usuario()
            {
                Nome = nome,
                Login = login,
                Email = email,
                Perfil = Perfil,
                Operacao = Operacao,
                Externo = isExterno,
                Status = true,
                IDEmpresa = (int)empresa
            };
            return usuario;
        }

        public  static TipoDocumento IncluirDocumento(string sigla, string descricao, EnumCategoriaVeiculo categoriaVeiculo, int isBloqueioImediato = (int)EnumTipoBloqueioImediato.Nao)
        {
            //Incluir documento
            var tipoDoc = new TipoDocumento
            {
                BloqueioImediato = isBloqueioImediato,
                DataAtualizacao = DateTime.Now,
                Descricao = descricao,
                IDCategoriaVeiculo = (int)categoriaVeiculo,
                IDEmpresa = (int) EnumEmpresa.Ambos,
                Obrigatorio = true,
                Operacao = "Ambos",
                QtdDiasBloqueio = isBloqueioImediato == (int)EnumTipoBloqueioImediato.Nao ? 0 : 30,
                Sigla = sigla,
                Status = true,
                tipoCadastro = 2,
                Alerta1 = 10,
                Alerta2 = 20,
                IDPais = 1
            };
            new TipoDocumentoBusiness().Adicionar(tipoDoc);
            return tipoDoc;
        }
    }
}