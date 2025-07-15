using Raizen.Framework.Utils.Extensions;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Raizen.UniCad
{
    public static class GenericDelegate
    {
        private static readonly string BLLName = "Raizen.UniCad.BLL";

        public static List<T> Listar<T>()
        {
            return Listar<T>(typeof(T).Name + "Business");
        }

        public static List<T> Listar<T>(string classe, string filtro = null)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(p => p.GetName().Name == BLLName);
            Type type = assembly.GetType(BLLName + "." + classe);

            MethodInfo methodInfo = type.GetMethod("Listar", new Type[0]);
            object classInstance = Activator.CreateInstance(type, null);

            return (List<T>)methodInfo.Invoke(classInstance, null);
        }

        public static List<T> ListarComplex<T>(Expression<Func<T, bool>> predicate)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(p => p.GetName().Name == BLLName);
            Type type = assembly.GetType(BLLName + "." + typeof(T).Name + "Business");

            MethodInfo methodInfo = type.GetMethod("Listar", new Type[1] { typeof(Expression<Func<T, bool>>) });
            object classInstance = Activator.CreateInstance(type, null);

            return (List<T>)methodInfo.Invoke(classInstance, new object[1] { predicate });
        }

        public static List<KeyValuePair<String, int>> ListarTipoProduto(bool listarDados)
        {
            return ListarTipoProduto(listarDados, null, true);
        }

        public static List<KeyValuePair<String, int>> ListarTipoProduto(bool listarDados, List<int> IDTipos)
        {
            return ListarTipoProduto(listarDados, IDTipos, true);
        }

        public static List<KeyValuePair<String, int>> ListarTipoProduto(bool listarDados, List<int> IDTipos, bool listarTodos)
        {
            var tipoProdutoList = new List<KeyValuePair<String, int>>();

            if (listarTodos)
            {
                tipoProdutoList.AddRange(EnumExtensions.GetKeyValueList<EnumTipoProduto>(true));
                return tipoProdutoList;
            }

            if (!listarDados)
                return tipoProdutoList;

            tipoProdutoList.AddRange(EnumExtensions.GetKeyValueList<EnumTipoProduto>(true).Where(kv => IDTipos.Contains(kv.Value)));

            return tipoProdutoList;
        }

        public static List<KeyValuePair<String, int>> ListarPais()
        {
            var enums = EnumExtensions.GetKeyValueList<EnumPais>(true).Where(p => p.Value != (int)EnumPais.Padrao).ToList();
            return enums;
        }

        public static List<KeyValuePair<String, int>> ListarTipoTreinamento()
        {
            var enums = EnumExtensions.GetKeyValueList<EnumTipoAgenda>();
            enums.RemoveAt(0);
            return enums;
        }

        public static List<KeyValuePair<String, int>> ListarEnum<T>()
        {
            return EnumExtensions.GetKeyValueList<T>();
        }

        public static List<ComposicaoEixo> ListarComposicaoEixoAtivo()
        {
            return new ComposicaoEixoBusiness().Listar(w => w.Ativo);
        }

        public static List<Terminal> ListarTerminal()
        {
            var lista = new TerminalBusiness().Listar();
            lista.ForEach(p => p.Nome = p.Sigla + " - " + p.Nome + (p.isPool ? " - POOL" : string.Empty));
            return lista;
        }

        public static IEnumerable<BloqueioImediato> BloqueioVerdadeiroOuFalso()
        {
            var lista = new List<BloqueioImediato>();
            lista.Add(new BloqueioImediato() { Flag = 0, Nome = "Não" });
            lista.Add(new BloqueioImediato() { Flag = 1, Nome = "Sim" });
            return lista;
        }

        public static IEnumerable<AcaoVencimento> AcaoVencimentoBloquearReprovar()
        {
            var lista = new List<AcaoVencimento>();
            lista.Add(new AcaoVencimento() { Flag = 1, Nome = "Bloquear" });
            lista.Add(new AcaoVencimento() { Flag = 2, Nome = "Reprovar" });
            lista.Add(new AcaoVencimento() { Flag = 0, Nome = "Sem ação" });
            return lista;
        }

        public static IEnumerable<TrueOrFalse> VerdadeiroOuFalso()
        {
            return VerdadeiroOuFalso(1);
        }

        public static IEnumerable<TrueOrFalse> VerdadeiroOuFalso(int idPais)
        {
            var lista = new List<TrueOrFalse>();
            lista.Add(new TrueOrFalse() { Flag = false, Nome = int.Equals(idPais, 1) ? "Não" : "No" });
            lista.Add(new TrueOrFalse() { Flag = true, Nome = int.Equals(idPais, 1) ? "Sim" : "Si" });
            return lista;
        }

        public static IEnumerable<TrueOrFalseNull> VerdadeiroOuFalsoOuBranco()
        {
            var lista = new List<TrueOrFalseNull>();
            lista.Add(new TrueOrFalseNull() { Flag = null, Nome = "" });
            lista.Add(new TrueOrFalseNull() { Flag = false, Nome = "Não" });
            lista.Add(new TrueOrFalseNull() { Flag = true, Nome = "Sim" });
            return lista;
        }

        public static List<TipoProduto> ListarTipoProdutoEab()
        {
            var lista = new TipoProdutoBusiness().Listar(w => w.ID == (int)EnumTipoProduto.Claros);
            return lista;
        }

        public static List<TipoProduto> ListarTipoProdutoEabArgentina()
        {
            var lista = new TipoProdutoBusiness().Listar(w => w.ID == (int)EnumTipoProduto.ClarosArg);
            return lista;
        }

        public static IEnumerable<Quantidade> QtdAlertas()
        {
            var lista = new List<Quantidade>();
            lista.Add(new Quantidade() { ID = "0", Valor = "0" });
            lista.Add(new Quantidade() { ID = "1", Valor = "1" });
            lista.Add(new Quantidade() { ID = "2", Valor = "2" });
            return lista;
        }

        public static IEnumerable<TrueOrFalse> AtivoInativo()
        {
            var lista = new List<TrueOrFalse>();
            lista.Add(new TrueOrFalse() { Flag = true, Nome = "Ativo" });
            lista.Add(new TrueOrFalse() { Flag = false, Nome = "Inativo" });

            return lista;
        }

        public static IEnumerable<YesOrNo> SimOuNao()
        {
            var lista = new List<YesOrNo>();
            lista.Add(new YesOrNo() { ID = "Não", Nome = "Não" });
            lista.Add(new YesOrNo() { ID = "Sim", Nome = "Sim" });
            return lista;
        }

        public static IEnumerable<LinhaNegocio> LinhaDeNegocio()
        {
            var lista = new List<LinhaNegocio>();
            lista.Add(new LinhaNegocio() { ID = "2", Nome = "Combustível " });
            lista.Add(new LinhaNegocio() { ID = "1", Nome = "EAB " });
            return lista;
        }

        public static IEnumerable<LinhaNegocio> LinhaDeNegocioAgentina()
        {
            var lista = new List<LinhaNegocio>();
            lista.Add(new LinhaNegocio() { ID = "2", Nome = "Combustibles" });
            lista.Add(new LinhaNegocio() { ID = "1", Nome = "EAB " });
            return lista;
        }

        public static IEnumerable<CifFob> CifFobCongeneres()
        {
            var lista = new List<CifFob>();
            lista.Add(new CifFob() { ID = "CIF", Nome = "CIF" });
            lista.Add(new CifFob() { ID = "FOB", Nome = "FOB" });
            lista.Add(new CifFob() { ID = "CON", Nome = "CONGÊNERES" });
            return lista;
        }

        public static IEnumerable<CifFob> CifOuFob()
        {
            var lista = new List<CifFob>();
            lista.Add(new CifFob() { ID = "CIF", Nome = "CIF" });
            lista.Add(new CifFob() { ID = "FOB", Nome = "FOB" });
            return lista;
        }

        public static IEnumerable<CifFob> CifFobOuAmbos()
        {
            var lista = new List<CifFob>();
            lista.Add(new CifFob() { ID = "Ambos", Nome = "Ambos" });
            lista.Add(new CifFob() { ID = "CIF", Nome = "CIF" });
            lista.Add(new CifFob() { ID = "FOB", Nome = "FOB" });
            return lista;
        }

        public static IEnumerable<InternoExterno> InternoOuExterno()
        {
            var lista = new List<InternoExterno>();
            lista.Add(new InternoExterno() { ID = "true", Nome = "Externo" });
            lista.Add(new InternoExterno() { ID = "false", Nome = "Interno" });
            return lista;
        }

        public class YesOrNo
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }

        public class InternoExterno
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }

        public class CifFob
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }

        public class LinhaNegocio
        {
            public string ID { get; set; }
            public string Nome { get; set; }
        }

        public class BloqueioImediato
        {
            public int Flag { get; set; }
            public string Nome { get; set; }
        }

        public class AcaoVencimento
        {
            public int Flag { get; set; }
            public string Nome { get; set; }
        }

        public class TrueOrFalse
        {
            public bool Flag { get; set; }
            public string Nome { get; set; }
        }

        public class TrueOrFalseNull
        {
            public bool? Flag { get; set; }
            public string Nome { get; set; }
        }

        public class Quantidade
        {
            public string ID { get; set; }
            public string Valor { get; set; }
        }
    }

    public interface IListable
    {
        List<T> Listar<T>();
    }

    public class SearchTypeAheadEntityView
    {
        public int? ID { get; set; }
        public string Name { get; set; }
    }

    public class SearchTypeAheadEntityArredView
    {
        public string cnpj { get; set; }
        public string Name { get; set; }
        public string Valor { get; set; }
    }
}