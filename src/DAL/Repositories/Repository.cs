using Raizen.UniCad.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Raizen.UniCad.DAL.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _contexto;

        protected Repository(DbContext contexto)
        {
            _contexto = contexto;
        }

        public DbContext DbContext
        {
            get { return _contexto; }
        }

        public void Adicionar(TEntity obj)
        {
            DbContext.Set<TEntity>().Add(obj);
            DbContext.SaveChanges();
        }

        public void AdicionarLista(IEnumerable<TEntity> lstObj)
        {
            DbContext.Set<TEntity>().AddRange(lstObj);
            DbContext.SaveChanges();
        }

        public void Excluir(TEntity obj)
        {
            DbContext.Set<TEntity>().Remove(obj);
            DbContext.SaveChanges();
        }

        public void ExcluirLista(IEnumerable<TEntity> obj)
        {
            DbContext.Set<TEntity>().RemoveRange(obj);
            DbContext.SaveChanges();
        }

        public virtual void Atualizar(TEntity obj)
        {
            var entry = DbContext.Entry(obj);
            DbContext.Set<TEntity>().Attach(obj);
            entry.State = EntityState.Modified;
            DbContext.SaveChanges();
        }

        public TEntity Selecionar(int id)
        {
            return DbContext.Set<TEntity>().Find(id);
        }

        public TEntity Selecionar(Expression<Func<TEntity, bool>> where)
        {
            return Selecionar(where, false);
        }

        public TEntity Selecionar(Expression<Func<TEntity, bool>> where, bool asNoTracking)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.Where(where).FirstOrDefault();
        }

        public IEnumerable<TEntity> SelecionarLista()
        {
            return DbContext.Set<TEntity>().ToArray();
        }

        public IEnumerable<TEntity> SelecionarLista(Expression<Func<TEntity, bool>> where)
        {
            return SelecionarLista(where, false);
        }

        public IEnumerable<TEntity> SelecionarLista(Expression<Func<TEntity, bool>> where, bool asNoTracking)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.Where(where).ToArray();
        }

        public IEnumerable<TEntity> SelecionarLista<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order)
        {
            return SelecionarLista(where, order, false);
        }

        public IEnumerable<TEntity> SelecionarLista<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order, bool asNoTracking)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.Where(where).OrderBy(order).ToArray();
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> where)
        {
            return DbContext.Set<TEntity>().Where(where).Any();
        }
    }
}
