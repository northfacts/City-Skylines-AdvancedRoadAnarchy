using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyTools
    {
        public bool AnarchyHook = false;

        private Dictionary<MethodInfo, RedirectCallsState> redirects = new Dictionary<MethodInfo, RedirectCallsState>();

        public void UpdateHook()
        {
            if (AnarchyHook) { DisableHook(); }
            else { EnableHook(); }
            AnarchyHook = !AnarchyHook;
        }
        
        public void EnableHook()
        {
            var allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            var method = typeof(NetTool).GetMethods(allFlags).Single(c => c.Name == "CanCreateSegment" && c.GetParameters().Length == 11);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("CanCreateSegment", allFlags)));

            method = typeof(NetTool).GetMethod("CheckNodeHeights", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("CheckNodeHeights", allFlags)));

            method = typeof(NetTool).GetMethod("CheckCollidingSegments", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("CheckCollidingSegments", allFlags)));

            method = typeof(BuildingTool).GetMethod("CheckCollidingBuildings", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("CheckCollidingBuildings", allFlags)));

            method = typeof(BuildingTool).GetMethod("CheckSpace", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("CheckSpace", allFlags)));

            method = typeof(Building).GetMethods(allFlags).Single(c => c.Name == "CheckZoning" && c.GetParameters().Length == 1);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("CheckZoning", allFlags)));

            //method = typeof(NetTool).GetMethod("GetElevation", allFlags);
            //redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("GetElevation", allFlags)));


            method = typeof(RoadAI).GetMethod("GetElevationLimits", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("GetElevationLimits", allFlags)));

            method = typeof(TrainTrackAI).GetMethod("GetElevationLimits", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("GetElevationLimits", allFlags)));

            method = typeof(PedestrianPathAI).GetMethod("GetElevationLimits", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("GetElevationLimits", allFlags)));

            //method = typeof(NetAI).GetMethod("BuildUnderground", allFlags);
            //redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(AdvancedRoadAnarchyTools).GetMethod("BuildUnderground", allFlags)));
        }

        public void DisableHook()
        {
            foreach (var kvp in redirects)
            {
                
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            redirects.Clear();
        }

        private static ToolBase.ToolErrors CanCreateSegment(NetInfo segmentInfo, ushort startNode, ushort startSegment, ushort endNode, ushort endSegment, ushort upgrading, Vector3 startPos, Vector3 endPos, Vector3 startDir, Vector3 endDir, ulong[] collidingSegmentBuffer)
        {
            return ToolBase.ToolErrors.None;
        }
        
        private static ToolBase.ToolErrors CheckNodeHeights(NetInfo info, FastList<NetTool.NodePosition> nodeBuffer)
        {
            return ToolBase.ToolErrors.None;
        }
        
        public static bool CheckCollidingBuildings(ulong[] buildingMask, ulong[] segmentMask)
        {
            return false;
        }

        public static bool CheckCollidingSegments(ulong[] segmentMask, ulong[] buildingMask, ushort upgrading)
        {
            return false;
        }

        private ToolBase.ToolErrors CheckSpace(BuildingInfo info, int relocating, Vector3 pos, float minY, float maxY, float angle, int width, int length, bool test, ulong[] collidingSegmentBuffer, ulong[] collidingBuildingBuffer)
        {
            return ToolBase.ToolErrors.None;
        }

        public bool CheckZoning(ItemClass.Zone zone)
        {
            return true;
        }

        private float GetElevation(NetInfo info)
        {
            var ele = (NetTool)ToolManager.instance.m_properties.CurrentTool;
            var mi = ele.GetType().GetField("m_elevation", BindingFlags.Instance | BindingFlags.NonPublic);
            return (float)Mathf.Clamp((int)mi.GetValue(ele), -512, 1920) * 6f;
        }

        public bool BuildUnderground()
        {
            return true;
        }

        public void GetElevationLimits(out int min, out int max)
        {
            min = -64;
            max = 64;
        }
    }
}
