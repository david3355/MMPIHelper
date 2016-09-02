using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MMPIHelper
{
    public class MSDosKeyStrokeSimulator : KeystrokeSimulator
    {
        private static void sendKeystroke(char ch)
        {
            uint intReturn = 0;
            NativeWIN32.INPUT structInput;
            structInput = new NativeWIN32.INPUT();
            structInput.type = (uint)1;
            structInput.ki.wScan = (ushort)NativeWIN32.MapVirtualKey((uint)ch, 0); ;
            structInput.ki.time = 0;
            structInput.ki.dwFlags = 0;
            structInput.ki.dwExtraInfo = 0;
            //keydown
            structInput.ki.wVk = (ushort)ch;
            intReturn = NativeWIN32.SendInput((uint)1, ref structInput, Marshal.SizeOf(structInput));
            //Keyup
            structInput.ki.dwFlags = NativeWIN32.KEYEVENTF_KEYUP;
            structInput.ki.wVk = (ushort)ch;
            intReturn = NativeWIN32.SendInput((uint)1, ref structInput, Marshal.SizeOf(structInput));
        }

        public static void SendCharNative(Process Process, char Character)
        {
            SetForegroundWindow(Process);
            sendKeystroke(Character);
        }

        public static void SendCharsNative(Process Process, char[] Characters)
        {
            SendCharsNative(Process, Characters, 0);
        }

        public static void SendCharsNative(Process Process, char[] Characters, int DelayMillisec)
        {
            SetForegroundWindow(Process);
            foreach (char ch in Characters)
            {
                sendKeystroke(ch);
                if (DelayMillisec > 0) Thread.Sleep(DelayMillisec);
            }
        }
    }
}
