using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;
using VRage;
using VRage.MyFixedPoint;

namespace SpaceEngineers
{
    class RefineryManager
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor

        List<IMyTerminalBlock> oreProcessors = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> refineries = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> arcFurnaces = new List<IMyTerminalBlock>();
        void Main()
        {
            initBlocks();
            redistribute(arcFurnaces);
            redistribute(refineries);
        }

        void initBlocks()
        {
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(oreProcessors);
            for (int i = 0; i < oreProcessors.Count; i++)
            {
                IMyRefinery oreProcessor = (IMyRefinery)oreProcessors[i];

                if (oreProcessor.DisplayNameText.ToLower().Contains("furnace"))
                {
                    arcFurnaces.Add(oreProcessor);
                }
                else
                {
                    refineries.Add(oreProcessor);
                }
            }
        }

        bool redistribute(List<IMyTerminalBlock> blocks)
        {
            IMyRefinery fullest = null;
            long fullestAmount = 0;
            IMyRefinery empty = null;
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyRefinery test = (IMyRefinery)blocks[i];
                long mass = (long)test.GetInventory(0).CurrentMass;

                if (mass == 0)
                {
                    if (empty == null)
                        empty = test;
                    continue;
                }

                if (mass > fullestAmount)
                {
                    fullest = test;
                    fullestAmount = mass;
                }
            }

            if (empty == null || fullest == null)
            {
                return true;
            }

            IMyInventory inv = fullest.GetInventory(0);
            inv.TransferItemTo(empty.GetInventory(0), 0, 0, true, inv.GetItems()[0].Amount * 0.5F);
            return false;
        }

        #endregion
    }
}
