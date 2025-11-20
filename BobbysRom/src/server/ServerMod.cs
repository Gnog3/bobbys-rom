using LogicAPI.Server;
using LogicWorld.Server.Circuitry;
using System;
using BobbysRom.Shared;

namespace BobbysRom.Server
{
    public class Server : ServerMod
    {
    }

    public class Rom : LogicComponent<IRomData>
    {
        protected override void SetDataDefaultValues()
        {
            Data.SizeX = 8;
            Data.SizeZ = 8;
            Data.Data = Array.Empty<byte>();
            
        }

        public override bool HasPersistentValues => true;

        private void SetByteAt(int index, int address)
        {
            byte value = 0;
            if (address < Data.Data.Length)
            {
                value = Data.Data[address];
            }
            for (int i = index; i < index + 8; i++)
            {
                Outputs[i].On = (value & 1) != 0;
                value >>= 1;
            }
        }
        
        protected override void DoLogicUpdate()
        {
            int address = 0;
            for (int i = 0; i < Inputs.Count; i++)
            {
                address += Inputs[i].On ? 1 << i : 0;
            }
            SetByteAt(0, address);
            if (Outputs.Count > 8)
            {
                SetByteAt(8, address + 1);
                if (Outputs.Count > 16)
                {
                    SetByteAt(16, address + 2);
                    SetByteAt(24, address + 3);
                }
            }
        }

        protected override void OnCustomDataUpdated()
        {
            QueueLogicUpdate();
        }
    }
}
