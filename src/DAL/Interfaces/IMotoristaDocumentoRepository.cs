using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;

namespace Raizen.UniCad.DAL.Interfaces
{
    public interface IMotoristaDocumentoRepository : IRepository<MotoristaDocumento>
    {
        List<MotoristaDocumentoView> GetDocumentosAVencer(DateTime data);

        List<MotoristaDocumentoView> GetDocumentosBloqueados(DateTime data);
    }
}
