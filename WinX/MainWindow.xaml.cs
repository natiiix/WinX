using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace WinX
{
    public partial class MainWindow : Window
    {
        #region Constants
        private const int BW_PROGRESS_OUTPUT = 1;
        private const int BW_PROGRESS_EXIT = 2;
        #endregion
        #region Objects
        private IOHandler handler = new IOHandler();

        private BackgroundWorker bwCommandHandler = new BackgroundWorker();

        private string strInput = string.Empty;
        private string strOutput = string.Empty;
        #endregion
        #region Events
        #region MainWindow
        public MainWindow()
        { InitializeComponent(); }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            handler.OutputReady += Handler_OutputReady;
            handler.ExitCommandOccurred += Handler_ExitCommandOccurred;

            bwCommandHandler.WorkerSupportsCancellation = true;
            bwCommandHandler.WorkerReportsProgress = true;
            bwCommandHandler.DoWork += bwInputHandler_DoWork;
            bwCommandHandler.ProgressChanged += BwInputHandler_ProgressChanged;
            bwCommandHandler.RunWorkerCompleted += bwInputHandler_RunWorkerCompleted;

            AppendOutput("WinX has successfully loaded!");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (bwCommandHandler.IsBusy)
            {
                bwCommandHandler.CancelAsync();
                while (bwCommandHandler.CancellationPending) continue;
            }
        }
        #endregion
        #region bwInputHandler
        private void bwInputHandler_DoWork(object sender, DoWorkEventArgs e)
        { handler.Handle(strInput); }

        private void BwInputHandler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                // Pushes the output string
                case BW_PROGRESS_OUTPUT:
                    AppendOutput();
                    break;

                // Exits the application
                case BW_PROGRESS_EXIT:
                    Application.Current.Shutdown();
                    break;

                default:
                    break;
            }
        }

        private void bwInputHandler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        { InputEnable(); }
        #endregion
        #region textBoxInput
        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle the input string when Enter key is pressed
            if (e.Key == Key.Enter)
                HandleInput();
            else if (e.Key == Key.Tab)
                HandleSuggestions();
        }
        #endregion
        #region handler
        private void Handler_OutputReady(object sender, IOHandler.OutputReadyEventArgs e)
        {
            if (e.Output.Length <= 0) return;

            // If the event was raised from a background thread
            // push the output string indirectly
            if (bwCommandHandler.IsBusy)
            {
                strOutput = e.Output;
                bwCommandHandler.ReportProgress(BW_PROGRESS_OUTPUT);
            }
            else
                AppendOutput(e.Output);
        }
        
        private void Handler_ExitCommandOccurred(object sender, EventArgs e)
        {
            // If the event was raised from a background thread
            // use ReportProgress to exit the application
            if (bwCommandHandler.IsBusy)
                bwCommandHandler.ReportProgress(BW_PROGRESS_EXIT);
            else
                Close();
        }
        #endregion
        #endregion
        #region I/O Handling
        #region HandleSuggestions()
        /// <summary>
        /// Display suggestions of possible commands for the currently entered input string
        /// </summary>
        private void HandleSuggestions()
        {
            // Get the input string from the input TextBox
            string input = textBoxInput.Text;

            if (input == string.Empty) return;

            InputDisable();

            // Get suggestions
            string suggestions = handler.GetSuggestions(input);

            // If there's at least one suggestion available
            // display suggestions, otherwise display an error
            if (suggestions == string.Empty)
                AppendOutput("No command suggestions found for \"" + input + "\"");
            else
                AppendOutput("Suggestions for \"" + input + "\":" + Environment.NewLine + suggestions);

            InputEnable();
        }
        #endregion
        #region HandleInput()
        /// <summary>
        /// Handles the string from input Textbox
        /// </summary>
        private void HandleInput()
        {
            // Copy the input string into a local variable
            // and clean the input TextBox
            strInput = textBoxInput.Text;
            textBoxInput.Text = string.Empty;

            // Let the BackgroundWorker process the input
            bwCommandHandler.RunWorkerAsync();
        }
        #endregion

        #region AppendOutput()
        /// <summary>
        /// Appends the output string to the output TextBox
        /// </summary>
        private void AppendOutput()
        {
            // Append the text from the output variable
            AppendOutput(strOutput);

            // Clean the output string
            strOutput = string.Empty;
        }

        private void AppendOutput(string output)
        {
            // There's nothing to append if the output string is empty
            if (output == string.Empty) return;

            // Make new line if there's already some text in the output TextBox
            if (textBoxOutput.Text != string.Empty)
                textBoxOutput.Text += Environment.NewLine;

            // Append the output string and scroll to the last line
            // (to ensure that the appended line will be visible)
            textBoxOutput.Text += output;
            textBoxOutput.ScrollToEnd();
        }
        #endregion
        #region Input Disable/Enable
        /// <summary>
        /// Disable input TextBox
        /// </summary>
        private void InputDisable()
        {
            textBoxInput.IsEnabled = false;
        }

        /// <summary>
        /// Enable input TextBox and make it focused
        /// </summary>
        private void InputEnable()
        {
            textBoxInput.IsEnabled = true;
            textBoxInput.Focus();
        }
        #endregion
        #endregion
    }
}
