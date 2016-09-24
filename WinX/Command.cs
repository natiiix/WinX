using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WinX
{
    abstract class Command
    {
        public string CommandString = string.Empty;
        public string Syntax = string.Empty;
        public string Description = string.Empty;

        /// <summary>
        /// Executes the specific command
        /// </summary>
        /// <param name="args">command arguments</param>
        public abstract string Execute(params string[] args);

        /*
        #region RemoveElement(ref string[] arr, int index)
        /// <summary>
        /// Removes an element from a string array
        /// </summary>
        /// <param name="arr">array to work with</param>
        /// <param name="index">index of the element to remove</param>
        protected static void RemoveElement(ref string[] arr, int index)
        {
            if (index >= arr.Length) return;

            for (int i = index; i < arr.Length - 1; i++)
                arr[i] = arr[i + 1];

            Array.Resize(ref arr, arr.Length - 1);
        }
        #endregion
        #region AppendElement(ref int[] arr, int element)
        /// <summary>
        /// Appends an element to an array
        /// </summary>
        /// <param name="arr">array to append to</param>
        /// <param name="element">element to append</param>
        protected static void AppendElement(ref int[] arr, int element)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.Length - 1] = element;
        }
        #endregion
        */
    }

    class CommandExit : Command
    {
        public CommandExit()
        {
            CommandString = "exit";
            Syntax = CommandString;
            Description = "Closes WinX";
        }

        public override string Execute(params string[] args) { return string.Empty; }
    }

    class CommandCMD : Command
    {
        public CommandCMD()
        {
            CommandString = "cmd";
            Syntax = CommandString + " [command] [arguments] [--hidden] [--dump]";
            Description = "Launches Command Line";
        }

        public override string Execute(params string[] args)
        {
            bool hidden = false;
            bool dump = false;

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i] == "--hidden")
                {
                    hidden = true;
                    args[i] = string.Empty;
                }
                else if (args[i] == "--dump")
                {
                    dump = true;
                    args[i] = string.Empty;
                }
            }

            // TODO: Get rid of empty arguments

            Process procCMD = new Process();

            procCMD.StartInfo.FileName = "cmd";
            if (args.Length > 0)
                procCMD.StartInfo.Arguments = "/C" + string.Join(" ", args);

            if (hidden)
            {
                procCMD.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                procCMD.StartInfo.CreateNoWindow = true;
            }

            if (dump)
            {
                procCMD.StartInfo.UseShellExecute = false;
                procCMD.StartInfo.RedirectStandardOutput = true;
            }

            procCMD.Start();

            if(dump)
            {
                procCMD.WaitForExit();
                return procCMD.StandardOutput.ReadToEnd();
            }

            return string.Empty;
        }
    }
}
