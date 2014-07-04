using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var p = new Program();
            p.Run();
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += ApplicationOnThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var mainWindow = new MainForm(new List<Effect>() {
                new FlangeBaby(),
                new SuperPitch(),
                new Chorus(),
                new Delay(),
                new Tremolo(),
                new EventHorizon()
                
                } );
            Application.Run(mainWindow);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            // Exception on background thread
            var ex = unhandledExceptionEventArgs.ExceptionObject as Exception;
            if (ex != null)
                ErrorForm.ShowErrorForm(ex);
        }

        private void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            ErrorForm.ShowErrorForm(threadExceptionEventArgs.Exception);
        }
    }
}
