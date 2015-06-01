using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyResolution
    {
        public int ScreenWidth;
        public int ScreenHeight;

        public float ButtonPositionX = (Screen.currentResolution.width / 2);
        public float ButtonPositionY = (Screen.currentResolution.height / 2);

        public float InfoPositionX = (Screen.currentResolution.width / 2) - 80;
        public float InfoPositionY = (Screen.currentResolution.height / 8);

    }


    public class AdvancedRoadAnarchySettings
    {
        
        
        public List<AdvancedRoadAnarchyResolution> Resolutions = new List<AdvancedRoadAnarchyResolution>();

        public bool EnableByDefault = false;
        public bool EnableInfoText = true;

        public AdvancedRoadAnarchyResolution GetResolutionData(int screenWidth, int screenHeight)
        {
            AdvancedRoadAnarchyResolution resolutionData = null;

            foreach (var resolution in this.Resolutions)
            {
                if (resolution.ScreenWidth == screenWidth && resolution.ScreenHeight == screenHeight)
                {
                    resolutionData = resolution;
                    break;
                }
            }

            if (resolutionData == null)
            {
                resolutionData = new AdvancedRoadAnarchyResolution();
                resolutionData.ScreenWidth = screenWidth;
                resolutionData.ScreenHeight = screenHeight;
                this.Resolutions.Add(resolutionData);
            }

            return resolutionData;
        }
    }
}
