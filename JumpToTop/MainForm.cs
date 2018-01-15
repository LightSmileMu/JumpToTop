using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace JumpToTop
{
    public partial class MainForm : Form
    {
        private MouseState _MouseState = MouseState.None;
        private readonly string adbPath = Application.StartupPath + @"\adb\adb.exe";
        private string browserPath = string.Empty;
        private string picDir = Application.StartupPath + "\\temp";
        private string tempFileName = string.Empty;

        Process p = new Process();

        /// <summary>
        ///     图案中心或者白点位置
        /// </summary>
        private Point End;

        /// <summary>
        ///     是否存在安卓
        /// </summary>
        private bool HasAndroid;

        /// <summary>
        ///     是否在画画
        /// </summary>
        private bool isDraw = false;

        /// <summary>
        ///     是否停止刷新界面
        /// </summary>
        private bool isStop;

        /// <summary>
        ///     画线坐标
        /// </summary>
        private int lineStartX = 0;

        private int lineStartY = 0;

        /// <summary>
        ///     坐标换算乘数
        /// </summary>
        private double multiplierX;

        private double multiplierY;

        /// <summary>
        ///     设备后插入延时执行
        /// </summary>
        private readonly Timer myTimer = new Timer(100);

        /// <summary>
        ///     黑人底部位置
        /// </summary>
        private Point Start;

        /// <summary>
        ///     滑动坐标
        /// </summary>
        private int StartX = 0;

        private int StartY = 0;

        public MainForm()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            InitBrowser();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            myTimer.AutoReset = false; //只需要执行一次
            myTimer.Elapsed += (o, e1) => { CheckHasAndroidModel(); };
            if (!Directory.Exists(picDir))
            {
                Directory.CreateDirectory(picDir);
            }

            Environment.CurrentDirectory = picDir;

            CheckHasAndroidModel();
        }

        private void InitBrowser()
        {
            //从注册表中读取默认浏览器可执行文件路径  
            var key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            if (key != null)
            {
                var s = key.GetValue("").ToString();
                browserPath = s.Substring(1, s.Length - 9);
            }
        }

        private void Search(string searchContent)
        {
            
            //s就是你的默认浏览器，不过后面带了参数，把它截去，不过需要注意的是：不同的浏览器后面的参数不一样！  
            //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"  
            Process.Start(browserPath, string.Format("http://www.baidu.com/s?wd={0}", searchContent));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x219)
            {
                Debug.WriteLine("WParam：{0} ,LParam:{1},Msg：{2}，Result：{3}", m.WParam, m.LParam, m.Msg, m.Result);
                if (m.WParam.ToInt32() == 7) //设备插入或拔出
                {
                    CheckHasAndroidModel();
                    myTimer.Start();
                }
            }
            try
            {
                base.WndProc(ref m);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     检测是否存在手机
        /// </summary>
        private void CheckHasAndroidModel()
        {
            var text = cmdAdb("shell getprop ro.product.model", false); //获取手机型号
            Debug.WriteLine("检查设备：" + text + "  T=" + DateTime.Now);
            if (text.Contains("no devices") || string.IsNullOrWhiteSpace(text))
            {
                HasAndroid = false;
                isStop = true;
                toolStripStatusLabel2.Text = "未检测到设备";
                btnSearch.Enabled = false;
            }
            else
            {
                HasAndroid = true;
                btnSearch.Enabled = true; 
                isStop = false;
                toolStripStatusLabel2.Text = text.Trim();
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }



        /// <summary>
        ///     执行adb命令
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="ischeck"></param>
        /// <returns></returns>
        private string cmdAdb(string arguments, bool ischeck = true)
        {
            if (ischeck && !HasAndroid)
            {
                return string.Empty;
            }
            var ret = string.Empty;

            p.StartInfo.FileName = adbPath;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true; //重定向标准输入    
            p.StartInfo.RedirectStandardOutput = true; //重定向标准输出   
            p.StartInfo.RedirectStandardError = true; //重定向错误输出   
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            ret = p.StandardOutput.ReadToEnd();



            //using (var p = new Process())
            //{
            //    p.StartInfo.FileName = adbPath;
            //    p.StartInfo.Arguments = arguments;
            //    p.StartInfo.UseShellExecute = false;
            //    p.StartInfo.RedirectStandardInput = true; //重定向标准输入   
            //    p.StartInfo.RedirectStandardOutput = true; //重定向标准输出   
            //    p.StartInfo.RedirectStandardError = true; //重定向错误输出   
            //    p.StartInfo.CreateNoWindow = true;
            //    p.Start();
            //    ret = p.StandardOutput.ReadToEnd();
            //    p.Close();
            //}
            return ret;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            return;
            while (true)
            {
                if (isStop)
                {
                    return;
                }

                DateTime start = DateTime.Now;

                //死循环截屏获取图片
                tempFileName = Guid.NewGuid() + ".png";
                cmdAdb("shell screencap -p /sdcard/" + tempFileName);
                DateTime end = DateTime.Now;
                // pictureBox1.ImageLocation = Environment.CurrentDirectory + "\\temp\\" + tempFileName;
                cmdAdb("pull /sdcard/" + tempFileName);
                if (File.Exists( tempFileName))
                {
                    //pictureBox1.BackgroundImage = new Bitmap(tempFileName);
                    using (var temp = Image.FromFile(tempFileName))
                    {
                        pictureBox1.Invoke(new Action(() => { pictureBox1.Image = new Bitmap(temp); }));
                    }
                    if (multiplierX == 0)
                    {
                        multiplierX = pictureBox1.Image.Width / (pictureBox1.Width + 0.00);
                        multiplierY = pictureBox1.Image.Height / (pictureBox1.Height + 0.00);
                    }
                }

                

                TimeSpan span = end - start;
                Console.WriteLine("this cost time {0} ms",span.TotalMilliseconds);
            }
        }

        private void GetScreen()
        {
            tempFileName = Guid.NewGuid() + ".png";
            cmdAdb("shell screencap -p /sdcard/" + tempFileName);          
            
            cmdAdb("pull /sdcard/" + tempFileName);
            cmdAdb("shell rm /sdcard/" + tempFileName);
            if (File.Exists(tempFileName))
            {
                using (var temp = Image.FromFile(tempFileName))
                {
                    pictureBox1.Invoke(new Action(() => { pictureBox1.Image = new Bitmap(temp); }));
                }
                if (multiplierX == 0)
                {
                    multiplierX = pictureBox1.Image.Width / (pictureBox1.Width + 0.00);
                    multiplierY = pictureBox1.Image.Height / (pictureBox1.Height + 0.00);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmdAdb("shell input keyevent  82 ");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cmdAdb("shell input keyevent  3 ");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cmdAdb("shell input keyevent 4 ");
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("Form1_DragEnter" + e.Data.GetDataPresent(DataFormats.FileDrop));
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) //判断拖来的是否是文件  
            {
                var files = (Array) e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    if (Path.GetExtension(files.GetValue(0).ToString())
                        .EndsWith(".apk", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Effect = DragDropEffects.Link;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None; //是则将拖动源中的数据连接到控件  
            //else e.Effect = DragDropEffects.None; 
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (Array) e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (cmdAdb("install " + file).Contains("100%"))
                {
                    MessageBox.Show("安装成功");
                }
            }
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            cmdAdb("shell input keyevent 26 ");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) //按下左键是黑人底部的坐标
            {
                Start = ((MouseEventArgs) e).Location;
            }
            else if (me.Button == MouseButtons.Right) //按下右键键是黑人底部的坐标
            {
                End = ((MouseEventArgs) e).Location;
                //计算两点直接的距离
                var value =
                    Math.Sqrt(Math.Abs(Start.X - End.X)*Math.Abs(Start.X - End.X) +
                              Math.Abs(Start.Y - End.Y)*Math.Abs(Start.Y - End.Y));
                Text = string.Format("两点之间的距离：{0}，需要按下时间：{1}", value, (3.999022243950134*value).ToString("0"));
                //3.999022243950134  这个是我通过多次模拟后得到 我这个分辨率的最佳时间
                cmdAdb(string.Format("shell input swipe 100 100 200 200 {0}", (3.999022243950134*value).ToString("0")));
            }
        }

        private void GetImage()
        {
        }

        private enum MouseState
        {
            None = 0,
            MouseLeftDown = 1,
            MouseRightDown = 2
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;

            GetScreen();

            Bitmap map = GetPart(tempFileName, 0, 0, 1080, (int)(1920 * (5.5 - 3) / 16.5), 0, (int)(1920 * 3 / 16.5));
                        
            string fileName = Path.Combine(Application.StartupPath, "temp", "test", Guid.NewGuid()+".png");            
            
            map.Save(fileName);

            if (File.Exists(fileName))
            {
                var searchContent = OcrHelper.GetOcrText(Application.StartupPath, fileName);
                searchContent = RemoveSpecialChar(searchContent);
                if (!string.IsNullOrEmpty(searchContent))
                {
                    Search(searchContent);
                }

                //File.Delete(fileName);
            }

            DateTime end = DateTime.Now;

            TimeSpan span = end - start;
            Console.WriteLine("this cost time {0} ms", span.TotalMilliseconds);
                       
        }

        private void DoSearch()
        {
            var files = Directory.GetFiles(Path.Combine(Application.StartupPath, "temp"));

            foreach (string file in files)
            {
                Bitmap map = GetPart(file, 0, 0, 1080, (int)(1920 * (5.5 - 3) / 16.5), 0, (int)(1920 * 3 / 16.5));

                FileInfo info = new FileInfo(file);
                string fileName = Path.Combine(Application.StartupPath, "temp", "test", info.Name);
                map.Save(fileName);

                if (File.Exists(fileName))
                {
                    var searchContent = OcrHelper.GetOcrText(Application.StartupPath, fileName);
                    searchContent = RemoveSpecialChar(searchContent);
                    if (!string.IsNullOrEmpty(searchContent))
                    {
                        Search(searchContent);
                    }

                    //File.Delete(fileName);
                }

                Thread.Sleep(1000);
            }
        }

        private string RemoveSpecialChar(string searchContent)
        {
            if (string.IsNullOrEmpty(searchContent))
            {
                return string.Empty;
            }
            if (searchContent.Contains("\r"))
            {
                searchContent = searchContent.Replace("\r", string.Empty);
            }
            if (searchContent.Contains("\n"))
            {
                searchContent = searchContent.Replace("\n", string.Empty);
            }
            if (searchContent.Contains("？"))
            {
                searchContent = searchContent.Replace("？", string.Empty);
            }
            if (searchContent.Contains("?"))
            {
                searchContent = searchContent.Replace("?", string.Empty);
            }
            return searchContent;
        }

        /// <summary>
        /// 获取图片指定部分
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pPartStartPointX">目标图片开始绘制处的坐标X值(通常为0)</param>
        /// <param name="pPartStartPointY">目标图片开始绘制处的坐标Y值(通常为0)</param>
        /// <param name="pPartWidth">目标图片的宽度</param>
        /// <param name="pPartHeight">目标图片的高度</param>
        /// <param name="pOrigStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="pOrigStartPointY">原始图片开始截取处的坐标Y值</param>
        private  Bitmap GetPart(string pPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
        {
            Image originalImg = Image.FromFile(pPath);

            Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);
            Graphics graphics = Graphics.FromImage(partImg);
            Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), 
                new Size(pPartWidth, pPartHeight));//目标位置
            Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), 
                new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

            graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);

            return partImg;
        }
    }
}