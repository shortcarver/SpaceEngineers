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

namespace InventoryManagement
{
    class InventoryState
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
        String lcdname = "[INVSTATE]";
        int barCharCount = 60;

        List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> oreProcessors = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> refineries = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> arcFurnaces = new List<IMyTerminalBlock>();

        void Main()
        {
            initBlocks();
            countInventories(containers, "Cargo", 0, false);
            countInventories(refineries, "Refineries", 0, true);
            countInventories(arcFurnaces, "Furnaces", 0, true);
        }

        private void countInventories(List<IMyTerminalBlock> blocks, String name, int inventory, Boolean append)
        {
            long maxvol = 0;
            long curvol = 0;

            for (int i = 0; i < blocks.Count; i++)
            {
                IMyTerminalBlock container = blocks[i];

                maxvol += (long)container.GetInventory(inventory).MaxVolume;
                curvol += (long)container.GetInventory(inventory).CurrentVolume;
            }
            if (maxvol > 0)
            {
                setPercent(name, ((curvol * 100) / maxvol), append);
            }
            else
            {
                setPercent(name, 0, append);
            }
        }

        private void setPercent(String label, long amount, Boolean append)
        {
            if (!append)
            {
                setText("", false);
            }
            setText(label + " " + amount + "%\r\n", true);
            String percent = repeat("|", (amount * barCharCount) / 100) + repeat("'", barCharCount - (amount * barCharCount) / 100);
            setText(percent + "\r\n", true);
        }

        private String repeat(String value, long number)
        {
            StringBuilder sb = new StringBuilder();
            for (long i = 0; i < number; i++)
            {
                sb.Append(value);
            }
            return sb.ToString();
        }

        private void setText(String text, Boolean append)
        {
            for (int i = 0; i < lcds.Count; i++)
            {
                ((IMyTextPanel)lcds[i]).WritePublicText(text, append);
            }
        }

        void initBlocks()
        {
            containers.Clear();
            oreProcessors.Clear();
            lcds.Clear();
            arcFurnaces.Clear();
            refineries.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);

            List<IMyTerminalBlock> temp = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(temp);

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].DisplayNameText.Contains(lcdname))
                {
                    lcds.Add(temp[i]);
                }
            }

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
        #endregion
    }
}
