using ColossalFramework.UI;
using UnityEngine;

namespace AdvancedRoadAnarchy
{
    public class AnarchyButton : UIButton
    {
        AnarchyTools tools = new AnarchyTools();
        //AnarchySettings settings = new AnarchySettings();
        private static AnarchyOptionBox optionbox = null;
        public float panelposX = 300f;
        public float panelposY = 200f;

        public static bool draggable { get; set; }
        private static UITextureAtlas myatlas = null;

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
            this.absolutePosition = new Vector3(200f, 100f);//AnarchySettings.Instance.ToggleButtonPositionX, AnarchySettings.Instance.ToggleButtonPositionY);
            this.disabledBgSprite = null;
            this.disabledFgSprite = null;
            this.focusedBgSprite = "AnarchyNormalBg";
            this.focusedFgSprite = null;
            this.atlas = CreateAtlas(size, size, "AnarchyIcons2.png", new[]
                                        {
                                            "AnarchyNormalBg",
                                            "AnarchyHoveredBg",
                                            "AnarchyPressedBg",
                                            "AnarchyNormalFg",
                                            "AnarchyHoveredFg",
                                            "AnarchyPressedFg",
                                            "AnarchyUnlockBg",
                                        });
            this.size = new Vector2(size, size);
            GameObject obj = new GameObject("AdvancedRoadAnarchyOption");
            obj.transform.parent = this.transform.parent;
            optionbox = obj.AddComponent<AnarchyOptionBox>();
            optionbox.CreateOptionBox(panelposX, panelposY);
            optionbox.Hide();
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
            }
            base.OnMouseMove(p);
        }

        public void UpdateButton()
        {
            //this.playAudioEvents = optionbox.isVisible ? false : true;
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
            if (this.containsMouse && Input.GetMouseButtonDown(0) && !draggable && !optionbox.isVisible)
            {
                tools.UpdateHook();
            }
            else if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && this.containsMouse)
            {
                optionbox.Show();
            }
            else if (Input.GetKeyDown(KeyCode.L) && !draggable && !optionbox.isVisible)
            {
                tools.UpdateHook();
            }
            else if (optionbox.isVisible)
            {
                if (optionbox.UnlockButton.containsMouse && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
                {
                    draggable = !draggable;
                    optionbox.CreateUnlockButton();
                }
            }
            UpdateButton();
        }

        private UITextureAtlas CreateAtlas(int width, int height, string file, string[] spriteNames)
        {
            if (myatlas == null)
            {
                var tex = new Texture2D(width, height, TextureFormat.ARGB32, false)
                {
                    filterMode = FilterMode.Bilinear,
                };

                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var textureStream = assembly.GetManifestResourceStream("AdvancedRoadAnarchy.png." + file))
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
                atlas.name = "AdvancedRoadAnarchy";

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
                    myatlas = atlas;
                }
            }
            
            return myatlas;
        }


    }
}
