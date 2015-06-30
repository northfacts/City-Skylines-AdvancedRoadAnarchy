using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using ColossalFramework.UI;

namespace AdvancedRoadAnarchy
{
    public struct limits
    {
        public int min;
        public int max;

        public limits(int min, int max)
        {
            this.min = min;
            this.max = max;
        } 
    }

    public struct AdvancedRoadAnarchyResolution
    {
 
        private Vector2 m_ButtonPosition;
        private Vector2 m_InfoPosition;

        public Vector2 size;

        public int width
        {
            get { return (int)size.x; }
        }
        public int height
        {
            get { return (int)size.y; }
        }

        public Vector2 ButtonPosition
        {
            get
            {
                if (this.m_ButtonPosition == Vector2.zero)
                    this.m_ButtonPosition = new Vector2(UIView.GetAView().fixedWidth / 2, UIView.GetAView().fixedHeight / 2);
                return this.m_ButtonPosition;
            }

            set
            {
                this.m_ButtonPosition = value;
            }
        }

        public Vector2 InfoPosition
        {
            get
            {
                if (this.m_InfoPosition == Vector2.zero)
                    this.m_InfoPosition = new Vector2((UIView.GetAView().fixedWidth / 2) - 80, UIView.GetAView().fixedHeight / 8);
                return this.m_InfoPosition;
            }

            set
            {
                this.m_InfoPosition = value;
            }
        }

    }

    public class AdvancedRoadAnarchyCustomElevationLimits
    {
        public limits RoadAlways = new limits(-10, 10);
        public limits RoadActivated = new limits(-30, 30);
        public limits TrackAlways = new limits(-10, 10);
        public limits TrackActivated = new limits(-30, 30);
        public limits PedestrianAlways = new limits(0, 5);
        public limits PedestrianActivated = new limits(0, 10);
    }


    public class AdvancedRoadAnarchySettings
    {
        [XmlIgnore]
        public UIComponent button;
        [XmlIgnore]
        public UIComponent infotext;
        [XmlIgnore]
        public UIComponent optionbox;
        
        public List<AdvancedRoadAnarchyResolution> ResolutionsList = new List<AdvancedRoadAnarchyResolution>();
        public AdvancedRoadAnarchyCustomElevationLimits ElevationLimits = new AdvancedRoadAnarchyCustomElevationLimits();

        public bool StartOnLoad = false;
        public bool InfoText = true;
        [XmlIgnore]
        public bool UnlockButton = false;
        public int CanCreateSegment = 2;
        public int CheckNodeHeights = 2;
        public int CheckCollidingSegments = 2;
        public int CheckCollidingBuildings = 2;
        public int CheckSpace = 2;
        public int CheckZoning = 2;
        public bool CustomElevationLimits = true;

        [XmlIgnore]
        public AdvancedRoadAnarchyResolution Resolutions = new AdvancedRoadAnarchyResolution();

        public Vector2 ScreenSize
        {
            get
            {
                var size = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
                return size;
            }
        }

        public void GetResolutionData()
        {
            var resolution = new AdvancedRoadAnarchyResolution();
            resolution = this.ResolutionsList.Find(x => x.size == this.ScreenSize);
            if (resolution.size != Vector2.zero)
                this.Resolutions = resolution;
            else
                this.Resolutions.size = this.ScreenSize;
        }


        public void OnResolutionChanged()
        {
            this.SaveResolutionData();
            this.Resolutions.size = this.ScreenSize;
            var fsc = GameObject.Find("FullScreenContainer").GetComponent<UIPanel>();
            if (this.button != null)
            {
                if ((this.button.absolutePosition.x + this.button.width) > fsc.size.x)
                    this.button.absolutePosition = new Vector2(fsc.size.x - this.button.width, this.button.absolutePosition.y);
                if ((this.button.absolutePosition.y + this.button.height) > UIView.GetAView().fixedHeight)
                    this.button.absolutePosition = new Vector2(this.button.absolutePosition.x, UIView.GetAView().fixedHeight - this.button.height);
            }
            if (this.infotext != null)
            {
                if ((this.infotext.absolutePosition.x + this.infotext.width) > fsc.size.x)
                    this.infotext.absolutePosition = new Vector2(fsc.size.x - this.infotext.width, this.infotext.absolutePosition.y);
                if ((this.infotext.absolutePosition.y + this.infotext.height) > UIView.GetAView().fixedHeight)
                    this.infotext.absolutePosition = new Vector2(this.infotext.absolutePosition.x, UIView.GetAView().fixedHeight - this.infotext.height);
            }
            if (this.optionbox != null)
            {
                if ((this.optionbox.absolutePosition.x + this.optionbox.width) > fsc.size.x)
                    this.optionbox.absolutePosition = new Vector2(fsc.size.x - this.optionbox.width, this.optionbox.absolutePosition.y);
                if ((this.optionbox.absolutePosition.y + this.optionbox.height) > UIView.GetAView().fixedHeight)
                    this.optionbox.absolutePosition = new Vector2(this.optionbox.absolutePosition.x, UIView.GetAView().fixedHeight - this.optionbox.height);
            }
        }

        public void SaveResolutionData()
        {
            int i = -1;
            i = this.ResolutionsList.FindIndex(x => x.size == this.Resolutions.size);
            if (i >= 0)
            {
                this.ResolutionsList[i] = this.Resolutions;
            }          
            else
                this.ResolutionsList.Add(new AdvancedRoadAnarchyResolution() { size = this.Resolutions.size, ButtonPosition = this.Resolutions.ButtonPosition, InfoPosition = this.Resolutions.InfoPosition });
        }

        public UIView uiView
        {
            get
            {
                UIView value = null;
                foreach (var ui in GameObject.FindObjectsOfType<UIView>())
                {
                    if (ui.name == "UIView")
                        value = ui;
                }
                return value;
            }
        }
    }
}
