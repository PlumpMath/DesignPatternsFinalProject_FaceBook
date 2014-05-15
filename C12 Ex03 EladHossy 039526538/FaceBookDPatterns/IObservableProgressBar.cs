using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceBookDPatterns
{
    public interface IObservableProgressBar
    {
        event Action Finished;
        
        event Action<int> Incremented;
        
        event Action<int> MaximumChanged;
        
        int Maximum { get; }
        
        int Value { get; }
    }
}
