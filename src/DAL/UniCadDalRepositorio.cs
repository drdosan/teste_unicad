using Raizen.Framework.Models;
using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.CodeFirst.ConfiguracaoModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using System.Data.Entity.Infrastructure;

namespace Raizen.UniCad.DAL
{
    public sealed class UniCadDalRepositorio<T> : Disposable, IUniCadDalRepositorio<T> where T : class
    {
        private readonly UniCadDalCodeFirst<T> _repositorioCodeFirst = null;
        private readonly int _timeOut = 1800;

        public UniCadDalRepositorio(int commandTimeout)
        {
            UniCadContexto contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado(), commandTimeout);
#if DEBUG
            contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
            this._repositorioCodeFirst = new UniCadDalCodeFirst<T>(contexto);
        }

        public UniCadDalRepositorio()
        {
            UniCadContexto contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado(), _timeOut);
#if DEBUG
            contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
            this._repositorioCodeFirst = new UniCadDalCodeFirst<T>(contexto);
        }

        public UniCadDalRepositorio(string nameConnectionString)
        {
            UniCadContexto contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado(), _timeOut);
#if DEBUG
            contexto.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
            this._repositorioCodeFirst = new UniCadDalCodeFirst<T>(contexto);
        }

        public int Add(T model)
        {
            return _repositorioCodeFirst.Add(model);
        }

        public int Update(T model)
        {
            return _repositorioCodeFirst.Update(model);
        }

        public int Delete(T model)
        {
            return _repositorioCodeFirst.Delete(model);
        }

        public int Delete(long Id)
        {
            return _repositorioCodeFirst.Delete(Id);
        }

        public int DeleteList(Expression<Func<T, bool>> where)
        {
            return _repositorioCodeFirst.DeleteList(where);
        }

        public T Get(long Id)
        {
            return _repositorioCodeFirst.Get(Id);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return _repositorioCodeFirst.Get(where);
        }

        public bool Any(Expression<Func<T, bool>> where)
        {
            return _repositorioCodeFirst.Any(where);
        }

        public bool Any(long Id)
        {
            return _repositorioCodeFirst.Any(Id);
        }

        public List<T> List()
        {
            return _repositorioCodeFirst.List();
        }

        public List<T> List(PaginadorModel paginador, Expression<Func<T, string>> order)
        {
            return _repositorioCodeFirst.List(paginador, order);
        }

        public List<T> List(Expression<Func<T, bool>> where)
        {
            return _repositorioCodeFirst.List(where);
        }

        public List<T> List(Expression<Func<T, bool>> where, Expression<Func<T, string>> order, PaginadorModel paginador)
        {
            return _repositorioCodeFirst.List(where, order, paginador);
        }

        public DbSet<TElement> ListComplex<TElement>() where TElement : class
        {
            return _repositorioCodeFirst.ListComplex<TElement>();
        }

        public void BulkInsert<TElement>(IList<TElement> dadosLista, string nomeTabelaDestino) where TElement : class
        {
            _repositorioCodeFirst.BulkInsert<TElement>(dadosLista, nomeTabelaDestino);
        }

        protected override void DisposeCore()
        {
            if (_repositorioCodeFirst != null)
            {
                _repositorioCodeFirst.Dispose();
            }
        }

        public int Count()
        {
            return _repositorioCodeFirst.ListComplex<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> where)
        {
            return _repositorioCodeFirst.ListComplex<T>().Where(where).Count();
        }

        public List<T> ExecutarProcedureComRetorno(string procedure, object[] parametros)
        {
            using (UniCadContexto contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado()))
            {
                return contexto.Database.SqlQuery<T>(procedure, parametros).ToList();
            }
        }

        public DataTable ExecutarProcedureComRetornoDataTable(string procedure, SqlParameter[] parametros)
        {
            using (SqlConnection conn = ConfigBuilder.GetConnection())
            {
                using (var cmd = new SqlCommand(procedure, conn))
                {
                    DataTable dataTable = new DataTable();
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var param in parametros)
                    {
                        cmd.Parameters.Add(param.ParameterName, param.SqlDbType).Value = param.Value;
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public List<T> ExecutarProcedureComRetorno<T>(string procedure, object[] parametros) where T : class
        {
            using (UniCadContexto contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado()))
            {
                var procedureResult = contexto.Database.SqlQuery<T>(procedure, parametros).ToList();
                return procedureResult;
            }
        }

        public List<T> ExecutarProcedureComRetornoD(string procedure, SqlParameter[] parametros)
        {
            using (SqlConnection conexao = ConfigBuilder.GetConnection())
            {
                conexao.Open();
                var p = new DynamicParameters();
                foreach (var t in parametros)
                {
                    p.Add(t.ParameterName, t.Value == (object)DBNull.Value ? null : t.Value);
                }
                var retorno = conexao.Query<T>(procedure, parametros, commandType: CommandType.StoredProcedure).ToList();
                conexao.Close();
                return retorno;
            }
        }

        public List<T> ExecutarProcedureComRetornoD<T>(string procedure, SqlParameter[] parametros) where T : class
        {
            using (var conexao = ConfigBuilder.GetConnection())
            {
                var p = new DynamicParameters();
                foreach (var t in parametros)
                {
                    p.Add(t.ParameterName, t.Value == (object)DBNull.Value ? null : t.Value);
                }
                conexao.Open();
                var retorno = conexao.Query<T>(procedure, p, commandType: CommandType.StoredProcedure).ToList();
                conexao.Close();
                return retorno;
            }
        }


        public int ExecutarProcedureComRetornoInteiro(string procedure, object[] parametros)
        {
            using (UniCadContexto contexto = new UniCadContexto(ConfigBuilder.GetConnection(), ConfigBuilder.GetModeloCompilado()))
            {
                return contexto.Database.SqlQuery<int>(procedure, parametros).First();
            }
        }
    }
}
