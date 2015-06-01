using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework.IO;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyOptionBox : UIPanel
    {
        static readonly string ARA = "AdvancedRoadAnarchy";
        private static readonly float OptionBoxTitleHeight = 50f;
        public float panelposX = 300f;
        public float panelposY = 200f;

        private UILabel title;
        private UIButton logo;
        private UIButton close;

        private AdvancedRoadAnarchyCheckbox checkbox = new AdvancedRoadAnarchyCheckbox();
        AdvancedRoadAnarchyTools tools = new AdvancedRoadAnarchyTools();
        AdvancedRoadAnarchyInfoText infotext = new AdvancedRoadAnarchyInfoText();
 
        public override void Awake()
        {
            base.Awake();
            this.transform.parent = AdvancedRoadAnarchyLoad.uiParent.transform;
            
            
            this.size = new Vector2(275f,172f);
            this.backgroundSprite = "MenuPanel";
            var titleObject = new GameObject(ARA + "Title");
            titleObject.transform.parent = this.transform;
            titleObject.transform.localPosition = Vector3.zero;
            this.title = titleObject.AddComponent<UILabel>();
            this.title.width = this.width;
            this.title.height = OptionBoxTitleHeight;
            this.title.text = "OPTIONS";
            this.title.textAlignment = UIHorizontalAlignment.Center;
            this.title.relativePosition = new Vector3((this.width / 2f) - (this.title.width / 2f), (OptionBoxTitleHeight / 2f) - (this.title.height / 2f));
            UIDragHandle uIDragHandle = new GameObject(ARA + "TitleDrag")
            {
                transform =
                {
                    parent = this.cachedTransform,
                    localPosition = Vector3.zero
                }
            }.AddComponent<UIDragHandle>();
            uIDragHandle.width = this.width;
            uIDragHandle.height = OptionBoxTitleHeight;
            var logoObject = new GameObject(ARA + "logo");
            logoObject.transform.parent = this.transform;
            logoObject.transform.localPosition = Vector3.zero;
            this.logo = logoObject.AddComponent<UIButton>();
            this.logo.size = new Vector2(OptionBoxTitleHeight -10f, OptionBoxTitleHeight -10f);
            this.logo.relativePosition = new Vector3(2f, 2f);
            this.logo.atlas = AdvancedRoadAnarchyButton.ButtonAtlas;
            this.logo.normalBgSprite = "AnarchyLogo";
            var closeObject = new GameObject(ARA + "close");
            closeObject.transform.parent = this.transform;
            closeObject.transform.localPosition = Vector3.zero;
            this.close = closeObject.AddComponent<UIButton>();
            this.close.size = new Vector2(30f, 30f);
            this.close.relativePosition = new Vector3(this.width - 2f - this.close.width, 2f);
            this.close.normalBgSprite = "buttonclose";
            this.close.hoveredBgSprite = "buttonclosehover";
            this.close.pressedBgSprite = "buttonclosepressed";
            this.close.eventClick += (component, param) =>
            {
                OptionBoxAction("close");
            };
            CreateCheckbox("UnlockButton", "Draggable", 55f, false);
            CreateCheckbox("StartOnLoad", "Enable mod by default", 92f, AdvancedRoadAnarchy.Settings.EnableByDefault);
            CreateCheckbox("InfoText", "Enable info text", 129, AdvancedRoadAnarchy.Settings.EnableInfoText);
            var resolutionData = AdvancedRoadAnarchy.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
            this.absolutePosition = new Vector3(0f, (resolutionData.ScreenHeight / 2) - (this.height / 2));
        }

        private void CreateCheckbox(string name, string label, float posy, bool check)
        {
            var l = new GameObject(ARA + name + "label");
            l.transform.parent = this.transform;
            l.transform.position = Vector3.zero;
            var buttonlabel = l.AddComponent<UILabel>();
            buttonlabel.text = label;
            buttonlabel.height = 15f;
            var o = new GameObject(ARA + name);
            o.transform.parent = this.transform;
            o.transform.position = Vector3.zero;
            var button = o.AddComponent<AdvancedRoadAnarchyCheckbox>();
            button.size = new Vector2(54f, 30f);
            button.relativePosition = new Vector3(this.width - 20f - button.width, posy - 6f);
            buttonlabel.relativePosition = new Vector3((20f), posy);
            button.IsChecked = check;
            button.eventClick += (component, param) =>
            {
                button.IsChecked = !button.IsChecked;
                OptionBoxAction(name);
            };
        }


        public void OptionBoxAction(string action)
        {
            switch (action)
            {
                case "StartOnLoad":
                    AdvancedRoadAnarchy.Settings.EnableByDefault = !AdvancedRoadAnarchy.Settings.EnableByDefault;
                    break;
                case "UnlockButton":
                    AdvancedRoadAnarchyButton.draggable = !AdvancedRoadAnarchyButton.draggable;
                    break;
                case "InfoText":
                    AdvancedRoadAnarchy.Settings.EnableInfoText = !AdvancedRoadAnarchy.Settings.EnableInfoText;
                    break;
                case "close":
                    AdvancedRoadAnarchyButton.draggable = false;
                    UIView.Find<AdvancedRoadAnarchyCheckbox>(ARA + "UnlockButton").IsChecked = false;
                    this.Hide();
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
}