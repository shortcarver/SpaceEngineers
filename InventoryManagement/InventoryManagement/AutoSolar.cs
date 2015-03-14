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
    class AutoSolar
    {
        IMyGridTerminalSystem GridTerminalSystem = null;

        #region CodeEditor
        List<IMyTerminalBlock> solars = new List<IMyTerminalBlock>();
        IMyProgrammableBlock program;
        IMyTextPanel debug;
        IMyMotorStator rotorRoll;
        IMyMotorStator rotorPitch;

        void Main()
        {
            initBlocks();

            List<ITerminalAction> actions = new List<ITerminalAction>();
            rotorRoll.GetActions(actions);

//            debug.WritePublicText("", false);
//            for (int i = 0; i < actions.Count; i++)
//            {
//                ITerminalAction action = actions[i];
//                debug.WritePublicText(action.Name.ToString() + "\r\n", true);
//            }

            debug.WritePublicText(rotorRoll.Displacement + "", false);
        }

        void initBlocks()
        {
            solars.Clear();

            List<IMyTerminalBlock> programs = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(programs);
            program = (IMyProgrammableBlock)programs[0];

            List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);
            debug = (IMyTextPanel)lcds[0];

            List<IMyTerminalBlock> rotors = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors);
            for (int i = 0; i < rotors.Count; i++)
            {
                IMyMotorStator rotor = (IMyMotorStator)rotors[i];

                if (rotor.DisplayNameText.ToLower().Contains("roll"))
                {
                    rotorRoll = rotor;
                }
                else if (rotor.DisplayNameText.ToLower().Contains("pitch"))
                {
                    rotorPitch = rotor;
                }

            }
        }
        #endregion
    }
}