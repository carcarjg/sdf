using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using OpenNETCF.Threading;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

using EventWaitHandle = OpenNETCF.Threading.EventWaitHandle;
using EventResetMode = OpenNETCF.Threading.EventResetMode;

namespace OpenNETCF.Timers
{
    /// <summary>
    /// Represents the method that will handle the Elapsed event of a Timer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);

    /// <summary>
    /// This precirsion timer can be used to generate recurring events in an application.  It is dependent on platform support of the Multimedia Timer, which is <b>not</b> in Windows mobile
    /// </summary>
    public unsafe class Timer2 : Component, ISupportInitialize
    {
        
        private delegate void TimerCallbackDelegate(uint uTimerID, uint uMsg, ref uint dwUser, ref uint dw1, ref uint dw2);

        private Thread m_elapsedThread;
        private EventWaitHandle m_waitHandle;
        private TimerCallbackDelegate m_callback;
        private GCHandle m_callbackHandle;
        private bool m_oneShot = false;
        private bool m_running = false;
        private int m_interval = 100;
        private int m_resolution = 10;
        private int m_timerID = 0;
        private ISynchronizationInvoker m_syncObject;

        /// <summary>
        /// Occurs when the interval elapses.
        /// </summary>
        public event ElapsedEventHandler Elapsed;

        private void ElapsedThreadProc()
        {
            while (true)
            {
                m_waitHandle.WaitOne();

                ElapsedEventArgs eea = new ElapsedEventArgs(DateTime.Now);

                foreach (ElapsedEventHandler eeh in Elapsed.GetInvocationList())
                {
                    // determine if we're supposed to Invoke or not
                    if(m_syncObject == null)
                        eeh(this, eea);
                    else
                        m_syncObject.Invoke(m_syncObject.TimerElapsed, new object[] { this, eea } );
                }
            }
        }

        // this hides the parameter ugliness from the managed caller
        private void TimerCallbackShim(uint uTimerID, uint uMsg, ref uint dwUser, ref uint dw1, ref uint dw2)
        {
            TimerCallback();
        }

        /// <summary>
        /// When overridden and UseCallback is true, this method will run when the timer Interval expires
        /// </summary>
        public virtual void TimerCallback()
        {
            throw new Exception("TimerCallback not implemented");
        }

        /// <summary>
        /// Initializes a new instance of the Timer class with an interval of 100ms and a resolution of 10ms
        /// </summary>
        public Timer2() : this(100, 10)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the Timer class with a resolution of 10ms
        /// </summary>
        /// <param name="interval">The Interval for the Timer</param>
        public Timer2(int interval)
            : this(interval, 10)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Timer class
        /// </summary>
        /// <param name="interval">The Interval for the Timer</param>
        /// <param name="resolution">The resolution for the Timer</param>
        public Timer2(int interval, int resolution)
        {
            m_waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_elapsedThread = new Thread(new System.Threading.ThreadStart(ElapsedThreadProc));
            m_elapsedThread.IsBackground = true;
            m_elapsedThread.Name = "SDF Timer2";

            this.Resolution = resolution;
            this.Interval = interval;

            if (this.Site != null)
            {
                try
                {
                    this.SynchronizingObject = (ISynchronizationInvoker)this.Site;
                }
                catch (Exception)
                {
                    throw new Exception("Timer2 container must derive from ISynchronizationInvoker");
                }
            }
        }

        /// <summary>
        /// When set, the overridden TimerCallback will be executed
        /// </summary>
        /// <remarks>if this is set, the TimerCallback <b>must</b> be overridden</remarks>
        public bool UseCallback
        {
            get
            {
                return m_callback != null;
            }
            set
            {
                if (value)
                {
                    m_callback = new TimerCallbackDelegate(TimerCallbackShim);
                    m_callbackHandle = GCHandle.Alloc(m_callback, GCHandleType.Pinned);   
                }
                else
                {
                    m_callbackHandle.Free();
                    m_callback = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Timer should raise the Elapsed event each time the specified interval elapses or only after the first time it elapses
        /// </summary>
        public bool AutoReset
        {
            get { return !m_oneShot; }
            set { m_oneShot = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Timer should raise the Elapsed event.
        /// </summary>
        public bool Enabled
        {
            get { return m_running; }

            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval at which to either raise the Elapsed event or run the TimerCallback method
        /// </summary>
        public int Interval
        {
            get{ return m_interval; }
            set 
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Interval must be greater than zero");
                }

                m_interval = value; 
            }
        }

        /// <summary>
        /// Resolution of the timer event, in milliseconds. The resolution increases with smaller values; a resolution of zero indicates periodic events should occur with the greatest possible accuracy. To reduce system overhead, however, you should use the maximum value appropriate for your application
        /// </summary>
        /// <remarks>Default value = 10ms</remarks>
        public int Resolution
        {
            get { return m_resolution; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Resolution must be greater than zero");
                }

                m_resolution = value;
            }
        }

        /// <summary>
        /// Starts raising the Elapsed event by setting Enabled to true
        /// </summary>
        /// <remarks>
        /// If Enabled is set to true and AutoReset is set to false, the Timer raises the Elapsed event only once, the first time he interval elapses. When Enabled is true and AutoReset is true, the Timer continues to raise the Elapsed event on the specified interval.
        /// You can also start timing by setting Enabled to true.
        /// <blockquote><b>Note</b>If AutoReset is false, the Start method must be called in order to start the count again.</blockquote>
        /// </remarks>
        public void Start()
        {
            if (m_running)
                return;

            NativeMethods.MMTimerEventType flags = 0;

            if(m_oneShot)
                flags |= NativeMethods.MMTimerEventType.OneShot;
            else
                flags |= NativeMethods.MMTimerEventType.Periodic;

            if (m_callback != null)
            {
                flags |= NativeMethods.MMTimerEventType.Callback;
                m_timerID = NativeMethods.timeSetEvent(m_interval, m_resolution, (IntPtr)m_callbackHandle, 0, flags);
            }
            else
            {
                // make sure no events are set after a Stop
                flags |= NativeMethods.MMTimerEventType.KillSynchronous;

                flags |= NativeMethods.MMTimerEventType.EventPulse;
                try
                {
                    m_timerID = NativeMethods.timeSetEvent(m_interval, m_resolution, m_waitHandle.Handle, 0, flags);
                }
                catch (MissingMethodException)
                {
                    throw new NotSupportedException("The required API 'timeSetEvent' is not available on this platform");
                }
            }

            m_running = true;
        }

        /// <summary>
        /// Stops raising the Elapsed event by setting Enabled to false.
        /// </summary>
        public void Stop()
        {
            if (!m_running)
                return;

            NativeMethods.timeKillEvent(m_timerID);

            if (m_callbackHandle.IsAllocated)
                m_callbackHandle.Free();

            m_running = false;
        }

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that are issued when an interval has elapsed
        /// </summary>
        /// <remarks>
        /// When SynchronizingObject is a null reference (Nothing in Visual Basic), the method that handles the Elapsed event is called on a thread from the system-thread pool. For more information on system-thread pools, see ThreadPool.
        /// When the Elapsed event is handled by a visual Windows Forms component, such as a button, accessing the component through the system-thread pool might result in an exception or just might not work. Avoid this effect by setting SynchronizingObject to a Windows Forms component, which causes the method that handles the Elapsed event to be called on the same thread that the component was created on.
        /// If the Timer is used inside Visual Studio in a Windows Forms designer, SynchronizingObject is automatically set to the control that contains the Timer. For example, if you place a Timer on a designer for Form1 (which inherits from Form), the SynchronizingObject property of Timer is set to the instance of Form1.
        /// </remarks>
        public ISynchronizationInvoker SynchronizingObject
        {
            set { m_syncObject = value; }
            get { return m_syncObject; }
        }

        #region IDisposable Members

        /// <summary>
        /// This member overrides Component.Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (m_running)
                Stop();

            if (m_callbackHandle.IsAllocated)
                m_callbackHandle.Free();

            // todo: stop worker thread

            base.Dispose(disposing);
        }

        #endregion

        #region ISupportInitialize Members

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }

        #endregion
    }
}
