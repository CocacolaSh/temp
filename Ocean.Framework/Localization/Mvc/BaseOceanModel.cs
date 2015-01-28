using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ocean.Framework.Mvc
{
    public partial class BaseOceanModel
    {
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

        }
    }

    public partial class BaseOceanEntityModel : BaseOceanModel
    {
        public virtual Guid Id { get; set; }
    }
}