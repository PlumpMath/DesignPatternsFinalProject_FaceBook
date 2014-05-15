using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FaceBookDPatterns
{
    public class CompositeObservableProgressBar : IObservableProgressBar
    {
        private ProgressBar m_ProgressBar;
        
        private List<IObservableProgressBar> m_ObservableProgressBars;
        
        private int m_NumOfFinishedBars = 0;
        
        public event Action Finished;
        
        public event Action<int> Incremented;
        
        private object m_LockObj = new object();
        
        private int m_Maximum = 0;
        
        private int m_Value;
        
        public int Value
        {
            get
            {
                return m_Value;
            }
        }

        public int Maximum
        {
            get
            {
                return m_Maximum;
            }
        }
        
        public event Action<int> MaximumChanged;

        public CompositeObservableProgressBar(List<IObservableProgressBar> i_BarsToObserves, ProgressBar i_ProgressBar)
        {
            m_ObservableProgressBars = i_BarsToObserves;
            foreach (IObservableProgressBar iObservable in m_ObservableProgressBars)
            {
                iObservable.Finished += new Action(i_Observed_ProgressBarFinished);
                iObservable.Incremented += new Action<int>(iObservable_Incremented);
                iObservable.MaximumChanged += new Action<int>(iObservable_MaximumChanged);
                m_Maximum += iObservable.Maximum;
            }

            if (i_ProgressBar != null)
            {
                m_ProgressBar = i_ProgressBar;
                m_ProgressBar.Maximum = m_Maximum;
            }
        }

        private void iObservable_Incremented(int i_Increment)
        {
            if (m_ProgressBar != null)
            {
                m_ProgressBar.Increment(i_Increment);
                m_Value += i_Increment;
            }

            if (Incremented != null)
            {
                Incremented.Invoke(i_Increment);
            }
        }

        private void iObservable_MaximumChanged(int obj)
        {
            m_Maximum = 0;
            foreach (IObservableProgressBar bar in m_ObservableProgressBars)
            {
                m_Maximum += bar.Maximum;
            }

            if (MaximumChanged != null)
            {
                MaximumChanged.Invoke(m_Maximum);
            }

            if (m_ProgressBar != null)
            {
                m_ProgressBar.Maximum = m_Maximum;
            }
        }

        public void AddIObservedProgressBar(IObservableProgressBar i_Observed)
        {
            m_ObservableProgressBars.Add(i_Observed);
            i_Observed.Finished += new Action(i_Observed_ProgressBarFinished);
        }

        public void RemoveIOvservedPorgressBar(IObservableProgressBar i_Observed)
        {
            m_ObservableProgressBars.Remove(i_Observed);
            i_Observed.Finished -= i_Observed_ProgressBarFinished;
            i_Observed.Incremented -= iObservable_Incremented;
            i_Observed.MaximumChanged -= iObservable_MaximumChanged;
        }

        private void i_Observed_ProgressBarFinished()
        {
            lock (m_LockObj)
            {
                m_NumOfFinishedBars++;
                if (m_NumOfFinishedBars == m_ObservableProgressBars.Count)
                {
                    if (Finished != null)
                    {
                        Finished.Invoke();
                    }
                }
            }
        }
    }
}
