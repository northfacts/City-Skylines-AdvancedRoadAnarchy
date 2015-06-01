using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyInfoText : UIPanel
    {
        AdvancedRoadAnarchyTools tools = new AdvancedRoadAnarchyTools();
        
        static readonly string ARA = "AdvancedRoadAnarchy";
        private UILabel text;
        private UIButton logo;
        private UIButton logo2;

        public override void Awake()
        {
            base.Awake();
            this.transform.parent = AdvancedRoadAnarchyLoad.uiParent.transform;
            this.backgroundSprite = "AnarchyInfoTextBg";
            this.height = 40f;
            this.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            var l = new GameObject(ARA + "InfoTextlabel");
            l.transform.parent = this.transform;
            l.transform.position = Vector3.zero;
            this.text = l.AddComponent<UILabel>();
            this.text.textColor = new Color32(255, 0, 0, 255);
            this.text.textAlignment = UIHorizontalAlignment.Center;
            this.text.isInteractive = false;
            var logoObject = new GameObject(ARA + "InfoTextlogo");
            logoObject.transform.parent = this.transform;
            logoObject.transform.localPosition = Vector3.zero;
            this.logo = logoObject.AddComponent<UIButton>();
            this.logo.size = new Vector2(this.height, this.height);

            this.logo.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            this.logo.normalFgSprite = "AnarchyLogo";
            this.logo.isInteractive = false;
            var logoObject2 = new GameObject(ARA + "InfoTextlogo");
            logoObject2.transform.parent = this.transform;
            logoObject2.transform.localPosition = Vector3.zero;
            this.logo2 = logoObject2.AddComponent<UIButton>();
            this.logo2.size = new Vector2(this.height, this.height);
            this.logo2.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            this.logo2.normalFgSprite = "AnarchyLogo";
            this.logo2.isInteractive = false;
            this.text.text = AdvancedRoadAnarchyButton.draggable ? "Unlocked, Drag me" : "Activated";
            this.width = this.logo.width + this.logo2.width + this.text.width;
            var resolutionData = AdvancedRoadAnarchy.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
            this.absolutePosition = new Vector3(resolutionData.InfoPositionX, resolutionData.InfoPositionY);
            this.width = this.logo.width + this.logo2.width + this.text.width;
            this.text.relativePosition = new Vector3((this.width / 2) - (this.text.width / 2), (this.height / 2) - (this.text.height / 2));
            this.logo.relativePosition = new Vector3(this.text.relativePosition.x - this.logo.width, 0f);
            this.logo2.relativePosition = new Vector3(this.text.relativePosition.x + this.text.width, 0f);
        }

        public override void Update()
        {
            this.text.text = AdvancedRoadAnarchyButton.draggable ? "Unlocked\nDrag me" : "Activated";
            this.text.relativePosition = new Vector3(this.text.relativePosition.x, (this.height / 2) - (this.text.height / 2));
            this.isInteractive = AdvancedRoadAnarchyButton.draggable;
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
            if (AdvancedRoadAnarchyButton.draggable && dragging)
            {
                this.position = new Vector3(this.position.x + p.moveDelta.x,
                this.position.y + p.moveDelta.y,
                this.position.z);
                var resolutionData = AdvancedRoadAnarchy.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
                resolutionData.InfoPositionX = this.absolutePosition.x;
                resolutionData.InfoPositionY = this.absolutePosition.y;
            }
            base.OnMouseMove(p);
        }
    }
}