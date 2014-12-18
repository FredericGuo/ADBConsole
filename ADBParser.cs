using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;            //for Streams
using System.Diagnostics;   //for Process
using System.Threading;     //to run commands concurrently
using System.Runtime.InteropServices;

namespace ADBConsole
{
     /*class ADBParser
    {
        public ADBParser()
        {            
        }

       
        public bool Parse(ref List<string> i_CVSLog, out Dictionary<int, CVSInfo> o_CVSLog)
        {
            o_CVSLog = new Dictionary<int, CVSInfo>();

            //int DEBUG_INFO_MAX_VER = 0;

            string tempString;
            //bool bSkippedHeader = false;
            for (int i = 0; i < i_CVSLog.Count; ++i)
            {
                //tempString = i_CVSLog[i];
                //if (tempString.Contains("-----"))
                //{
                //    bSkippedHeader = true;
                //    continue;
                //}
                //else if (!bSkippedHeader)
                //{
                //    continue;
                //}

                //start parse the record
                CVSInfo CVSRecord = new CVSInfo();

                for (int j = i; j < i_CVSLog.Count; ++j)
                {
                    tempString = i_CVSLog[j];

                    if (0 == tempString.IndexOf("revision"))
                    {
                        int[] RevNum = ParseRevision(tempString);
                        CVSRecord.Revision = RevNum;
                    }
                    else if (0 == tempString.IndexOf("date"))
                    {
                        CVSRecord.Datetime = ParseDateTime(tempString);
                    }
                    else if (0 == tempString.IndexOf("branches"))
                    {
                        //do nothing
                    }
                    else if (tempString.Contains("-----") ||
                             tempString.Contains("====="))  //<--last record use "=="
                    {
                        o_CVSLog.Add(CVSRecord.Revision[CVSRecord.Revision.Length - 1], CVSRecord);

                        //if (DEBUG_INFO_MAX_VER < CVSRecord.Revision[CVSRecord.Revision.Length - 1])
                        //{
                        //    DEBUG_INFO_MAX_VER = CVSRecord.Revision[CVSRecord.Revision.Length - 1];
                        //}

                        i = j; // i is used by external loop, 
                        // "++i" will be called immediately.
                        break;
                    }
                    else
                    {
                        //not all comments contains "SCR". 
                        //Early version has no such limitation.
                        CVSRecord.Comment = tempString;
                    }
                }
            }

            //Console.WriteLine("Revision count: {0}", DEBUG_INFO_MAX_VER);

            //Debug.Assert(o_CVSLog.Count == DEBUG_INFO_MAX_VER);

            return true;
        }
        
        private DateTime ParseDateTime(string tempString)
        {
            //tempString should be like: "date: 2010/03/18 15:01:38; author..."

            //get date
            int startPos = tempString.IndexOf(":");
            Debug.Assert(-1 != startPos);
            int endPos = tempString.IndexOf(" ", startPos + 2);
            Debug.Assert(-1 != endPos);
            string subString = tempString.Substring( startPos+1, endPos-startPos-1);
            string[] DateStr = subString.Split('/');
            Debug.Assert( 3 == DateStr.Length );
            int[] DateNum = new int[3];
            try
            {
                DateNum[0] = Convert.ToInt32(DateStr[0].Trim());
                DateNum[1] = Convert.ToInt32(DateStr[1].Trim());
                DateNum[2] = Convert.ToInt32(DateStr[2].Trim());
            }
            catch (FormatException err)
            {
                Console.WriteLine("ParseDateTime exception: " + err.Message);
                DateNum[0] = 99999; //put a odd number here
                DateNum[1] = 99999; //put a odd number here
                DateNum[2] = 99999; //put a odd number here

            }

            //get time
            startPos = endPos;                
            endPos = tempString.IndexOf(";", startPos + 2);
            Debug.Assert(-1 != endPos);
            subString = tempString.Substring(startPos + 1, endPos - startPos - 1);
            string[] TimeStr = subString.Split(':');
            Debug.Assert(3 == TimeStr.Length);
            int[] TimeNum = new int[3];
            try
            {
                TimeNum[0] = Convert.ToInt32(TimeStr[0].Trim());
                TimeNum[1] = Convert.ToInt32(TimeStr[1].Trim());
                TimeNum[2] = Convert.ToInt32(TimeStr[2].Trim());
            }
            catch (FormatException err)
            {
                Console.WriteLine("ParseDateTime exception: " + err.Message);
                TimeNum[0] = 99999; //put a odd number here
                TimeNum[1] = 99999; //put a odd number here
                TimeNum[2] = 99999; //put a odd number here

            }

            DateTime ret = new DateTime(
            DateNum[0],
            DateNum[1],
            DateNum[2],
            TimeNum[0],
            TimeNum[1],
            TimeNum[2]);

            return ret;
        }

        private int[] ParseRevision(string tempString)
        {
            //tempString should be like: "revision 1.1.6.18"

            int startPos = tempString.IndexOf(" ");
            Debug.Assert(-1 != startPos);
            string subString = tempString.Substring( startPos+1 );

            string[] NumStr = subString.Split('.');
            int[] ret = new int[NumStr.Length];

            for (int i = 0; i < NumStr.Length; ++i)
            {
                subString = NumStr[i].Trim();
                try
                {
                    ret[i] = Convert.ToInt32(subString);
                }
                catch (FormatException err)
                {
                    Console.WriteLine("ParseRevision exception: " + err.Message);
                    ret[i] = 99999; //put a odd number here

                }
            }

            return ret;
        }
    }
    */
    class ADBAccess
    {
        Process processCmd;

        Thread CMDThread;

        bool m_bCMDWriting;
        bool m_bWinWriting;

        Queue m_CMDOutputQueue;
        Queue m_CMDOutputQueueReader;
        Queue m_CMDOutputQueueWriter;

        Queue m_WinOutputQueueReader;

        public Queue WinOutputQueueReader
        {
            set { m_WinOutputQueueReader = value; }
        }

        public string CMDOutputDequeue()
        {
            if (0 == m_CMDOutputQueueReader.Count)
            {
                throw new System.Exception("CMDOutputDequeue 0 size.");
            }

            return m_CMDOutputQueueReader.Dequeue().ToString();
        }

        public bool isCMDOutputQueueEmpty()
        {
            return 0 == m_CMDOutputQueueReader.Count;
        }

        public ADBAccess()
        {            
            m_bCMDWriting = false;
            m_bWinWriting = false;

            processCmd = null;

            m_CMDOutputQueue = new Queue();
            m_CMDOutputQueueReader = Queue.Synchronized(m_CMDOutputQueue);
            m_CMDOutputQueueWriter = Queue.Synchronized(m_CMDOutputQueue);

            m_WinOutputQueueReader = null;

            string errorStr;

            CMDThread = new Thread(new ThreadStart(RunCMD));            
        }

        public void StartCMD()
        {
            if (CMDThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                CMDThread.Start();
            }
        }

        private void RunCMD()
        {
            StringBuilder strInputCMD = new StringBuilder();
            processCmd = new Process();
            processCmd.StartInfo.FileName = "cmd.exe";
            processCmd.StartInfo.CreateNoWindow = true;
            processCmd.StartInfo.UseShellExecute = false;
            processCmd.StartInfo.RedirectStandardOutput = true;
            processCmd.StartInfo.RedirectStandardInput = true;
            processCmd.StartInfo.RedirectStandardError = true;
            processCmd.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
            processCmd.ErrorDataReceived += new DataReceivedEventHandler(CmdErrorDataHandler);
            processCmd.Start();
            processCmd.BeginOutputReadLine();

            while (true)            
            {
                try
                {
                    if (m_bWinWriting) //don't read/write concurrently
                        continue;
                    
                    if (0 < m_WinOutputQueueReader.Count)
                    {
                        strInputCMD.Append(m_WinOutputQueueReader.Dequeue());
                        strInputCMD.Append("\n");
                        processCmd.StandardInput.WriteLine(strInputCMD);
                        strInputCMD.Remove(0, strInputCMD.Length);
                    }
                    System.Threading.Thread.Sleep(400);                  
                }
                catch (Exception err)
                {
                    Console.WriteLine("Stop CMD thread." + err.Message);
                    Cleanup();
                    break;
                }
            }
        }

        public void Exit()
        {
            if ( processCmd != null && !processCmd.HasExited)
            {
                try { 
                    processCmd.Kill(); 
                }
                catch (Exception err)
                {
                    Console.WriteLine("Cleanup exception: " + err.Message);
                };
            }
        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {         
            if(!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    m_bCMDWriting = true;                    
                    m_CMDOutputQueueWriter.Enqueue(outLine.Data);
                    m_bCMDWriting = false;
                }
                catch (Exception err)
                {
                    Console.WriteLine("CMD Thread Exception: " + err.Message);
                }
            }
        }

        private void CmdErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    Console.WriteLine("CMD Error: " + outLine.Data);
                }
                catch (Exception err)
                {
                    Console.WriteLine("CMD Error Handler Exception: " + err.Message);
                }

            }
        }

        private void Cleanup()
        {
            try { processCmd.Kill(); }
            catch (Exception err)
            {
                Console.WriteLine("Cleanup exception: " + err.Message);
            };
            
            m_CMDOutputQueue.Clear();
        }
    }
}
