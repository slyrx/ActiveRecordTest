using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.Collections;
using Castle.ActiveRecord.Queries;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries.Modifiers;
using NHibernate.Exceptions;
using NHibernate.Criterion;
using Castle.ActiveRecord.Framework;
using NHibernate.Cfg;

namespace ActiveRecordTest.Model
{

    public class DAO<T> : ActiveRecordBase<T>
    {
        private static string LogTag = "ActiveRecordTest.Model.DAO";

        /// <summary>
        /// 获得一个对象实体
        /// </summary>
        /// <param name="id">ID主键</param>
        /// <returns>对象实体</returns>
        #region T DAO_GetModelById(int id)
        public static T DAO_GetModelById(int id)
        {
            try
            {
                return DAO<T>.Find(id);
            }
            catch (Exception ex)
            {
                ////LogHelper.WriteLog(LogTag + ".FindById(int id)", ex);
                return default(T);
            }
        }
        #endregion

        /// <summary>
        /// 获得一个对象实体
        /// </summary>
        /// <param name="Field_Name">参数名</param>
        /// <param name="Field_Content">参数值</param>
        /// <returns>对象实体</returns>
        #region T DAO_FindFirst(string Field_Name, string Field_Content)
        public static T DAO_FindFirst(string Field_Name, string Field_Content)
        {
            try
            {
                return DAO<T>.FindFirst(Expression.Eq(Field_Name, Field_Content));
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".DAO_FindFirst(string Field_Name, string Field_Content)", ex);
                return default(T);
            }
        }
        public static T DAO_FindFirst(string Field_Name, int Field_Content)
        {
            try
            {
                return DAO<T>.FindFirst(Expression.Eq(Field_Name, Field_Content));
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".DAO_FindFirst(string Field_Name, string Field_Content)", ex);
                return default(T);
            }
        }

        /// <summary>
        /// 获得一个对象实体
        /// </summary>
        /// <param name="criteria">条件集合</param>
        /// <returns></returns>
        public static T DAO_FindFirst(params ICriterion[] criteria)
        {
            try
            {
                return DAO<T>.FindFirst(criteria);
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".DAO_FindFirst(string Field_Name, string Field_Content)", ex);
                return default(T);
            }
        }
        #endregion

        /// <summary>
        /// 添加记录
        /// </summary>
        #region bool DAO_Add(T entity)
        public static bool DAO_Add(T entity)
        {
            try
            {
                ActiveRecordBase<T>.Save(entity);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".DAO_Add(T entity) ", ex);
                return false;
            }
        }
        #endregion


        /// <summary>
        /// 保存记录
        /// </summary>
        #region bool DAO_Update((T entity)
        public static bool DAO_Update(T entity)
        {
            try
            {
                ActiveRecordBase<T>.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".Update((T entity) ", ex);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <returns></returns>
        #region Delete
        public static bool DAO_Delete(int id)
        {
            try
            {
                ;
                ActiveRecordBase<T>.Delete(DAO<T>.Find(id));
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".Delete(int id)", ex);
                return false;
            }
        }
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="where">条件删除</param>
        /// <returns></returns>
        public static bool DAO_Delete(string where)
        {
            try
            {
                DeleteAll(where);
                return true;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".Delete(string where)", ex);
                return false;
            }
        }
        #endregion


        /// <summary>
        /// 获得一个统计值
        /// </summary>
        /// <param name="Hql"></param>
        /// <param name="paramList"></param>
        /// <returns>数字</returns>
        #region int DAO_GetCount(string where)
        public static int DAO_GetCount(string where)
        {
            try
            {
                T obj = (T)System.Reflection.Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).ToString());
                string hql = "";
                if (where.Trim() == String.Empty)
                {
                    hql = string.Format(" select count(*) from {0}",
                        obj.GetType().ToString());
                }
                else
                {
                    hql = string.Format(" select count(*) from {0} {1}",
                        obj.GetType().ToString(),
                        where.ToUpper().StartsWith("WHERE") ? where : "WHERE " + where);
                }


                ScalarQuery query = new ScalarQuery(typeof(T), hql);
                return Convert.ToInt32(ExecuteQuery(query));
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".GetNum(string Hql, ArrayList paramList)", ex);
                return -1;
            }
        }
        #endregion

        /// <summary>
        /// 执行HQL语句
        /// </summary>
        /// <param name="Hql"></param>
        /// <returns>数字</returns>
        #region int DAO_ExecuteSql(string Hql)
        public static int DAO_ExecuteSql(string Hql)
        {
            try
            {
                ScalarQuery scalarQuery = new ScalarQuery(typeof(T), Hql);
                object o = ExecuteQuery(scalarQuery);
                return int.Parse(o.ToString());
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".GetNum(string Hql, ArrayList paramList)", ex);
                return -1;
            }
        }
        #endregion


        /// <summary>
        /// 根据属性查询对象
        /// </summary>
        /// <param name="parameters">属性集合</param>
        /// <param name="orders">排序集合</param>
        /// <param name="BeginNO">开始记录号</param>
        /// <param name="EndNO">结束记录号</param>
        /// <returns>对象集合</returns>
        #region IList<T> DAO_GetListByProperty(IList<QueryParameter> parameters, IList<QueryOrder> orders, int BeginNO, int EndNO)
        public static IList<T> DAO_GetList(String where, String order, int BeginNO, int EndNO)
        {
            try
            {
                T obj = (T)System.Reflection.Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).ToString());
                string hql = "";
                if (where.Trim() == String.Empty)
                {
                    hql = string.Format("from {0}",
                        obj.GetType().ToString());
                }
                else
                {
                    hql = string.Format("from {0} {1}",
                        obj.GetType().ToString(),
                        where.ToUpper().StartsWith("WHERE") ? where : "WHERE " + where);
                }

                if (order != String.Empty) hql += " " + order;

                SimpleQuery simpleQuery = new SimpleQuery(typeof(T), hql);
                simpleQuery.SetQueryRange(BeginNO, EndNO);
                return (IList<T>)ExecuteQuery(simpleQuery);
            }

            catch (Exception ex)
            {
                //LogHelper.WriteLog(LogTag + ".GetListByProperty(IList<QueryParameter> parameters, IList<QueryOrder> orders)", ex);
                return default(IList<T>);
            }
        }
        #endregion


        /// <summary>
        /// HQL查询分页
        /// </summary>
        /// <param name="baseQuery">HQL</param>
        /// <param name="queryParameters">参数对象集合</param>
        /// <param name="orders">排序对象集合</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="rowCount">返回记录总数 </param>
        /// <param name="resultPage">返回 当前页码 </param>
        /// <returns>对象集合</returns>
        #region IList<T> DAO_GetListByPage(String where, String order, int pageSize, int currentPage, out int rowCount, out int resultPage)
        public static IList<T> DAO_GetListByPage(String where, String order, int pageSize, int currentPage, out int rowCount, out int resultPage)
        {
            try
            {
                T obj = (T)System.Reflection.Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).ToString());
                string hqlCount = "";
                if (where.Trim() == String.Empty)
                {
                    hqlCount = string.Format("select count(*) from {0}",
                        obj.GetType().ToString());
                }
                else
                {
                    hqlCount = string.Format("select count(*) from {0} {1}",
                        obj.GetType().ToString(),
                        where.ToUpper().StartsWith("WHERE") ? where : "WHERE " + where);
                }
                ScalarQuery scalarQuery = new ScalarQuery(typeof(T), hqlCount);
                object o = ExecuteQuery(scalarQuery);
                rowCount = int.Parse(o.ToString());



                IList<T> result = new List<T>();
                string hql = "";
                if (where.Trim() == String.Empty)
                {
                    hql = string.Format("from {0}",
                        obj.GetType().ToString());
                }
                else
                {
                    hql = string.Format("from {0} {1}",
                        obj.GetType().ToString(),
                        where.ToUpper().StartsWith("WHERE") ? where : "WHERE " + where);
                }

                if (order != String.Empty) hql += " " + order;

                SimpleQuery simpleQuery = new SimpleQuery(typeof(T), hql);
                resultPage = currentPage > 0 ? currentPage - 1 : 0;
                int pageCount = (rowCount - 1) / pageSize + 1;
                if (resultPage < 0)
                {
                    resultPage = 0;
                }
                if (resultPage >= pageCount)
                {
                    resultPage = pageCount - 1;
                }
                int startRow = resultPage * pageSize;
                if (startRow >= rowCount)
                {
                    startRow = rowCount - 1;
                }
                simpleQuery.SetQueryRange(startRow, pageSize);

                return (IList<T>)ExecuteQuery(simpleQuery);
            }

            catch (Exception ex)
            {
                rowCount = 0;
                resultPage = 0;
                //LogHelper.WriteLog(LogTag + ".GetListByProperty(string baseQuery, object[] queryParameters, IList<QueryOrder> orders, int pageSize, int currentPage, out int rowCount, out int resultPage)", ex);
                return default(IList<T>);
            }
        }
        #endregion

    }

}
