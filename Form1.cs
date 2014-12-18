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
        StringBuilder adbOutputStrings;
        ADBAccess m_ADBAccess;
        bool m_bSuspendLogout;
        
        //bool m_bCMDWriting;
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

            //m_bCMDWriting = false;
            m_bCMDQueuing = false;
            m_adbListenThread = null;
            bool m_bSuspendLogout = false;
            bTest = true;

            m_ADBCommandQueue = new Queue();
            m_ADBCommandQueueReader = Queue.Synchronized(m_ADBCommandQueue);
            m_ADBCommandQueueWriter = Queue.Synchronized(m_ADBCommandQueue);

            m_ADBAccess = new ADBAccess();
            m_ADBAccess.WinOutputQueueReader = m_ADBCommandQueueReader;

            /*
            string line2 = @"12-15 14:39:27.889 I/ActivityManager(  566): Killing 26999:com.google.android.apps.docs/u0a42i12 (adj 9): isolated not needed";            
             * 

            string dateValue = "";
            string timeValue = "";
            char tagLevel;
            string tagName = "";
            try
            {            
                string pattern = @"^\d{2}[-]\d{2}";
                Match match = Regex.Match(line2, pattern);
                if(match.Success)
                {
                    dateValue = match.Value;
                }

                pattern = @"\d{2}[:]\d{2}[:]\d{2}[.]\d{3}";
                match = Regex.Match(line2, pattern);
                if (match.Success)
                {
                    timeValue = match.Value;
                }

                pattern = @"[IDWE][/]\w+\s*[(]\s*\d+[)]";
                match = Regex.Match(line2, pattern);
                if (match.Success)
                {
                    tagLevel = match.Value[0];

                    int startBracket = match.Value.IndexOf('(');
                    if( 2 <= startBracket )
                    {
                        tagName = match.Value.Substring(2, startBracket - 2);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception from Regex");
            }*/
        }

        private void StartListen()
        {
            adbOutputStrings = new StringBuilder();

            while (true)
            {
                try
                {
                    //if (m_bCMDWriting) //don't read/write concurrently
                    //    continue;

                    if (!m_ADBAccess.isCMDOutputQueueEmpty())
                    {
                        adbOutputStrings.Append(m_ADBAccess.CMDOutputDequeue());
                        if (0 == adbOutputStrings.Length)
                        {
                            break;
                        }
                        else
                        {
                            adbOutputStrings.Append("\r\n");
                            Application.DoEvents();

                            if ( !m_bSuspendLogout )
                            {
                                DisplayMessage(adbOutputStrings.ToString());
                                adbOutputStrings.Remove(0, adbOutputStrings.Length);
                            }
                        }
                    }                  
                }
                catch (Exception err)
                {
                    Console.WriteLine("WinListen Thread Exception: " + err.Message);
                    //Cleanup();
                    break;
                }
                System.Threading.Thread.Sleep(2);
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
                //if (!ParsingCVSLog)
                //{

                                
                try
                {
                    /*
                    string pattern = @"^\d{2}[-]\d{2}";
                    Match match = Regex.Match(message, pattern);
                    string dateValue = "";
                    if (match.Success)
                    {
                        dateValue = match.Value;
                    }

                    pattern = @"\d{2}[:]\d{2}[:]\d{2}[.]\d{3}";
                    match = Regex.Match(message, pattern);
                    string timeValue = "";
                    if (match.Success)
                    {
                        timeValue = match.Value;
                    }
                    */
                    string pattern = @"[VIDWEF][/]\w+\s*[(]\s*\d+[)]";
                    Match match = Regex.Match(message, pattern);
                    if (match.Success)
                    {
                        char tagLevel = match.Value[0];

                        switch (tagLevel)
                        {
                            case 'I':
                            case 'V':
                                {
                                    consoleBox.SelectionColor = Color.Green;
                                    break;
                                }
                            case 'D':
                                {
                                    consoleBox.SelectionColor = Color.Blue;
                                    break;
                                }
                            case 'W':
                                {
                                    consoleBox.SelectionColor = Color.Orange;
                                    break;
                                }
                            case 'E':
                            case 'F':
                                {
                                    consoleBox.SelectionColor = Color.Red;
                                    break;
                                }
                            default:
                                {
                                    consoleBox.SelectionColor = Color.White; //should not go here
                                    break;
                                }
                        }
                        /*int startBracket = match.Value.IndexOf('(');
                        if (2 <= startBracket)
                        {
                            string tagName = match.Value.Substring(2, startBracket - 2);
                            tagName.trim();
                        }*/
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception from Regex");
                }

                consoleBox.AppendText(message);

                /*
                if (bTest)
                {
                    consoleBox.SelectionColor = Color.Red;
                    bTest = false;
                }
                else
                {
                    if (Color.White != consoleBox.SelectionColor)
                    {
                        consoleBox.SelectionColor = Color.White;
                    }
                }*/
                //consoleBox.SelectedText = Environment.NewLine + "Test selected text";

                consoleBox.SelectionStart = consoleBox.Text.Length;
                consoleBox.ScrollToCaret();

                //set focus back to input textbox
                //textBox2.Focus();

                /*
                if (m_bDownloadingString && message == "U string.stx\r\n")
                {
                    m_bDownloadingString = false;
                    string argument = m_CVSAccess.TempFolder + @"\" + "string.stx";

                    m_WaitDialog.CloseDialog = true;

                    if (OpenNotepadRBtn.Checked)
                    {
                        System.Diagnostics.Process.Start("notepad", argument);
                    }
                    else
                    {
                        SaveStringToDisk(argument);
                    }
                }*/
                //}
                /*
                if (ParsingCVSLog)
                {
                    string result = m_CVSAccess.TempFolder.ToUpper() + ">" + FINISH + "\r\n";
                    if ((0 == HistoryList.Items.Count) &&
                        (message.ToUpper() == result.ToUpper()))
                    {
                        //incorrect username or password
                        //failed to communicate with CVS server
                        //OR
                        //no string.stx under specified tag branch.                                                                                    
                        ParsingCVSLog = false;
                        m_WaitDialog.CloseDialog = true;
                        textBox1.AppendText(@" >>>>  no string history information found.");
                        MessageBox.Show("Can not get string log, \r\nPlease check CVS account info and branch Tag");
                    }
                    else
                    {
                        PushMessageToLogQueue(message);
                    }
                }*/
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
                    //ADBCMD = @"C:\SDK_64-20140702\sdk\platform-tools\adb.exe";
                    ADBCMD = "adb logcat -v time";
                    m_bCMDQueuing = true;
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
            m_ADBCommandQueue.Clear();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            m_ADBAccess.Exit();
            Cleanup();
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}