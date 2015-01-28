using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Entity.DTO.Plugins
{
    public class PrizeSiteUser
    {
        public PrizeSiteUser()
        {
            Users = new List<Guid>();
        }

        public IList<Guid> Users { get; set; }

        public int Prize_Index{get;set;}
    }
}
