// Assignment 4
// Pete Myers
// OIT, Spring 2018
// Handout

using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace SimpleShell
{
    public class Terminal
    {
        // NOTE: performs line discipline over driver
        private TerminalDriver driver;
        private LineQueue completedLineQueue;
        private Handler handler;

        public Terminal(TerminalDriver driver)
        {
            completedLineQueue = new LineQueue();
            handler = new Handler(driver, completedLineQueue);

            this.driver = driver;
            this.driver.InstallInterruptHandler(handler);
        }

        public void Connect()
        {
            // TODO
        }

        public void Disconnect()
        {
            // TODO
        }

        public bool Echo { get { return handler.Echo; } set { handler.Echo = value; } }

        public string ReadLine()
        {
            // NOTE: blocks until a line of text is available
            // TODO
            return null;
        }

        public void Write(string line)
        {
            // TODO
        }

        public void WriteLine(string line)
        {
            // TODO
        }

        private class LineQueue
        {
            private Queue<string> theQueue;
            private Mutex mutex;
            private ManualResetEvent hasItemsEvent;

            public LineQueue()
            {
                this.theQueue = new Queue<string>();
                this.mutex = new Mutex();
                this.hasItemsEvent = new ManualResetEvent(false);   // initially is empty
            }

            public void Insert(string s)
            {
                // wait until both there is capacity and we have the mutex
                // insert into the buffer
                // signal any threads waiting to remove an object
                // TODO
            }

            public string Remove()
            {
                // wait until there is at least one object in the queue and we have the mutex
                // remove the item from the buffer
                // block any threads waiting to remove, if the queue is empty
                // TODO

                return null;
            }

            public int Count()
            {
                // wait until we have the mutex
                // return the number of items in the queue
                // TODO
                return 0;
            }
        }

        class Handler : TerminalInterruptHandler
        {
            private TerminalDriver driver;
            private List<char> partialLineQueue;
            private LineQueue completedLineQueue;

            public Handler(TerminalDriver driver, LineQueue completedLineQueue)
            {
                this.driver = driver;
                this.completedLineQueue = completedLineQueue;
                this.partialLineQueue = new List<char>();
            }

            public bool Echo { get; set; }

            public void HandleInterrupt(TerminalInterrupt interrupt)
            {
                switch (interrupt)
                {
                    case TerminalInterrupt.CHAR:
                        // queue up the characters until we have a completed line
                        // TODO
                        break;

                    case TerminalInterrupt.ENTER:
                        // get all the characters from the partial line queue and create a completed line
                        // TODO
                        break;

                    case TerminalInterrupt.BACK:
                        // throw away the last character entered
                        // TODO
                        break;
                }
            }
        }
    }
}
