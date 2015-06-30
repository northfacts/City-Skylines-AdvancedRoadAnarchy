using ColossalFramework.UI;
using ColossalFramework;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyOptionBox : UIPanel
    {
        private static readonly float OptionBoxTitleHeight = 50f;

        private enum TabList
        {
            Main,
            Rules,
            Elevation
        }

        private Dictionary<string, UIComponent> optionObject = new Dictionary<string, UIComponent>();

        public override void Awake()
        {
            base.Awake();
            this.opacity = 0.01f;
            this.transform.parent = AdvancedRoadAnarchy.Settings.uiView.transform;
            this.size = new Vector2(295f,400f);
            this.backgroundSprite = "MenuPanel";
            var title = this.AddUIComponent<UILabel>();
            title.width = this.width;
            title.height = OptionBoxTitleHeight;
            title.text = "OPTIONS";
            title.textAlignment = UIHorizontalAlignment.Center;
            title.relativePosition = new Vector2((this.width / 2f) - (title.width / 2f), (OptionBoxTitleHeight / 2f) - (title.height / 2f));
            var logo = this.AddUIComponent<UIButton>();
            logo.size = new Vector2(OptionBoxTitleHeight -10f, OptionBoxTitleHeight -10f);
            logo.relativePosition = new Vector2(2f, 2f);
            logo.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            logo.normalBgSprite = "AnarchyLogo";
            var d = new GameObject("AdvancedRoadAnarchyTitleDrag");
            d.transform.parent = this.cachedTransform;
            d.transform.localPosition = Vector2.zero;
            var DragHandle = d.AddComponent<UIDragHandle>();
            DragHandle.size = new Vector2(this.width, OptionBoxTitleHeight);
            var close = this.AddUIComponent<UIButton>();
            close.size = new Vector2(30f, 30f);
            close.relativePosition = new Vector2(this.width - 2f - close.width, 2f);
            close.normalBgSprite = "buttonclose";
            close.hoveredBgSprite = "buttonclosehover";
            close.pressedBgSprite = "buttonclosepressed";
            this.playAudioEvents = true;
            close.eventMouseDown += (component, param) =>
            {
                this.Hide();
            };            
            CreateTab();
            this.absolutePosition = new Vector2(0f, (AdvancedRoadAnarchy.Settings.Resolutions.height / 2) - (this.height / 2));
            this.opacity = 1f;
            PanelTabSwitch(Enum.GetName(typeof(TabList), 0));
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            PlayClickSound(this);
            if (this.isVisible)
                PanelTabSwitch(Enum.GetName(typeof(TabList), 0));
            else
            {
                AdvancedRoadAnarchy.Settings.UnlockButton = false;
                UIComponent unlock;
                optionObject.TryGetValue("UnlockButton", out unlock);
                unlock.GetComponent<AdvancedRoadAnarchyCheckbox>().IsChecked = false;
            }
        }

        private void CreateCheckbox(string name, string label, ref float start, float space, bool check, UIComponent uic)
        {
            var button = uic.AddUIComponent<AdvancedRoadAnarchyCheckbox>();
            button.size = new Vector2(54f, 30f);
            button.relativePosition = new Vector2(this.width - 22f - button.width, start + space);
            button.IsChecked = check;
            button.name = name;
            optionObject.Add(name, button);
            button.eventMouseDown += (component, param) =>
            {
                var value = (bool)AdvancedRoadAnarchy.Settings.GetType().GetField(name).GetValue(AdvancedRoadAnarchy.Settings);
                value = !value;
                AdvancedRoadAnarchy.Settings.GetType().GetField(name).SetValue(AdvancedRoadAnarchy.Settings, value);
                button.IsChecked = value;
                OptionBoxAction(name, button);
            };
            var buttonlabel = uic.AddUIComponent<UILabel>();
            buttonlabel.text = label;
            buttonlabel.height = 15f;
            buttonlabel.relativePosition = new Vector2((22f), (button.relativePosition.y + (button.height / 2)) - (buttonlabel.height / 2));
            start += button.height + space;
        }

        private void CreateTab()
        {
            var Tab = Enum.GetNames(typeof(TabList));
            float posx = 5f;
            int index = 0;
            foreach (string tab in Tab)
            {
                index += 1;
                var button = this.AddUIComponent<UIButton>();
                button.text = tab;
                button.size = new Vector2((this.width - 10f) / Tab.Length, 22f);
                button.relativePosition = new Vector2(posx + ((index - 1) * button.width), OptionBoxTitleHeight - 7f);
                button.playAudioEvents = true;
                button.normalBgSprite = "GenericTab";
                button.pressedBgSprite = "GenericTabPressed";
                button.hoveredBgSprite = "GenericTabHovered";
                button.focusedBgSprite = null;
                button.disabledBgSprite = "GenericTabDisabled";
                var panel = this.AddUIComponent<UIPanel>();
                panel.size = new Vector2(this.width - 10f, this.height - OptionBoxTitleHeight - button.height);
                panel.backgroundSprite = "InfoPanelBack";
                panel.relativePosition = new Vector2(posx, button.relativePosition.y + button.height + 1f);
                OptionBoxAction("CreateTab" + tab, panel);
                button.eventMouseDown += (component, param) =>
                {
                    PanelTabSwitch(tab);
                };
                panel.Hide();
                optionObject.Add("Tab" + tab, button);
                optionObject.Add("Panel" + tab, panel);
                
            }
        }

        private void PanelTabSwitch(string active)
        {
            foreach (string name in Enum.GetNames(typeof(TabList)))
            {
                UIComponent b;
                UIComponent p;
                optionObject.TryGetValue("Tab" + name, out b);
                optionObject.TryGetValue("Panel" + name, out p);
                var button = b.GetComponent<UIButton>();
                var panel = p.GetComponent<UIPanel>();
                if (active == name)
                {
                    button.hoveredBgSprite = null;
                    button.pressedBgSprite = null;
                    button.normalBgSprite = "GenericTabFocused";
                    button.textColor = Color.black;
                    panel.Show();
                }
                else
                {
                    button.pressedBgSprite = "GenericTabPressed";
                    button.hoveredBgSprite = "GenericTabHovered";
                    button.normalBgSprite = "GenericTab";
                    button.textColor = Color.white;
                    panel.Hide();
                }
            }
        }

        private void CreateModeButton(string name, string label, ref float start, float space, int mode, UIComponent uic)
        {
            
            var button = uic.AddUIComponent<AdvancedRoadAnarchyModeButton>();
            button.size = new Vector2(70f, 35f);
            button.relativePosition = new Vector2(this.width - 22f - button.width, start + space);
            button.m_mode = mode;
            button.name = name;
            optionObject.Add(name, button);
            button.playAudioEvents = true;
            button.eventMouseDown += (component, param) =>
            {
                button.m_mode += 1;
                if (button.m_mode == 3) { button.m_mode = 0; }
                OptionBoxAction(name, button);
            };
            var buttonlabel = uic.AddUIComponent<UILabel>();
            buttonlabel.text = label;
            buttonlabel.height = 15f;
            buttonlabel.relativePosition = new Vector2(22f, (button.relativePosition.y + (button.height / 2)) - (buttonlabel.height / 2));
            start += button.height + space;
        }

        public void OptionBoxAction(string action, UIComponent uic)
        {
            float start;
            float space;
            
            switch (action)
            {
                case "CreateTabMain":
                    start = 8;
                    space = 7;
                    CreateCheckbox("UnlockButton", "Draggable", ref start, space, false, uic);
                    CreateCheckbox("StartOnLoad", "Enable mod by default", ref start, space, AdvancedRoadAnarchy.Settings.StartOnLoad, uic);
                    CreateCheckbox("InfoText", "Enable info text", ref start, space, AdvancedRoadAnarchy.Settings.InfoText, uic);
                    break;
                case "CreateTabRules":
                    start = 10f;
                    space = 2f;
                    CreateModeButton("CanCreateSegment", "Check segment creation", ref start, space, AdvancedRoadAnarchy.Settings.CanCreateSegment, uic);
                    CreateModeButton("CheckNodeHeights", "Limit slope height", ref start, space, AdvancedRoadAnarchy.Settings.CheckNodeHeights, uic);
                    CreateModeButton("CheckCollidingSegments", "Colliding segments", ref start, space, AdvancedRoadAnarchy.Settings.CheckCollidingSegments, uic);
                    CreateModeButton("CheckCollidingBuildings", "Colliding buildings", ref start, space, AdvancedRoadAnarchy.Settings.CheckCollidingBuildings, uic);
                    CreateModeButton("CheckSpace", "Space availaable", ref start, space, AdvancedRoadAnarchy.Settings.CheckSpace, uic);
                    CreateModeButton("CheckZoning", "Zoning limitation", ref start, space, AdvancedRoadAnarchy.Settings.CheckZoning, uic);
                    CreateCheckbox("GetElevationLimits", "Custom elevation limits", ref start, space, AdvancedRoadAnarchy.Settings.CustomElevationLimits, uic);
                    break;
                case "CanCreateSegment":
                    var test = new AnimatedVector4(new Vector4(0, 0, 30f, 30f), new Vector4(500f, 500f, 800f, 800f), 20f);
                    
                    break;
                case "CheckNodeHeights":
                    break;
                case "CheckCollidingSegments":
                    break;
            }
        }

    }

    public class AdvancedRoadAnarchyCheckbox : UISprite
    {
        public bool IsChecked { get; set; }
        private static UITextureAtlas OnOffAtlas = null;

        public override void Awake()
        {
            base.Awake();
            if (OnOffAtlas == null)
            {
                OnOffAtlas = AdvancedRoadAnarchyButton.CreateAtlas("OnOffAtlas", 316, 82, "OnOff.png", new[]
                                            {
                                                "AnarchyOn",
                                                "AnarchyOff",
                                            });
            }
            this.atlas = OnOffAtlas;
            IsChecked = false;
            spriteName = "AnarchyOff";
            playAudioEvents = true;
        }

        public override void Update()
        {
            base.Update();
            spriteName = IsChecked ? "AnarchyOn" : "AnarchyOff";
        }
    }

    public class AdvancedRoadAnarchyModeButton : UISprite
    {
        public int m_mode { get; set; }
        private static UITextureAtlas ModeAtlas = null;

        private enum mode
        {
            Never,
            Always,
            Normal
        }
        
        public override void Awake()
        {
            base.Awake();
            if (ModeAtlas == null)
            {
                ModeAtlas = AdvancedRoadAnarchyButton.CreateAtlas("ModeAtlas", 462, 82, "ButtonMode.png", new[]
                                        {
                                            "AnarchyNever",
                                            "AnarchyAlways",
                                            "AnarchyNormal",
                                        });
            }
            this.atlas = ModeAtlas;
            this.m_mode = (int)mode.Never;
            spriteName = "AnarchyNever";
            playAudioEvents = true;
        }

        public override void Update()
        {
            base.Update();
            spriteName = "Anarchy" + (mode)Enum.ToObject(typeof(mode), this.m_mode);
        }


    }
}