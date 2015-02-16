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
    class GatherProducts
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
        String INGOT_STORAGE = "Ingot";
        String COMPONENT_STORAGE = "Comp";

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
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refinery_like);
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
        }

        void cleanOutRefineries()
        {
            for (int i = 0; i < refinery_like.Count; i++)
            {
                IMyRefinery refinery = (IMyRefinery)refinery_like[i];

                IMyInventory inv = refinery.GetInventory(1);

                // move mats
                IMyCargoContainer container = findCargo(inv, INGOT_STORAGE);
                transferAllTo(inv, container.GetInventory(0));
            }
        }

        void cleanOutAssemblers()
        {
            for (int i = 0; i < assemblers.Count; i++)
            {
                IMyAssembler assem = (IMyAssembler)assemblers[i];
                IMyInventory inv = assem.GetInventory(1);

                // move parts
                IMyCargoContainer container = findCargo(inv, COMPONENT_STORAGE);
                transferAllTo(inv, container.GetInventory(0));
            }
        }

        void transferAllTo(IMyInventory source, IMyInventory dest)
        {
            while (source.GetItems().Count > 0)
            {
                source.TransferItemTo(dest, 0, null, true, null);
            }

        }

        IMyCargoContainer findCargo(IMyInventory sibling, String type)
        {
            IMyCargoContainer selected = null;
            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer container = (IMyCargoContainer)containers[i];

                if (!container.GetInventory(0).IsFull && sibling.IsConnectedTo(container.GetInventory(0)))
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