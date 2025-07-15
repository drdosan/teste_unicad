using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;

namespace Raizen.UniCad.DAL.Interfaces
{
    public interface IPlacaDocumentoRepository : IRepository<PlacaDocumento>
    {
        List<PlacaDocumentoView> GetDocumentosAVencer(DateTime data);

        List<PlacaDocumentoView> GetDocumentosBloqueados(DateTime data);
    }
}
