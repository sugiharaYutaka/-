using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace 自己管理アプリ
{
    internal class Button_Ex :Button
    {
        UserControl3 usrCtrl3;
        SubWindow_Ex ts;
        public Button_Ex(UserControl3 obj, SubWindow_Ex obj1)
        {
            usrCtrl3 = obj;
            ts = obj1;
        }
        private string Appname;
        // ボタンへのイベントをセットする
        public void eventMaking( string appname)
        {
            this.Click += new RoutedEventHandler(doClickEvent);
            Appname = appname;
        }

        // ボタンへのイベントを解除する
        public void eventSuspend()
        {
            this.Click -= new RoutedEventHandler(doClickEvent);
        }

        // クリックイベントの実体(参照するリストボックスに文言テキストを追加)
        public void doClickEvent(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer("data\\button.wav");
            player.Play();
            ts = new SubWindow_Ex();
            usrCtrl3.ReturnObj(ts);
            usrCtrl3.stackPanel_in.Children.Clear();
            
            ts.textBlock_title.Text = Appname;
            usrCtrl3.stackPanel_in.Margin = new Thickness(20, 20, usrCtrl3.stackPanel_out.Width+20, 20);
            
            usrCtrl3.stackPanel_in.Children.Add(ts);
        }
    }
}
