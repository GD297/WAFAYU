using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class PendingOrder
    {
        public int OrderId { get; set; }
        public int SpacePackageId { get; set; }

        public virtual Order Order { get; set; }
        public virtual SpacePackage SpacePackage { get; set; }
    }
}
