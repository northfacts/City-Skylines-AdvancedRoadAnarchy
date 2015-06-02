using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyButton : UIButton
    {
        AdvancedRoadAnarchyTools tools = new AdvancedRoadAnarchyTools();
        UIComponent infotext = null;
        UIComponent optionbox = null;

        public static bool draggable { get; set; }
        public static UITextureAtlas ButtonAtlas = null;
        static readonly string ARA = "AdvancedRoadAnarchy";

        public override bool canFocus
        {
            get
            {
                return false;
            }
        }

        public override void Start()
        {
            base.Start();
            draggable = false;
            const int size = 43;
            this.playAudioEvents = true;
            var resolutionData = AdvancedRoadAnarchy.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
            this.absolutePosition = new Vector3(resolutionData.ButtonPositionX, resolutionData.ButtonPositionY);
            this.disabledBgSprite = null;
            this.disabledFgSprite = null;
            
            this.focusedFgSprite = null;
            if (ButtonAtlas == null)
            {
                ButtonAtlas = CreateAtlas(ARA, size, size, "AnarchyIcons.png", new[]
                                            {
                                                "AnarchyNormalBg",
                                                "AnarchyHoveredBg",
                                                "AnarchyPressedBg",
                                                "AnarchyNormalFg",
                                                "AnarchyHoveredFg",
                                                "AnarchyPressedFg",
                                                "AnarchyUnlockBg",
                                                "AnarchyLogo",
                                                "AnarchyInfoTextBg",
                                            });
            }
            this.atlas = ButtonAtlas;
            this.size = new Vector2(size, size);
            this.focusedBgSprite = "AnarchyNormalBg";
            this.normalBgSprite = "ButtonMenu";
            tools.AnarchyHook = AdvancedRoadAnarchy.Settings.EnableByDefault;
            UpdateButton();
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
            if (draggable && dragging)
            {
                this.position = new Vector3(this.position.x + p.moveDelta.x,
                this.position.y + p.moveDelta.y,
                this.position.z);
                var resolutionData = AdvancedRoadAnarchy.Settings.GetResolutionData(Screen.currentResolution.width, Screen.currentResolution.height);
                resolutionData.ButtonPositionX = this.absolutePosition.x;
                resolutionData.ButtonPositionY = this.absolutePosition.y;
            }
            base.OnMouseMove(p);
        }

        public void UpdateButton()
        {
            this.playAudioEvents = draggable ? false : true;
            if (draggable)
            {
                this.normalFgSprite = null;
                this.normalBgSprite = "AnarchyUnlockBg";
                this.hoveredFgSprite = "AnarchyNormalFg";
                this.hoveredBgSprite = "AnarchyUnlockBg";
                this.pressedFgSprite = "AnarchyPressedFg";
                this.pressedBgSprite = "AnarchyUnlockBg";
            }
            else if (tools.AnarchyHook)
            {
                this.normalFgSprite = "AnarchyNormalFg";
                this.normalBgSprite = "AnarchyNormalBg";
                this.hoveredFgSprite = "AnarchyNormalFg";
                this.hoveredBgSprite = "AnarchyNormalBg";
                this.pressedFgSprite = "AnarchyNormalFg";
                this.pressedBgSprite = "AnarchyNormalBg";
            }
            else
            {
                this.normalFgSprite = null;
                this.normalBgSprite = "AnarchyNormalBg";
                this.hoveredFgSprite = "AnarchyHoveredFg";
                this.hoveredBgSprite = "AnarchyHoveredBg";
                this.pressedFgSprite = "AnarchyPressedFg";
                this.pressedBgSprite = "AnarchyPressedBg";
            }

        }

        public override void Update()
        {
            if (((this.containsMouse && Input.GetMouseButtonDown(0)) || (Input.GetKeyDown(KeyCode.L) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))) && !draggable)
            {
                tools.UpdateHook();
            }
            else if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && this.containsMouse)
            {
                if (optionbox != null)
                {
                    optionbox.isVisible = !optionbox.isVisible;
                    if (!optionbox.isVisible)
                    {
                        draggable = false;
                        UIView.Find<AdvancedRoadAnarchyCheckbox>(ARA + "UnlockButton").IsChecked = false;
                    }
                }
                else
                {
                    UIView option = UIView.GetAView();
                    optionbox = option.AddUIComponent(typeof(AdvancedRoadAnarchyOptionBox));
                }
            }
            if (AdvancedRoadAnarchy.Settings.EnableInfoText && (draggable || tools.AnarchyHook))
            {
                if (infotext != null)
                    infotext.Show();
                else
                {
                    UIView info = UIView.GetAView();
                    infotext = info.AddUIComponent(typeof(AdvancedRoadAnarchyInfoText));
                }
            }
            else if (infotext != null)
                infotext.Hide();
            UpdateButton();
        }

        public static UITextureAtlas CreateAtlas(string name, int width, int height, string file, string[] spriteNames)
        {
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var textureStream = assembly.GetManifestResourceStream(ARA + ".png." + file))
            {
                var buf = new byte[textureStream.Length];
                textureStream.Read(buf, 0, buf.Length);
                tex.LoadImage(buf);
                tex.Apply(true, false);
            }

            var atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            var material = Object.Instantiate(UIView.Find<UITabstrip>("ToolMode").atlas.material);
            material.mainTexture = tex;

            atlas.material = material;
            atlas.name = name;

            for (var i = 0; i < spriteNames.Length; ++i)
            {
                var uw = 1.0f / spriteNames.Length;

                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = tex,
                    region = new Rect(i * uw, 0, uw, 1),
                };

                atlas.AddSprite(sprite);
            }
            return atlas;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (optionbox != null)
                GameObject.Destroy(optionbox.gameObject);
            if (infotext != null)
                GameObject.Destroy(infotext.gameObject);
        }
    }
}
