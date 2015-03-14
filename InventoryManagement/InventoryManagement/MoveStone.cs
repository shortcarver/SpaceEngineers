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
    class MoveStone
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
 //Note that MoveStone is most effective if behind a conveyor sorter. Else it might come back, but end of the queue. 
        String STONE_STORAGE = "Stone"; 
 
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
 
                // move mats 
                IMyInventory inv = refinery.GetInventory(1); 
                IMyCargoContainer container = findCargo(inv, STONE_STORAGE); 
                transferStoneTo(inv, container.GetInventory(0)); 
 
                inv = refinery.GetInventory(0); 
                transferStoneTo(inv, container.GetInventory(0)); 
            } 
        } 
 
        void cleanOutAssemblers() 
        { 
            for (int i = 0; i < assemblers.Count; i++) 
            { 
                IMyAssembler assem = (IMyAssembler)assemblers[i]; 
 
                // move gravel 
                IMyInventory inv = assem.GetInventory(0); 
                IMyCargoContainer container = findCargo(inv, STONE_STORAGE); 
                transferStoneTo(inv, container.GetInventory(0)); 
            } 
        } 
 
        void transferStoneTo(IMyInventory source, IMyInventory dest) 
        { 
            //This collects both ingot and ore type "Stone" 
            List<int> stoneIdxs = new List<int>(); 
            List<IMyInventoryItem> sourceItems = source.GetItems(); 
            for (int i = 0; i < sourceItems.Count; i++) 
            { 
                if (sourceItems[i].Content.SubtypeId.ToString() == "Stone") 
                { 
                    stoneIdxs.Add(i); 
                } 
            } 
 
            for (int i = stoneIdxs.Count - 1; i >= 0; i--) 
            { 
                source.TransferItemTo(dest, stoneIdxs[i], null, true, null); 
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
