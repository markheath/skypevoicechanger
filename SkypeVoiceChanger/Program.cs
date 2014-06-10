using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using SkypeVoiceChanger;
using SkypeVoiceChanger.Effects;

namespace SkypeFx
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
            Application.Run(MainWindow);
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
