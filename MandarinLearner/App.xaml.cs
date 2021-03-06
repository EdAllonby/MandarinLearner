﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using log4net.Config;

namespace MandarinLearner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// When the application has started, give it an appropriate logging mechanism, and show the log in view.
        /// </summary>
        /// <param name="e">Startup event args.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            // Console must be started before configuring log4net.
            ConsoleManager.Show();
            SetupLogging("log4netDebug.config");
#else
            SetupLogging("log4netRelease.config");
#endif
            Thread.CurrentThread.Name = "Main Thread";

            base.OnStartup(e);
        }

        private static void SetupLogging(string logConfigName)
        {
            string assemblyPath = Assembly.GetAssembly(typeof(App)).Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            if (assemblyDirectory != null)
            {
                var uri = new Uri(Path.Combine(assemblyDirectory, logConfigName));

                XmlConfigurator.Configure(uri);
            }
        }

        /// <summary>
        /// Close the console if shown.
        /// </summary>
        /// <param name="e">Exit event args.</param>
        protected override void OnExit(ExitEventArgs e)
        {
#if DEBUG
            ConsoleManager.Hide();
#endif
            base.OnExit(e);
        }
    }
}