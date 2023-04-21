using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace 自己管理アプリ
{
    class MyTransition
    {
        BlurEffect be;
        private double easing;
        private bool easingEndFlag;
        double time;
        double timeMax;
        double xx = 0;
        public MyTransition() 
        {
            be = new BlurEffect();
            be.Radius = 0;
            be.KernelType = KernelType.Gaussian;
            easing= 0;
            ///同期用
            easingEndFlag = false;
            time = 1;

        }
        public void timeInit(double time)
        {
            timeMax = time;
        }
        public void EasingInit(double easingInit)
        {
            easing = easingInit;
            easingEndFlag = true;
            time = 1;
        }
        /// <summary>
        /// イージング用の値を返す。１から第1引数までを加算する
        /// 呼び出した回数で値が変化するのでループで呼び出してください
        /// </summary>
        /// <param name="end">イージングの値の終点</param>
        /// <returns>イージング用の値</returns>
        public double EasingIn(double end)
        {
            double t = time / timeMax;
            //easing =  -end* t *t *(t- timeMax*);
            easing = (end * 2) * t - (t * t * t);
            time++;
            if (end < easing)
            {
                easingEndFlag = true;
                return end;
            }
            return easing; 
        }
        /// <summary>
        /// イージング用の値を返す。第１引数からまでを減算する
        /// 呼び出した回数で値が変化するのでループで呼び出してください
        /// </summary>
        /// <param name="start">イージングの値の終点</param>
        /// <param name="end">イージングの値の始点</param>
        /// <returns>イージング用の値</returns>
        public double EasingOut(double end)
        {
            double t = time / timeMax;
            easing -= end * t * t * t + 1;
            time++;
            if (0 >= easing)
            {
                easingEndFlag = true;
                return 1;
            }
            return easing;
        }
        /// <summary>
        /// 第一引数で指定したオブジェクトにぼかし処理を施す。
        /// 呼び出した回数だけぼかしが濃くなるのでループで呼び出してください
        /// </summary>
        /// <param name="obj">ぼかす対象のオブジェクト</param>
        /// <param name="end">ぼかしの濃さの限界</param>
        /// <returns>/// <returns>ぼかし処理がendまで到達したらfalseを返す。
        /// Easing関数と同期している場合はイージングの処理が終わってからfalseを返す</returns>
        public bool Blur_In(Grid obj, double end, bool easingSync)
        {
            timeMax = end;
            be.Radius++;
            obj.Effect = be;
            if (end <= be.Radius)
            {
                obj.Effect = be;
                be.Radius = end;
                if (easingSync || easingEndFlag)
                    return false;
                else if (easingSync == false)
                    return false;
                
                    
            }
            return true;
        }
        /// <summary>
        /// 第一引数で指定したオブジェクトのぼかし処理を薄める。
        /// 呼び出した回数だけぼかしが薄くなるのでループで呼び出してください
        /// </summary>
        /// <param name="obj">ぼかす対象のオブジェクト</param>
        /// <param name="end">ぼかしを薄める限界</param>
        
        /// <returns>ぼかし処理がendまで到達したらfalseを返す。
        /// Easing関数と同期している場合はイージングの処理が終わってからfalseを返す</returns>
        public bool Blur_Out(Grid obj, double end,bool easingSync)
        {
            timeMax = end;
            be.Radius--;
            obj.Effect = be;
            if (0 > be.Radius)
            {
                obj.Effect = be;
                be.Radius = 0;
                if (easingSync || easingEndFlag)
                    return false;
                else if (easingSync == false)
                    return false;
            }
            return true;
        }

    }
}
