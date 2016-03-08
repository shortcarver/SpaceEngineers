using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Definitions;
using VRage;
using VRage.MyFixedPoint;

namespace SpaceEngineers
{
    class GatherProducts
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
        String INGOT_STORAGE = "Ingot";
        String COMPONENT_STORAGE = "Comp";

        IMyCargoContainer INGOTS = null;
        IMyCargoContainer COMPS = null;

        List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> refinery_like = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();

        void Main()
        {
            initBlocks();

            cleanOutRefineries();
            cleanOutAssemblers();
        }

        void initBlocks()
        {
            containers.Clear();
            refinery_like.Clear();
            assemblers.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refinery_like);
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
        }

        void cleanOutRefineries()
        {
            for (int i = 0; i < refinery_like.Count; i++)
            {
                IMyRefinery refinery = (IMyRefinery)refinery_like[i];

                VRage.ModAPI.Ingame.IMyInventory inv = refinery.GetInventory(1);

                // move mats
                IMyCargoContainer container = getIngots();
                transferAllTo(inv, container.GetInventory(0));
            }
        }

        void cleanOutAssemblers()
        {
            for (int i = 0; i < assemblers.Count; i++)
            {
                IMyAssembler assem = (IMyAssembler)assemblers[i];
                VRage.ModAPI.Ingame.IMyInventory inv = assem.GetInventory(1);

                // move parts
                IMyCargoContainer container = getComps();
                transferAllTo(inv, container.GetInventory(0));
            }
        }

        void transferAllTo(VRage.ModAPI.Ingame.IMyInventory source, VRage.ModAPI.Ingame.IMyInventory dest)
        {
            while (source.GetItems().Count > 0)
            {
                source.TransferItemTo(dest, 0, null, true, null);
            }
        }

        IMyCargoContainer getIngots()
        {
            if (INGOTS == null)
            {
                INGOTS = findCargo(INGOT_STORAGE);
            }

            if (INGOTS.GetInventory(0).CurrentVolume.RawValue * 100 / INGOTS.GetInventory(0).MaxVolume.RawValue > 90)
            {
                INGOTS = findCargo(INGOT_STORAGE);
            }
            return INGOTS;
        }

        IMyCargoContainer getComps()
        {
            if (COMPS == null)
            {
                COMPS = findCargo(COMPONENT_STORAGE);
            }

            if (COMPS.GetInventory(0).CurrentVolume.RawValue * 100 / COMPS.GetInventory(0).MaxVolume.RawValue > 90)
            {
                COMPS = findCargo(COMPONENT_STORAGE);
            }
            return COMPS;
        }

        IMyCargoContainer findCargo(String type)
        {
            IMyCargoContainer selected = null;
            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer container = (IMyCargoContainer)containers[i];
                int max = (int)container.GetInventory(0).MaxVolume;
                int cur = (int)container.GetInventory(0).CurrentVolume;

                if (cur * 100 / max < 90)
                {
                    if (container.DisplayNameText.Contains(type))
                    {
                        return container;
                    }
                    else
                    {
                        selected = container;
                    }
                }
            }
            return selected;
        }

        #endregion
    }
}