using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.Data.Entity;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Raizen.UniCad.BLL.Util;
using System.Data.SqlClient;
using System.Data;

namespace Raizen.UniCad.BLL
{
    public class LogDocumentosBusiness : UniCadBusinessBase<LogDocumentos>
    {
        public List<LogDocumentosView> ListarLogDocumentos(LogDocumentosFiltro filtro, PaginadorModel paginador)
        {

            using (UniCadDalRepositorio<LogDocumentos> repositorio = new UniCadDalRepositorio<LogDocumentos>())
            {
                IQueryable<LogDocumentosView> query = GetQueryLogDocumentos(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderByDescending(i => i.Data)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }

        }

        public int ListarLogDocumentosCount(LogDocumentosFiltro filtro)
        {

            using (UniCadDalRepositorio<LogDocumentos> repositorio = new UniCadDalRepositorio<LogDocumentos>())
            {
                IQueryable<LogDocumentosView> query = GetQueryLogDocumentos(filtro, repositorio);
                return query.Count();
            }

        }

        private IQueryable<LogDocumentosView> GetQueryLogDocumentos(LogDocumentosFiltro filtro, IUniCadDalRepositorio<LogDocumentos> repositorio)
        {
            IQueryable<LogDocumentosView> query = (from app in repositorio.ListComplex<LogDocumentos>().AsNoTracking().OrderByDescending(i => i.Data)
                                                   where
                                                   (string.IsNullOrEmpty(filtro.Nome) || app.Nome.Contains(filtro.Nome))
                                                   && (string.IsNullOrEmpty(filtro.Email) || app.Email.Contains(filtro.Email))
                                                   && (string.IsNullOrEmpty(filtro.Mensagem) || app.Mensagem.Contains(filtro.Mensagem))
                                                   && (!filtro.DataInicio.HasValue || app.Data >= filtro.DataInicio)
                                                   && (!filtro.DataFim.HasValue || app.Data <= filtro.DataFim)
                                                   select new LogDocumentosView
                                                   {
                                                       ID = app.ID,
                                                       Email = app.Email,
                                                       Mensagem = app.Mensagem,
                                                       Data = app.Data,
                                                       Nome = app.Nome
                                                   });
            
            return query;
        }
    }
}

