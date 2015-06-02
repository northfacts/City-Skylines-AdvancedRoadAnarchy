using ColossalFramework.UI;
using ICities;
using UnityEngine;
using ColossalFramework.IO;

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
			UIView aView = UIView.GetAView();
			this.button = aView.AddUIComponent(typeof(AdvancedRoadAnarchyButton));
		}
		public override void OnLevelUnloading()
		{
			AdvancedRoadAnarchySerializer.SaveSettings(AdvancedRoadAnarchy.Settings);
			if (this.button != null)
			{
				UnityEngine.Object.Destroy(this.button.gameObject);
			}
			base.OnLevelUnloading();
		}
	}
}
