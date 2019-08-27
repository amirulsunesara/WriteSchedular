using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WriteScheduler.Models;
using WriteScheduler.Interfaces;
using System.IO;

namespace WriteScheduler.Schedulers
{
    class RoundRobinScheduler : Interfaces.IWriteScheduler
    {
        private RoundRobin p1;
        private RoundRobin p2;
        private RoundRobin p3;
        private RoundRobin p4;
        private RoundRobin p5;


        private Thread t1;
        private Thread t2;
        private Thread t3; 
        private Thread t4; 
        private Thread t5; 


        // Queue of Model consist of properties of each file
        private Queue<RoundRobin> queueOfModel = new Queue<RoundRobin>();
        // Queue of Threads will handle each model
        private Queue<Thread> queueOfThreads = new Queue<Thread>();
        // Any other thread is blocked from acquiring the lock and waits until the lock is released.
        private object lockObj = new object();

        public RoundRobinScheduler()
        {
            // Initialize thread model
            //Interface reference is type casted to round robin class
            p1 = (RoundRobin)Write("p1", Encoding.ASCII.GetBytes("this a first model"));
            p2 = (RoundRobin)Write("p2", Encoding.ASCII.GetBytes("this a second model"));
            p3 = (RoundRobin)Write("p3", Encoding.ASCII.GetBytes("this a third model"));
            p4 = (RoundRobin)Write("p4", Encoding.ASCII.GetBytes("this a fourth model"));
            p5 = (RoundRobin)Write("p5", Encoding.ASCII.GetBytes("this a fifth model"));

            //Initialize threads
            t1 = new Thread(() => p1.Write(p1.getFileName(), p1.getFileContent()));
            t2 = new Thread(() => p2.Write(p2.getFileName(), p2.getFileContent()));
            t3 = new Thread(() => p3.Write(p3.getFileName(), p3.getFileContent()));
            t4 = new Thread(() => p4.Write(p4.getFileName(), p4.getFileContent()));
            t5 = new Thread(() => p5.Write(p5.getFileName(), p5.getFileContent()));
        }

        private bool IsAllProcessComplete()
        {
            return p1.IsProcessComplete() && p2.IsProcessComplete() && p3.IsProcessComplete() && p4.IsProcessComplete() && p5.IsProcessComplete();
        }
        
        private void BeginProcess(RoundRobin model, Thread modelThread)
        {
            try
            {
                // Below method check whether process is initialized
                if (model.getIsInitialized())
                {
                    
                    // Normally, thread goes to stopped state when finishes its job.
                    //Checking if thread state is stopped and process is incomplete
                    if (modelThread.ThreadState != ThreadState.Stopped && !model.IsProcessComplete())
                    {
                       
                        if (modelThread.ThreadState == ThreadState.Suspended)
                        {
                            Console.WriteLine("Thread of file " + model.getFileName() + " is resuming ");
                            modelThread.Resume();
                            Console.WriteLine("Thread of file " + model.getFileName() + " is resumed");
                        }
                        else if (modelThread.ThreadState == ThreadState.Unstarted)
                        {
                            Console.WriteLine("Thread of file " + model.getFileName() + " is starting ");
                            modelThread.Start();
                            Console.WriteLine("Thread of file " + model.getFileName() + " is started");
                        }

                        //If thread does not finish its task within specific time quantum (1 millisecond) then suspend current thread.
                        if (!modelThread.Join(TimeSpan.FromMilliseconds(model.getTimeQuantum())))
                        {
                            if (model.IsProcessComplete() == false && modelThread.ThreadState == ThreadState.Running)
                            {
                                model.SetProcessComplete(false);
                                modelThread.Suspend();
                            }
                        }
                        else
                        {
                            //else thread is able to finish its task withing specific quantum so set process to complete
                            model.SetProcessComplete(true);
                        }

                    }

                }
            }
            catch (Exception ex) { model.SetProcessComplete(true); }

        }

        private void DoProcessJob(RoundRobin working, Thread workingThread)
        {
               // used lock to prevent interruption from other threads
                lock (lockObj)
                {
                    BeginProcess(working, workingThread);
                }
        }


        private void EnqueueModels()
        {
            queueOfModel.Enqueue(p1);
            queueOfModel.Enqueue(p2);
            queueOfModel.Enqueue(p3);
            queueOfModel.Enqueue(p4);
            queueOfModel.Enqueue(p5);
        }

        private void EnqueueThreads()
        {
            //we are enqueing all thread at same instant while assuming that arrival time is same. 
            queueOfThreads.Enqueue(t1);
            queueOfThreads.Enqueue(t2);
            queueOfThreads.Enqueue(t3);
            queueOfThreads.Enqueue(t4);
            queueOfThreads.Enqueue(t5);
        }

        private void SetProcessTimeQuantum(RoundRobin model, double time)
        {
            model.setTimeQuantum(time);
        }

        private void IsJobLeft(RoundRobin model, Thread modelThread, Queue<RoundRobin> queueOfModel, Queue<Thread> queueOfThreads)
        {
            // We are checking if we have done our time sliced job, but yet, we are not completely done (we need more time slices)
            // Putting the model at the end of model queue, also put the Model Thread which handle our model at the end of  ModelThreads queue.
            if (model.getIsInitialized() && !model.IsProcessComplete())
            {
                queueOfModel.Enqueue(model);
                queueOfThreads.Enqueue(modelThread);
                Console.WriteLine("File writing process exceed the time quantum: " + model.getTimeQuantum() + " milliseconds, Enqueuing process of file: " + model.getFileName() + "\n");
            }
        }

        public void doRoundRobin()
        {
            EnqueueModels();
            EnqueueThreads();

            
            while (!IsAllProcessComplete() && queueOfModel.Count != 0 && queueOfThreads.Count != 0)
            {
                RoundRobin model = queueOfModel.Dequeue(); // dequeue the model
                Thread modelThread = queueOfThreads.Dequeue(); // dequeue the model thread

                SetProcessTimeQuantum(model, 1); // we are setting Time Quantum to 1 milliseconds i.e each write operation to file has only 1 milliseconds, if time is exceeded then current process is later enqueued
                DoProcessJob(model, modelThread); // processing job
                SleepMainThread();
                IsJobLeft(model, modelThread, queueOfModel, queueOfThreads);

            }
        }
        private void SleepMainThread()
        {
            // Below line disables the MainThread 
            /* This is important because the Mainthread will run this entire 
             loop and dequeue other working model & Modelthread so it is important to tell
             that We are busy, and stay here till we are really done */
             Thread.Sleep(1000);
            
        }

        public IDevice Write(string name, byte[] data)
        {
            return new RoundRobin(name,data);
        }
    }
}
