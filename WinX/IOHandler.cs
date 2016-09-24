using System;

namespace WinX
{
    class IOHandler
    {
        #region OutputReady Event
        public event EventHandler<OutputReadyEventArgs> OutputReady;

        public class OutputReadyEventArgs
        {
            public string Output = string.Empty;

            public OutputReadyEventArgs(string output)
            {
                Output = output;
            }
        }

        protected virtual void OnOutputReady(OutputReadyEventArgs e)
        {
            OutputReady?.Invoke(this, e);
        }
        #endregion
        #region ExitCommandOccurred Event
        public event EventHandler ExitCommandOccurred;

        protected virtual void OnExitCommandOccurred(EventArgs e)
        {
            ExitCommandOccurred?.Invoke(this, e);
        }
        #endregion

        #region Commands
        private Command[] Commands =
        {
            new CommandExit(),
            new CommandCMD()
        };
        #endregion

        #region IOHandler()
        public IOHandler()
        {
            
        }
        #endregion

        #region Handle(string input)
        public void Handle(string input)
        {
            if (input.Length <= 0) return;

            string[] inputArr = input.Split(' ');

            string inputCommand = inputArr[0];
            string[] inputArgs = RemoveCommandString(inputArr);

            foreach (Command comm in Commands)
                if (inputArr[0].ToLower() == comm.CommandString)
                {
                    if (comm.GetType() == typeof(CommandExit))
                        ExitCommandOccurred.Invoke(this, new EventArgs());
                    else
                    {
                        string output = string.Empty;

                        if ((output = comm.Execute(inputArgs)) != string.Empty)
                            OutputReady.Invoke(this, new OutputReadyEventArgs(output));
                    }

                    return;
                }

            OutputReady.Invoke(this, new OutputReadyEventArgs("Invalid command \"" + inputCommand + "\"!"));
        }
        #endregion
        #region RemoveCommandString(string[] arrIn)
        /// <summary>
        /// Removes the command string from the input string array
        /// </summary>
        /// <param name="arrIn">input array</param>
        /// <returns>Returns command arguments</returns>
        private static string[] RemoveCommandString(string[] arrIn)
        {
            string[] arrOut = new string[arrIn.Length - 1];

            for (int i = 0; i < arrOut.Length; i++)
                arrOut[i] = arrIn[i + 1];

            return arrOut;
        }
        #endregion

        #region GetSuggestions(string input)
        public string GetSuggestions(string input)
        {
            if (input.Length <= 0) return string.Empty;

            string output = string.Empty;

            foreach (Command comm in Commands)
                if (comm.CommandString.Contains(input.ToLower()))
                    AppendSuggestion(ref output, comm.Syntax + " - " + comm.Description);

            return output;
        }
        #endregion
        #region AppendSuggestion(ref string str, string suggestion)
        private static void AppendSuggestion(ref string str, string suggestion)
        {
            if (suggestion == string.Empty) return;

            if (str != string.Empty)
                str += Environment.NewLine;

            str += suggestion;
        }
        #endregion
    }
}
