using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using JumpToTop.utils;

namespace JumpToTop
{
    public partial class MainForm : Form
    {
        #region private filed

        private const string picName = "65AFC37E-F595-4351-941C-1B1BFF35E2D8.png";
        private readonly string adbPath = Application.StartupPath + @"\adb\adb.exe";
        private string picDir = Application.StartupPath + "\\temp";
        /// <summary>
        ///     是否存在安卓
        /// </summary>
        private bool hasAndroid;
        /// <summary>
        ///     是否停止刷新界面
        /// </summary>
        private bool isStop;
        /// <summary>
        ///     坐标换算乘数
        /// </summary>
        private double multiplierX;
        private double multiplierY;
        /// <summary>
        ///     设备后插入延时执行
        /// </summary>
        private readonly Timer myTimer = new Timer(100);

        #endregion

        #region constructor

        public MainForm()
        {
            InitializeComponent();
        }

        #endregion     

        #region override method

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

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x219)
            {
                LogHelper.Debug(string.Format("WParam：{0} ,LParam:{1},Msg：{2}，Result：{3}", m.WParam, m.LParam, m.Msg, m.Result));
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
            catch (Exception ex)
            {
                LogHelper.Error("MainForm.WndProc:" + ex.Message);

                toolStripStatusLabel2.Text = "未检测到设备";
            }
        }

        #endregion

        #region event

        private void btnSearch_Click(object sender, EventArgs e)
        {      
            try
            {
                DoSearch();
            }
            catch(Exception ex)
            {
                LogHelper.Error("MainForm.btnSearch_Click:" + ex.Message);
            }
        }


        #endregion

        #region private method

        private void DoSearch()
        {
            SaveAndroidScreenToDisk();

            Bitmap map = CreatPicturePart(picName, 0, 0, 1080, (int)(1920 * (5.5 - 3) / 16.5), 0, (int)(1920 * 3 / 16.5));

            string fileName = Path.Combine(Application.StartupPath, "temp", Guid.NewGuid() + ".png");

            map.Save(fileName);

            if (File.Exists(fileName))
            {
                var searchContent = OcrHelper.GetOcrText(Application.StartupPath, fileName);
                searchContent = RemoveSpecialChar(searchContent);
                if (!string.IsNullOrEmpty(searchContent))
                {
                    SearchByBrowser(searchContent);
                }
                if (File.Exists(picName))
                {
                    File.Delete(picName);
                }
                File.Delete(fileName);
            }
        }

        /// <summary>
        ///     检测是否存在手机
        /// </summary>
        private void CheckHasAndroidModel()
        {
            var text = ExcuteAdbCmd("shell getprop ro.product.model", false); //获取手机型号
            LogHelper.Debug(string.Format("检查设备：{0}" , text));
            if (text.Contains("no devices") || string.IsNullOrWhiteSpace(text))
            {
                hasAndroid = false;
                isStop = true;
                toolStripStatusLabel2.Text = "未检测到设备";
                btnSearch.Enabled = false;
            }
            else
            {
                hasAndroid = true;
                btnSearch.Enabled = true;
                isStop = false;
                toolStripStatusLabel2.Text = text.Trim();                
            }
        }

        /// <summary>
        ///     执行adb命令
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="ischeck"></param>
        /// <returns></returns>
        private string ExcuteAdbCmd(string arguments, bool ischeck = true)
        {
            if (ischeck && !hasAndroid)
            {
                return string.Empty;
            }
            var ret = string.Empty;

            using (var p = new Process())
            {
                p.StartInfo.FileName = adbPath;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true; //重定向标准输入   
                p.StartInfo.RedirectStandardOutput = true; //重定向标准输出   
                p.StartInfo.RedirectStandardError = true; //重定向错误输出   
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                ret = p.StandardOutput.ReadToEnd();
                p.Close();
            }
            return ret;
        }

        private void SaveAndroidScreenToDisk()
        {          
            ExcuteAdbCmd("shell screencap -p /sdcard/" + picName);

            ExcuteAdbCmd("pull /sdcard/" + picName);

            ExcuteAdbCmd("shell rm /sdcard/" + picName);

            if (File.Exists(picName))
            {
                using (var temp = Image.FromFile(picName))
                {
                    pictureBox1.Invoke(new Action(() => { pictureBox1.Image = new Bitmap(temp); }));
                }                
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
        private Bitmap CreatPicturePart(string pPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
        {
            Bitmap partImg = new Bitmap(1, 1);

            if (File.Exists(pPath))
            {
                Image originalImg = Image.FromFile(pPath);

                partImg = new Bitmap(pPartWidth, pPartHeight);
                Graphics graphics = Graphics.FromImage(partImg);
                Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY),
                    new Size(pPartWidth, pPartHeight));//目标位置
                Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY),
                    new Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）
                graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
            }          
            return partImg;
        }

        private void SearchByBrowser(string searchContent)
        {            
            if (string.IsNullOrEmpty(searchContent))
            {
                return;
            }

            try
            {
                Process.Start(string.Format("http://www.baidu.com/s?wd={0}", searchContent));
            }
            catch(Exception ex)
            {
                LogHelper.Error("MainForm.Search:"+ex.Message);
            }
            
        }

        #endregion
    }
}