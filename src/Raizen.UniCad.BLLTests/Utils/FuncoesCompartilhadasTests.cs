using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLLTests
{    
    public static class FuncoesCompartilhadasTests
    {
        #region Motorista

        public static Motorista CriarMotorista(string cnh, string categoriaCnh, int idTransportadora, string cpf,
            List<MotoristaDocumentoView> docs, string email, int idEmpresa, string localNascimento,
            DateTime dataNascimento, string nome, string operacao, string orgaoEmissor, string orgaoEmissorCnh, string rg, string telefone)
        {

            var moto = new Motorista
            {
                
                Documentos = docs,
                Email = email,
                IDEmpresa = idEmpresa,                
                IDTransportadora = idTransportadora,
                DataAtualizazao = DateTime.Now,
                Nome = nome,
                Operacao = operacao,                
                Telefone = telefone,
                EmailSolicitante = email,
                LoginUsuario = "tr009592",
                IdPais = EnumPais.Brasil,
                MotoristaBrasil = new MotoristaBrasil()
                {
                    CNH = cnh,
                    CategoriaCNH = categoriaCnh,
                    CPF = cpf,
                    LocalNascimento = localNascimento,
                    Nascimento = dataNascimento,
                    OrgaoEmissor = orgaoEmissor,
                    OrgaoEmissorCNH = orgaoEmissorCnh,
                    RG = rg
                }
            };
            
            return moto;
        }
        public static Motorista GetMotoristaBrasil01(string cpf)
        {
            return new Motorista
            {
                Anexo = "arquivo.pdf",
                Ativo = true,                
                IDEmpresa = 1,
                IDStatus = 4,
                IDTransportadora = 2,
                Nome = "JOSÉ DE TESTE DA SILVA",
                Operacao = "COB",
                Telefone = "1231231233",
                Email = "jose@empresa.com.br",
                CodigoEasyQuery = "4291782-4352560",
                Observacao = "Observacão Teste",
                Justificativa = "Justificativa Teste",
                PIS = "7856312456",
                IdPais = EnumPais.Brasil,
                DataAtualizazao = DateTime.Now,
                MotoristaBrasil = new MotoristaBrasil()
                {
                    CategoriaCNH = "E",
                    CNH = "12345654784",
                    CPF = cpf,
                    RG = "414552550",
                    OrgaoEmissor = "SSP/SP",
                    OrgaoEmissorCNH = "SSP/SP",
                    Nascimento = DateTime.Parse("1982-01-10"),
                    LocalNascimento = "Araraquara"
                }
            };
        }

        public static Motorista GetMotoristaBrasil02(string cpf)
        {
            return new Motorista
            {
                Anexo = "arquivo2.pdf",
                Ativo = true,
              
                IDEmpresa = 1,
                IDStatus = 4,
                IDTransportadora = 2,
                Nome = "JOÃO DE TESTE DA SILVA",
                Operacao = "COB",
                Telefone = "3123123123",
                Email = "jose@empresa.com.br",
                CodigoEasyQuery = "4291782-4126432",
                Observacao = "Observacão Teste 2",
                Justificativa = "Justificativa Teste 2",
                PIS = "4568521647",
                IdPais = EnumPais.Brasil,
                DataAtualizazao = DateTime.Now,
                MotoristaBrasil = new MotoristaBrasil()
                {
                    CategoriaCNH = "A",
                    CNH = "48745654321",
                    CPF = cpf,
                    RG = "457854568",
                    OrgaoEmissor = "SSP/SP",
                    OrgaoEmissorCNH = "SSP/SP",
                    Nascimento = DateTime.Parse("1982-01-10"),
                    LocalNascimento = "Sertãozinho"
                }
            };
        }

        public static Motorista GetMotoristaArgentina01(string dni)
        {
            return new Motorista
            {
                IDEmpresa = 2,
                IDStatus = 4,
                IDTransportadora = 2,
                Nome = "JUAN TIESTE SOSA",
                Operacao = "COB",
                Telefone = "12312312312",
                Email = "juan@empresa.com.br",
                Anexo = "arquivo3.jpg",
                CodigoEasyQuery = "1111243-4352560",
                Ativo = true,
                Observacao = "Observación Teste 3",
                Justificativa = "Justificativa Teste 3",
                PIS = "789789789",
                IdPais = EnumPais.Argentina,
                DataAtualizazao = DateTime.Now,
                MotoristaArgentina = new MotoristaArgentina()
                {
                    DNI = dni,
                    Apellido = "TESTE",
                    CUITTransportista = "12345678911"
                }
            };
        }

        public static Motorista GetMotoristaArgentina02(string dni)
        {
            return new Motorista
            {
                IDEmpresa = 2,
                IDStatus = 4,
                IDTransportadora = 2,
                Nome = "CARLOS TIESTE SOSA",
                Operacao = "COB",
                Telefone = "1245113413",
                Email = "carlos@empresa.com.br",
                Anexo = "arquivo4.jpg",
                CodigoEasyQuery = "1111243-3123123",
                Ativo = true,
                Observacao = "Observación Teste 4",
                Justificativa = "Justificativa Teste 4",
                PIS = "34534673",
                IdPais = EnumPais.Argentina,                
                DataAtualizazao = DateTime.Now,
                MotoristaArgentina = new MotoristaArgentina()
                {
                    DNI = dni,
                    Apellido = "TESTE 4",
                    CUITTransportista = "12345678910"
                }
            };
        }

        #endregion

        #region Transportadora

        public static Transportadora CriarTransportadoraBrasil(string cnpjCpf, int idEmpresa, string razaoSocial, string ibm)
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
                Operacao = "CIF",
                IdPais = (int)EnumPais.Brasil
            };
            return transp;
        }

        #endregion

        #region Cliente

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

        #endregion

        #region Usuario

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
                IDEmpresa = (int)empresa,
                IDPais = EnumPais.Brasil                
            };
            return usuario;
        }

        #endregion

        #region Documento

        public static TipoDocumento IncluirDocumento(string sigla, string descricao, EnumCategoriaVeiculo categoriaVeiculo, int isBloqueioImediato = (int)EnumTipoBloqueioImediato.Nao)
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

        #endregion

        #region Placa

        public static Placa GetPlacaBrasil01(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                Chassi = "12345678101213",
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,                
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_BR1",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Brasil,
                PlacaBrasil = new PlacaBrasil()
                {
                    Renavam = "1234567890",
                    IDEstado = 1,
                    Cidade = "Cidade teste",
                    CPFCNPJ = "14774281000144"
                }
            };
        }

        public static Placa GetPlacaBrasil02(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                Chassi = "12345678101213",
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,                
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_BR2",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Brasil,
                PlacaBrasil = new PlacaBrasil()
                {
                    Renavam = "1234567890",
                    IDEstado = 1,
                    Cidade = "Cidade teste",
                    CPFCNPJ = "14774281000144"
                }
            };
        }

        public static Placa GetPlacaArgentina01(string placaNumero, int idTipoVeiculo, bool comSetaPadrao, int idUsuario = 0, int? idTransportadora = null, string operacao = "CIF")
        {
            return new Placa
            {
                idUsuario = idUsuario,
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_ARG1",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Tara = 1,
                Operacao = operacao,
                IDTransportadora = idTransportadora,
                IDPais = EnumPais.Argentina,
                PlacaArgentina = new PlacaArgentina()
                {
                    CUIT = "CUITTeste01",
                    PBTC = 102.34
                }
            };
        }

        public static Placa GetPlacaArgentina02(string placaNumero, int idTipoVeiculo, bool comSetaPadrao)
        {
            return new Placa
            {
                AnoFabricacao = 2015,
                AnoModelo = 2015,
                BombaDescarga = false,
                CameraMonitoramento = false,
                DataAtualizacao = DateTime.Now.Date,
                DataNascimento = new DateTime(1980, 01, 01),
                EixosDistanciados = true,
                EixosPneusDuplos = true,
                NumeroEixosDistanciados = 2,
                NumeroEixosPneusDuplos = 2,
                IDCategoriaVeiculo = 1,
                IDTipoVeiculo = idTipoVeiculo,
                IDTipoProduto = 1,
                Marca = "VOLVO_ARG2",
                Modelo = "v230",
                MultiSeta = comSetaPadrao,
                NumeroAntena = "1020304050",
                NumeroEixos = 2,
                PlacaVeiculo = placaNumero,
                PossuiAbs = true,
                RazaoSocial = "teste",
                Tara = 1,
                Operacao = "CIF",
                IDTransportadora = new TransportadoraBusiness().Listar().FirstOrDefault()?.ID,
                IDPais = EnumPais.Argentina,
                PlacaArgentina = new PlacaArgentina()
                {
                    CUIT = "CUITTeste02",
                    PBTC = 102.34
                }
            };
        }

        #endregion
    }
}