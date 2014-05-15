using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    // A generic class that 'holds' a collection, and implements the 
    // 'IFilterableCollection<T> interface, thus, can be filtered.
    // To implement the filter, this class uses the  VISITOR pattern, by holding
    // a Visitor - an instance of CollectionFilterVisiotr<T>.

    public class FilterableCollection<T> : 
        IFilterableCollection<T>
    {
        private ICollection<T> m_Collection;
        
        public ICollection<T> Collection
        {
            get { return m_Collection; }
        }
        
        // holds a ref to the filter class (the VISITOR)
        CollectionFilterVisitor<T> m_MyFilter;
        

        
        public FilterableCollection(ICollection<T> i_PostCollection)
        {
            m_Collection = i_PostCollection;
            m_MyFilter = new CollectionFilterVisitor<T>(this);
        }

        public void Filter(Predicate<T> i_FilterStrategy)
        {
            m_MyFilter.Filter(i_FilterStrategy);
        }

        public void Reset()
        {
            m_MyFilter.Reset();
        }
    }
}
