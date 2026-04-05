using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using WellDataImporter.Common;
using WellDataImporter.Models;
using WellDataImporter.Services;

namespace WellDataImporter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel(IDataProcessingService dataService)
        {
            var fileSystem = new FileSystemAdapter();
            _dataService = dataService;
        }

        [RelayCommand]
        private async Task ImportFileAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = AppConstants.CsvFilter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                IsBusy = true;
                StatusMessage = Resources.Resources.StatusLoading;
                Summaries.Clear();
                Errors.Clear();

                try
                {
                    var result = await Task.Run(() => _dataService.ImportCsvAsync(openFileDialog.FileName));
                    foreach (var summary in result.Summaries)
                    {
                        Summaries.Add(summary);
                    }

                    foreach (var error in result.Errors)
                    {
                        Errors.Add(error);
                    }

                    ExportJsonCommand.NotifyCanExecuteChanged();
                    StatusMessage = string.Format(Resources.Resources.StatusReadDone, Summaries.Count, Errors.Count);
                }
                catch (Exception ex)
                {
                    StatusMessage = Resources.Resources.StatusReadError;
                    MessageBox.Show(ex.Message, Resources.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private async Task ExportJsonAsync()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = AppConstants.JsonFilter,
                FileName = AppConstants.JsonFileName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                IsBusy = true;
                StatusMessage = Resources.Resources.StatusExporting;
                try
                {
                    await Task.Run(() => _dataService.ExportToJsonAsync([.. Summaries], saveFileDialog.FileName));
                    StatusMessage = Resources.Resources.StatusExportingDone;
                    MessageBox.Show(Resources.Resources.MsgExportSuccess, Resources.Resources.Success, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    StatusMessage = Resources.Resources.StatusExportingError;
                    MessageBox.Show(ex.Message, Resources.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanExport() => Summaries.Count > 0;

        [ObservableProperty] private ObservableCollection<WellSummary> _summaries = new();
        [ObservableProperty] private ObservableCollection<ImportError> _errors = new();
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _statusMessage = Resources.Resources.StatusIdle;
        private readonly IDataProcessingService _dataService;
    }
}