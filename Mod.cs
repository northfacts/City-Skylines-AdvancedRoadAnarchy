using ICities;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchy : IUserMod
    {
        public static AdvancedRoadAnarchySettings Settings = new AdvancedRoadAnarchySettings();
        public static double version = 1.4;
        
        public string Name
        {
            get { return "Advanced Road Anarchy " + version; }
        }
        
        public string Description
        {
            get { return "Remove restrictions for all roads and railway tracks."; }
        }
    }
}
