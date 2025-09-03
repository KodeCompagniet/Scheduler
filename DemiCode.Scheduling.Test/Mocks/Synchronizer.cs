// © 2001 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Threading;
[Serializable]
internal class WorkItem: IAsyncResult
{
   private object[] _args;
   object _asyncState;
   bool _completed;
   Delegate _method;
   ManualResetEvent _event;
   object _methodReturnedValue;
   internal WorkItem(object AsyncState,Delegate method,object[] args)
   {
      _asyncState = AsyncState;
      _method = method;
      _args =  args;
      _event = new ManualResetEvent(false);
      _completed = false;
   }
   //IAsyncResult properties 
   object IAsyncResult.AsyncState
   { 
      get
      {
         return _asyncState;
      }
   }
   WaitHandle IAsyncResult.AsyncWaitHandle
   { 
      get
      {
         return _event;
      }
   }
   bool IAsyncResult.CompletedSynchronously
   {
      get
      {
         return false;
      }
   }
   bool IAsyncResult.IsCompleted
   { 
      get
      {
         return Completed;
      }
   }
   bool Completed
   {
      get
      {
         lock(this)
         {
            return _completed;
         }
      }
      set
      {
         lock(this)
         {
            _completed = value;
         }
      }
   }

   //This method is called on the worker thread to execute the method
   internal void CallBack()
   {
      MethodReturnedValue = _method.DynamicInvoke(_args);
      //Method is done. Signal the world
      _event.Set();
      Completed = true;
   }
   internal object MethodReturnedValue
   {
      get
      {
         object methodReturnedValue;
         lock(this)
         {
            methodReturnedValue = _methodReturnedValue;
         }
         return methodReturnedValue;
      }
      set
      {
         lock(this)
         {
            _methodReturnedValue = value;
         }
      }

   }
}

public class Synchronizer: ISynchronizeInvoke,IDisposable
{
   WorkerThread m_WorkerThread;
   public bool InvokeRequired 
   {
      get
      {
         bool res = object.ReferenceEquals (Thread.CurrentThread, m_WorkerThread);
         return  res;
      }
   }
   public IAsyncResult BeginInvoke(Delegate method, object[] args)
   {
      WorkItem result = new WorkItem(null,method,args);
      m_WorkerThread.QueueWorkItem(result);
      return result;
   }
   public object EndInvoke(IAsyncResult result)
   {
      result.AsyncWaitHandle.WaitOne();
      WorkItem workItem = (WorkItem)result;
      return  workItem.MethodReturnedValue;
   }
   public object Invoke(Delegate method, object[] args)
   {
      IAsyncResult asyncResult;
      asyncResult = BeginInvoke(method,args);
      return EndInvoke(asyncResult);
   }
   public Synchronizer()
   {
      m_WorkerThread = new WorkerThread(this);
   }
   ~Synchronizer()
   {
   }
   public void Dispose()
   {
      m_WorkerThread.Kill();
   }
   internal protected class WorkerThread 
   {
      public Thread m_ThreadObj;
      bool m_EndLoop;
      Mutex m_EndLoopMutex;
      AutoResetEvent m_ItemAdded;
      Synchronizer m_Synchronizer;
      Queue m_WorkItemQueue;

      internal void QueueWorkItem(WorkItem workItem)
      {
         lock(m_WorkItemQueue.SyncRoot)
         {
            m_WorkItemQueue.Enqueue(workItem);
            m_ItemAdded.Set();
         }
      }
      internal WorkerThread(Synchronizer synchronizer)
      {
         m_Synchronizer = synchronizer;
         m_EndLoop = false;
         m_ThreadObj = null;
         m_EndLoopMutex = new Mutex();
         m_ItemAdded = new AutoResetEvent(false);
         m_WorkItemQueue = new Queue();
         CreateThread(true);
      }
      bool EndLoop
      {
         set
         {
            m_EndLoopMutex.WaitOne();
            m_EndLoop = value;
            m_EndLoopMutex.ReleaseMutex();
         }
         get
         {
            bool result = false;
            m_EndLoopMutex.WaitOne();
            result = m_EndLoop;
            m_EndLoopMutex.ReleaseMutex();
            return result;
         }
      }
      Thread CreateThread(bool autoStart)
      {
         if(m_ThreadObj != null)
         {
            Debug.Assert(false);
            return m_ThreadObj;
         }
         ThreadStart threadStart = new ThreadStart(Run);
         m_ThreadObj = new Thread(threadStart);
         m_ThreadObj.Name = "Synchronizer Worker Thread";
         if(autoStart == true)
         {
            m_ThreadObj.Start();
         }
         return m_ThreadObj;
      }
      void Start()
      {
         Debug.Assert(m_ThreadObj != null);
         Debug.Assert(m_ThreadObj.IsAlive == false);
         m_ThreadObj.Start();
      }
      bool QueueEmpty
      {
         get
         {
            lock(m_WorkItemQueue.SyncRoot)
            {
               if(m_WorkItemQueue.Count > 0)
               {
                  return false;
               }
               return true;
            }
         }
      }
      WorkItem GetNext()
      {
         if(QueueEmpty)
         {
            return null;
         }
         lock(m_WorkItemQueue.SyncRoot)
         {
            return (WorkItem)m_WorkItemQueue.Dequeue();
         }
      }
      void Run()
      {
         while(EndLoop == false)
         { 
            while(QueueEmpty == false)
            {
               if(EndLoop == true)
               {
                  return;
               }
               WorkItem workItem = GetNext();
               workItem.CallBack();
            }
            m_ItemAdded.WaitOne();
         }
      }
      public void Kill()
      { 
         //Kill is called on client thread - must use cached thread object
         Debug.Assert(m_ThreadObj != null);
         if(m_ThreadObj.IsAlive == false)
         {
            return;
         }
         EndLoop = true;
         m_ItemAdded.Set();

         //Wait for thread to die
         m_ThreadObj.Join();
         if(m_EndLoopMutex != null)
         {
            m_EndLoopMutex.Close();
         }
         if(m_ItemAdded != null)
         {
            m_ItemAdded.Close();
         }
      }
   }
}
