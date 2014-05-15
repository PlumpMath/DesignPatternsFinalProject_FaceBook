using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FaceBookDPatterns
{
    public class ObservableProgressBar : IObservableProgressBar
    {
        private ProgressBar m_ProgressBar;

        public int Value
        {
            get
            {
                return m_ProgressBar.Value;
            }
        }

        public ObservableProgressBar(ProgressBar i_ProgressBar)
        {
            m_ProgressBar = i_ProgressBar;
        }
        
        public event Action Finished;
        
        public event Action<int> Incremented;
        
        public event Action<int> MaximumChanged;

        public int Maximum
        {
            get
            {
                return m_ProgressBar.Maximum;
            }
            
            set
            {
                m_ProgressBar.Maximum = value;
                if (MaximumChanged != null)
                {
                    MaximumChanged.Invoke(value);
                }
            }
        }
        
        public void IncrementByOne()
        {
            m_ProgressBar.Value++;
            if (Incremented != null)
            {
                Incremented.Invoke(1);
            }

            if (m_ProgressBar.Value == m_ProgressBar.Maximum)
            {
                if (Finished != null)
                {
                    Finished.Invoke();
                }
            }
        }

        public void Reset()
        {
            m_ProgressBar.Value = 0;
        }

        public void FinishProgress()
        {
            int increment = Maximum - Value;
            m_ProgressBar.Value = m_ProgressBar.Maximum;

            if (Incremented != null)
            {
                Incremented(increment);
            }

            Finished.Invoke();
        }
    }
}
