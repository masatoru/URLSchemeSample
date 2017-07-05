using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace URLSchemeSample.Wpf
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length >= 1)
            {
                foreach (var arg in e.Args.Select((v,i)=> new{v,i}))
                {
                    if(!string.IsNullOrEmpty(arg.v))        
                        MessageBox.Show($"{arg.i+1}つ目の引数:{arg.v}");
                }
            }

            //引数がない時はURL Schemeを登録する
            if (e.Args.Length == 0)
            {
                RegistUrlScheme();
            }

            //アプリケーションを終了させる
            Application.Current.Shutdown();
        }

        // アプリケーションのフルパス
        string AppPath 
        {
            get {
                var exePath = Environment.GetCommandLineArgs()[0];
                return System.IO.Path.GetFullPath(exePath);
            }
        }

        // URLスキームを登録する
        void RegistUrlScheme()
        {
            try
            {
                var protocolName = "sample-app2";
                //var mainKey = Registry.ClassesRoot.CreateSubKey(protocolName);
                var mainKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{protocolName}");
                mainKey.SetValue("", $"URL:{protocolName} Protocol");
                mainKey.SetValue("URL Protocol", "");

                var defaultIcon = mainKey.CreateSubKey("DefaultIcon");
                defaultIcon.SetValue("", Application.Current.StartupUri.ToString());
                defaultIcon.SetValue("", $"\"{AppPath}\"");

                var shellKey = mainKey.CreateSubKey("shell");
                var openKey = shellKey.CreateSubKey("open");
                var commandKey = openKey.CreateSubKey("command");
                commandKey.SetValue("", $"\"{AppPath}\" \"%1\" \"%2\" \"%3\" \"%4\" \"%5\" \"%6\"");

                MessageBox.Show($"URLスキーム(CurrentUser)を登録しました\n\nコマンドから以下を実行してください\nstart {protocolName}://Hello1 Hello2 Hello3");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"アプリケーションを管理者権限(右クリック→管理者として実行)で実行してください\n\nMSG={ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"URLスキームを登録できませんでした\n\nMSG={ex.Message}");
            }
        }
    }
}
