using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.Win32;
using static Windows.Win32.PInvoke;
using Microsoft.UI;
using Windows.Foundation.Metadata;

namespace WinMediaID.Classes
{
    partial class DisableAutorun
    {
        /*
        private IntPtr _queryCancelAutoPlay = 0;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
       public static extern IntPtr RegisterWindowMessage(string lpString);
       
        protected override void WndProc(ref IntPtr m)
        {
            //if the QueryCancelAutoPlay message id has not been registered...
            if (_queryCancelAutoPlay == 0)
                _queryCancelAutoPlay = RegisterWindowMessage("QueryCancelAutoPlay");

            //if the window message id equals the QueryCancelAutoPlay message id
            if (m == _queryCancelAutoPlay)
            {
                m = 1;
            }
        } */
    }
}
