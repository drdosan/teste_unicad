using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using System.Data;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    /// <summary>
    /// Classe business base do projeto, já realiza as operações de CRUD por Generics
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UniCadBusinessBase<T> : BaseBusiness where T : class
    {
        #region Adicionar

        public virtual bool Adicionar(T model)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return (repositorio.Add(model) > 0);
            }
        }

        #endregion

        #region Atualizar

        public virtual bool Atualizar(T model)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return (repositorio.Update(model) > 0);
            }
        }

        #endregion

        #region Excluir

        public virtual bool Excluir(T model)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return (repositorio.Delete(model) > 0);
            }
        }

        public virtual bool Excluir(int id)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return (repositorio.Delete(id) > 0);
            }
        }

        public virtual bool ExcluirLista(Expression<Func<T, bool>> where)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return (repositorio.DeleteList(where) > 0);
            }
        }

        #endregion

        #region Selecionar

        public virtual T Selecionar(int id)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return repositorio.Get(id);
            }
        }

        public virtual T Selecionar(Expression<Func<T, bool>> where)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return repositorio.Get(where);
            }
        }

        #endregion

        #region Existe


        public virtual bool Existe(Expression<Func<T, bool>> where)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return repositorio.Any(where);
            }
        }

        #endregion

        #region Listar

        public virtual List<T> Listar()
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return repositorio.List();
            }
        }

        public virtual List<T> ExecutarProcedureComRetorno(string procedure, object[] parametros)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>())
            {
                return repositorio.ExecutarProcedureComRetorno(procedure, parametros);
            }
        }

        public virtual DataTable ExecutarProcedureComRetornoDataTable(string procedure, SqlParameter[] parametros)
        {
            using (UniCadDalRepositorio<DataTable> repositorio = new UniCadDalRepositorio<DataTable>())
            {
                return repositorio.ExecutarProcedureComRetornoDataTable(procedure, parametros);
            }
        }

        public virtual List<T> ExecutarProcedureComRetorno<T>(string procedure, object[] parametros) where T : class
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>())
            {
                return repositorio.ExecutarProcedureComRetorno<T>(procedure, parametros);
            }
        }

        public virtual int ExecutarProcedureComRetornoInteiro(string procedure, object[] parametros)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>())
            {
                return repositorio.ExecutarProcedureComRetornoInteiro(procedure, parametros);
            }
        }

        public virtual List<T> Listar(Expression<Func<T, bool>> where)
        {
            using (UniCadDalRepositorio<T> repositorio = new UniCadDalRepositorio<T>("UniCadContext"))
            {
                return repositorio.List(where);
            }
        }
        #endregion
    }
}
