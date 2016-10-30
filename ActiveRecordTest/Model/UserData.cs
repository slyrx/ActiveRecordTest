using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace ActiveRecordTest.Model
{
    [ActiveRecord("UserData")]
    class UserData : ActiveRecordBase<UserData>
    {
        [PrimaryKey(PrimaryKeyType.Identity)]
        public int ID { get; set; }

        [Property()]
        public int UserID { get; set; }


        [BelongsTo("UserID", Column = "UserID")]
        public Question User { get; set; }

        [Property()]
        public String LoginTime { get; set; }

        [Property()]
        public String Data { get; set; }
    }
}
