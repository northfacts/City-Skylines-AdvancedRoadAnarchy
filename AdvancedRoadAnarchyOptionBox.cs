using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyOptionBox : UIPanel
    {
        private readonly float OptionBoxTitleHeight = 50f;

        private UITextureAtlas OnOffAtlas = AdvancedRoadAnarchyButton.CreateAtlas("OnOffAtlas", 316, 82, "OnOff.png", new[] { "AnarchyOn", "AnarchyOff", });

        private UICheckBox m_unlock;
        private UICheckBox m_startonload;
        private UICheckBox m_infotext;
        private UICheckBox m_elevationlimits;
        private UICheckBox m_checknodeheights;
        private UICheckBox m_outside;

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
            m_unlock = CreateCheckbox("UnlockButton", "Draggable", ref start);
            m_unlock.eventVisibilityChanged += (c, s) =>
            {
                PlayClickSound(this);
                if (!m_unlock.isVisible)
                {
                    m_unlock.isChecked = false;
                }
            };
            m_startonload = CreateCheckbox("StartOnLoad", "Enable mod by default", ref start);
            m_infotext = CreateCheckbox("InfoText", "Enable info text", ref start);
            m_elevationlimits = CreateCheckbox("ElevationLimits", "Elevation limits", ref start);
            m_checknodeheights = CreateCheckbox("CheckNodeHeights", "Slope Limits", ref start);
            m_outside = CreateCheckbox("CreateNode", "Enable outside map", ref start);

            this.height = start + 6;

            this.absolutePosition = new Vector2(0f, (AdvancedRoadAnarchy.Settings.Resolutions.height / 2) - (this.height / 2));
            this.opacity = 1f;
        }

        private UICheckBox CreateCheckbox(string name, string label, ref float start)
        {
            UICheckBox checkbox = this.AddUIComponent<UICheckBox>();

            checkbox.size = new Vector2(251f, 30f);
            checkbox.relativePosition = new Vector2(22f, start + 4);
            checkbox.clipChildren = true;
            checkbox.name = name;

            checkbox.label = checkbox.AddUIComponent<UILabel>();
            checkbox.label.text = label;
            checkbox.label.relativePosition = new Vector2(0f, 2f);

            UISprite sprite = checkbox.AddUIComponent<UISprite>();
            sprite.atlas = OnOffAtlas;
            sprite.spriteName = "AnarchyOff";
            sprite.size = new Vector2(54f, 30f);
            sprite.relativePosition = new Vector2(checkbox.width - sprite.width, 0f);

            checkbox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkbox.checkedBoxObject).atlas = OnOffAtlas;
            ((UISprite)checkbox.checkedBoxObject).spriteName = "AnarchyOn";
            checkbox.checkedBoxObject.size = new Vector2(54f, 30f);
            checkbox.checkedBoxObject.relativePosition = Vector3.zero;

            checkbox.eventCheckChanged += (c, s) =>
            {
                AdvancedRoadAnarchy.Settings.GetType().GetProperty(name).SetValue(AdvancedRoadAnarchy.Settings, s, null);
            };
            checkbox.eventMouseDown += (c, p) =>
            {
                PlayClickSound(this);
            };

            checkbox.isChecked = (bool)AdvancedRoadAnarchy.Settings.GetType().GetProperty(name).GetValue(AdvancedRoadAnarchy.Settings, null);

            start += checkbox.height + 4;

            return checkbox;
        }

  
    }
}