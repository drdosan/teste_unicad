using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Raizen.UniCad.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Adiciona a entidade
        /// </summary>
        void Adicionar(TEntity obj);

        /// <summary>
        /// Adiciona uma lista de entidades
        /// </summary>
        void AdicionarLista(IEnumerable<TEntity> lstObj);

        /// <summary>
        /// Exclui a entidade
        /// </summary>
        void Excluir(TEntity obj);

        /// <summary>
        /// Exclui lista de entidades
        /// </summary>
        /// <param name="obj"></param>
        void ExcluirLista(IEnumerable<TEntity> obj);

        /// <summary>
        /// Edita a entidade
        /// </summary>
        void Atualizar(TEntity obj);

        /// <summary>
        /// Obtém a entidade por Id
        /// </summary>
        TEntity Selecionar(int id);

        /// <summary>
        /// Obtem a entidade pelo expressão do where informada
        /// </summary>
        TEntity Selecionar(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// Obtem a entidade pelo expressão do where informada e permite desabilitar o rastreamento do ORM
        /// </summary>
        TEntity Selecionar(Expression<Func<TEntity, bool>> where, bool asNoTracking);

        /// <summary>
        /// Retorna uma lista de entidades
        /// </summary>
        IEnumerable<TEntity> SelecionarLista();

        /// <summary>
        /// Obtem a lista da entidade pelo expressão do where informada
        /// </summary>
        IEnumerable<TEntity> SelecionarLista(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// Obtem a lista da entidade pelo expressão do where informada e permite desabilitar o rastreamento do ORM
        /// </summary>
        IEnumerable<TEntity> SelecionarLista(Expression<Func<TEntity, bool>> where, bool asNoTracking);

        /// <summary>
        /// Obtem a lista da entidade pelo expressão do where e ordem
        /// </summary>
        IEnumerable<TEntity> SelecionarLista<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order);

        /// <summary>
        /// Obtem a lista da entidade pelo expressão do where e ordem e permite desabilitar o rastreamento do ORM
        /// </summary>
        IEnumerable<TEntity> SelecionarLista<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order, bool asNoTracking);

        /// <summary>
        /// Verifica se a entidade existe com o where informado
        /// </summary>
        bool Exists(Expression<Func<TEntity, bool>> where);
    }
}
