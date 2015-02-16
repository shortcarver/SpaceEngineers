using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;
using VRage;
using VRage.MyFixedPoint;

namespace SpaceEngineers
{
    class CodeEditorEmulator
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
        List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> refinery_like = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> refineries = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> arcFurnaces = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();
        IMyProgrammableBlock program;

        void Main()
        {
            initBlocks();

            // restacks
            /*
            setEnabled(refinery_like, true);
            setConveyers(refinery_like, false);
            cleanOutRefineries();
            setConveyers(arcFurnaces, true);
             * */

            // setConveyers(refineries, true);

            // re-arrange
            bool arcs = redistribute(arcFurnaces);
            bool refs = redistribute(refineries);

            if (arcs && refs)
            {
                program.SetCustomName("Done");
            }
            else
            {
                program.SetCustomName("Success");
            }
        }

        void initBlocks()
        {
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refinery_like);
            for (int i = 0; i < refinery_like.Count; i++)
            {
                IMyRefinery re = (IMyRefinery)refinery_like[i];

                if (re.DisplayNameText.ToLower().Contains("furnace"))
                {
                    arcFurnaces.Add(re);
                }
                else
                {
                    refineries.Add(re);
                }
            }

            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);

            List<IMyTerminalBlock> programs = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(programs);
            program = (IMyProgrammableBlock)programs[0];
        }

        bool redistribute(List<IMyTerminalBlock> blocks)
        {
            IMyRefinery fullest = null;
            long full_amount = 0;
            IMyRefinery empty = null;
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyRefinery test = (IMyRefinery)blocks[i];
                if ((long)test.GetInventory(0).CurrentMass > full_amount)
                {
                    fullest = test;
                    full_amount = (long)test.GetInventory(0).CurrentMass;
                }
                else if (test.GetInventory(0).CurrentMass.RawValue == 0)
                {
                    empty = test;
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

        void setConveyers(List<IMyTerminalBlock> blocks, bool isEnabled)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyRefinery re = (IMyRefinery)blocks[i];
                if (re.UseConveyorSystem != isEnabled)
                {
                    re.ApplyAction("UseConveyor");
                }
            }
        }

        void setEnabled(List<IMyTerminalBlock> blocks, bool isEnabled)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyFunctionalBlock re = (IMyFunctionalBlock)blocks[i];
                if (re.Enabled != isEnabled)
                {
                    re.ApplyAction("OnOff");
                }
            }
        }

        void cleanOutRefineries()
        {
            for (int i = 0; i < refinery_like.Count; i++)
            {
                IMyRefinery refinery = (IMyRefinery)refinery_like[i];

                IMyInventory inv = refinery.GetInventory(1);

                // move mats
                IMyCargoContainer container = findCargo(inv, "Ingot");
                transferAllTo(inv, container.GetInventory(0));

                // move Raw
                inv = refinery.GetInventory(0);
                container = findCargo(inv, "Ore");

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
                IMyCargoContainer container = findCargo(inv, "Comp");
                transferAllTo(inv, container.GetInventory(0));

                // move mats
                inv = assem.GetInventory(0);
                container = findCargo(inv, "Ingots");

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
        IMyCargoContainer findCargo(String type)
        {
            IMyCargoContainer selected = null;
            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer container = (IMyCargoContainer)containers[i];

                if (!container.GetInventory(0).IsFull)
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