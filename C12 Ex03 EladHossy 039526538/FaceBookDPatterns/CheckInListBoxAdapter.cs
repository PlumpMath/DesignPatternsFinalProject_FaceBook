using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public class CheckInListBoxAdapter
    {
        public Checkin Checkin { get; set; }
        
        public string FriendNameAndCheckinName
        {
            get { return Checkin.From.Name + ":" + Checkin.Place.Name; }
        }

        public CheckInListBoxAdapter(Checkin i_Chekcin)
        {
            Checkin = i_Chekcin;
        }

        public override string ToString()
        {
            return Checkin.Place.Name;
        }
    }
}
