using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace FaceBookDPatterns
{
    public class AppSettings
    {
        public string AccessToken { get; set; }

        public bool AutoLogin { get; set; }

        public bool AutoStartFeatures { get; set; }

        private string m_FileName = "faceBookAppSettings";
        
        public string FileName 
        { 
            get 
            { 
                return m_FileName; 
            }
        }

        public void SaveToFile()
        {
            using (FileStream stream = new FileStream(m_FileName, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                serializer.Serialize(stream, this);
            }
        }

        public void LoadFromFile()
        {
            if (File.Exists(m_FileName))
            {
                using (FileStream stream = new FileStream(FileName, FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                    AppSettings loadedSettings = (AppSettings)serializer.Deserialize(stream);
                    AccessToken = loadedSettings.AccessToken;
                    AutoLogin = loadedSettings.AutoLogin;
                    AutoStartFeatures = loadedSettings.AutoStartFeatures;
                }
            }
        }
    }
}
