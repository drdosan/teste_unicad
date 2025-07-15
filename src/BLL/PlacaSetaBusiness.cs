
using System;
using System.Collections.Generic;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System.Linq;

namespace Raizen.UniCad.BLL
{
    public class PlacaSetaBusiness : UniCadBusinessBase<PlacaSeta>
    {
        public List<PlacaSeta> ListarAlteracoes(List<PlacaSeta> placaSolicitacaoSetas, List<PlacaSeta> placaOficialSetas)
        {
            //deverá ordernar as duas listas para equiparar seta com seta, caso de diferença no número de setas deverá 
            //acusar na validação da placa que é chamada antes desse método
            List<PlacaSeta> setas = new List<PlacaSeta>();
            List<PlacaSeta> setaOficial = placaOficialSetas.OrderBy(o => o.ID).ToList(); 
            int i = 0;
            foreach (var seta in placaSolicitacaoSetas.OrderBy(o => o.ID))
            {
                seta.isVolumeAlterado =
                            seta.VolumeCompartimento1 != setaOficial[i].VolumeCompartimento1
                        || seta.VolumeCompartimento2 != setaOficial[i].VolumeCompartimento2
                        || seta.VolumeCompartimento3 != setaOficial[i].VolumeCompartimento3
                        || seta.VolumeCompartimento4 != setaOficial[i].VolumeCompartimento4
                        || seta.VolumeCompartimento5 != setaOficial[i].VolumeCompartimento5
                        || seta.VolumeCompartimento6 != setaOficial[i].VolumeCompartimento6
                        || seta.VolumeCompartimento7 != setaOficial[i].VolumeCompartimento7
                        || seta.VolumeCompartimento8 != setaOficial[i].VolumeCompartimento8
                        || seta.VolumeCompartimento9 != setaOficial[i].VolumeCompartimento9
                        || seta.VolumeCompartimento10 != setaOficial[i].VolumeCompartimento10;

                seta.isPrincipalAlterado =
                            seta.CompartimentoPrincipal1 != setaOficial[i].CompartimentoPrincipal1
                        || seta.CompartimentoPrincipal2 != setaOficial[i].CompartimentoPrincipal2
                        || seta.CompartimentoPrincipal3 != setaOficial[i].CompartimentoPrincipal3
                        || seta.CompartimentoPrincipal4 != setaOficial[i].CompartimentoPrincipal4
                        || seta.CompartimentoPrincipal5 != setaOficial[i].CompartimentoPrincipal5
                        || seta.CompartimentoPrincipal6 != setaOficial[i].CompartimentoPrincipal6
                        || seta.CompartimentoPrincipal7 != setaOficial[i].CompartimentoPrincipal7
                        || seta.CompartimentoPrincipal8 != setaOficial[i].CompartimentoPrincipal8
                        || seta.CompartimentoPrincipal9 != setaOficial[i].CompartimentoPrincipal9
                        || seta.CompartimentoPrincipal10 != setaOficial[i].CompartimentoPrincipal10;
                i++;
                setas.Add(seta);
            }
            return setas;
        }
    }
}

