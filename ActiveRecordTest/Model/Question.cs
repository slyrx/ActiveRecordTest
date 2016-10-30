using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using System.Collections;

namespace ActiveRecordTest.Model
{
    [ActiveRecord("Questions")]
    class Question : ActiveRecordBase<Question>
    {
        [PrimaryKey(PrimaryKeyType.Identity, "QuestionID")]
        public int QuestionID { get; set; }

        [Property()]
        public String QuestionText { get; set; }

        [Property()]
        public Byte[] QuestionInfo { get; set; }

        [Property()]
        public Byte[] MediaContent { get; set; }

        [Property()]
        public int Media_width { get; set; }

        [Property()]
        public int Media_height { get; set; }

        [Property()]
        public String AnswerText { get; set; }

        [Property()]
        public Byte[] AnswerInfo { get; set; }

        [Property()]
        public Byte[] AnswerMediaContent { get; set; }

        [Property()]
        public int AnswerMedia_width { get; set; }

        [Property()]
        public int AnswerMedia_height { get; set; }

        [Property()]
        public String Comment { get; set; }

        [Property()]
        public Boolean IsWrong { get; set; }


        public static Question Find(int id)
        {
            return (Question)FindByPrimaryKey(typeof(Question), id);
        }
    }
}
