using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace QuartzAdmin.web.Models
{
    [ActiveRecord(Table="tbl_instanceproperties")]
    public class InstancePropertyModel : ActiveRecordBase<InstancePropertyModel>
    {
		[PrimaryKey("instancepropertyid", Generator = PrimaryKeyType.Sequence, SequenceName = "tbl_instanceproperties_instancepropertyid_seq")]
        public virtual int InstancePropertyID { get; set; }

        [BelongsTo("instanceid", NotNull=true)]
        public virtual InstanceModel ParentInstance { get; set; }
        //public virtual int InstanceID{ get; set; }

        [Property("propertyname", NotNull=true)]
        public virtual string PropertyName { get; set; }

		[Property("propertyvalue", NotNull = true)]
        public virtual string PropertyValue { get; set; }
    }
}
