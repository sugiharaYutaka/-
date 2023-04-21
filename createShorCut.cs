using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Windows;
namespace 自己管理アプリ
{
	class createShortCut
	{
		public createShortCut()
		{
            // ショートカットのリンク先(起動するプログラムのパス)
            string targetPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            string filename = Path.GetFileName(targetPath) + ".lnk";
                
            // ショートカットそのもののパス
            string shortcutPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup),@filename);

            // WshShellを作成
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            // ショートカットのパスを指定して、WshShortcutを作成
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            // ①リンク先
            shortcut.TargetPath = targetPath;
            // ②引数
            shortcut.Arguments = "/a /b /c";
            // ④実行時の大きさ 1が通常、3が最大化、7が最小化
            shortcut.WindowStyle = 7;
            // ⑤コメント
            shortcut.Description = "自己管理アプリのショートカット";
            // ⑥アイコンのパス 自分のEXEファイルのインデックス0のアイコン
            //shortcut.IconLocation = Application.ExecutablePath + ",0";

            // ショートカットを作成
            //shortcutPath += "\\" + filename + ".lnk";
            try
            {
                if (System.IO.File.Exists(shortcutPath) != true)
                    shortcut.Save();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString() );
            }

            // 後始末
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);

        }
		
	}
}
