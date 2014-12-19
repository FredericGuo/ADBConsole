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
