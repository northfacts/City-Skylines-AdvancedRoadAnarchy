using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using ColossalFramework;
using ColossalFramework.Math;

namespace AdvancedRoadAnarchy
{
    public class AdvancedRoadAnarchyTools
    {
        public enum RulesList
        {
            CanCreateSegment,
            CheckNodeHeights,
            CheckCollidingSegments,
            TestNodeBuilding,
            CheckCollidingBuildings,
            CheckSpace,
            GetElevationLimits
        }

        public bool AnarchyHook = false;

        public static BindingFlags allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        public static Type NetToolFine = null;
        //public static ToolBase instance
        //{
        //    get
        //    {
        //        if (NetToolFine != null)
        //            return UnityEngine.Object.FindObjectOfType(NetToolFine) as ToolBase;
        //        else
        //            return null;
        //    }
        //}
        public float TerrainStep
        {
            get
            {
                float step = (float)NetToolFine.GetField("m_terrainStep", allFlags).GetValue(NetToolFine);
                if (step != AdvancedRoadAnarchy.Settings.TerrainStep)
                    AdvancedRoadAnarchy.Settings.TerrainStep = step;
                return step;
            }
            set
            {
                if (AdvancedRoadAnarchy.Settings.TerrainStep < (float)NetToolFine.GetField("m_terrainStep", allFlags).GetValue(NetToolFine))
                    AdvancedRoadAnarchy.Settings.TerrainStep = (float)NetToolFine.GetField("m_terrainStep", allFlags).GetValue(NetToolFine);
                else
                    NetToolFine.GetField("m_terrainStep", allFlags).SetValue(NetToolFine, value);
            }
        }

        public struct Redirection
        {
            private bool m_Status;
            private Dictionary<MethodInfo, RedirectCallsState> m_CallState;
            private List<MethodInfo> FromList;

            public MethodInfo From
            {
                set
                {
                    if (FromList == null)
                        FromList = new List<MethodInfo>();
                    FromList.Add(value);
                }
            }
            public MethodInfo To { get; set; }

            public bool Status
            {
                get { return this.m_Status; }
                set
                {
                    if (value != this.m_Status)
                    {
                        if (value)
                        {
                            if (m_CallState == null)
                                m_CallState = new Dictionary<MethodInfo, RedirectCallsState>();
                            foreach (var from in FromList)
                            {
                                this.m_CallState.Add(from, RedirectionHelper.RedirectCalls(from, To));
                            }
                        }
                        else
                        {
                            foreach (var kvp in m_CallState)
                            {
                                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
                            }
                            m_CallState.Clear();
                        }
                        this.m_Status = value;
                    }
                }
            }
        }



        public void Initialize()
        {
            if (AppDomain.CurrentDomain.GetAssemblies().Any(q => q.FullName.Contains("FineRoadHeights")))
            {
                NetToolFine = Type.GetType("NetToolFine, FineRoadHeights");
                TerrainStep = AdvancedRoadAnarchy.Settings.TerrainStep;
                if (!(UnityEngine.Object.FindObjectOfType(NetToolFine) as ToolBase))
                    NetToolFine = null;
            }
            foreach (RulesList rule in Enum.GetValues(typeof(RulesList)))
            {
                var add = new Redirection();
                switch (rule)
                {
                    case RulesList.CanCreateSegment:
                        if (NetToolFine != null)
                            add.From = NetToolFine.GetMethods(allFlags).Single((MethodInfo c) => c.Name == "CanCreateSegment" && c.GetParameters().Length == 11);
                        else
                            add.From = typeof(NetTool).GetMethods(allFlags).Single((MethodInfo c) => c.Name == "CanCreateSegment" && c.GetParameters().Length == 11);
                        break;
                    case RulesList.CheckNodeHeights:
                        if (NetToolFine != null)
                            add.From = NetToolFine.GetMethod("CheckNodeHeights", allFlags);
                        add.From = typeof(NetTool).GetMethod("CheckNodeHeights", allFlags);
                        break;
                    case RulesList.CheckCollidingSegments:
                        add.From = typeof(NetTool).GetMethod("CheckCollidingSegments", allFlags);
                        break;
                    case RulesList.CheckCollidingBuildings:
                        add.From = typeof(BuildingTool).GetMethod("CheckCollidingBuildings", allFlags);
                        break;
                    case RulesList.CheckSpace:
                        add.From = typeof(BuildingTool).GetMethod("CheckSpace", allFlags);
                        break;
                    case RulesList.GetElevationLimits:
                        if (NetToolFine != null)
                            add.From = NetToolFine.GetMethods(allFlags).Single((MethodInfo c) => c.Name == "GetAdjustedElevationLimits" && c.GetParameters().Length == 3);
                        else
                        {
                            add.From = typeof(RoadAI).GetMethod("GetElevationLimits", allFlags);
                            add.From = typeof(PedestrianPathAI).GetMethod("GetElevationLimits", allFlags);
                            add.From = typeof(TrainTrackAI).GetMethod("GetElevationLimits", allFlags);
                        }
                        break;
                    case RulesList.TestNodeBuilding:
                        add.From = typeof(NetTool).GetMethod("TestNodeBuilding", allFlags);
                        break;
                }
                add.To = typeof(AdvancedRoadAnarchyTools).GetMethod(rule.ToString(), allFlags);
                AdvancedRoadAnarchy.Settings.rules.Add(rule, add);
            }
            if (AdvancedRoadAnarchy.Settings.ElevationLimits)
            {
                Redirection value;
                AdvancedRoadAnarchy.Settings.rules.TryGetValue(RulesList.GetElevationLimits, out value);
                value.Status = true;
                AdvancedRoadAnarchy.Settings.rules[RulesList.GetElevationLimits] = value;
            }
            else
                AdvancedRoadAnarchy.Settings.m_ElevationLimits = false;
            if (AdvancedRoadAnarchy.Settings.StartOnLoad)
            {
                AnarchyHook = AdvancedRoadAnarchy.Settings.StartOnLoad;
                EnableHook();
            }
        }

        public void UpdateHook()
        {
            if (AnarchyHook) { DisableHook(); }
            else { EnableHook(); }
            AnarchyHook = !AnarchyHook;
        }
        
        public void EnableHook()
        {
            for (int i = 0; i < AdvancedRoadAnarchy.Settings.rules.Count; i++)
            {
                var rule = AdvancedRoadAnarchy.Settings.rules.ElementAt(i);
                var value = rule.Value;
                value.Status = true;
                AdvancedRoadAnarchy.Settings.rules[rule.Key] = value;
            }
        }

        public void DisableHook()
        {
            for (int i = 0; i < AdvancedRoadAnarchy.Settings.rules.Count; i++)
            {
                var rule = AdvancedRoadAnarchy.Settings.rules.ElementAt(i);
                if (rule.Key == RulesList.GetElevationLimits && AdvancedRoadAnarchy.Settings.ElevationLimits)
                    continue;
                var value = rule.Value;
                value.Status = false;
                AdvancedRoadAnarchy.Settings.rules[rule.Key] = value;
            }
        }
        
        public static bool CheckCollidingBuildings(ulong[] buildingMask, ulong[] segmentMask)
        {
            return false;
        }

        private ToolBase.ToolErrors CheckSpace(BuildingInfo info, int relocating, Vector3 pos, float minY, float maxY, float angle, int width, int length, bool test, ulong[] collidingSegmentBuffer, ulong[] collidingBuildingBuffer)
        {
            return ToolBase.ToolErrors.None;
        }
                
        public void GetElevationLimits(out int min, out int max)
        {
            float step = 12f;
            if (NetToolFine != null)
                step = TerrainStep;
            min = Mathf.RoundToInt(-120 / step);
            max = Mathf.RoundToInt(255 / step);
        }

        //public static ToolBase.ToolErrors CreateNode(NetInfo info, NetTool.ControlPoint startPoint, NetTool.ControlPoint middlePoint, NetTool.ControlPoint endPoint, FastList<NetTool.NodePosition> nodeBuffer, int maxSegments, bool test, bool visualize, bool autoFix, bool needMoney, bool invert, bool switchDir, ushort relocateBuildingID, out ushort node, out ushort segment, out int cost, out int productionRate)
        //{
        //    ushort num;
        //    ushort num2;
        //    ToolBase.ToolErrors toolErrors = ToolBase.ToolErrors.None;
        //    if (instance != null)
        //    {
        //        var methodInfo = NetToolFine.GetMethods(allFlags).Single((MethodInfo c) => c.Name == "CreateNode" && c.GetParameters().Length == 18);
        //        object classInstance = Activator.CreateInstance(NetToolFine, null);
        //        object[] parameters = new object[] { info, startPoint, middlePoint, endPoint, nodeBuffer, maxSegments, test, visualize, autoFix, needMoney, invert, switchDir, relocateBuildingID, null, null, null, null, null };
        //        toolErrors = (ToolBase.ToolErrors)methodInfo.Invoke(classInstance, parameters);
        //        num = (ushort)parameters[13];
        //        num2 = (ushort)parameters[14];
        //        segment = (ushort)parameters[15];
        //        cost = (int)parameters[16];
        //        productionRate = (int)parameters[17];
        //    }
        //    else
        //        toolErrors = NetTool.CreateNode(info, startPoint, middlePoint, endPoint, nodeBuffer, maxSegments, test, visualize, autoFix, needMoney, invert, switchDir, relocateBuildingID, out num, out num2, out segment, out cost, out productionRate);
        //    if (toolErrors == ToolBase.ToolErrors.None)
        //    {
        //        if (num2 != 0)
        //        {
        //            node = num2;
        //        }
        //        else
        //        {
        //            node = num;
        //        }
        //    }
        //    else
        //    {
        //        node = 0;
        //    }
        //    return ToolBase.ToolErrors.None;
        //}

        private static ToolBase.ToolErrors TestNodeBuilding(BuildingInfo info, Vector3 position, Vector3 direction, ushort ignoreNode, ushort ignoreSegment, ushort ignoreBuilding, bool test, ulong[] collidingSegmentBuffer, ulong[] collidingBuildingBuffer)
        {
            Vector2 vector = new Vector2(direction.x, direction.z) * ((float)info.m_cellLength * 4f - 0.8f);
            Vector2 vector2 = new Vector2(direction.z, -direction.x) * ((float)info.m_cellWidth * 4f - 0.8f);
            if (info.m_circular)
            {
                vector2 *= 0.7f;
                vector *= 0.7f;
            }
            Vector2 a = VectorUtils.XZ(position);
            Quad2 quad = default(Quad2);
            quad.a = a - vector2 - vector;
            quad.b = a - vector2 + vector;
            quad.c = a + vector2 + vector;
            quad.d = a + vector2 - vector;
            ToolBase.ToolErrors toolErrors = ToolBase.ToolErrors.None;
            float minY = Mathf.Min(position.y, Singleton<TerrainManager>.instance.SampleRawHeightSmooth(position));
            float maxY = position.y + info.m_generatedInfo.m_size.y;
            Singleton<NetManager>.instance.OverlapQuad(quad, minY, maxY, info.m_class.m_layer, ignoreNode, 0, ignoreSegment, collidingSegmentBuffer);
            Singleton<BuildingManager>.instance.OverlapQuad(quad, minY, maxY, info.m_class.m_layer, ignoreBuilding, ignoreNode, 0, collidingBuildingBuffer);
            
            if (!Singleton<BuildingManager>.instance.CheckLimits())
            {
                toolErrors |= ToolBase.ToolErrors.TooManyObjects;
            }
            return toolErrors;
        }

        private static ToolBase.ToolErrors CanCreateSegment(NetInfo segmentInfo, ushort startNode, ushort startSegment, ushort endNode, ushort endSegment, ushort upgrading, Vector3 startPos, Vector3 endPos, Vector3 startDir, Vector3 endDir, ulong[] collidingSegmentBuffer)
        {
            return ToolBase.ToolErrors.None;
        }

        private static ToolBase.ToolErrors CheckNodeHeights(NetInfo info, FastList<NetTool.NodePosition> nodeBuffer)
        {
            return ToolBase.ToolErrors.None;
        }

        public static bool CheckCollidingSegments(ulong[] segmentMask, ulong[] buildingMask, ushort upgrading)
        {
            return false;
        }
    }
}
