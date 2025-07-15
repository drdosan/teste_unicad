
using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.DAL;
using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.DAL.Repositories;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;
using Raizen.UniCad.SAL.Interfaces;

namespace Raizen.UniCad.BLL
{
    public class PlacaClienteBusiness : UniCadBusinessBase<PlacaCliente>, IPlacaClienteBusiness
    {
        private readonly IPlacaRepository _placaRepository;
        private readonly IComposicaoRepository _composicaoRepository;
        private readonly IPlacaClienteRepository _placaClienteRepository;
        private readonly IWsIntegraSAP _wsIntegraSAP;

        public PlacaClienteBusiness()
        {
            var contexto = GetContext();

            _placaRepository = new PlacaRepository(contexto);
            _composicaoRepository = new ComposicaoRepository(contexto);
            _placaClienteRepository = new PlacaClienteRepository(contexto);
            _wsIntegraSAP = new WsIntegraSAP();
        }

        public PlacaClienteBusiness(IPlacaRepository placaRepository,
                                    IComposicaoRepository composicaoRepository,
                                    IPlacaClienteRepository placaClienteRepository,
                                    IWsIntegraSAP wsIntegraSAP)
        {
            _placaRepository = placaRepository;
            _composicaoRepository = composicaoRepository;
            _placaClienteRepository = placaClienteRepository;
            _wsIntegraSAP = wsIntegraSAP;
        }

        UniCadContexto GetContext()
        {
            var contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado());
#if DEBUG
            contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
            return contexto;
        }

        public List<PlacaClienteView> ListarClientesPlaca(int IDPlaca)
        {
            return _placaClienteRepository.BuscaClientesPlaca(IDPlaca).ToList();
        }


        public List<PlacaClienteView> ListarClientesPorPlaca(int IDPlaca, int IDUsuarioCliente = 0)
        {
            return _placaClienteRepository.BuscaClientesPlaca(IDPlaca, IDUsuarioCliente)
                    .Distinct()
                    .ToList();
        }

        public string ExcluirPlacaCliente(int idComposicao, int idPlaca, int[] placaClientes)
        {
            var composicao = _composicaoRepository.Selecionar(idComposicao);
            var placa = _placaRepository.Selecionar(idPlaca);

            var clientes = ListarClientesPorPlaca(idPlaca);
            clientes.RemoveAll(pc => placaClientes.Contains(pc.ID));

            if (composicao.IDEmpresa != (int)EnumEmpresa.EAB)
            {
                placa.Clientes = clientes;

                var retornoSap = _wsIntegraSAP.ExcluirPlacaClienteSap(placa);

                if (!string.IsNullOrEmpty(retornoSap))
                {
                    return retornoSap;
                }
            }

            var idClientesAtivos = clientes.Select(c => c.IDCliente).ToArray();
            EqualizarPlacasCliente(composicao, idClientesAtivos);
            return GetExcluirPlacaClienteMessage(placa.IDPais);
        }

        private void EqualizarPlacasCliente(Composicao composicao, int[] idClientesAtivos)
        {
            int?[] placas = new int?[] { composicao.IDPlaca1, composicao.IDPlaca2, composicao.IDPlaca3, composicao.IDPlaca4 };

            foreach (int? placa in placas)
            {
                if (placa.HasValue)
                {
                    var clientesExcluidos = _placaClienteRepository.SelecionarLista(pc => pc.IDPlaca == placa && !idClientesAtivos.Contains(pc.IDCliente));
                    _placaClienteRepository.ExcluirLista(clientesExcluidos);
                }
            }
        }

        private string GetExcluirPlacaClienteMessage(EnumPais pais)
        {
            switch (pais)
            {
                case EnumPais.Argentina:
                    return "Placa eliminado con éxito";
                default:
                    return "Placa removida com Sucesso";
            }
        }
    }
}

