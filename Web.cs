using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace 自己管理アプリ
{
    internal class Web
    {
        /// <summary>
        /// 今日のコロナ感染者数が入る。クラスをインスタンス化すると初期化される。
        /// </summary>
        public string covidNum;
        public Web()
        {
            const string url = "https://www.academic-gihara0655.com/jikokanri_csv/covid.html";
            WebClient wc = new();
            wc.Encoding = Encoding.UTF8;
            string html = "error";
            //webさーばからhtmlのソースを取得
            try
            {
                html = wc.DownloadString(url);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message +"\nSSLサーバー証明書の有効期限が切れています。開発者に問い合わせてください");
            }
            wc.Dispose();
            //htmlのソースから数字のみを取り出す
            covidNum = Regex.Replace(html, @"[^0-9]", "");
        }
    }
}
//コードが少ないのはwebサーバーで陽性者数を取得して、
//C#で参照しやすいようにhtmlファイルにしているからです
//このクラスではwebサーバーにあるhtmlのソースを取得しているだけ
//webサーバーではpythonでこのサイトを(https://covid19.mhlw.go.jp/)スクレイピングしてます
//毎朝四時更新
