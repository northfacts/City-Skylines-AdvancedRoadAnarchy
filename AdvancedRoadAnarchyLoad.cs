using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyLoad : LoadingExtensionBase
    {
        UIComponent button;

        public static UIView uiParent
        {
            get
            {
                UIView value = null;
                foreach (var uiView in GameObject.FindObjectsOfType<UIView>())
                {
                    if (uiView.name == "UIView")
                        value = uiView;
                }
                return value;
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            AdvancedRoadAnarchy.Settings = AdvancedRoadAnarchySerializer.LoadSettings();
            UIView v = UIView.GetAView();
            button = v.AddUIComponent(typeof(AdvancedRoadAnarchyButton));
            Debug.Log(AdvancedRoadAnarchy.Settings.EnableInfoText.ToString());
        }

        public override void OnLevelUnloading()
        {
            AdvancedRoadAnarchySerializer.SaveSettings(AdvancedRoadAnarchy.Settings);
            if (button != null)
                GameObject.Destroy(button.gameObject);
            base.OnLevelUnloading();
        }
    }
}
