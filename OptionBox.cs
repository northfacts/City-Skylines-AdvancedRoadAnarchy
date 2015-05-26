﻿using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AnarchyOptionBox : UIPanel
    {
        //private static readonly float OptionBoxWidth = 215f;
        private static readonly float OptionBoxTitleHeight = 50f;

        //public UIButton UnlockButton;
        private AnarchyCheckbox UnlockButton = null;
        private AnarchyCheckbox StartOnLoad = null;
        private UILabel title;
        private UIButton logo;
        private UIButton close;

        private AnarchyCheckbox checkbox = new AnarchyCheckbox();
        
        public void CreateOptionBox(float x, float y)
        {
            this.absolutePosition = new Vector3(x, y);
            this.size = new Vector2(255f,135f);
            this.backgroundSprite = "MenuPanel";
            var titleObject = new GameObject("Title");
            titleObject.transform.parent = this.transform;
            titleObject.transform.localPosition = Vector3.zero;
            this.title = titleObject.AddComponent<UILabel>();
            this.title.width = this.width;
            this.title.height = OptionBoxTitleHeight;
            this.title.text = "Options";
            this.title.textAlignment = UIHorizontalAlignment.Center;
            this.title.relativePosition = new Vector3((this.width / 2f) - (this.title.width / 2f), (OptionBoxTitleHeight / 2f) - (this.title.height / 2f));
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
            var logoObject = new GameObject("logo");
            logoObject.transform.parent = this.transform;
            logoObject.transform.localPosition = Vector3.zero;
            this.logo = logoObject.AddComponent<UIButton>();
            this.logo.size = new Vector2(OptionBoxTitleHeight -10f, OptionBoxTitleHeight -10f);
            this.logo.relativePosition = new Vector3(2f, 2f);
            this.logo.atlas = AnarchyButton.ButtonAtlas;
            this.logo.normalBgSprite = "AnarchyNeonLogo";
            var closeObject = new GameObject("close");
            closeObject.transform.parent = this.transform;
            closeObject.transform.localPosition = Vector3.zero;
            this.close = closeObject.AddComponent<UIButton>();
            this.close.size = new Vector2(30f, 30f);
            this.close.relativePosition = new Vector3(this.width - 2f - this.close.width, 2f);
            this.close.normalBgSprite = "buttonclose";
            this.close.hoveredBgSprite = "buttonclosehover";
            this.close.pressedBgSprite = "buttonclosepressed";
            this.close.eventClick += (Component, param) =>
            {
                this.Hide();
            };
            CreateCheckbox("UnlockButton", UnlockButton, "Draggable button", 55f, false);
            CreateCheckbox("StartOnLoad", StartOnLoad, "Enable by default", 92f, true);
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
            button.size = new Vector2(54f, 30f);
            button.relativePosition = new Vector3(this.width - 20f - button.width, posy - 6f);
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
                case "UnlockButton":
                    AnarchyButton.draggable = !AnarchyButton.draggable;
                    break;
            }
        }

    }

    public class AnarchyCheckbox : UISprite
    {
        public bool IsChecked { get; set; }
        private static UITextureAtlas OnOffAtlas = null;

        public override void Awake()
        {
            base.Awake();
            if (OnOffAtlas == null)
            {
                this.atlas = AnarchyButton.CreateAtlas("OnOffAtlas", 316, 82, "OnOff.png", new[]
                                            {
                                                "AnarchyOn",
                                                "AnarchyOff",
                                            });
            }
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