using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class Category : BaseEntity
    {
        public Category()
        {
            //Products = new List<Product>();
        }
        /// <summary>
        /// 类别名称 
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
    }
}