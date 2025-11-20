using JimmysUnityUtilities;
using LogicAPI.Client;
using LogicAPI.Data;
using LogicWorld.ClientCode.Resizing;
using LogicWorld.Rendering.Components;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.BinaryStuff;
using LogicWorld.SharedCode.Components;
using UnityEngine;
using System;
using BobbysRom.Shared;

namespace BobbysRom.Client
{
    public class Client : ClientMod
    {
    }

    public class Rom : ComponentClientCode<IRomData>, IResizableX, IResizableZ
    {
        protected override void SetDataDefaultValues()
        {
            Data.SizeX = 8;
            Data.SizeZ = 8;
            Data.Data = Array.Empty<byte>();
        }

        // ResizableX
        private int _previousSizeX;

        public int SizeX
        {
            get => Data.SizeX;
            set => Data.SizeX = value;
        }

        public int MinX => 1;
        public int MaxX => 16;
        public float GridIntervalX => 1f;

        // ResizableZ
        private int _previousSizeZ;

        public int SizeZ
        {
            get => Data.SizeZ;
            set => Data.SizeZ = value;
        }

        public int MinZ { get; private set; } = 8;
        public int MaxZ { get; } = 32;

        public float GridIntervalZ => 1f;
        
        protected override void DataUpdate()
        {
            QueueFrameUpdate();
            if (OutputCount == 32)
            {
                MinZ = 32;
            }
            else if (OutputCount == 16)
            {
                MinZ = 16;
            }
            else if (InputCount == 24)
            {
                MinZ = 12;
            }
            else
            {
                MinZ = 8;
            }

            int x = SizeX.Clamp(MinX, MaxX);
            int z = SizeZ.Clamp(MinZ, MaxZ);
            if (SizeX != x)
            {
                SizeX = x;
            }

            if (SizeZ != z)
            {
                SizeZ = z;
            }
            if (SizeX != _previousSizeX || SizeZ != _previousSizeZ)
            {
                _previousSizeX = SizeX;
                _previousSizeZ = SizeZ;
                SetBlockScale(0, new Vector3(SizeX, 1.0f, SizeZ));
                for (int i = 0; i < InputCount; i++)
                {
                    // float zInput = 8.0f / InputCount * i * z / 8.0f;
                    float zInput = i * 0.5f - 0.25f;
                    SetInputPosition((byte)i, new Vector3(-0.5f, 0.5f, zInput));
                }

                for (int i = 0; i < OutputCount; i++)
                {
                    float xOutput = SizeX - 0.5f;
                    // float zOutput = 8.0f / OutputCount * i * z / 8.0f;
                    float zOutput = i;
                    SetOutputPosition((byte)i, new Vector3(xOutput, 0.5f, zOutput));
                }
            }
        }
    }

    public struct RomPrefabIdentifier
    {
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public int SizeX { get; set; }
        public int SizeZ { get; set; }
    }

    public class RomPrefabGenerator : DynamicPrefabGenerator<RomPrefabIdentifier>
    {
        protected override RomPrefabIdentifier GetIdentifierFor(ComponentData componentData)
        {
            int x;
            int z;
            var m = new CustomDataManager<IRomData>();
            bool result = m.TryDeserializeData(componentData.CustomData);
            if (!result)
            {
                x = 8;
                z = 8;
            }
            else
            {
                x = m.Data.SizeX;
                z = m.Data.SizeZ;
            }
            return new RomPrefabIdentifier()
            {
                InputCount = componentData.InputCount,
                OutputCount = componentData.OutputCount,
                SizeX = x,
                SizeZ = z
            };
        }

        protected override Prefab GeneratePrefabFor(RomPrefabIdentifier identifier)
        {
            int x = identifier.SizeX;
            int z = identifier.SizeZ;
            ComponentInput[] inputArray = new ComponentInput[identifier.InputCount];
            for (int i = 0; i < identifier.InputCount; i++)
            {
                // float zInput = 8.0f / identifier.InputCount * i * z / 8.0f;
                float zInput = i * 0.5f - 0.25f;
                inputArray[i] = new ComponentInput()
                {
                    Position = new Vector3(-0.5f, 0.5f, zInput),
                    Rotation = new Vector3(0.0f, 0.0f, 90.0f),
                    Length = 0.6f
                };
            }

            ComponentOutput[] outputArray = new ComponentOutput[identifier.OutputCount];
            for (int i = 0; i < identifier.OutputCount; i++)
            {
                float xOutput = x - 0.5f;
                // float zOutput = 8.0f / identifier.OutputCount * i * z / 8.0f;
                float zOutput = i;
                outputArray[i] = new ComponentOutput()
                {
                    Position = new Vector3(x, 0.5f, zOutput),
                    Rotation = new Vector3(0.0f, 0.0f, -90.0f),
                };
            }

            return new Prefab()
            {
                Blocks = new Block[]
                {
                    new Block()
                    {
                        Position = new Vector3(-0.5f, 0.0f, -0.5f),
                        Scale = new Vector3(x, 1.0f, z),
                        MeshName = "OriginCube"
                    }
                },
                Inputs = inputArray,
                Outputs = outputArray
            };
        }

        public override (int inputCount, int outputCount) GetDefaultPegCounts()
        {
            return (inputCount: 8, outputCount: 8);
        }
    }

    public class RomPlacingRulesGenerator : DynamicPlacingRulesGenerator<(int, int)>
    {
        protected override (int, int) GetIdentifierFor(ComponentData componentData)
        {
            var m = new CustomDataManager<IRomData>();
            bool result = m.TryDeserializeData(componentData.CustomData);
            if (!result)
            {
                return (8, 8);
            }

            return (m.Data.SizeX, m.Data.SizeZ);
        }

        protected override PlacingRules GeneratePlacingRulesFor((int, int) dimensions)
        {
            return new PlacingRules()
            {
                OffsetDimensions = new Vector2Int(dimensions.Item1, dimensions.Item2),
                DefaultOffset = new Vector2Int(0, 0),
                GridPlacingDimensions = new Vector2Int(dimensions.Item1, dimensions.Item2)
            };
        }
    }
}
