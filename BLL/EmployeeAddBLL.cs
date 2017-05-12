using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Langben.DAL.Model;
using Common;
using System.Data;

namespace Langben.BLL
{
    /// <summary>
    /// 增加员工 
    /// </summary>
    public partial class EmployeeAddBLL : IBLL.IEmployeeAddBLL, IDisposable
    {
        /// <summary>
        /// 私有的数据访问上下文
        /// </summary>
        protected SysEntities db;
        /// <summary>
        /// 增加员工的数据库访问对象
        /// </summary>
        EmployeeAddRepository repository = new EmployeeAddRepository();
        /// <summary>
        /// 构造函数，默认加载数据访问上下文
        /// </summary>
        public EmployeeAddBLL()
        {
            db = new SysEntities();
        }
        /// <summary>
        /// 已有数据访问上下文的方法中调用
        /// </summary>
        /// <param name="entities">数据访问上下文</param>
        public EmployeeAddBLL(SysEntities entities)
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
        public List<EmployeeAdd> GetByParam(int? id, int page, int rows, string order, string sort, string search, ref int total)
        {
            IQueryable<EmployeeAdd> queryData = repository.GetData(db, order, sort, search);
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
                    //if (item.SuppliersId != null && item.Suppliers != null)
                    //{
                    //    item.SuppliersIdOld = item.Suppliers.Name.GetString();//                            
                    //}

                    if (item.CompanyEmployeeRelationId != null && item.CompanyEmployeeRelation != null)
                    {
                        item.CompanyEmployeeRelationIdOld = item.CompanyEmployeeRelation.EmployeeId.GetString();//                            
                    }

                    if (item.PoliceInsuranceId != null && item.PoliceInsurance != null)
                    {
                        item.PoliceInsuranceIdOld = item.PoliceInsurance.Name.GetString();//                            
                    }

                    if (item.PoliceAccountNatureId != null && item.PoliceAccountNature != null)
                    {
                        item.PoliceAccountNatureIdOld = item.PoliceAccountNature.Name.GetString();//                            
                    }

                    if (item.PoliceOperationId != null && item.PoliceOperation != null)
                    {
                        item.PoliceOperationIdOld = item.PoliceOperation.Name.GetString();//                            
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
        public List<EmployeeAdd> GetByParam(int id, string order, string sort, string search)
        {
            IQueryable<EmployeeAdd> queryData = repository.GetData(db, order, sort, search);

            return queryData.ToList();
        }
        /// <summary>
        /// 创建一个增加员工
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="db">数据库上下文</param>
        /// <param name="entity">一个增加员工</param>
        /// <returns></returns>
        public bool Create(ref ValidationErrors validationErrors, EmployeeAdd entity)
        {
            try
            {
                repository.Create(entity);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        /// <summary>
        ///  创建增加员工集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entitys">增加员工集合</param>
        /// <returns></returns>
        public bool CreateCollection(ref ValidationErrors validationErrors, IQueryable<EmployeeAdd> entitys)
        {
            try
            {
                if (entitys != null)
                {
                    int count = entitys.Count();
                    if (count == 1)
                    {
                        return this.Create(ref validationErrors, entitys.FirstOrDefault());
                    }
                    else if (count > 1)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            repository.Create(db, entitys);
                            if (count == repository.Save(db))
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
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        /// <summary>
        /// 删除一个增加员工
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">一增加员工的主键</param>
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
        /// 删除增加员工集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="deleteCollection">增加员工的集合</param>
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
        ///  创建增加员工集合
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entitys">增加员工集合</param>
        /// <returns></returns>
        public bool EditCollection(ref ValidationErrors validationErrors, IQueryable<EmployeeAdd> entitys)
        {
            try
            {
                if (entitys != null)
                {
                    int count = entitys.Count();
                    if (count == 1)
                    {
                        return this.Edit(ref validationErrors, entitys.FirstOrDefault());
                    }
                    else if (count > 1)
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            repository.Edit(db, entitys);
                            if (count == repository.Save(db))
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
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        /// <summary>
        /// 编辑一个增加员工
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entity">一个增加员工</param>
        /// <returns></returns>
        public bool Edit(ref ValidationErrors validationErrors, EmployeeAdd entity)
        {
            try
            {
                repository.Edit(db, entity);
                repository.Save(db);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }

        public List<EmployeeAdd> GetAll()
        {
            return repository.GetAll(db).ToList();
        }

        /// <summary>
        /// 根据主键获取一个增加员工
        /// </summary>
        /// <param name="id">增加员工的主键</param>
        /// <returns>一个增加员工</returns>
        public EmployeeAdd GetById(int id)
        {
            return repository.GetById(db, id);
        }


        /// <summary>
        /// 根据SuppliersIdId，获取所有增加员工数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<EmployeeAdd> GetByRefSuppliersId(int id)
        {
            return repository.GetByRefSuppliersId(db, id).ToList();
        }

        /// <summary>
        /// 根据CompanyEmployeeRelationIdId，获取所有增加员工数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<EmployeeAdd> GetByRefCompanyEmployeeRelationId(int id)
        {
            return repository.GetByRefCompanyEmployeeRelationId(db, id).ToList();
        }

        /// <summary>
        /// 根据PoliceInsuranceIdId，获取所有增加员工数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<EmployeeAdd> GetByRefPoliceInsuranceId(int id)
        {
            return repository.GetByRefPoliceInsuranceId(db, id).ToList();
        }

        /// <summary>
        /// 根据PoliceAccountNatureIdId，获取所有增加员工数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<EmployeeAdd> GetByRefPoliceAccountNatureId(int id)
        {
            return repository.GetByRefPoliceAccountNatureId(db, id).ToList();
        }

        /// <summary>
        /// 根据PoliceOperationIdId，获取所有增加员工数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<EmployeeAdd> GetByRefPoliceOperationId(int id)
        {
            return repository.GetByRefPoliceOperationId(db, id).ToList();
        }

        public void Dispose()
        {

        }

    }
}

