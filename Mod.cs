using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchy : IUserMod
    {
        public static AdvancedRoadAnarchySettings Settings = new AdvancedRoadAnarchySettings();
        public static double version = 1.2;
        
        public string Name
        {
            get { return "Advanced Road Anarchy"; }
        }
        
        public string Description
        {
            get { return "Remove restrictions for all roads and railway tracks."; }
        }
    }
}
