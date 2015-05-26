using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedRoadAnarchy
{
    public class AnarchySettingsResolution
    {
        public int ScreenWidth;
        public int ScreenHeight;

        public float ButtonPositionX = -1.55f;
        public float ButtonPositionY = 0.97f;

        public int InfoWidth;
        public int InfoHeight;
        public float InfoPositionX;
        public float InfoPositionY;

    }


    public class AnarchySettings
    {
        public List<AnarchySettingsResolution> Resolutions = new List<AnarchySettingsResolution>();

        public bool Draggable = false;

        public AnarchySettingsResolution GetResolutionData(int screenWidth, int screenHeight)
        {
            AnarchySettingsResolution resolutionData = null;

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
                resolutionData = new AnarchySettingsResolution();
                resolutionData.ScreenWidth = screenWidth;
                resolutionData.ScreenHeight = screenHeight;
                this.Resolutions.Add(resolutionData);
            }

            return resolutionData;
        }
    }
}
