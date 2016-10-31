using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Castle.ActiveRecord.Framework;
using ActiveRecordTest.Model;
using Castle.ActiveRecord;
using log4net;
using System.Reflection;
using Castle.ActiveRecord.Queries;
using NHibernate.Criterion;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections;
using System.Drawing;
using MyUtilityLibrary.KeyUtility;
using System.Collections.ObjectModel;
using System.Windows.Interop;

namespace ActiveRecordTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ILog log = LogManager.GetLogger("");
		public static String dataBaseName = "";
        private KeyboardListener keyListener = new KeyboardListener();

        public MainWindow()
        {
            InitializeComponent();
        }

        public void setDBConfig()
        {
            Type type = Type.GetType("ActiveRecordTest.Model.TestBase1", false, false);

            String myAssemblyName = Assembly.GetExecutingAssembly().FullName;

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load("../../TestCases/ActiveRecordConfig.xml");
            //xmlDoc.DocumentElement.
            String xmlPath;
            if (dataBaseName.Equals("logical"))
            {
                xmlPath = @"../../TestCases/logicalConfig.xml";
            }
            else if (dataBaseName.Equals("yuwen"))
            {
                xmlPath = @"../../TestCases/yuwenConfig.xml";
            }
            else if (dataBaseName.Equals("math"))
            {
                xmlPath = @"../../TestCases/mathConfig.xml";
            }
            else if (dataBaseName.Equals("erws"))
            {
                xmlPath = @"../../TestCases/erwsConfig.xml";
            }
            else
            {
                //xmlPath = @"../../TestCases/ActiveRecordConfig.xml";
                xmlPath = @"../../TestCases/" + dataBaseName + @"Config.xml";
            }

            XDocument xDoc = null;
            if (File.Exists(xmlPath))
            {
                xDoc = XDocument.Load(xmlPath);
                dataBaseNameInput.Text = "logical Loaded";
            }
            else
            {
                MessageBox.Show(xmlPath + @"该文件不存在，请确认文件及对应数据库已经创建");
                Environment.Exit(0);
            }

            

            var query = from xElement in xDoc.Descendants("config")
                        where xElement.HasAttributes == false
                        select (from xElement1 in xElement.Descendants("add")
                                where xElement1.Attribute("key").Value == "connection.connection_string"
                                select xElement1).First();
            XElement findXdoc = query.First();

            // 获取数据库连接配置
            //IConfigurationSource source = new Castle.ActiveRecord.Framework.Config.XmlConfigurationSource("../../TestCases/ActiveRecordConfig.xml");

            IConfigurationSource source = new Castle.ActiveRecord.Framework.Config.XmlConfigurationSource(xmlPath);


            // 载入程序集中所有 ActiveRecord 类。
            ActiveRecordStarter.Initialize(source, typeof(ActiveRecordBase), typeof(TestBase1), typeof(TestBase2),
                typeof(Question), typeof(UserData), typeof(User1), typeof(User2));

            // 删除数据库架构
            //Castle.ActiveRecord.ActiveRecordStarter.DropSchema();

            // 创建数据库架构(该方法会删除同名表后再创建)
            //Castle.ActiveRecord.ActiveRecordStarter.CreateSchema();
        }

        private void switchDB()
        {
            ActiveRecordStarter.ResetInitializationFlag();

            // 获取数据库连接配置
            IConfigurationSource source = new Castle.ActiveRecord.Framework.Config.XmlConfigurationSource("../../TestCases/ActiveRecordConfig_1.xml");

            // 载入程序集中所有 ActiveRecord 类。
            ActiveRecordStarter.Initialize(source, typeof(ActiveRecordBase), typeof(TestBase1), typeof(TestBase2),
                typeof(Question), typeof(UserData), typeof(User1), typeof(User2));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 配置数据库
            //setDBConfig();
            // key 快捷键注册，但要求窗体激活时才能使用
            keyListener.KeyDown += new RawKeyEventHandler(keyListener_KeyDown);

            //MessageBox.Show("已结束！");

            #region 测试用例
            //BasicTest();    // 增删查改
            //switchDB();

            //BasicTest();    // 增删查改

            //OneToManyTest(); // 一对多
            //DAOTest();    // 条件查询等

            //MultiDBTest();  // 跨数据库查询

            //ConditionTest();    // 多条件查询
            #endregion

            //加载快捷键事件处理函数 快捷键注册，可以全局使用
            HotKeySettingsManager.Instance.RegisterGlobalHotKeyEvent += Instance_RegisterGlobalHotKeyEvent;

        }

        #region 快捷键设定
        /// <summary>
        /// 记录快捷键注册项的唯一标识符
        /// </summary>
        private Dictionary<EHotKeySetting, int> m_HotKeySettings = new Dictionary<EHotKeySetting, int>();


        private IntPtr m_Hwnd = new IntPtr();

        private bool Instance_RegisterGlobalHotKeyEvent(ObservableCollection<HotKeyModel> hotKeyModelList)
        {
            return InitHotKey(hotKeyModelList);
        }

        private bool InitHotKey(ObservableCollection<HotKeyModel> hotKeyModelList = null)
        {
            var list = hotKeyModelList ?? HotKeySettingsManager.Instance.LoadDefaultHotKey();
            // 注册全局快捷键
            string failList = HotKeyHelper.RegisterGlobalHotKey(list, m_Hwnd, out m_HotKeySettings);
            if (string.IsNullOrEmpty(failList))
                return true;
            MessageBoxResult mbResult = MessageBox.Show(string.Format("无法注册下列快捷键\n\r{0}是否要改变这些快捷键？", failList), "提示", MessageBoxButton.YesNo);
            // 弹出热键设置窗体
            if (mbResult == MessageBoxResult.Yes)
            {
                return false;
            }
            return true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // 获取窗体句柄
            m_Hwnd = new WindowInteropHelper(this).Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(m_Hwnd);
            // 添加处理程序
            if (hWndSource != null) hWndSource.AddHook(WndProc);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            // 注册热键
            InitHotKey();
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
        {
            var hotkeySetting = new EHotKeySetting();
            switch (msg)
            {
                case HotKeyManager.WM_HOTKEY:
                    int sid = wideParam.ToInt32();
                    if (sid == m_HotKeySettings[EHotKeySetting.插入题意])
                    {
                        hotkeySetting = EHotKeySetting.插入题意;
                        //TODO 执行插入题意操作
                        InsertTopicImage();
                    }
                    else if (sid == m_HotKeySettings[EHotKeySetting.插入答案])
                    {
                        hotkeySetting = EHotKeySetting.插入答案;
                        //TODO 执行插入答案操作
                        insertAnswerPicAndAddOneToDB();
                    }

                    //MessageBox.Show(string.Format("触发【{0}】快捷键", hotkeySetting.ToString()));
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion

        void keyListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            log.Debug("press key : " + args.Key.ToString());

            //if (args.Key == Key.G)
            //{
            //    InsertTopicImage();

            //}
            //else if (args.Key == Key.H)
            //{
            //    InsertAnswerImage();
            //}
            // not need else
        }

        private void MultiDBTest()
        {
            User1 user1 = DAO<User1>.Find(6);
            log.Debug("user1::UserID = " + user1.UserID.ToString());
            log.Debug("user1::用户名 = " + user1.用户名);
        }

        private void DAOTest()
        {
            UserData data = DAO<UserData>.DAO_FindFirst("Data", "data3");
            log.Debug("UserID = " + data.UserID.ToString());
        }

        private void OneToManyTest()
        {

            Question u = Question.Find(3);
            //label1.Content = ((UserData)(u.Data[0])).Data;
        }

        private void ConditionTest()
        {
            // 可以直接增加多个条件
            Question u = Question.FindFirst(Restrictions.Eq("用户名", "u4"), Restrictions.Eq("Param1", "1"));
            //NHibernate.Criterion

            //label1.Content = u.用户名;
        }

        private void BasicTest()
        {

            Question u = Question.Find(3);
            //label1.Content = u.用户名;



            u = Question.Find(3);



            //通过剪切板进入到数据库
            System.Windows.IDataObject clip1 = System.Windows.Clipboard.GetDataObject();

            bool ifCanGetClilp = clip1.GetDataPresent(typeof(System.Drawing.Bitmap));
            //object clipBitMap = clip1.GetData(DataFormats.Bitmap);
            Bitmap clipBitMap = (Bitmap)clip1.GetData(typeof(System.Drawing.Bitmap));

            String imagePath = @"../../TestCases/temp.png";
            //另存为临时文件
            clipBitMap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            u.Media_width = clipBitMap.Width;
            u.Media_height = clipBitMap.Height;
            clipBitMap.Dispose();

            //读一个二进制文件，将该文件内容写入到DB中

            FileStream fs1 = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            Byte[] tempFile3 = new Byte[fs1.Length];
            fs1.Read(tempFile3, 0, (int)fs1.Length);

            u.MediaContent = tempFile3;
            u.AnswerText = "测试";
            u.UpdateAndFlush();
            fs1.Close();

            u = Question.TryFind(4);
            if (u == null)
            {
                u = new Question();
                u.QuestionID = 4;
                //u.用户名 = "u4";
                u.CreateAndFlush();
            }
            else
            {
                u.DeleteAndFlush();
            }
        }

        private void InsertQuestionImage(object sender, RoutedEventArgs e)
        {
            InsertTopicImage();


        }

        private void InsertTopicImage()
        {
            //获取剪切板上的图片，如果剪切板上的图片为空，则提示
            System.Windows.IDataObject clip1 = System.Windows.Clipboard.GetDataObject();
            bool ifCanGetClilp = clip1.GetDataPresent(typeof(System.Drawing.Bitmap));

            if (true == ifCanGetClilp)
            {
                Bitmap clipBitMap = (Bitmap)clip1.GetData(typeof(System.Drawing.Bitmap));
                String imagePath = @"../../TestCases/temp.png";
                clipBitMap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                int QuestionID = 0;
                int.TryParse(textBox4.Text, out QuestionID);
                try
                {
                    Question u = Question.Find(QuestionID);
                    u.Media_width = clipBitMap.Width;
                    u.Media_height = clipBitMap.Height;
                    clipBitMap.Dispose();
                    FileStream fs1 = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    Byte[] tempFile3 = new Byte[fs1.Length];
                    fs1.Read(tempFile3, 0, (int)fs1.Length);
                    u.MediaContent = tempFile3;
                    u.UpdateAndFlush();
                    fs1.Close();
                    textBlock1.Text = "题目图片："+ u.Media_width.ToString();
                }
                catch
                {
                    MessageBox.Show("找不到题序");
                }

                if (isNeedPlusOne)
                {
                    //数据库加一
                    AddQuestionNumer();
                    AddNewData();
                }


            }
        }

        private void AddQuestionNumer(object sender, RoutedEventArgs e)
        {
            AddQuestionNumer();
        }

        private void AddQuestionNumer()
        {
            int QuestionID = 0;
            int.TryParse(textBox4.Text, out QuestionID);
            QuestionID++;
            textBox4.Text = QuestionID.ToString();
        }

        private void AddNewData(object sender, RoutedEventArgs e)
        {
            AddNewData();
        }

        private void AddNewData()
        {
            textBlock3.Text = textBox4.Text;
            int topicNum = 0;
            int.TryParse(textBlock3.Text, out topicNum);
            if (topicNum >= 5)
            {
                textBlock9.Text = (topicNum - 4).ToString() + " " + (topicNum - 3).ToString() + " "
                    + (topicNum - 2).ToString() + " " + (topicNum - 1).ToString();
            }
            else
            {
                textBlock9.Text = "该序号前少于4道题";
            }
            Question u = new Question();
            u.CreateAndFlush();
            textBlock1.Text = "已增加完毕";
        }

        private void InsertQuestionText(object sender, RoutedEventArgs e)
        {
            int QuestionID = 0;
            int.TryParse(textBox4.Text, out QuestionID);
            try
            {
                Question u = Question.Find(QuestionID);
                u.QuestionText = textBox1.Text;
                u.UpdateAndFlush();
                textBlock1.Text = "题意完成";

            }
            catch
            {
                MessageBox.Show("找不到题序");
            }
        }

        private void InsertAnswerText(object sender, RoutedEventArgs e)
        {
            int QuestionID = 0;
            int.TryParse(textBox4.Text, out QuestionID);
            try
            {
                Question u = Question.Find(QuestionID);
                u.AnswerText = textBox2.Text;
                u.UpdateAndFlush();
                textBlock1.Text = "答案完成";
            }
            catch
            {
                MessageBox.Show("找不到题序");
            }
        }

        private void InsertAnswerImage(object sender, RoutedEventArgs e)
        {
            InsertAnswerImage();
        }

        private void InsertAnswerImage()
        {
            //获取剪切板上的图片，如果剪切板上的图片为空，则提示
            System.Windows.IDataObject clip1 = System.Windows.Clipboard.GetDataObject();
            bool ifCanGetClilp = clip1.GetDataPresent(typeof(System.Drawing.Bitmap));

            if (true == ifCanGetClilp)
            {
                Bitmap clipBitMap = (Bitmap)clip1.GetData(typeof(System.Drawing.Bitmap));
                String imagePath = @"../../TestCases/temp.png";
                clipBitMap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                int QuestionID = 0;
                int.TryParse(textBox4.Text, out QuestionID);
                try
                {
                    Question u = Question.Find(QuestionID);
                    u.AnswerMedia_width = clipBitMap.Width;
                    u.AnswerMedia_height = clipBitMap.Height;
                    clipBitMap.Dispose();
                    FileStream fs1 = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    Byte[] tempFile3 = new Byte[fs1.Length];
                    fs1.Read(tempFile3, 0, (int)fs1.Length);
                    u.AnswerMediaContent = tempFile3;
                    u.UpdateAndFlush();
                    fs1.Close();
                    textBlock1.Text = "答案图片："+u.AnswerMedia_width.ToString();
                }
                catch
                {
                    MessageBox.Show("找不到题序");
                }



            }
        }

        private void AddComment(object sender, RoutedEventArgs e)
        {
            int QuestionID = 0;
            int.TryParse(textBox4.Text, out QuestionID);
            try
            {
                Question u = Question.Find(QuestionID);
                u.Comment = textBox3.Text;
                u.UpdateAndFlush();
                textBlock1.Text = "吐槽完成";
            }
            catch
            {
                MessageBox.Show("找不到题序");
            }
        }

        private void OnGetDataCount(object sender, RoutedEventArgs e)
        {
            textBlock4.Text = Question.FindAll().Length.ToString();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = "B";
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = "C";
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = "A";
        }

        private void button12_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = "D";
        }

        private void button13_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "课文";
        }

        private void button14_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "1";
        }

        private void button15_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "2";
        }

        private void button16_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "3";
        }

        private void button17_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "4";
        }

        private void button18_Click(object sender, RoutedEventArgs e)
        {
            //查询ID内容
            //textBlock4.Text = Question.FindAll().Length.ToString();
            int id = 0;

            int.TryParse(textBox5.Text, out id);
            
            Question tempQ = Question.Find(id);
            textBox6.Text = tempQ.QuestionText;
            textBox7.Text = tempQ.AnswerText;
        }

        private void button19_Click(object sender, RoutedEventArgs e)
        {
            //int id = 0;

            //int.TryParse(textBox6.Text, out id);
            //String sql = "SELECT * from Questions where QuestionText like '%1%'";
            //Question tempQ = Question.ExecuteQuery();
            //textBox5.Text = tempQ.QuestionID.ToString();
            //textBox7.Text = tempQ.AnswerText;
        }

        private void button20_Click(object sender, RoutedEventArgs e)
        {
            textBox4.Text = textBlock4.Text;
        }

        private void button21_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "5";
        }
		
	    private void ChangeDataBase(object sender, RoutedEventArgs e)
        {
            dataBaseName = dataBaseNameInput.Text;
            setDBConfig();
        }

        private void OpenExamSystem(object sender, RoutedEventArgs e)
        {
            TopicViewerPC viewerPC = new TopicViewerPC();
            viewerPC.ShowDialog();
        }

        private void insertAnswerPicAndAddOneToDB(object sender, RoutedEventArgs e)
        {
            ////插入答案图片
            //InsertAnswerImage();

            ////数据库加一
            //AddQuestionNumer();
            //AddNewData();
            insertAnswerPicAndAddOneToDB();
        }

        private void insertAnswerPicAndAddOneToDB()
        {
            //插入答案图片
            InsertAnswerImage();

            //数据库加一
            AddQuestionNumer();
            AddNewData();
        }

        private bool isNeedPlusOne = false;
        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            isNeedPlusOne = true;
        }

        private void checkBox1_UnChecked(object sender, RoutedEventArgs e)
        {
            isNeedPlusOne = false;
        }
    }
}
