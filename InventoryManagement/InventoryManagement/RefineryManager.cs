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
        List<IMyTerminalBlock> o2Gens = new List<IMyTerminalBlock>();
        List<string> arcMetals = new List<string>();
        void Main()
        {
            initBlocks();
            redistributeArc();
            redistribute(arcFurnaces);
            redistribute(refineries);
            redistribute(o2Gens);
        }

        void initBlocks()
        {
            oreProcessors.Clear();
            refineries.Clear();
            arcFurnaces.Clear();

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

            GridTerminalSystem.GetBlocksOfType<IMyOxygenGenerator>(o2Gens);

            //Don't know how to extract this from the game, but is unlikely to change
            arcMetals.Add("Iron");
            arcMetals.Add("Cobalt");
            arcMetals.Add("Nickel");
        }

        bool redistribute(List<IMyTerminalBlock> blocks)
        {
            IMyTerminalBlock fullest = null;
            long fullestAmount = 0;
            IMyTerminalBlock empty = null;
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyTerminalBlock test = (IMyTerminalBlock)blocks[i];
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

        void redistributeArc()
        {
            //Will move arc-able ores from non-empty refineries to empty arc furnaces.
            List<IMyRefinery> arcShortlist = new List<IMyRefinery>();
            for (int j=0; j < arcFurnaces.Count; j++) {
                IMyRefinery arc = (IMyRefinery)arcFurnaces[j];
                if (arc.GetInventory(0).CurrentMass == 0)
                {
                    arcShortlist.Add(arc);
                }
            }

            if (arcShortlist.Count == 0)
            {
                return;
            }

            for (int i = 0; i < refineries.Count; i++)
            {
                IMyRefinery refinery = (IMyRefinery)refineries[i];
                int arcIdx = 0;
                IMyInventory inv = refinery.GetInventory(0);  
                for (int j = 0; j < inv.GetItems().Count; j++)
                {
                    if (arcMetals.Contains(inv.GetItems()[j].Content.SubtypeId.ToString()))
                    {
                        IMyRefinery other = arcShortlist[arcIdx];
                        VRage.MyFixedPoint amount = inv.GetItems()[j].Amount * 0.5F;
                        if (amount < (VRage.MyFixedPoint)100.0F)
                        {
                            //For small amounts stuck high in the queue, it can get stuck dividing
                            //smaller and smaller chunks that the furnace can process in an instant
                            amount = inv.GetItems()[j].Amount;
                        }

                        inv.TransferItemTo(other.GetInventory(0), j, 0, true, amount);
                        arcIdx += 1;
                        if (arcIdx >= arcShortlist.Count)
                        {
                            return;
                        }

                    }
                }
            }
        }

        #endregion
    }
}
