using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core;

namespace Ocean.Entity
{
    public class Product : BaseEntity
    {
        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        /// <summary>
        /// 产品名称 
        /// </summary>
        public string Name { set; get; }
    }
}