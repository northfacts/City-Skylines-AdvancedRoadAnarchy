using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using ColossalFramework;
using ColossalFramework.UI;
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
            GetElevationLimits,
            CreateNode
        }

        public static bool FindFRTCheckbox = false;
        private static bool m_AnarchyHook = false;
        public static bool AnarchyHook
        {
            get { return m_AnarchyHook; }
            set
            {
                for (int i = 0; i < rules.Count; i++)
                {
                    var rule = rules.ElementAt(i);
                    var v = rule.Value;
                    v.Status = value;
                    rules[rule.Key] = v;
                }
                m_AnarchyHook = value;
            }
        }

        public static BindingFlags allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        private static UICheckBox FineRoadTool = null;

        public static Dictionary<RulesList, Redirection> rules = new Dictionary<RulesList, Redirection>();

        public struct Redirection
        {
            private bool m_Status;
            private Dictionary<MethodInfo, RedirectCallsState> m_CallState;
            private List<MethodInfo> FromList;
            private bool m_Lock;
            private bool m_LockState;
            public bool superLock;

            public bool Lock
            {
                get { return m_Lock; }
                set
                {
                    if (value != this.m_Lock && this.superLock != true)
                    {
                        if (value)
                        {
                            this.Status = this.m_LockState;
                            this.m_Lock = value;
                        }
                        else
                        {
                            this.m_Lock = value;
                            this.Status = AdvancedRoadAnarchyTools.AnarchyHook;
                        }
                    }
                }
            }
            public bool LockState
            {
                set { this.m_LockState = value; }
            }

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
                        if (m_Lock == false && superLock == false)
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
        }

        public static void CheckFineRoadTool()
        {
            FineRoadTool = UIView.GetAView().FindUIComponent<UICheckBox>("FRT_StraightSlope");
            if (FineRoadTool != null)
            {
                FineRoadTool.eventCheckChanged += (c, state) =>
                {
                    Redirection rule;
                    rules.TryGetValue(RulesList.CheckNodeHeights, out rule);
                    if (state)
                    {
                        rule.Lock = true;
                        rule.superLock = true;
                    }
                    else
                    {
                        rule.superLock = false;
                        rule.Lock = AdvancedRoadAnarchy.Settings.CheckNodeHeights;
                    }
                    rules[RulesList.CheckNodeHeights] = rule;
                };
                FindFRTCheckbox = false;
                Debug.Log("Auto disable straight slope activated");
            }
        }


        public static void Initialize()
        {
            if (AppDomain.CurrentDomain.GetAssemblies().Any(q => q.FullName.Contains("FineRoadTool")))
            {
                FindFRTCheckbox = true;
                Debug.Log("FineRoadTool was found");
            }
            foreach (RulesList rule in Enum.GetValues(typeof(RulesList)))
            {
                var add = new Redirection();
                switch (rule)
                {
                    case RulesList.CheckNodeHeights:
                        add.From = typeof(NetTool).GetMethod("CheckNodeHeights", allFlags);
                        add.LockState = false;
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
                        add.From = typeof(RoadAI).GetMethod("GetElevationLimits", allFlags);
                        add.From = typeof(PedestrianPathAI).GetMethod("GetElevationLimits", allFlags);
                        add.From = typeof(TrainTrackAI).GetMethod("GetElevationLimits", allFlags);
                        add.LockState = true;
                        break;
                    case RulesList.TestNodeBuilding:
                        add.From = typeof(NetTool).GetMethod("TestNodeBuilding", allFlags);
                        break;
                    case RulesList.CanCreateSegment:
                        add.From = typeof(NetTool).GetMethods(allFlags).Single((MethodInfo c) => c.Name == "CanCreateSegment" && c.GetParameters().Length == 12);
                        break;
                    case RulesList.CreateNode:
                        add.From = typeof(NetTool).GetMethods(allFlags).Single((MethodInfo c) => c.Name == "CreateNode" && c.GetParameters().Length == 17);
                        add.LockState = true;
                        break;
                }
                add.To = typeof(AdvancedRoadAnarchyTools).GetMethod(rule.ToString(), allFlags);
                rules.Add(rule, add);
            }
            Debug.Log("ARA initialized");
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
            min = Mathf.RoundToInt(-120 / step);
            max = Mathf.RoundToInt(256 / step);
        }

        private static ToolBase.ToolErrors TestNodeBuilding(BuildingInfo info, Vector3 position, Vector3 direction, ushort ignoreNode, ushort ignoreSegment, ushort ignoreBuilding, bool test, ulong[] collidingSegmentBuffer, ulong[] collidingBuildingBuffer)
        {
            Vector2 vector = new Vector2(direction.x, direction.z) * ((float)info.m_cellLength * 4f - 0.8f);
            Vector2 vector2 = new Vector2(direction.z, -direction.x) * ((float)info.m_cellWidth * 4f - 0.8f);
            if (info.m_circular)
            {
                vector2 *= 0.7f;
                vector *= 0.7f;
            }
            ItemClass.CollisionType collisionType = ItemClass.CollisionType.Terrain;
            if (info.m_class.m_layer == ItemClass.Layer.WaterPipes)
            {
                collisionType = ItemClass.CollisionType.Underground;
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
            Singleton<NetManager>.instance.OverlapQuad(quad, minY, maxY, collisionType, info.m_class.m_layer, ignoreNode, 0, ignoreSegment, collidingSegmentBuffer);
            Singleton<BuildingManager>.instance.OverlapQuad(quad, minY, maxY, collisionType, info.m_class.m_layer, ignoreBuilding, ignoreNode, 0, collidingBuildingBuffer);

            if (!Singleton<BuildingManager>.instance.CheckLimits())
            {
                toolErrors |= ToolBase.ToolErrors.TooManyObjects;
            }
            return toolErrors;
        }

        private static ToolBase.ToolErrors CanCreateSegment(NetInfo segmentInfo, ushort startNode, ushort startSegment, ushort endNode, ushort endSegment, ushort upgrading, Vector3 startPos, Vector3 endPos, Vector3 startDir, Vector3 endDir, ulong[] collidingSegmentBuffer, bool testEnds)
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

        public static ToolBase.ToolErrors CreateNode(NetInfo info, NetTool.ControlPoint startPoint, NetTool.ControlPoint middlePoint, NetTool.ControlPoint endPoint, FastList<NetTool.NodePosition> nodeBuffer, int maxSegments, bool test, bool visualize, bool autoFix, bool needMoney, bool invert, bool switchDir, ushort relocateBuildingID, out ushort node, out ushort segment, out int cost, out int productionRate)
        {
            ToolBase.ToolErrors toolErrors = ToolBase.ToolErrors.None;
            toolErrors = NetTool.CreateNode(info, startPoint, middlePoint, endPoint, nodeBuffer, maxSegments, test, test, visualize, autoFix, needMoney, invert, switchDir, relocateBuildingID, out ushort num, out ushort num2, out segment, out cost, out productionRate);
            if (toolErrors == ToolBase.ToolErrors.None)
            {
                if (num2 != 0)
                {
                    node = num2;
                }
                else
                {
                    node = num;
                }
            }
            else
            {
                if (toolErrors.IsFlagSet(ToolBase.ToolErrors.OutOfArea))
                    toolErrors = toolErrors.ClearFlags(ToolBase.ToolErrors.OutOfArea);
                Debug.Log("out of area: " + (int)ToolBase.ToolErrors.OutOfArea + " objetc collision: " + (int)ToolBase.ToolErrors.ObjectCollision + " toolerror: " + (int)toolErrors);
                node = 0;
            }
            return toolErrors;
        }
    }
}
