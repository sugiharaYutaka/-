using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.IO.Pipes;
using System.Diagnostics;
using System.Windows.Shapes;

namespace 自己管理アプリ
{
    public class process
    {
        private List<string> AppNameList;
        private string AppNameBuff;
        private int TimeCount;

        public process()
        {
            DateTime dateTime = DateTime.Now;
            if(dateTime.Day == 1)
            {
                File.Delete("data\\process.ini");
                File.Create("data\\process.ini");
            }    
            AppNameList = new List<string>();
            AppNameBuff = "null";
            TimeCount = 0;
            ini();
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        [DllImport("psapi.dll", CharSet = CharSet.Ansi)]
        private static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder lpBaseName, uint nSize);
        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, [MarshalAs(UnmanagedType.U4)] int nSize);


        [DllImport("KERNEL32.DLL")]
        public static extern uint WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("KERNEL32.DLL")]
        static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);
        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        /// <summary>
        /// Timer.Tick()で呼び出す用メソッド。processクラスの情報を更新する。Timerのintervalは1000のみ
        /// </summary>
        /// <returns>操作中のプロセスの名前</returns>
        public string UpdateForeGround()
        {
            //開いているプロセスを特定するハンドルを取得
            var processHnd = GetForegroundWindow();
            //ハンドルからプロセスIDを取得
            GetWindowThreadProcessId(processHnd, out uint processID);
            string rt = "nodata";
            //processHndがゼロではない場合　→　何も開いていない場合
            if (processHnd != IntPtr.Zero)
            {
                //プロセスIDからプロセス情報を取得
                var handle = OpenProcess(0x0400 | 0x0010, false, processID);
                var buffer = new StringBuilder(255);
                //GetModuleBaseName(handle, IntPtr.Zero, buffer, (uint)buffer.Capacity);
                GetModuleFileNameEx(handle, IntPtr.Zero, buffer, (int)buffer.Capacity);
                //Debug.Print(buffer.ToString().ToLower());
                System.Diagnostics.Debug.Print("現在のファアグラウンドプロセスは　" + SplitString(buffer.ToString()));


                //handleの初期化
                handle = IntPtr.Zero;
                
                rt = SplitString(buffer.ToString());
                if (rt == "自己管理アプリ.exe")
                    rt = "nodata";
                else
                {
                    UpdateList(rt);
                    calcTime(rt);
                    AppNameBuff = rt;
                }
            }
            return rt;
        }

        private void UpdateList(string processName)
        {
            if (AppNameList.IndexOf(processName) == -1)
            {
                AppNameList.Add(processName);
            }
        }

        private string SplitString(string str)
        {
            char[] delimiterChars = { '\\' };
            const string searchStr = ".exe";
            const int searchCnt = 4;
            string[] wordArray = str.Split(delimiterChars);
            foreach (string word in wordArray)
            {
                if (word.IndexOf(searchStr) >= searchCnt)
                {
                    return word;
                }
            }
            return "error";
        }



        private void ini()
        {
            StringBuilder sectionNames = new StringBuilder(256);
            byte[] byteAry = new byte[1024];
            string path = "data\\process.ini";
            if (Directory.Exists("data") == false)
            {
                Directory.CreateDirectory("data");
            }
            if (File.Exists(path) == false)
            {
                File.Create(path);
            }

            IntPtr ptr = Marshal.StringToHGlobalAnsi(new String('\0', 1024));
            int length = GetPrivateProfileSectionNames(ptr, 1024, path);

            if (0 < length)
            {
                String result = Marshal.PtrToStringAnsi(ptr, length).TrimEnd(new char[] { '\0' });
                Array.ForEach<String>(result.Split('\0'), s =>
                {
                    AppNameList.Add(s);
                });
            }
        }

        private void calcTime(string AppName)
        {
            int count = 0;
            
            
            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString(AppName, "ActiveTime", "error", sb, 64, "data\\process.ini");
            if (sb.ToString() != "error")
            {
                    count = int.Parse(sb.ToString()) + 1;
                    WritePrivateProfileString(AppName, "ActiveTime", count.ToString(), "data\\process.ini");
            }
            else
            {
                WritePrivateProfileString(AppName, "ActiveTime", "0", "data\\process.ini");
            }
            System.Diagnostics.Debug.Print(sb.ToString());
        }
        
        /// <summary>
        /// dataフォルダーの中にあるiniファイルから起動時間を返す
        /// </summary>
        /// <param name="AppName">取得したいアプリの名前。アプリ名を取得したい場合はGetAppNameメソッドを使用してください</param>
        /// <returns>起動時間を秒単位で返す。エラー時は-1が返る</returns>
        public int GetActiveTime(string AppName)
        {
            StringBuilder sb = new StringBuilder(64);
            string path = "data\\process.ini";
            if (Directory.Exists("data") == false)
            {
                Directory.CreateDirectory("data");
            }
            if (File.Exists(path) == false)
            {
                File.Create(path);
            }

            GetPrivateProfileString(AppName, "ActiveTime", "error", sb, 64, path);
            if (sb.ToString() != "error")
                return int.Parse(sb.ToString());
            else
                return -1;
        }
        /// <summary>
        /// 今まで起動されたアプリの名前が格納されたリストを返す
        /// </summary>
        /// <returns>アプリの名前が格納されたリスト</returns>
        public List<string> GetAppNameList()
        {
            return AppNameList;
        }
        /// <summary>
        /// 引数で指定したプロセスの起動時間が指定された時間に達しているかチェックする
        /// </summary>
        /// <param name="AppName"></param>
        /// <returns>指定時間に達している場合true それ以外はfalse</returns>
        public bool CheckMaxtime(string AppName)
        {
            StringBuilder sb = new StringBuilder();
            const string path = "data\\process.ini";

            GetPrivateProfileString(AppName, "MaxTime", "error", sb, 64, path);
            string Maxtime = sb.ToString();
            GetPrivateProfileString(AppName, "ActiveTime", "error", sb, 64, path);
            string Activetime = sb.ToString();

            if (Maxtime != "__null__" && Maxtime != "error"　&& Activetime != "error")
            {
                if (int.Parse(Activetime) >= int.Parse(Maxtime))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public int GetMaxtimeDiff(string AppName)
        {
            StringBuilder sb = new StringBuilder();
            const string path = "data\\process.ini";
            int diff = -1;
            GetPrivateProfileString(AppName, "MaxTime", "error", sb, 64, path);
            string Maxtime = sb.ToString();

            GetPrivateProfileString(AppName, "ActiveTime", "error", sb, 64, path);
            string Activetime = sb.ToString();
            

            if (Maxtime != "__null__" && Maxtime != "error" && Activetime != "error")
            {
                diff = int.Parse(Maxtime) - int.Parse(Activetime);
                return diff;
            }
            else
                return diff;
        }
        /// <summary>
        /// 引数で指定したアプリのタスクを切る。
        /// </summary>
        /// <param name="Appname"></param>
        public void KillProcess(string Appname)
        {
            char[] delimiterChars = { '.' };
            const string searchStr = "exe";
            string[] wordArray = Appname.Split(delimiterChars);
            foreach (string word in wordArray)
            {
                if (word.IndexOf(searchStr) != 0)
                {
                    Appname = word;
                    break;
                }
            }
            Process[] ps = Process.GetProcessesByName(Appname);
            foreach (Process pro in ps)
            {
                pro.Kill();
            }
        }
    }
}
