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
using System.Windows.Shapes;
using ActiveRecordTest.Model;
using System.IO;
using System.Xml.Linq;

namespace ActiveRecordTest
{
    /// <summary>
    /// TopicViewerPC.xaml 的交互逻辑
    /// </summary>
    public partial class TopicViewerPC : Window
    {
        public TopicViewerPC()
        {
            InitializeComponent();
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {



        }

        private Question m_CurrentQuestion;
        private int m_TopicNum = -1;
        private Question[] qList;
        private void ShowTopicContext(object sender, RoutedEventArgs e)
        {
            qList = Question.FindAll();
            m_TopicNum = 0;
            m_CurrentQuestion = qList[m_TopicNum];
            
            textBlock3.Text = "当前题号为：" + (m_TopicNum + 1).ToString();

            ShowTopicContext();
            CheckTheCheckBoxState();

        }

        private void ShowAnswerContext(object sender, RoutedEventArgs e)
        {
            //当前题目的答案文字更新
            textBlock2.Text = m_CurrentQuestion.AnswerText;

            //当前题目答案的图片更新
            image2.Source = CreateBitmapImage(m_CurrentQuestion.AnswerMediaContent, image2);

            //答案panel加入到滚动条View中
            groupBox1.Visibility = System.Windows.Visibility.Visible;
        }

        private BitmapImage CreateBitmapImage(byte[] mediaContent, Image img)
        {
            if (mediaContent == null)
            {
                return null;
            }

            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.StreamSource = new MemoryStream(mediaContent);
            bitImg.EndInit();

            //设置图片宽度和长度
            if (bitImg.Width > scrollViewer1.Width)
            {
                img.Width = scrollViewer1.Width - 25;
            }
            else
            {
                img.Width = bitImg.Width;
            }


            img.Height = (img.Width * bitImg.Height) / bitImg.Width;

            return bitImg;
        }

        private void nextTopic(object sender, RoutedEventArgs e)
        {
            m_TopicNum++;
            if (m_TopicNum >= qList.Length)
            {
                //超出list范围
                MessageBox.Show("已达到题目最末端");
                return;
            }
            else
            {
                m_CurrentQuestion = qList[m_TopicNum];
            }

            ClearAnswerPanel();


            //显示下一题题目的内容
            ShowTopicContext();
            textBlock3.Text = "当前题号为：" + (m_TopicNum + 1).ToString();
            CheckTheCheckBoxState();
        }

        private void lastTopic(object sender, RoutedEventArgs e)
        {
            m_TopicNum--;
            if (m_TopicNum < 0)
            {
                MessageBox.Show("已经达到最前端");
                return;
            }
            else
            {
                m_CurrentQuestion = qList[m_TopicNum];
            }

            ClearAnswerPanel();

            //显示上一题题目的内容
            ShowTopicContext();
            textBlock3.Text = "当前题号为：" + (m_TopicNum + 1).ToString();
            CheckTheCheckBoxState();
        }

        private void ShowTopicContext()
        {
            textBlock1.Text = m_CurrentQuestion.QuestionText;
            image1.Source = CreateBitmapImage(m_CurrentQuestion.MediaContent, image1);

            //更新答案存在的状态
            //答案的文字内容和图片内容都不为空时显示有
            if ((!String.IsNullOrEmpty(m_CurrentQuestion.AnswerText)) || (!(m_CurrentQuestion.AnswerMediaContent == null)))
            {
                textBlock6.Text = "答案：有";

            }
            else
            {
                textBlock6.Text = "答案：无";
            }
            

            //吐槽状态显示
            if(String.IsNullOrEmpty(m_CurrentQuestion.Comment))
            {
                 textBlock7.Text ="吐槽状态：无";
            }
            else
            {
                textBlock7.Text = "吐槽状态：有";
            }
           

        }

        private void ClearAnswerPanel()
        {
            //清空答案部分的内容
            //当前题目的答案文字更新
            textBlock2.Text = "";

            //当前题目答案的图片更新
            image2.Source = null;
            //隐藏答案Panel
            groupBox1.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void jumpToDirectTopic(object sender, RoutedEventArgs e)
        {
            int selectIndex = -1;
            int.TryParse(textBox1.Text, out selectIndex);
            m_TopicNum = selectIndex - 1;
            jumpTOSpecialTopic();
        }

        private void ShowComment(object sender, RoutedEventArgs e)
        {
            textBox2.Text = m_CurrentQuestion.Comment;
            textBlock5.Text = "Clear!";
        }

        private void submmitComment(object sender, RoutedEventArgs e)
        {
            try
            {
                m_CurrentQuestion.Comment = textBox2.Text;
                m_CurrentQuestion.UpdateAndFlush();
                textBlock5.Text = "吐槽已提交！";
            }
            catch
            {
                MessageBox.Show("吐槽添加有误！");
            }
        }

        private void changeDataBase(object sender, RoutedEventArgs e)
        {
            MainWindow.dataBaseName = textBox2.Text;
        }

        private void onChecked(object sender, RoutedEventArgs e)
        {
            //向数据库对应字段写入boolean值，确认是否为错题
            m_CurrentQuestion.IsWrong = true;
            m_CurrentQuestion.UpdateAndFlush();
            textBlock5.Text = "错题已提交！";

        }

        private bool isGetFromDB = false;

        private void CheckTheCheckBoxState()
        {
            //检查数据库中的错题状态，如果是错题则更新为Check状态
            if (m_CurrentQuestion.IsWrong == true)
            {
                checkBox1.IsChecked = true;
            }
            else
            {
                checkBox1.IsChecked = false;
            }
        }

        private void onTouchMove(object sender, TouchEventArgs e)
        {
            //触摸移动事件
        }

        private void GetLastTestPosition(object sender, RoutedEventArgs e)
        {
            //从配置文件中读取上次做到的位置
            String xmlPath = "../../TestCases/TopicConfig.xml";
            XElement xml = XElement.Load(xmlPath);

            var query = from x in xml.Descendants("topicNum")
                        where x.Attribute("databaseName").Value == MainWindow.dataBaseName
                        select x;

            if (query.Count() > 0)
            {
                XElement tempXml = query.First();
                int.TryParse(tempXml.Value, out m_TopicNum);
                m_TopicNum = m_TopicNum - 1;

                //执行跳转动作
                jumpTOSpecialTopic();
            }
            else
            {
                //配置文件中没有此数据库
            }
        }

        private void jumpTOSpecialTopic()
        {
            m_CurrentQuestion = qList[m_TopicNum];

            //清除答案
            ClearAnswerPanel();
            //显示题目
            ShowTopicContext();
            textBlock3.Text = "当前题号为：" + (m_TopicNum + 1).ToString();
        }

        private void onClosed(object sender, EventArgs e)
        {
            //写配置文件,记录上次做到的位置
            String xmlPath = "../../TestCases/TopicConfig.xml";
            XElement xml = XElement.Load(xmlPath);

            var query = from x in xml.Descendants("topicNum")
                        where x.Attribute("databaseName").Value == MainWindow.dataBaseName
                        select x;

            if (query.Count() > 0)
            {
                XElement tempXml = query.First();
                tempXml.Value = (m_TopicNum + 1).ToString();
            }
            else
            {
                XElement tempXml = new XElement("topicNum");
                if (String.IsNullOrEmpty(MainWindow.dataBaseName))
                {
                    MainWindow.dataBaseName = "TestDatabase1";
                }

                tempXml.SetAttributeValue("databaseName", MainWindow.dataBaseName);
                tempXml.Value = (m_TopicNum + 1).ToString();
                xml.Add(tempXml);
            }

            xml.Save(xmlPath);
        }
    }
}
