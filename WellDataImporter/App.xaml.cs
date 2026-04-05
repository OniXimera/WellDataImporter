using System.Configuration;
using System.Data;
using System.Windows;
using WellDataImporter.Services;
using WellDataImporter.Services.Calculators;
using WellDataImporter.ViewModels;

namespace WellDataImporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // App.xaml.cs
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var fileSystem = new FileSystemAdapter();
            var dataService = new DataProcessingService(
                new CsvWellReader(fileSystem),
                new WellSummaryCalculator(),
                new JsonWellExporter(fileSystem)
            );

            var viewModel = new MainViewModel(dataService);
            var mainWindow = new MainWindow { DataContext = viewModel };
            mainWindow.Show();
        }
    }
}
