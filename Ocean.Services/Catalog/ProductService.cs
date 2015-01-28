using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;
using System.Transactions;

namespace Ocean.Services
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        public ProductService(IRepository<Product> productRepository, IDbContext context)
            : base(productRepository, context)
        {

            
        }
        public void ProductTest()
        {
            //try
            //{
            //    using (var trans = new TransactionScope())
            //    {
            //        Product p = new Product() { Name = "事务测试-1", CreateDate = DateTime.Now };
            //        Product p2 = new Product() { Name = "事务测试0", CreateDate = DateTime.Now };
            //        this.Insert(p);
            //        this.Insert(p2);
            //        //throw new Exception("测试事务");
            //        trans.Complete();
            //    }
            //}
            //catch
            //{ }

            try
            {
                _context.BeginTransaction();
                Product p = new Product() { Name = "事务测试1", CreateDate = DateTime.Now };
                Product p2 = new Product() { Name = "事务测试2", CreateDate = DateTime.Now };
                this.Insert(p);
                this.Insert(p2);
                ProductTransTest();
                //throw new Exception("测试事务");
                this.ExcuteSql("update Product set Name='事务测试修改1' where Id='DB1B9855-577C-48EF-8ECE-85F7E63F1095'");
                _context.Commit();
            }
            catch
            {
                _context.Rollback();
            }
            try
            {
                _context.BeginTransaction();
                Product p3 = new Product() { Name = "事务测试3", CreateDate = DateTime.Now };
                Product p4 = new Product() { Name = "事务测试4", CreateDate = DateTime.Now };
                this.Insert(p3);
                this.Insert(p4);
                //throw new Exception("测试事务");
                this.ExcuteSql("update Product set Name='事务测试修改4' where Id='D56D4A8F-3B44-4A8B-836C-87624367EA17'");
                _context.Commit();
            }
            catch
            {
                _context.Rollback();
            }
        }

        public void ProductTransTest()
        {
            try
            {
                _context.BeginTransaction();
                Product p3 = new Product() { Name = "事务测试5", CreateDate = DateTime.Now };
                Product p4 = new Product() { Name = "事务测试6", CreateDate = DateTime.Now };
                this.Insert(p3);
                this.Insert(p4);
                //throw new Exception("测试事务");
                this.ExcuteSql("update Product set Name='事务测试修改4' where Id='D56D4A8F-3B44-4A8B-836C-87624367EA17'");
                _context.Commit();
            }
            catch
            {
                _context.Rollback();
            }
        }
    }
}