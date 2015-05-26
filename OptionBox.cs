using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AnarchyOptionBox : UIPanel
    {
        private static readonly float OptionBoxWidth = 215f;
        private static readonly float OptionBoxTitleHeight = 50f;

        public UIButton UnlockButton;
        public AnarchyCheckbox StartOnLoad;
        public UILabel title;

        private AnarchyCheckbox checkbox = new AnarchyCheckbox();
        
        public void CreateOptionBox(float x, float y)
        {
            this.absolutePosition = new Vector3(x, y);
            this.width = OptionBoxWidth;
            this.height = OptionBoxTitleHeight + 55f;
            this.backgroundSprite = "MenuPanel";
            var titleObject = new GameObject("Title");
            titleObject.transform.parent = this.transform;
            titleObject.transform.localPosition = Vector3.zero;
            this.title = titleObject.AddComponent<UILabel>();
            this.title.width = this.width;
            this.title.height = OptionBoxTitleHeight;
            this.title.text = "Advanced Road Anarchy\nOption";
            this.title.textAlignment = UIHorizontalAlignment.Center;
            this.title.position = new Vector3((this.width / 2f) - (title.width / 2f), -20f + (title.height / 2f));
            UIDragHandle uIDragHandle = new GameObject("TitleDrag")
            {
                transform =
                {
                    parent = this.cachedTransform,
                    localPosition = Vector3.zero
                }
            }.AddComponent<UIDragHandle>();
            uIDragHandle.width = this.width;
            uIDragHandle.height = OptionBoxTitleHeight;
            CreateUnlockButton();
            CreateCheckbox("StartOnLoad", StartOnLoad, "Startup on loading", 78f, true);
        }

        public void CreateUnlockButton()
        {
            if (this.UnlockButton != null)
            {
                GameObject.Destroy(this.UnlockButton.gameObject);
            }
            var unlockObject = new GameObject("unlock");
            unlockObject.transform.parent = this.transform;
            unlockObject.transform.position = Vector3.zero;
            this.UnlockButton = unlockObject.AddComponent<UIButton>();
            this.UnlockButton.relativePosition = new Vector3(15f, (OptionBoxTitleHeight + 1f));
            this.UnlockButton.normalBgSprite = "ButtonMenu";
            this.UnlockButton.hoveredBgSprite = "ButtonMenuHovered";
            this.UnlockButton.pressedBgSprite = "ButtonMenuPressed";
            this.UnlockButton.width = this.width - 30f;
            this.UnlockButton.height = 20f;
            this.UnlockButton.playAudioEvents = true;
            this.UnlockButton.text = AnarchyButton.draggable ? "Button unlocked" : "Button Locked";
            this.UnlockButton.textColor = AnarchyButton.draggable ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);
            this.UnlockButton.hoveredTextColor = AnarchyButton.draggable ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);
            this.UnlockButton.pressedTextColor = AnarchyButton.draggable ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);
        }

        private void CreateCheckbox(string name, AnarchyCheckbox button, string label, float posy, bool check)
        {
            var l = new GameObject(name);
            l.transform.parent = this.transform;
            l.transform.position = Vector3.zero;
            var buttonlabel = l.AddComponent<UILabel>();
            buttonlabel.text = label;
            buttonlabel.height = 15f;
            var o = new GameObject(name);
            o.transform.parent = this.transform;
            o.transform.position = Vector3.zero;
            button = o.AddComponent<AnarchyCheckbox>();
            button.size = new Vector2(15f, 15f);
            button.relativePosition = new Vector3(this.width - 20f - button.width, posy);
            buttonlabel.relativePosition = new Vector3((20f), posy);
            button.IsChecked = check;
            button.eventClick += (component, param) =>
            {
                button.IsChecked = !button.IsChecked;
                CheckboxAction(name,button.IsChecked);
            };
        }

        public void CheckboxAction(string action,bool check)
        {
            switch (action)
            {
                case "StartOnLoad":
                    if (check)
                        Debug.Log("activer");
                    else
                        Debug.Log("désactiver");
                    break;
            }
        }

    }

    public class AnarchyCheckbox : UISprite
    {
        public bool IsChecked { get; set; }

        public override void Awake()
        {
            base.Awake();
            IsChecked = false;
            spriteName = "AchievementCheckedFalse";
            playAudioEvents = true;
        }

        public override void Update()
        {
            base.Update();
            spriteName = IsChecked ? "AchievementCheckedTrue" : "AchievementCheckedFalse";
        }
    }

 /*   public class AnarchyOnOff : UISprite
    {
        public bool IsChecked { get; set; }

        public override void Awake()
        {
            base.Awake();
            IsChecked = false;
            spriteName = ""
        }
    }*/
}