
using System.Collections.Generic;
using Raizen.Framework.UserSystem.Proxy;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class PerfilBusiness : UniCadBusinessBase<Perfil>
    {
        public List<Perfil> ListarUserSystem(bool todos)
        {
            var proxy = new ClientService();

            var app = proxy.CarregarEstruturaAplicacao("UNICA");
            var perfis = proxy.ListarPerfilApp(app.Data.Id);

            List<Perfil> perfils = new List<Perfil>();
            foreach (var item in perfis.Data)
            {
                //R2) Verificar a necessidade de preenchimento da lista de perfis do sistema
                if (todos ||!(item.Nome.Contains(EnumPerfil.CLIENTE_ACS) || item.Nome.Contains(EnumPerfil.CLIENTE_ACS_ARGENTINA)) || item.Nome == "Administrador TI")
                    perfils.Add(new Perfil { Nome = item.Nome, ID = item.Id });
            }
            return perfils;
        }

        public class SearchTypeAheadEntity
        {
            public string ShortCode { get; set; }
            public string Name { get; set; }
            public string Login { get; set; }
        }
    }
}

