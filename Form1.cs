using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolCelsiusChk
{
    public partial class Form1 : Form
    {
        IWebDriver _driver;

        public string Id { get; set; }
        public string Password { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            btnRun.Enabled = false;

            try
            {
                Run();
                _driver.Quit();
                this.Close();
            }
            catch (Exception ex)
            {
                _driver.Quit();
                this.Close();
            }

            btnRun.Enabled = true;
        }

        public void Run()
        {
            Selenium();

            if (CheckHRP() == false)
            {
                return;
            }
        }

        public bool CheckHRP()
        {
            string url = "https://hrp.hosp.nycu.edu.tw:8082/logon.aspx";
            DriverGoUrl(url);

            //登入HRP
            IWebElement IAccount = _driver.FindElement(By.Id("edtAccount"));
            IWebElement IPassword = _driver.FindElement(By.Id("edtPassword"));

            IAccount.Clear();
            IPassword.Clear();

            IAccount.SendKeys(Id);
            IPassword.SendKeys(Password);

            IWebElement ILogon = _driver.FindElement(By.Id("btnLogon"));
            ILogon.Click();

            DriverWait();
            Thread.Sleep(10000);
            DriverWait();

            IWebElement IStartReal2 = _driver.FindElement(By.Id("lblStartReal2"));
            IWebElement IEndReal2 = _driver.FindElement(By.Id("lblEndReal2"));

            string StartReal2 = IStartReal2.Text;
            string EndReal2 = IEndReal2.Text;

            // 要同時有上班跟下班打卡紀錄
            if (StartReal2.Length > 4 && EndReal2.Length > 4)
            {
                LoginCelsiusChk();
                return true;
            }

            return false;
        }


        public void LoginCelsiusChk()
        {
            string url = $"http://hisupdatea.ymuh.ym.edu.tw/HisReport/Routine/Other/CelsiusChk.aspx?UID={Id}";

            DriverGoUrl(url);

            Random rnd = new Random();
            double min = 36.0;
            double max = 37.5;
            double randomNumber = rnd.NextDouble() * (max - min) + min; // min <= randomNumber < max 之間的亂數
            randomNumber = Math.Round(randomNumber, 1);

            //體溫
            IWebElement ITemperature_Text = _driver.FindElement(By.Name("Temperature_Text"));
            ITemperature_Text.Clear();
            ITemperature_Text.SendKeys(randomNumber.ToString());

            //確認存檔
            IWebElement ISave_Btn = _driver.FindElement(By.Name("Save_Btn"));
            ISave_Btn.Submit();
        }


        public void Selenium()
        {
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;//關閉黑色cmd窗口

            //_driver = new ChromeDriver();
            _driver = new ChromeDriver(driverService);
        }

        public void DriverGoUrl(string url)
        {
            //開啟網頁
            _driver.Navigate().GoToUrl(url);

            DriverWait();

            //睡一下
            Thread.Sleep(5000);
        }

        public void DriverWait()
        {
            //隱式等待 - 直到畫面跑出資料才往下執行
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10000);
        }
        int _timeLeft = 10;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_timeLeft <= 0)
            {
                btnRun.PerformClick();
            }
            else
            {
                _timeLeft = _timeLeft - 1;
                labTimeLeft.Text = _timeLeft.ToString();
            }
            Application.DoEvents();
        }

        // <summary>
        /// kill chromedriver.exe
        /// </summary>
        private void KillChrome()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;// 接受來自呼叫程式的輸入資訊
            p.StartInfo.RedirectStandardOutput = true;// 由呼叫程式獲取輸出資訊
            p.StartInfo.RedirectStandardError = true;//重定向標準錯誤輸出
            p.StartInfo.CreateNoWindow = true; //不跳出cmd視窗

            p.Start();// 啟動程式
            p.StandardInput.WriteLine("taskkill /F /IM chromedriver.exe /T");//向cmd視窗傳送輸入資訊
            //p.StandardInput.WriteLine("taskkill /f /im chrome.exe");//向cmd視窗傳送輸入資訊
            p.Close();//關閉程式
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KillChrome();
        }
    }
}
