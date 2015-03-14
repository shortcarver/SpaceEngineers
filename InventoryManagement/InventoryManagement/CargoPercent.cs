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
    class CargoPercent
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
        String lcdname = "Cargo Status";

        List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
        IMyTextPanel lcd;

        long maxvol = 0;
        long curvol = 0;

        void Main()
        {
            maxvol = 0;
            curvol = 0;
            initBlocks();

            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer container = (IMyCargoContainer)containers[i];

                    maxvol += (long)container.GetInventory(0).MaxVolume;
                    curvol += (long)container.GetInventory(0).CurrentVolume;

            }
            if (maxvol > 0)
            {
                lcd.WritePublicText("Storage: " + ((curvol * 100) / maxvol) + "%", false);
            }
            else
            {
                lcd.WritePublicText("Storage: 0%", false);
            }

        }

        void initBlocks()
        {
            containers.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
            if (lcd == null)
                lcd = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(lcdname);
        }
        #endregion
    }
}
