using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyInfoText : UIPanel
    {
        private UILabel text;

        public override void Awake()
        {
            base.Awake();
            this.transform.parent = AdvancedRoadAnarchy.Settings.uiView.transform;
            this.backgroundSprite = "AnarchyInfoTextBg";
            this.height = 40f;
            this.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            this.text = this.AddUIComponent<UILabel>();
            this.text.textColor = new Color32(255, 0, 0, 255);
            this.text.textAlignment = UIHorizontalAlignment.Center;
            this.text.isInteractive = false;
            var logo = this.AddUIComponent<UIButton>();
            logo.size = new Vector2(this.height, this.height);
            logo.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            logo.normalFgSprite = "AnarchyLogo";
            logo.isInteractive = false;
            var logo2 = this.AddUIComponent<UIButton>();
            logo2.size = new Vector2(this.height, this.height);
            logo2.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            logo2.normalFgSprite = "AnarchyLogo";
            logo2.isInteractive = false;
            this.text.text = AdvancedRoadAnarchy.Settings.UnlockButton ? "Unlocked\nDrag me" : "Activated";
            this.width = logo.width + logo2.width + this.text.width;
            this.absolutePosition = AdvancedRoadAnarchy.Settings.Resolutions.InfoPosition;
            this.width = logo.width + logo2.width + this.text.width;
            this.text.relativePosition = new Vector3((this.width / 2) - (this.text.width / 2), (this.height / 2) - (this.text.height / 2));
            logo.relativePosition = new Vector3(this.text.relativePosition.x - logo.width, 0f);
            logo2.relativePosition = new Vector3(this.text.relativePosition.x + this.text.width, 0f);
        }

        public override void Update()
        {
            text.text = AdvancedRoadAnarchy.Settings.UnlockButton ? "Unlocked\nDrag me" : "Activated";
            this.text.relativePosition = new Vector3(this.text.relativePosition.x, (this.height / 2) - (this.text.height / 2));
            this.isInteractive = AdvancedRoadAnarchy.Settings.UnlockButton;
        }

        private bool dragging = false;
        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            dragging = true;
            base.OnMouseDown(p);
        }

        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            dragging = false;
            base.OnMouseUp(p);
        }

        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            if (AdvancedRoadAnarchy.Settings.UnlockButton && dragging)
            {
                var ratio = UIView.GetAView().ratio;
                this.position = new Vector3(this.position.x + (p.moveDelta.x * ratio),
                this.position.y + (p.moveDelta.y * ratio),
                this.position.z);
                AdvancedRoadAnarchy.Settings.Resolutions.InfoPosition = this.absolutePosition;
            }
            base.OnMouseMove(p);
        }
    }
}