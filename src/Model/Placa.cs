
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;

using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;

namespace Raizen.UniCad.Model
{
    public class Placa : PlacaBase, ICloneable
    {
        [NotMapped]
        public int idUsuario {get;set; }

        [NotMapped]
        public int? idPlacaOficial { get; set; }

        [NotMapped]
        public int idTipoParteVeiculo { get; set; }

        [NotMapped]
        public int idTipoComposicao { get; set; }

        [NotMapped]
        public List<PlacaClienteView> Clientes { get; set; }

        [NotMapped]
        public List<PlacaSeta> Setas { get; set; }
        [NotMapped]
        public PlacaSeta SetaPrincipal { get; set; }

        [NotMapped]
        public List<PlacaDocumentoView> Documentos { get; set; }
        [NotMapped]
        public bool SomenteLiberacaoAcesso { get; set; }
        [NotMapped]
        public decimal Volume { get; set; }
        [NotMapped]
        public PlacaAlteradaView PlacaAlteracoes { get; set; }
        [NotMapped]
        public int Linha { get; set; }
        [NotMapped]
        public int LinhaNegocio { get; set; }
        [NotMapped]
        public Placa PlacaOficial { get; set; }
        [NotMapped]
        public List<CompartimentoView> Compartimentos { get; set; }
        [NotMapped]
        public string Uf { get; set; }
        [NotMapped]
        public int StatusComposicao { get; set; }
        [NotMapped]
        public int Numero {get;set; }
        [NotMapped]
        public bool IsInativar { get; set; }
        [NotMapped]
        public bool IsInativo { get; set; }
        [NotMapped]
        public bool somenteVisualizacao { get; set; }
        [NotMapped]
        public PlacaBrasil PlacaBrasil { get; set; }
        [NotMapped]
        public PlacaArgentina PlacaArgentina { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}


