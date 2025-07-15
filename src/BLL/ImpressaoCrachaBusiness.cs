using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Raizen.UniCad.BLL.Util;
using System.Data.SqlClient;
using System.Data;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.DAL.Repositories;

namespace Raizen.UniCad.BLL
{
    public class ImpressaoCrachaBusiness : IDisposable
    {
        bool disposed;
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        private readonly IMotoristaDocumentoRepository _motoristaDocumentoRepository;

        public ImpressaoCrachaBusiness()
        {
            _motoristaDocumentoRepository = new MotoristaDocumentoRepository(Config.GetContext());
        }
        

        public MotoristaView BuscarMotorista(ImpressaoCrachaFiltro filtro, bool isEditar = false)
        {
            filtro.CPF = filtro.CPF.RemoveCharacter();
            DateTime dataHoje = DateTime.Now.Date;
            MotoristaFiltro motoFiltro = new MotoristaFiltro
            {
                CPF = filtro.CPF,
            };

            var motorista = _motoristaBll.ListarMotorista(motoFiltro).FirstOrDefault();

            return motorista;
        }


        public MotoristaView BuscarMotoristaArgentina(ImpressaoCrachaFiltro filtro, bool isEditar = false)
        {
            filtro.DNI = filtro.DNI.RemoveCharacter();
            DateTime dataHoje = DateTime.Now.Date;
            MotoristaFiltro motoFiltro = new MotoristaFiltro
            {
                DNI = filtro.DNI,
            };

            var motorista = _motoristaBll.ListarMotorista(motoFiltro).FirstOrDefault();

            return motorista;
        }

        public ImpressaoCrachaRetornoView ValidarMotoristaImpressaoCracha(MotoristaView motorista)
        {
            switch (motorista.IDStatus)
            {
                case (int)EnumStatusMotorista.EmAprovacao:
                    return new ImpressaoCrachaRetornoView { IdMotorista = 0, NomeMotorista = "",IDStatus = motorista.IDStatus, MensagemSituacao = "N�o foi poss�vel imprimir o crach�, pois o cadastro est� aguardando Aprova��o.", AptoParaImpressaoDeCracha = false };
                case (int)EnumStatusMotorista.Reprovado:
                    return new ImpressaoCrachaRetornoView { IdMotorista = 0, NomeMotorista = "", IDStatus = motorista.IDStatus, MensagemSituacao = "N�o foi poss�vel imprimir o crach�, pois o cadastro est� Reprovado.", AptoParaImpressaoDeCracha = false, Justificativa = motorista.Justificativa };
                case (int)EnumStatusMotorista.Bloqueado:
                    return new ImpressaoCrachaRetornoView { IdMotorista = 0, NomeMotorista = "", IDStatus = motorista.IDStatus, MensagemSituacao = "N�o foi poss�vel realizar a impress�o do motorista solicitado. Por gentileza, entrar em contato com o Time Raizen.", AptoParaImpressaoDeCracha = false};
                default:
                    #region Verificar documentos

                    List<MotoristaDocumentoView> listaDocumentosMotorista = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(motorista.ID).ToList();

                    if (listaDocumentosMotorista.Where(d => d.DataVencimento < DateTime.Today).Any())
                    {
                        return new ImpressaoCrachaRetornoView
                        {
                            IdMotorista = 0,
                            NomeMotorista = "",
                            IDStatus = motorista.IDStatus,
                            MensagemSituacao = "N�o foi poss�vel imprimir o crach�, porque existem documentos vencidos:" ,
                            AptoParaImpressaoDeCracha = false,
                            Documentos = listaDocumentosMotorista.Where(d => d.DataVencimento < DateTime.Today).ToList()
                        };

                    }
                    #endregion
                    break;
            }

            return new ImpressaoCrachaRetornoView
            {
                IdMotorista = motorista.ID,
                NomeMotorista = motorista.Nome,
                IDStatus = motorista.IDStatus,
                MensagemSituacao = @"Para que o CART�O DE IDENTIFICA��O seja v�lido � necess�rio que esteja impresso, com foto 3x4 e assinado pelo transportador e motorista.
                            Em seguida,
                                            dever� ser apresentado na base para que seja validado e assinado pelo representante Ra�zen.Atentar que o uso do CART�O DE
                               IDENTIFICA��O � obrigat�rio para acesso � base
                            ",
                AptoParaImpressaoDeCracha = true
            };

        }


        public ImpressaoCrachaRetornoView ValidarMotoristaArgentinoImpressaoCracha(MotoristaView motorista)
        {
            switch (motorista.IDStatus)
            {
                case (int)EnumStatusMotorista.EmAprovacao:
                    return new ImpressaoCrachaRetornoView { IdMotorista = 0, NomeMotorista = "", IDStatus = motorista.IDStatus, MensagemSituacao = "No fue posible imprimir la placa, ya que el registro est� pendiente de aprobaci�n.", AptoParaImpressaoDeCracha = false };
                case (int)EnumStatusMotorista.Reprovado:
                    return new ImpressaoCrachaRetornoView { IdMotorista = 0, NomeMotorista = "", IDStatus = motorista.IDStatus, MensagemSituacao = "No fue posible imprimir la placa, porque el registro est� Desaprobado.", AptoParaImpressaoDeCracha = false, Justificativa = motorista.Justificativa };
                case (int)EnumStatusMotorista.Bloqueado:
                    return new ImpressaoCrachaRetornoView { IdMotorista = 0, NomeMotorista = "", IDStatus = motorista.IDStatus, MensagemSituacao = "No fue posible imprimir el conductor solicitado. Comun�quese con el equipo Raizen.", AptoParaImpressaoDeCracha = false };
                default:
                    #region Verificar documentos

                    List<MotoristaDocumentoView> listaDocumentosMotorista = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(motorista.ID).ToList();

                    if (listaDocumentosMotorista.Where(d => d.DataVencimento < DateTime.Today).Any())
                    {
                        return new ImpressaoCrachaRetornoView
                        {
                            IdMotorista = 0,
                            NomeMotorista = "",
                            IDStatus = motorista.IDStatus,
                            MensagemSituacao = "No fue posible imprimir la placa, porque hay documentos vencidos:",
                            AptoParaImpressaoDeCracha = false,
                            Documentos = listaDocumentosMotorista.Where(d => d.DataVencimento < DateTime.Today).ToList()
                        };

                    }
                    #endregion
                    break;
            }

            return new ImpressaoCrachaRetornoView
            {
                IdMotorista = motorista.ID,
                NomeMotorista = motorista.Nome,
                IDStatus = motorista.IDStatus,
                MensagemSituacao = @"Para que la TARJETA DE IDENTIFICACI�N sea v�lida, debe estar impresa, con foto 3x4 y firmada por el transportista y el conductor.
                            En seguida,
                                            debe ser presentado sobre la base para que pueda ser validado y firmado por el representante de Ra�zen. Intente que el uso del
                               La IDENTIFICACI�N es obligatoria para acceder a la base.
                            ",
                AptoParaImpressaoDeCracha = true
            };

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _motoristaBll.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

