using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using ColossalFramework.UI;

namespace AdvancedRoadAnarchy
{
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

    public class AdvancedRoadAnarchySettings
    {
        private bool m_UnlockButton = false;
        private bool m_StartOnLoad = false;
        private bool m_InfoText = true;
        private bool m_ElevationLimits = true;
        private bool m_CheckNodeHeights = false;
        private bool m_CreateNode = false;
        [XmlIgnore]
        public UIComponent button;
        [XmlIgnore]
        public UIComponent infotext;
        [XmlIgnore]
        public UIComponent optionbox;
        
        public List<AdvancedRoadAnarchyResolution> ResolutionsList = new List<AdvancedRoadAnarchyResolution>();

        [XmlIgnore]
        public bool UnlockButton
        {
            get { return m_UnlockButton; }
            set { m_UnlockButton = value; }
        }

        public bool StartOnLoad
        {
            get { return m_StartOnLoad; }
            set { m_StartOnLoad = value; }
        }

        public bool InfoText
        {
            get { return m_InfoText; }
            set { m_InfoText = value; }
        }

        public bool ElevationLimits
        {
            get { return m_ElevationLimits; }
            set
            {
                AdvancedRoadAnarchyTools.Redirection rule;
                AdvancedRoadAnarchyTools.rules.TryGetValue(AdvancedRoadAnarchyTools.RulesList.GetElevationLimits, out rule);
                rule.Lock = value;
                AdvancedRoadAnarchyTools.rules[AdvancedRoadAnarchyTools.RulesList.GetElevationLimits] = rule;
                m_ElevationLimits = value;
            }
        }

        public bool CheckNodeHeights
        {
            get { return m_CheckNodeHeights; }
            set
            {
                AdvancedRoadAnarchyTools.Redirection rule;
                AdvancedRoadAnarchyTools.rules.TryGetValue(AdvancedRoadAnarchyTools.RulesList.CheckNodeHeights, out rule);
                rule.Lock = value;
                AdvancedRoadAnarchyTools.rules[AdvancedRoadAnarchyTools.RulesList.CheckNodeHeights] = rule;
                m_CheckNodeHeights = value;
            }
        }

        public bool CreateNode
        {
            get { return m_CreateNode; }
            set
            {
                AdvancedRoadAnarchyTools.Redirection rule;
                AdvancedRoadAnarchyTools.rules.TryGetValue(AdvancedRoadAnarchyTools.RulesList.CreateNode, out rule);
                rule.Lock = value;
                AdvancedRoadAnarchyTools.rules[AdvancedRoadAnarchyTools.RulesList.CreateNode] = rule;
                m_CreateNode = value;
            }
        }

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
            OnResolutionChanged();
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
