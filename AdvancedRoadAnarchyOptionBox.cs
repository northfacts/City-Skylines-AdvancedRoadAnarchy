using ColossalFramework.UI;
using UnityEngine;
using System.Collections.Generic;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyOptionBox : UIPanel
    {
        private readonly float OptionBoxTitleHeight = 50f;

        private Dictionary<string, UIComponent> optionObject = new Dictionary<string, UIComponent>();

        public override void Awake()
        {
            base.Awake();
            this.opacity = 0.01f;
            this.transform.parent = AdvancedRoadAnarchy.Settings.uiView.transform;
            this.size = new Vector2(295f,235f);
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
            float start = 2 + OptionBoxTitleHeight;
            float space = 4;
            CreateCheckbox("UnlockButton", "Draggable", ref start, space, false);
            CreateCheckbox("StartOnLoad", "Enable mod by default", ref start, space, AdvancedRoadAnarchy.Settings.StartOnLoad);
            CreateCheckbox("InfoText", "Enable info text", ref start, space, AdvancedRoadAnarchy.Settings.InfoText);
            CreateCheckbox("ElevationLimits", "Elevation limits", ref start, space, AdvancedRoadAnarchy.Settings.ElevationLimits);
            CreateCheckbox("CheckNodeHeights", "Slope Limits", ref start, space, AdvancedRoadAnarchy.Settings.CheckNodeHeights);
            this.absolutePosition = new Vector2(0f, (AdvancedRoadAnarchy.Settings.Resolutions.height / 2) - (this.height / 2));
            this.opacity = 1f;
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            PlayClickSound(this);
            if (!this.isVisible)
            {
                AdvancedRoadAnarchy.Settings.UnlockButton = false;
                UIComponent unlock;
                optionObject.TryGetValue("UnlockButton", out unlock);
                unlock.GetComponent<AdvancedRoadAnarchyCheckbox>().IsChecked = false;
            }
        }

        private void CreateCheckbox(string name, string label, ref float start, float space, bool check)
        {
            var button = this.AddUIComponent<AdvancedRoadAnarchyCheckbox>();
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
            };
            var buttonlabel = this.AddUIComponent<UILabel>();
            buttonlabel.text = label;
            buttonlabel.height = 15f;
            buttonlabel.relativePosition = new Vector2((22f), (button.relativePosition.y + (button.height / 2)) - (buttonlabel.height / 2));
            start += button.height + space;
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
}