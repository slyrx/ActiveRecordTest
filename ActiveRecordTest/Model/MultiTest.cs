using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace ActiveRecordTest.Model
{
    [ActiveRecord()]
    public abstract class TestBase1 : ActiveRecordBase
    {
    }

    [ActiveRecord()]
    public abstract class TestBase2 : ActiveRecordBase
    {
    }

    [ActiveRecord("Users")]
    public class User1 : TestBase1
    {
        [PrimaryKey(PrimaryKeyType.Identity, "UserID")]
        public int UserID { get; set; }

        [Property()]
        public String 用户名 { get; set; }
    }

    [ActiveRecord("Users")]
    public class User2 : TestBase2
    {
        [PrimaryKey(PrimaryKeyType.Identity, "UserID")]
        public int UserID { get; set; }
    }

}
