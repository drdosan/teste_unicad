using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;
using Raizen.UniCad.Model.Base;

namespace Raizen.UniCad.Model.View
{
    public class PlacaClienteView
    {
        public int Colunas { get; set; }
        public int ID { get; set; }
        public int IDCliente { get; set; }
        public int IDPlaca { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public string Ibm { get; set; }

        #region Constructors

        public PlacaClienteView()
        {

        }

        public PlacaClienteView(Cliente cliente)
        {
            this.IDCliente = cliente.ID;
            this.RazaoSocial = MontaRazaoSocial(cliente);
        }

        #endregion

        #region Private methods

        private static string MontaRazaoSocial(Cliente cliente)
        {
            return $"{cliente.IBM} - {cliente.CNPJCPF} - {cliente.RazaoSocial}";
        }

        #endregion
    }
}