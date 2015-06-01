using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;
using ColossalFramework.IO;


namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchySerializer
    {
        public static string PathFilename()
        {
            string text = Path.Combine(DataLocation.saveLocation, "AdvancedRoadAnarchy.xml");
            if (!Directory.Exists(Path.GetDirectoryName(text)))
                Directory.CreateDirectory(Path.GetDirectoryName(text));
            return text;
        }

        public static AdvancedRoadAnarchySettings LoadSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AdvancedRoadAnarchySettings));
            AdvancedRoadAnarchySettings settings = null;
            try
            {
                using (StreamReader streamReader = new StreamReader(AdvancedRoadAnarchySerializer.PathFilename()))
                {
                    settings = (AdvancedRoadAnarchySettings)serializer.Deserialize(streamReader);
                }
            }
            catch
            {
                settings = new AdvancedRoadAnarchySettings();
            }
            return settings;
        }

        public static void SaveSettings(AdvancedRoadAnarchySettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AdvancedRoadAnarchySettings));
            using (StreamWriter streamWriter = new StreamWriter(AdvancedRoadAnarchySerializer.PathFilename()))
            {
                serializer.Serialize(streamWriter, settings);
            }
        }
    }
}