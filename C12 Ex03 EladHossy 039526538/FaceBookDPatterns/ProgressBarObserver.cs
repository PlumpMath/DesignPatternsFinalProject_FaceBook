using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace FaceBookDPatterns
{
    public class ProgressBarObserver1
    {
        private List<ProgressBar> m_ObservedProgressBars = new List<ProgressBar>();

        public event Action AllProgressBarsFinished;

        public void AddProgressBarToObserve(ProgressBar i_ProgressBar)
        {
            m_ObservedProgressBars.Add(i_ProgressBar);
        }

        public void Observe()
        {
            int finished = 0;
            new Thread(() =>
                {
                    while (finished < m_ObservedProgressBars.Count)
                    {
                        foreach (ProgressBar progressBar in m_ObservedProgressBars)
                        {
                            if (progressBar.Value == 5)
                            {
                                finished++;
                            }
                        }
                    }

                    AllProgressBarsFinished.Invoke();
                }).Start();
        }
    }
}
