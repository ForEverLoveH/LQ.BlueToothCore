﻿using FreeSql;
using LQ.BlueToothCore.IService.IService.IFreeSqlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LQ.BlueToothCore.Service.Service.FreeSqlService
{
    /// <summary>
    /// 
    /// </summary>
    public class FreeSqlServer:IFreeSqlServer
    {
        private readonly IFreeSql _freeSql = BaseServer.sqlite;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Find<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            return _freeSql.Select<T>().Where(expression).ToOne();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSelect"></typeparam>
        /// <returns></returns>
        public List<TSelect> Find<T, TSelect>(Expression<Func<T, TSelect>> expression) where T : class
        {
            return _freeSql.Select<T>().Distinct().ToList<TSelect>(expression);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<dynamic> Find<T>(Expression<Func<T, dynamic>> expression) where T : class
        {
            return _freeSql.Select<T>().Distinct().ToList<dynamic>(expression);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="express"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        public void FindAggregate<T>(Expression<Func<ISelectGroupingAggregate<T>, object>> express, out object key) where T : class
        {
            _freeSql.Select<T>().Aggregate<object>(express, out key);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<dynamic> FindDistinct<T>(Expression<Func<T, dynamic>> expression) where T : class
        {
            return _freeSql.Select<T>().Distinct().ToList<dynamic>(expression);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> FindAsync<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            return await _freeSql.Select<T>().Where(expression).ToOneAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Orderexp"></param>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> FindList<T>(Expression<Func<T, object>> Orderexp = null, Expression<Func<T, bool>> expression = null) where T : class
        {
            return expression == null ? _freeSql.Select<T>().OrderBy(Orderexp).ToList() : _freeSql.Select<T>().Where(expression).OrderBy(Orderexp).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Orderexp"></param>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1">排序语句</param>
        /// <param name="expression">查询语句</param>
        /// <returns></returns>
        public async Task<List<T>> FindListAsync<T>(Expression<Func<T, object>> expression1 = null, Expression<Func<T, bool>> expression = null) where T : class
        {
            if (expression == null && expression1 != null) return await _freeSql.Select<T>().OrderBy(expression1).ToListAsync();
            else if(expression1 != null && expression1 != null)
            {
                return await _freeSql.Select<T>().Where(expression).OrderBy(expression1).ToListAsync();
            }
            else
            {
                return await _freeSql.Select<T>().ToListAsync();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Insert<T>(T entity) where T : class
        {
            return _freeSql.Insert<T>(entity).ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(T entity) where T : class
        {
            return await _freeSql.Insert<T>(entity).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Insert<T>(List<T> entities) where T : class
        {
            return _freeSql.Insert(entities).ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(List<T> entity) where T : class
        {
            return await _freeSql.Insert<T>(entity).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int InsertOrUpdate<T>(T entity) where T : class
        {
            return _freeSql.InsertOrUpdate<T>().SetSource(entity).IfExistsDoNothing().ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> InsertOrUpdateAsync<T>(T entity) where T : class
        {
            return await _freeSql.InsertOrUpdate<T>().SetSource(entity).IfExistsDoNothing().ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int InsertOrUpdate<T>(List<T> entity) where T : class
        {
            return _freeSql.InsertOrUpdate<T>().SetSource(entity).IfExistsDoNothing().ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> InsertOrUpdateAsync<T>(List<T> entity) where T : class
        {
            return await _freeSql.InsertOrUpdate<T>().SetSource(entity).IfExistsDoNothing().ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Update<T>(T entity) where T : class
        {
            return _freeSql.Update<T>(entity).ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(T entity) where T : class
        {
            return await _freeSql.Update<T>(entity).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="expression1"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Update<T>(Expression<Func<T, bool>> expression, Expression<Func<T, bool>> expression1) where T : class
        {
            return _freeSql.Update<T>().Set(expression).Where(expression1).ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="expression1"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> expression, Expression<Func<T, bool>> expression1) where T : class
        {
            return await _freeSql.Update<T>().Set(expression).Where(expression1).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Delete<T>(T entity) where T : class
        {
            return _freeSql.Delete<T>(entity).ExecuteAffrows();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(T entity) where T : class
        {
            return await _freeSql.Delete<T>(entity).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Delete<T>(Expression<Func<T, bool>> expression) where T : class
        {
            T t = Find<T>(expression);
            return Delete<T>(t);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression) where T : class
        {
            T t = Find<T>(expression);
            return await DeleteAsync<T>(t);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExcuteSql<T>(string sql, params object[] parameters) where T : class
        {
            return _freeSql.Ado.Query<T>(sql, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExcuteSqlAsync<T>(string sql, params object[] parameters) where T : class
        {
            return await _freeSql.Ado.ExecuteNonQueryAsync(sql, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExcuteSql(string sql, params object[] parameters)
        {
            return _freeSql.Ado.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete<T>(string sql) where T : class
        {
            return _freeSql.Delete<T>().Where(sql).ExecuteAffrows();
        }
    }
}
