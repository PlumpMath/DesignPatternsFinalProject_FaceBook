using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceBookDPatterns
{
    interface IFilterableCollection<T>
    {
        ICollection<T> Collection { get; }
        void Filter(Predicate<T> i_FilterStrategy);
        void Reset();
    }
}
