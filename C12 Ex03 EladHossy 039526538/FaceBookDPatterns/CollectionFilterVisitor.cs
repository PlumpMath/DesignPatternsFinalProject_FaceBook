using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    // This class implements a general Filter mechanism for any ICollectoin collection,
    // and can be used as a VISITOR of some other class that implements the 
    // IFilterableCollection interface.
    // The filter algorithm is using the STRATEGY pattern, by getting the Strategy desicion
    // function, as a parameter to the Filter method.
    
    class CollectionFilterVisitor<T>
    {
        IFilterableCollection<T> m_FilterableCollection;
        ICollection<T> m_CollectionToFilter;
        List<T> m_SavedCollection;

        public CollectionFilterVisitor(IFilterableCollection<T> i_FilterableCollection)
        {
            m_FilterableCollection = i_FilterableCollection;
            m_CollectionToFilter = i_FilterableCollection.Collection;
            m_SavedCollection = new List<T>();

            // save the original collection to be able to restore it
            foreach (T item in m_CollectionToFilter)
            {
                m_SavedCollection.Add(item);
            }
        }

        public void Filter(Predicate<T> i_FilterStrategy)
        {
            List<T> filtered = new List<T>();
            foreach (T item in m_CollectionToFilter)
            {
                if (i_FilterStrategy(item))
                {
                    filtered.Add(item);
                }
            }

            m_CollectionToFilter.Clear();
            foreach (T item in filtered)
            {
                m_CollectionToFilter.Add(item);
            }
        }

        public void Reset()
        {
            m_CollectionToFilter.Clear();
            foreach (T item in m_SavedCollection)
            {
                m_CollectionToFilter.Add(item);
            }
        }

    }
}
