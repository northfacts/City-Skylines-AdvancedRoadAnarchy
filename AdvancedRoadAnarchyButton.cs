using ColossalFramework.UI;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyButton : UIButton
    {
        AdvancedRoadAnarchyTools tools = new AdvancedRoadAnarchyTools();
        
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
            AdvancedRoadAnarchy.Settings.UnlockButton = false;
            const int size = 43;
            this.playAudioEvents = true;
            this.absolutePosition = AdvancedRoadAnarchy.Settings.Resolutions.ButtonPosition;
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
            UIView info = UIView.GetAView();
            AdvancedRoadAnarchy.Settings.infotext = info.AddUIComponent(typeof(AdvancedRoadAnarchyInfoText));
            AdvancedRoadAnarchy.Settings.infotext.Hide();
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
            if (AdvancedRoadAnarchy.Settings.UnlockButton && dragging)
            {
                var ratio = UIView.GetAView().ratio;
                this.position = new Vector3(this.position.x + (p.moveDelta.x * ratio),
                this.position.y + (p.moveDelta.y * ratio),
                this.position.z);
                AdvancedRoadAnarchy.Settings.Resolutions.ButtonPosition = this.absolutePosition;
            }
            base.OnMouseMove(p);
        }

        public void UpdateButton()
        {
            this.playAudioEvents = AdvancedRoadAnarchy.Settings.UnlockButton ? false : true;
            if (AdvancedRoadAnarchy.Settings.UnlockButton)
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
            if (AdvancedRoadAnarchy.Settings.rules.Count == 0)
                tools.Initialize();
            if (this.containsMouse && Input.GetMouseButtonDown(0) && !AdvancedRoadAnarchy.Settings.UnlockButton)
                tools.UpdateHook();
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.L) && !AdvancedRoadAnarchy.Settings.UnlockButton)
            {
                PlayClickSound(this);
                tools.UpdateHook();
            }
            if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && this.containsMouse)
            {
                if (AdvancedRoadAnarchy.Settings.optionbox != null)
                    AdvancedRoadAnarchy.Settings.optionbox.isVisible = !AdvancedRoadAnarchy.Settings.optionbox.isVisible;
                else
                {
                    PlayClickSound(this);
                    UIView option = UIView.GetAView();
                    AdvancedRoadAnarchy.Settings.optionbox = option.AddUIComponent(typeof(AdvancedRoadAnarchyOptionBox));
                }
            }
            if (AdvancedRoadAnarchy.Settings.InfoText && (AdvancedRoadAnarchy.Settings.UnlockButton || tools.AnarchyHook))
                 AdvancedRoadAnarchy.Settings.infotext.Show();
            else if (AdvancedRoadAnarchy.Settings.infotext.isVisible)
                AdvancedRoadAnarchy.Settings.infotext.Hide();
            if (AdvancedRoadAnarchy.Settings.ScreenSize != AdvancedRoadAnarchy.Settings.Resolutions.size)
                AdvancedRoadAnarchy.Settings.OnResolutionChanged();
            if (AdvancedRoadAnarchy.Settings.ElevationLimits != AdvancedRoadAnarchy.Settings.m_ElevationLimits)
            {
                if (!tools.AnarchyHook)
                {
                    AdvancedRoadAnarchyTools.Redirection rule;
                    AdvancedRoadAnarchy.Settings.rules.TryGetValue(AdvancedRoadAnarchyTools.RulesList.GetElevationLimits, out rule);
                    rule.Status = AdvancedRoadAnarchy.Settings.ElevationLimits;
                    AdvancedRoadAnarchy.Settings.rules[AdvancedRoadAnarchyTools.RulesList.GetElevationLimits] = rule;
                }
                AdvancedRoadAnarchy.Settings.m_ElevationLimits = AdvancedRoadAnarchy.Settings.ElevationLimits;
            }
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
            for (int i = 0; i < AdvancedRoadAnarchy.Settings.rules.Count; i++)
            {
                var rule = AdvancedRoadAnarchy.Settings.rules.ElementAt(i);
                var value = rule.Value;
                value.Status = false;
                AdvancedRoadAnarchy.Settings.rules[rule.Key] = value;
            }
            AdvancedRoadAnarchy.Settings.rules.Clear();
            if (AdvancedRoadAnarchy.Settings.optionbox != null)
                GameObject.Destroy(AdvancedRoadAnarchy.Settings.optionbox.gameObject);
            if (AdvancedRoadAnarchy.Settings.infotext != null)
                GameObject.Destroy(AdvancedRoadAnarchy.Settings.infotext.gameObject);
        }
    }
}
