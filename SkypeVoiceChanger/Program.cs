using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Forms;

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
            Compose();
            Application.ThreadException += ApplicationOnThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Run(MainWindow);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            // Exception on background thread
            // TODO: improve on this (log and allow reporting)
            if (unhandledExceptionEventArgs.ExceptionObject is Exception)
            MessageBox.Show(((Exception)(unhandledExceptionEventArgs.ExceptionObject)).Message, "Unexpected Error");
        }

        private void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            // Exception on GUI Thread
            // TODO: improve on this (log and allow reporting)
            MessageBox.Show(threadExceptionEventArgs.Exception.Message, "Unexpected Error");

        }

        /// <summary>
        /// use managed extensibility framework to discover effects and load them into the main form
        /// </summary>
        private void Compose()
        {
            var catalog = new AggregatingComposablePartCatalog();
            var mainAssemblyCatalog = new AttributedAssemblyPartCatalog(this.GetType().Assembly);

            catalog.Catalogs.Add(mainAssemblyCatalog);
            var container = new CompositionContainer(catalog);
            
            container.AddPart(this);
            container.Compose();
        }

        [Import(typeof(MainForm))]
        public Form MainWindow { get; set; }


    }
}
