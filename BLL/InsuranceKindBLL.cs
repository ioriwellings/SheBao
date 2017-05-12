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
    /// 社保种类 
    /// </summary>
    public partial  class InsuranceKindBLL : IBLL.IInsuranceKindBLL, IDisposable
    {
        /// <summary>
        /// 私有的数据访问上下文
        /// </summary>
        protected SysEntities db;
        /// <summary>
        /// 社保种类的数据库访问对象
        /// </summary>
        InsuranceKindRepository repository = new InsuranceKindRepository();
        /// <summary>
        /// 构造函数，默认加载数据访问上下文
        /// </summary>
        public InsuranceKindBLL()
        {
            db = new SysEntities();
        }
        /// <summary>
        /// 已有数据访问上下文的方法中调用
        /// </summary>
        /// <param name="entities">数据访问上下文</param>
        public InsuranceKindBLL(SysEntities entities)
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
        public List<InsuranceKind> GetByParam(int? id, int page, int rows, string order, string sort, string search, ref int total)
        {

            
            IQueryable<InsuranceKind> queryData = repository.GetData(db, order, sort, search);
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
                        if (item.City != null && item.City != null)
                        { 
                                item.CityOld = item.City1.Name.GetString();//                            
                        }                  
 
                        if (item.PoliceOperation != null)
                        {
                            item.PoliceOperationId = string.Empty;
                            foreach (var it in item.PoliceOperation)
                            {
                                item.PoliceOperationId += it.Name + ' ';
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
        public List<InsuranceKind> GetByParam(int id, string order, string sort, string search)
        {
            IQueryable<InsuranceKind> queryData = repository.GetData(db, order, sort, search);
            
            return queryData.ToList();
        }
        /// <summary>
        /// 创建一个社保种类
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="db">数据库上下文</param>
        /// <param name="entity">一个社保种类</param>
        /// <returns></returns>
       public bool Create(ref ValidationErrors validationErrors, SysEntities db, InsuranceKind entity)
        {   
            int count = 1;
        
            foreach (string item in entity.PoliceOperationId.GetIdSort())
            {
                PoliceOperation sys = new PoliceOperation { Id = Convert.ToInt32(item) };
                db.PoliceOperation.Attach(sys);
                entity.PoliceOperation.Add(sys);
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
        /// 创建一个社保种类
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entity">一个社保种类</param>
        /// <returns></returns>
        public bool Create(ref ValidationErrors validationErrors, InsuranceKind entity)
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
        ///  创建社保种类集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entitys">社保种类集合</param>
        /// <returns></returns>
        public bool CreateCollection(ref ValidationErrors validationErrors, IQueryable<InsuranceKind> entitys)
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
        /// 删除一个社保种类
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">一个社保种类的主键</param>
        /// <returns></returns>  
        public bool Delete(ref ValidationErrors validationErrors, int id)
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
        /// 删除社保种类集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="deleteCollection">主键的社保种类</param>
        /// <returns></returns>    
        public bool DeleteCollection(ref ValidationErrors validationErrors, int[] deleteCollection)
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
        ///  创建社保种类集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entitys">社保种类集合</param>
        /// <returns></returns>
        public bool EditCollection(ref ValidationErrors validationErrors, IQueryable<InsuranceKind> entitys)
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
        /// 编辑一个社保种类
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="db">数据上下文</param>
        /// <param name="entity">一个社保种类</param>
        /// <returns>是否编辑成功</returns>
       public bool Edit(ref ValidationErrors validationErrors, SysEntities db, InsuranceKind entity)
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
            
            List<string> addPoliceOperationId = new List<string>();
            List<string> deletePoliceOperationId = new List<string>();
            DataOfDiffrent.GetDiffrent(entity.PoliceOperationId.GetIdSort(), entity.PoliceOperationIdOld.GetIdSort(), ref addPoliceOperationId, ref deletePoliceOperationId);
            List<PoliceOperation> listEntityPoliceOperation = new List<PoliceOperation>();
            if (deletePoliceOperationId != null && deletePoliceOperationId.Count() > 0)
            {                
                foreach (var item in deletePoliceOperationId)
                {
                    PoliceOperation sys = new PoliceOperation { Id = Convert.ToInt32(item) };
                    listEntityPoliceOperation.Add(sys);
                    entity.PoliceOperation.Add(sys);
                }                
            } 

            InsuranceKind editEntity = repository.Edit(db, entity);
            
         
            if (addPoliceOperationId != null && addPoliceOperationId.Count() > 0)
            {
                foreach (var item in addPoliceOperationId)
                {
                    PoliceOperation sys = new PoliceOperation { Id = Convert.ToInt32(item) };
                    db.PoliceOperation.Attach(sys);
                    editEntity.PoliceOperation.Add(sys);
                    count++;
                }
            }
            if (deletePoliceOperationId != null && deletePoliceOperationId.Count() > 0)
            { 
                foreach (PoliceOperation item in listEntityPoliceOperation)
                {
                    editEntity.PoliceOperation.Remove(item);
                    count++;
                }
            } 

            if (count == repository.Save(db))
            {
                return true;
            }
            else
            {
                validationErrors.Add("编辑社保种类出错了");
            }
            return false;
        }
        /// <summary>
        /// 编辑一个社保种类
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entity">一个社保种类</param>
        /// <returns>是否编辑成功</returns>
        public bool Edit(ref ValidationErrors validationErrors, InsuranceKind entity)
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
        public List<InsuranceKind> GetAll()
        {            
            return repository.GetAll(db).ToList();          
        }     
        
        /// <summary>
        /// 根据主键获取一个社保种类
        /// </summary>
        /// <param name="id">社保种类的主键</param>
        /// <returns>一个社保种类</returns>
        public InsuranceKind GetById(int id)
        {          
            return repository.GetById(db, id);           
        }
        
        /// <summary>
        /// 获取在该表一条数据中，出现的所有外键实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>外键实体集合</returns>
        public List<PoliceOperation> GetRefPoliceOperation(int id)
        { 
            return repository.GetRefPoliceOperation(db, id).ToList();
        }
        /// <summary>
        /// 获取在该表中出现的所有外键实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>外键实体集合</returns>
        public List<PoliceOperation> GetRefPoliceOperation()
        { 
            return repository.GetRefPoliceOperation(db).ToList();
        }

        
        /// <summary>
        /// 根据CityId，获取所有社保种类数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<InsuranceKind> GetByRefCity(string id)
        {
            return repository.GetByRefCity(db, id).ToList();                      
        }

        public void Dispose()
        {
           
        }
    }
}

