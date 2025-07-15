
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class ChecklistComposicaoBusiness : UniCadBusinessBase<ChecklistComposicao>
    {
        public string AdicionarChecklist(ChecklistComposicao checklist)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 5))
            {
                Adicionar(checklist);
                var compBll = new ComposicaoBusiness();
                var comp = compBll.Selecionar(checklist.IDComposicao);
                comp.checkList = checklist;
                var idStatus = !checklist.Aprovado ? (int)EnumStatusComposicao.Bloqueado : comp.IDStatus;
                EnumTipoIntegracaoSAP tipoIntegracao = checklist.Aprovado ? EnumTipoIntegracaoSAP.AprovarCheckList : EnumTipoIntegracaoSAP.ReprovarCheckList;
                comp.tipoIntegracao = tipoIntegracao;
                var retorno = compBll.AtualizarComposicao(comp, false, enviaEmail: false,idStatus: idStatus);
                //compBll.IntegrarSAP(comp, tipoIntegracao);
                if (!retorno) return comp.Mensagem;
                var emails = new List<Usuario>();
                var placaClienteBll = new PlacaClienteBusiness();
                var placas = new PlacaBusiness().ListarPorComposicao(comp);

                foreach (var placa1 in placas)
                {
                    if (comp.Operacao == "FOB")
                    {
                        var cli = placaClienteBll.Listar(p => p.IDPlaca == placa1.ID);
                        if (cli != null && cli.Any())
                        {
                            var clientes = cli.Select(p => p.IDCliente);
                            var usuarioClientes = new UsuarioClienteBusiness().Listar(p => clientes.Any(b => b == p.IDCliente)).GroupBy(p => p.IDUsuario);
                            foreach (var usuario in usuarioClientes)
                            {
                                if (!emails.Any(p => p.ID == usuario.Key))
                                {
                                    var user = new UsuarioBusiness().Selecionar(w => w.ID == usuario.Key && w.Status);
                                    if (user != null)
                                        emails.Add(user);
                                }
                            }
                        }
                    }
                    else
                    {
                        var transps = new UsuarioTransportadoraBusiness().Listar(p => p.IDTransportadora == placa1.IDTransportadora).GroupBy(p => p.IDUsuario);
                        foreach (var usuario in transps)
                        {
                            if (!emails.Any(p => p.ID == usuario.Key))
                            {
                                var user = new UsuarioBusiness().Selecionar(w => w.ID == usuario.Key && w.Status);
                                if (user != null)
                                    emails.Add(user);
                            }                                
                        }

                        //CSCUNI-665
                        if (comp.IDEmpresa == (int)EnumEmpresa.Ambos)
                        {
                            var transps2 = new UsuarioTransportadoraBusiness().Listar(p => p.IDTransportadora == placa1.IDTransportadora2).GroupBy(p => p.IDUsuario);
                            foreach (var usuario in transps)
                            {
                                if (!emails.Any(p => p.ID == usuario.Key))
                                {
                                    var user = new UsuarioBusiness().Selecionar(w => w.ID == usuario.Key && w.Status);
                                    if (user != null)
                                        emails.Add(user);
                                }
                            }
                        }
                    }
                }

                StringBuilder placa = new StringBuilder();
                foreach (var item in placas)
                {
                    placa.Append(" - " + item.PlacaVeiculo);
                }

                EnumPais idPais = new PlacaBusiness().Selecionar(comp.IDPlaca1.Value).IDPais;
                foreach (var email in emails)
                {
                    string assunto = Config.GetConfig(EnumConfig.TituloChecklist, (int)idPais);
                    string aprovacao = checklist.Aprovado ? "Aprovado" : "Reprovado";
                    string corpoEmail = string.Format(Config.GetConfig(EnumConfig.CorpoChecklist, (int)idPais), email.Nome, aprovacao, placa, checklist.Justificativa);
                    Email.Enviar(email.Email, assunto, corpoEmail);
                }

                transactionScope.Complete();
                return "Gravado com sucesso!";
            }
        }
    }
}

