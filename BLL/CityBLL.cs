using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;

namespace Langben.BLL
{
    /// <summary>
    /// 缴纳地 
    /// </summary>
    public partial  class CityBLL : IBLL.ICityBLL, IDisposable
    {
        /// <summary>
        /// 私有的数据访问上下文
        /// </summary>
        protected SysEntities db;
        /// <summary>
        /// 缴纳地的数据库访问对象
        /// </summary>
        CityRepository repository = new CityRepository();
        /// <summary>
        /// 构造函数，默认加载数据访问上下文
        /// </summary>
        public CityBLL()
        {
            db = new SysEntities();
        }
        /// <summary>
        /// 已有数据访问上下文的方法中调用
        /// </summary>
        /// <param name="entities">数据访问上下文</param>
        public CityBLL(SysEntities entities)
        {
            db = entities;
        }
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="order">排序字段</param>
        /// <param name="sort">升序asc（默认）还是降序desc</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<City> GetByParam(string id, int page, int rows, string order, string sort, string search, ref int total)
        {

            
            IQueryable<City> queryData = repository.GetData(db, order, sort, search);
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryData = queryData.Take(rows);
                }
                else
                {
                    queryData = queryData.Skip((page - 1) * rows).Take(rows);
                }
                
                    foreach (var item in queryData) 
                    { 
                        if (item.PoliceAccountNature != null)
                        {
                            item.PoliceAccountNatureId = string.Empty;
                            foreach (var it in item.PoliceAccountNature)
                            {
                                item.PoliceAccountNatureId += it.Name + ' ';
                            }                         
                        } 

                    }
 
            }
            return queryData.ToList();
        }
                /// <summary>
        /// 查询的数据 /*在6.0版本中 新增*/
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="order">排序字段</param>
        /// <param name="sort">升序asc（默认）还是降序desc</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<City> GetByParam(string id, string order, string sort, string search)
        {
            IQueryable<City> queryData = repository.GetData(db, order, sort, search);
            
            return queryData.ToList();
        }
        /// <summary>
        /// 创建一个缴纳地
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="db">数据库上下文</param>
        /// <param name="entity">一个缴纳地</param>
        /// <returns></returns>
       public bool Create(ref ValidationErrors validationErrors, SysEntities db, City entity)
        {   
            int count = 1;
        
            foreach (string item in entity.PoliceAccountNatureId.GetIdSort())
            {
                PoliceAccountNature sys = new PoliceAccountNature { Id = Convert.ToInt32(item) };
                db.PoliceAccountNature.Attach(sys);
                entity.PoliceAccountNature.Add(sys);
                count++;
            }

            repository.Create(db, entity);
            if (count == repository.Save(db))
            {
                return true;
            }
            else
            {
                validationErrors.Add("创建出错了");
            }
            return false;
        }
        /// <summary>
        /// 创建一个缴纳地
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entity">一个缴纳地</param>
        /// <returns></returns>
        public bool Create(ref ValidationErrors validationErrors, City entity)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                { 
                    if (Create(ref validationErrors, db, entity))
                    {
                        transactionScope.Complete();
                        return true;
                    }
                    else
                    {
                        Transaction.Current.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);                
            }
            return false;
        }
        /// <summary>
        ///  创建缴纳地集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entitys">缴纳地集合</param>
        /// <returns></returns>
        public bool CreateCollection(ref ValidationErrors validationErrors, IQueryable<City> entitys)
        {
            try
            {
                if (entitys != null)
                {
                    int flag = 0, count = entitys.Count();
                    if (count > 0)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            foreach (var entity in entitys)
                            {
                                if (Create(ref validationErrors, db, entity))
                                {
                                    flag++;
                                }
                                else
                                {
                                    Transaction.Current.Rollback();
                                    return false;
                                }
                            }
                            if (count == flag)
                            {
                                transactionScope.Complete();
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);                
            }
            return false;
        }

      /// <summary>
        /// 删除一个缴纳地
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">一个缴纳地的主键</param>
        /// <returns></returns>  
        public bool Delete(ref ValidationErrors validationErrors, string id)
        {
            try
            {
                return repository.Delete(id) == 1;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);                
            }
            return false;
        }
        /// <summary>
        /// 删除缴纳地集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="deleteCollection">主键的缴纳地</param>
        /// <returns></returns>    
        public bool DeleteCollection(ref ValidationErrors validationErrors, string[] deleteCollection)
        {
            try
            {
                if (deleteCollection != null)
                { 

                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            repository.Delete(db, deleteCollection);
                            if (deleteCollection.Length == repository.Save(db))
                            {
                                transactionScope.Complete();
                                return true;
                            }
                            else
                            {
                                Transaction.Current.Rollback();
                            }
                        }
                    
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);                
            }
            return false;
        }
        /// <summary>
        ///  创建缴纳地集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entitys">缴纳地集合</param>
        /// <returns></returns>
        public bool EditCollection(ref ValidationErrors validationErrors, IQueryable<City> entitys)
        {
            if (entitys != null)
            {
                try
                {
                    int flag = 0, count = entitys.Count();
                    if (count > 0)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            foreach (var entity in entitys)
                            {
                                if (Edit(ref validationErrors, db, entity))
                                {
                                    flag++;
                                }
                                else
                                {
                                    Transaction.Current.Rollback();
                                    return false;
                                }
                            }
                            if (count == flag)
                            {
                                transactionScope.Complete();
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    validationErrors.Add(ex.Message);
                    ExceptionsHander.WriteExceptions(ex);                
                }
            }
            return false;
        }
        /// <summary>
        /// 编辑一个缴纳地
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="db">数据上下文</param>
        /// <param name="entity">一个缴纳地</param>
        /// <returns>是否编辑成功</returns>
       public bool Edit(ref ValidationErrors validationErrors, SysEntities db, City entity)
        {  /*                       
                           * 不操作 原有 现有
                           * 增加   原没 现有
                           * 删除   原有 现没
                           */
            if (entity == null)
            {
                return false;
            }
            int count = 1;            
            
            List<string> addPoliceAccountNatureId = new List<string>();
            List<string> deletePoliceAccountNatureId = new List<string>();
            DataOfDiffrent.GetDiffrent(entity.PoliceAccountNatureId.GetIdSort(), entity.PoliceAccountNatureIdOld.GetIdSort(), ref addPoliceAccountNatureId, ref deletePoliceAccountNatureId);
            List<PoliceAccountNature> listEntityPoliceAccountNature = new List<PoliceAccountNature>();
            if (deletePoliceAccountNatureId != null && deletePoliceAccountNatureId.Count() > 0)
            {                
                foreach (var item in deletePoliceAccountNatureId)
                {
                    PoliceAccountNature sys = new PoliceAccountNature { Id = Convert.ToInt32(item) };
                    listEntityPoliceAccountNature.Add(sys);
                    entity.PoliceAccountNature.Add(sys);
                }                
            } 

            City editEntity = repository.Edit(db, entity);
            
         
            if (addPoliceAccountNatureId != null && addPoliceAccountNatureId.Count() > 0)
            {
                foreach (var item in addPoliceAccountNatureId)
                {
                    PoliceAccountNature sys = new PoliceAccountNature { Id = Convert.ToInt32(item) };
                    db.PoliceAccountNature.Attach(sys);
                    editEntity.PoliceAccountNature.Add(sys);
                    count++;
                }
            }
            if (deletePoliceAccountNatureId != null && deletePoliceAccountNatureId.Count() > 0)
            { 
                foreach (PoliceAccountNature item in listEntityPoliceAccountNature)
                {
                    editEntity.PoliceAccountNature.Remove(item);
                    count++;
                }
            } 

            if (count == repository.Save(db))
            {
                return true;
            }
            else
            {
                validationErrors.Add("编辑缴纳地出错了");
            }
            return false;
        }
        /// <summary>
        /// 编辑一个缴纳地
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entity">一个缴纳地</param>
        /// <returns>是否编辑成功</returns>
        public bool Edit(ref ValidationErrors validationErrors, City entity)
        {           
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                { 
                    if (Edit(ref validationErrors, db, entity))
                    {
                        transactionScope.Complete();
                        return true;
                    }
                    else
                    {
                        Transaction.Current.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);                
            }
            return false;
        }
        public List<City> GetAll()
        {            
            return repository.GetAll(db).ToList();          
        }     
        
        /// <summary>
        /// 根据主键获取一个缴纳地
        /// </summary>
        /// <param name="id">缴纳地的主键</param>
        /// <returns>一个缴纳地</returns>
        public City GetById(string id)
        {          
            return repository.GetById(db, id);           
        }
        
        /// <summary>
        /// 获取在该表一条数据中，出现的所有外键实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>外键实体集合</returns>
        public List<PoliceAccountNature> GetRefPoliceAccountNature(string id)
        { 
            return repository.GetRefPoliceAccountNature(db, id).ToList();
        }
        /// <summary>
        /// 获取在该表中出现的所有外键实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>外键实体集合</returns>
        public List<PoliceAccountNature> GetRefPoliceAccountNature()
        { 
            return repository.GetRefPoliceAccountNature(db).ToList();
        }

        
        public void Dispose()
        {
           
        }
    }
}

