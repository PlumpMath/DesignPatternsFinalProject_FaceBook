using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Api.Maps.Service.StaticMaps;
using Google.Api.Maps.Service.Geocoding;
using Google.Api.Maps.Service;

namespace FaceBookDPatterns
{
    public class GoogleMapsFacadeSingleton
    {
        // since this is a SINGLETON - static ref to the single instance
        private static GoogleMapsFacadeSingleton s_SingleInstance;

        // public access point to the single instance + JIT creation
        public static GoogleMapsFacadeSingleton Instance
        {
            get
            {
                if (s_SingleInstance == null)
                {
                    s_SingleInstance = new GoogleMapsFacadeSingleton();
                }

                return s_SingleInstance;
            }
        }
       
        // since this is a SINGLETON, the ctor is private
        private GoogleMapsFacadeSingleton()
        {
        }

        // a simple intuitive public access point for the client to get a map from Google, by coordinates.
        public string GetMap(string i_Coordinates)
        {
            var map = new StaticMap();
            map.Center = i_Coordinates;
            map.Language = "hebrew";
            map.Zoom = "14";
            map.Size = "400x400";
            map.Sensor = "false";
            map.Markers = map.Center;
            string url = map.ToUri().ToString();
            return url;
        }

        // the private function that does all the 'hard work' for querying address
        private string getAddressPartByType(string i_Address, AddressType i_AddressType)
        {
            string addressPart = string.Empty;
            var request = new GeocodingRequest();
            request.Address = i_Address;
            request.Sensor = "false";
            var response = GeocodingService.GetResponse(request);

            if (response.Status == ServiceResponseStatus.Ok)
            {
                var components = response.Results[0].Components;
                foreach (AddressComponent addressComponent in components)
                {
                    foreach (AddressType addressType in addressComponent.Types)
                    {
                        if (addressType == i_AddressType)
                        {
                            addressPart = addressComponent.LongName;
                        }
                    }
                }
            }

            return addressPart;
        }
        
        // 'wrappers' for the 'getAddressPartByType' private function.
        // each wrapper method gets other part of the address, as the client requires.
        // this simplify (thus, FACADE) the client work with the google maps services with simple and intuitive 
        // public access points.
        ////

        public string GetCityName(string i_Address)
        {
            return getAddressPartByType(i_Address, AddressType.Locality);
        }

        public string GetCountryName(string i_Address)
        {
            return getAddressPartByType(i_Address, AddressType.Country);
        }

        // and the list can go on for all the AddressType types....
    }
}
