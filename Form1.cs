using System;
//using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;    //to run commands concurrently
using System.Text.RegularExpressions; 

namespace ADBConsole
{
    public partial class Form1 : Form
    {
        bool bTest;

        Thread m_adbListenThread;
        Queue adbOutputQueue;
        ADBAccess m_ADBAccess;
        bool m_bSuspendLogout;
        System.IO.StreamWriter m_logFile;
        
        String m_tagFilter;
        Boolean m_tagFilterEnabled;

        bool m_bCMDQueuing;

        //This queue contains ADB commands.
        //Reader is for dequeue the request CMD by ADBAccess
        //Writer is for enqueue new CMD by this Form1 class.
        Queue m_ADBCommandQueue;
        Queue m_ADBCommandQueueReader;
        Queue m_ADBCommandQueueWriter;

        private void StartADBThread()
        {
            m_adbListenThread = new Thread(new ThreadStart(StartListen));
            m_adbListenThread.Start();
        }

        public Form1()
        {
            InitializeComponent();

            m_bCMDQueuing = false;
            m_adbListenThread = null;
            bTest = true;
            m_tagFilter = "";
            m_tagFilterEnabled = false;

            String FILE_NAME = "AdbMessage";
            int suffix = 1;

            String fileName = @".\" + FILE_NAME + ".log";
            while( System.IO.File.Exists( fileName))
            {
                fileName = @".\" + FILE_NAME + suffix.ToString() + ".log";
                ++suffix;
            }

            fileLocationMsg.Text = @"The log file is saved at : " + fileName;
            m_logFile = new System.IO.StreamWriter( fileName );

            m_ADBCommandQueue = new Queue();
            m_ADBCommandQueueReader = Queue.Synchronized(m_ADBCommandQueue);
            m_ADBCommandQueueWriter = Queue.Synchronized(m_ADBCommandQueue);

            m_ADBAccess = new ADBAccess();
            m_ADBAccess.WinOutputQueueReader = m_ADBCommandQueueReader;
        }

        private void StartListen()
        {
            adbOutputQueue = new Queue(1000,2);
            
            while (true)
            {
                try
                {
                    if (!m_ADBAccess.isCMDOutputQueueEmpty())
                    {
                        adbOutputQueue.Enqueue(m_ADBAccess.CMDOutputDequeue() + "\r\n");
                    }

                    Application.DoEvents();

                    if (0 == adbOutputQueue.Count)
                    {                        
                        //only flush at idle time
                        m_logFile.Flush();
                        //only sleep when there is no string in the queue.
                        System.Threading.Thread.Sleep(2);
                    }
                    else
                    {                      
                         if ( !m_bSuspendLogout )
                         {
                             DisplayMessage(adbOutputQueue.Dequeue().ToString());
                         }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("WinListen Thread Exception: " + err.Message);
                    //Cleanup();
                    break;
                }                
            }
        }

        private delegate void DisplayDelegate(string message);
        private void DisplayMessage(string message)
        {
            if (consoleBox.InvokeRequired)
            {
                Invoke(new DisplayDelegate(DisplayMessage), new object[] { message });
            }
            else
            {                               
                try
                {
                    string pattern = @"[VIDWEF][/](\w|[-.:])+\s*[(]\s*\d+[)]";
                    Match match = Regex.Match(message, pattern);
                    if (match.Success)
                    {
                        if (m_tagFilterEnabled)
                        {
                            int startBracket = match.Value.IndexOf('(');
                            if (2 <= startBracket)
                            {
                                string tagName = match.Value.Substring(2, startBracket - 2);
                                
                                //not necessary/right, just easy the debugger
                                tagName.Trim();
                                tagName = tagName.ToUpper();

                                if (0 < tagName.Length)
                                {
                                    if (tagName.Equals(m_tagFilter))
                                    {
                                        consoleBox.SelectionBackColor = Color.SlateGray;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //consoleBox.SelectionBackColor = Color.SlateGray;
                        }

                        char tagLevel = match.Value[0];

                        switch (tagLevel)
                        {
                            case 'I':
                            case 'V':
                                {
                                    consoleBox.SelectionColor = Color.LightGreen;
                                    //consoleBox.SelectionBackColor = Color.SlateGray;
                                    break;
                                }
                            case 'D':
                                {
                                    consoleBox.SelectionColor = Color.DeepSkyBlue;
                                    //consoleBox.SelectionBackColor = Color.SlateGray;
                                    break;
                                }
                            case 'W':
                                {
                                    consoleBox.SelectionColor = Color.LightSalmon;
                                    //consoleBox.SelectionBackColor = Color.SlateGray;
                                    break;
                                }
                            case 'E':
                            case 'F':
                                {
                                    consoleBox.SelectionColor = Color.Tomato;
                                    //consoleBox.SelectionBackColor = Color.SlateGray;
                                    break;
                                }
                            default:
                                {
                                    consoleBox.SelectionColor = Color.White; //should not go here
                                    break;
                                }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception from Regex");
                }

                if (bTest)
                {
                    consoleBox.AppendText(message);
                    m_logFile.Write(message);
                }

                int maxLines = 2000;
                int delta = 100; //to improve performance, delete 100 lines each time
                if (consoleBox.Lines.Length > maxLines + delta)
                {
                    //Console.WriteLine(" >>>>>>>>>>>>>>> delete ");
                    consoleBox.SelectionStart = 0;

                    int pos = consoleBox.GetFirstCharIndexFromLine(consoleBox.Lines.Length - maxLines);

                    consoleBox.Select(0, pos);
                    consoleBox.SelectedText = "";
                    
                }

                consoleBox.SelectionStart = consoleBox.Text.Length;
                consoleBox.ScrollToCaret();

            }
        }

        private void ADBStartBtn_Click(object sender, EventArgs e)
        {
            if (null == m_adbListenThread)
            {
                StartADBThread();
                System.Threading.Thread.Sleep(100);
                m_ADBAccess.StartCMD();
                System.Threading.Thread.Sleep(200);

                //send demo command
                {
                    string ADBCMD;
                    //ADBCMD = "dir";
                    ADBCMD = "adb logcat -c";
                    m_bCMDQueuing = true;
                    m_ADBCommandQueueWriter.Enqueue(ADBCMD);
                    ADBCMD = "adb logcat -v time";
                    m_ADBCommandQueueWriter.Enqueue(ADBCMD);
                    m_bCMDQueuing = false;
                }
            }
            else
            {
                m_bSuspendLogout = false;
            }
        }

        private void ADBStopBtn_Click(object sender, EventArgs e)
        {
            m_bSuspendLogout = true;
        }

        private void Cleanup()
        {
            m_logFile.Close();
            m_ADBCommandQueue.Clear();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            m_ADBAccess.Exit();
            Cleanup();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void TagEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckState.Checked == this.TagEnable.CheckState)
            {
                m_tagFilter = TagNameBox.Text.ToUpper();
                m_tagFilterEnabled = true;
                TagNameBox.Enabled = false;
            }
            else
            {
                m_tagFilterEnabled = false;
                m_tagFilter = "";
                TagNameBox.Enabled = true;
            }
        }
    }
}