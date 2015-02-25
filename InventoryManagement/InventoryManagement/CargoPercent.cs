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
        String trashContainerName = "trash";

        List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
        IMyTextPanel lcd;

        long maxvol = 0;
        long curvol = 0;
        long trashmaxvol = 0;
        long trashcurvol = 0;

        void Main()
        {
            initBlocks();

            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer container = (IMyCargoContainer)containers[i];

                // count trash
                if (container.DisplayNameText.ToLower().Contains(trashContainerName))
                {
                    trashmaxvol += (long)container.GetInventory(0).MaxVolume;
                    trashcurvol += (long)container.GetInventory(0).CurrentVolume;
                }
                else
                {
                    maxvol += (long)container.GetInventory(0).MaxVolume;
                    curvol += (long)container.GetInventory(0).CurrentVolume;
                }
            }
            if (maxvol > 0)
            {
                lcd.WritePublicText("Storage: " + ((curvol * 100) / maxvol) + "%", false);
            }
            else
            {
                lcd.WritePublicText("Storage: 0%", false);
            }
            if (trashmaxvol > 0)
            {
                lcd.WritePublicText("Trash: " + ((trashcurvol * 100) / trashmaxvol) + "%", true);
            }
            else
            {
                lcd.WritePublicText("Trash: 0%", true);
            }
        }

        void initBlocks()
        {
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
            lcd = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(lcdname);
        }
        #endregion
    }
}
