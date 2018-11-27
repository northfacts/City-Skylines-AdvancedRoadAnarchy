using ColossalFramework.UI;
using ICities;

namespace AdvancedRoadAnarchy
{
	public class AdvancedRoadAnarchyLoad : LoadingExtensionBase
	{
		public override void OnLevelLoaded(LoadMode mode)
		{
            AdvancedRoadAnarchyTools.Initialize();
            AdvancedRoadAnarchy.Settings = AdvancedRoadAnarchySerializer.LoadSettings();
            AdvancedRoadAnarchy.Settings.GetResolutionData();
            AdvancedRoadAnarchyTools.AnarchyHook = AdvancedRoadAnarchy.Settings.StartOnLoad;
            UIView aView = UIView.GetAView();
            AdvancedRoadAnarchy.Settings.button = aView.AddUIComponent(typeof(AdvancedRoadAnarchyButton));
		}
		public override void OnLevelUnloading()
		{
            AdvancedRoadAnarchy.Settings.SaveResolutionData();
            AdvancedRoadAnarchySerializer.SaveSettings(AdvancedRoadAnarchy.Settings);
            if (AdvancedRoadAnarchy.Settings.button != null)
			{
                UnityEngine.Object.Destroy(AdvancedRoadAnarchy.Settings.button.gameObject);
			}
			base.OnLevelUnloading();
		}
	}
}
