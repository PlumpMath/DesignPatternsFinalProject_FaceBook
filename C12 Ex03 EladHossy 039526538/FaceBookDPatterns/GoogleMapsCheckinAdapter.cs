using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public class GoogleMapsCheckinAdapter
    {
        // an ADAPTER that converts a Facebook Checkin to coordinates, in order to send it as a parameter for the Google maps 
        // FACADE - SINGLETON object.
        private Checkin m_Checkin;

        public GoogleMapsCheckinAdapter(Checkin i_Checkin)
        {
            m_Checkin = i_Checkin;
        }

        public string ToCoordinates()
        {
            return m_Checkin.Place.Location.Latitude.ToString() + "," + m_Checkin.Place.Location.Longitude.ToString();
        }
    }
}
