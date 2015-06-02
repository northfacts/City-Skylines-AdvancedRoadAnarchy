using System.IO;
using System.Xml.Serialization;
using ColossalFramework.IO;


namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchySerializer
    {
        public static string PathFilename()
        {
            string ModConfigDir = Path.Combine(DataLocation.localApplicationData, "ModConfig");
            string ModDir = Path.Combine(DataLocation.localApplicationData, ModConfigDir + "\\AdvancedRoadAnarchy\\AdvancedRoadAnarchy.xml");
            if (!Directory.Exists(Path.GetDirectoryName(ModConfigDir)))
                Directory.CreateDirectory(Path.GetDirectoryName(ModConfigDir));
            if (!Directory.Exists(Path.GetDirectoryName(ModDir)))
                Directory.CreateDirectory(Path.GetDirectoryName(ModDir));
            return ModDir;
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