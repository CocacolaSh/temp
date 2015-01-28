using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core.Data;
using Ocean.Data;

namespace Ocean.Services
{
    public class CategoryService : ServiceBase<Category>, ICategoryService
    {
        public CategoryService(IRepository<Category> CategoryRepository, IDbContext context)
            : base(CategoryRepository, context)
        {
        }
    }
}