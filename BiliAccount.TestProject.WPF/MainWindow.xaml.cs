﻿using BiliAccount.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Windows;

namespace BiliAccount.TestProject.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private Account account;

        private string tmp_token;

        #endregion Private Fields

        #region Public Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        ///<summary>
        /// equiv of PHP's var dump for an object’s properties because i cbf writing all the properties out.
        ///</summary>
        ///<param name="info"></param>
        private static string var_dump(object info)
        {
            StringBuilder sb = new StringBuilder();

            Type t = info.GetType();
            FieldInfo[] props = t.GetFields();
            sb.AppendFormat("{0,-25} {1}", "Name", "Value");
            sb.AppendLine();

            foreach (FieldInfo prop in props)
            {
                try
                {
                    if (prop.GetValue(info) != null)
                    {
                        sb.AppendFormat("{0,-25} {1}", prop.Name, prop.GetValue(info).ToString());
                        sb.AppendLine();
                    }
                }
                catch { }
            }

            return sb.ToString();
        }

        private void Browser_OnVaildate_Success(string challenge, string key, string validate)
        {
            Device_Verify.Send_SMS(challenge, key, tmp_token, validate);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string code = Device_Verify.Verify(textBox.Text, tmp_token);
            Device_Verify.GetAccount(code, ref account);
            System.Windows.Forms.MessageBox.Show(var_dump(account));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"测试版本：");
            Console.WriteLine($"BiliAccount：{AssemblyName.GetAssemblyName("BiliAccount.dll").Version.ToString()}");
            Console.WriteLine($"BiliAccount.Geetest：{AssemblyName.GetAssemblyName("BiliAccount.Geetest.dll").Version.ToString()}");
            Console.WriteLine($"BiliAccount.Geetest.Controls：{AssemblyName.GetAssemblyName("BiliAccount.Geetest.Controls.dll").Version.ToString()}");
            Console.WriteLine("账号");
            string username = "";
            Console.WriteLine("密码");
            string pwd = "";
            account = ByPassword.LoginByPassword(username, pwd);
            Console.WriteLine(var_dump(account));

            browser.OnVaildate_Success += Browser_OnVaildate_Success;
            if (account.LoginStatus == Account.LoginStatusEnum.NeedSafeVerify)
            {
                //tmp_token = Device_Verify.Url2TmpToken(account.Url);
                //browser.StartVaildate(account.Url);
                Geetest.Controls.WPF.GeetestBrowserWindow.ShowValidateWindowDialog(account.Url, ref account);
                Console.WriteLine(var_dump(account));
            }
        }

        #endregion Private Methods
    }
}