using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WindowsInput.Native;
using WindowsInput;
using System.Runtime.InteropServices;
using System.Media;

namespace NetflixLauncher
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private const
        String PROCESS_NAME = "ApplicationFrameHost",
               WINDOW_TITLE = "Netflix";

        static void Main(string[] args)
        {
            Process netflixProcess = new Process();


            DetectNetflixOpened();

            StartNetflix();

            WaitForSeconds(1);

            if (!FindNetflix(ref netflixProcess))
            {
                SystemSounds.Hand.Play();
                Environment.Exit(1);
            }

            EnterFullscreenNetflix(netflixProcess);

            //DetectNetflixClosed(netflixProcess.Id);
        }


        private static void DetectNetflixOpened()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Equals(PROCESS_NAME)
                && process.MainWindowTitle.Equals(WINDOW_TITLE))
                {
                    process.Kill();
                    break;
                }
            }
        }

        private static void StartNetflix()
        {
            String dir = @"C:\Program Files (x86)\Windows Kits\10\App Certification Kit\microsoft.windows.softwarelogo.appxlauncher.exe",
                   appID = "4DF9E0F8.Netflix_mcm4njqhnhss8!Netflix.App";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = dir;
            startInfo.Arguments = appID;
            startInfo.ErrorDialog = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process proc = new Process();
            proc.StartInfo = startInfo;
            proc.Start();
        }

        private static void WaitForSeconds(double seconds)
        {
            System.Threading.Thread.Sleep((int)(seconds * 1000));
        }

        private static bool FindNetflix(ref Process netflixProcess)
        {
            Process[] processlist = Process.GetProcesses();

            bool netflixFound = false;

            foreach (Process process in processlist)
            {
                //Console.WriteLine("TITLE: " + process.MainWindowTitle + " || NAME: " + process.ProcessName);
                if ((process.MainWindowTitle.Equals(WINDOW_TITLE)) && (process.ProcessName.Equals(PROCESS_NAME)))
                {
                    //Console.WriteLine("FOUND!!!");
                    netflixProcess = process;
                    netflixFound = true;
                    break;
                }
            }
            return netflixFound;
        }

        private static void EnterFullscreenNetflix(Process netflixProcess)
        {
            SetForegroundWindow(netflixProcess.MainWindowHandle);

            InputSimulator sim = new InputSimulator();
            sim.Keyboard.ModifiedKeyStroke(new[] { VirtualKeyCode.LWIN, VirtualKeyCode.LSHIFT }, new[] { VirtualKeyCode.RETURN });
        }

        private static void DetectNetflixClosed(int netflixId)
        {
            Process.GetProcessById(netflixId).WaitForExit();
        }

        //private static void DetectNetflixClosed(int netflixId)
        //{
        //    bool procIDExists = true;
        //    while (procIDExists)
        //    {
        //        WaitForSeconds(4);

        //        try
        //        {
        //            Process.GetProcessById(netflixId);
        //        }
        //        catch (Exception e)
        //        {
        //            procIDExists = false;
        //        }
        //    }
        //}
    }
}
