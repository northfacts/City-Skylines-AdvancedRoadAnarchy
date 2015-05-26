using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace AdvancedRoadAnarchy
{
    public class AnarchyLoad : LoadingExtensionBase
    {
        UIComponent uiComponent;
                
        public override void OnLevelLoaded(LoadMode mode)
        {
            UIView v = UIView.GetAView();
            uiComponent = v.AddUIComponent(typeof(AnarchyButton));
        }

        public override void OnReleased()
        {
            if (uiComponent != null)
            {
                UnityEngine.Object.Destroy(uiComponent);
            }
        }
    }
}
