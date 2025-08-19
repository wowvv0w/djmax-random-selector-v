using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using DjmaxRandomSelectorV.ViewModels;
using DjmaxRandomSelectorV.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DjmaxRandomSelectorV
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
