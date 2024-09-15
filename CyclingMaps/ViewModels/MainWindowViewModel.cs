namespace CyclingMaps.ViewModels;

using CyclingMaps.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string watts = "200";

    [ObservableProperty]
    private string seconds = "3600";

    [ObservableProperty]
    private string weight = "80";

    [ObservableProperty]
    private Track track;
}
