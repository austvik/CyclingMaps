namespace CyclingMaps.Views;

using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CyclingMaps.Models;
using CyclingMaps.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        Debug.WriteLine("Opened");

        var viewModel = (this.DataContext as MainWindowViewModel);
        if (viewModel != null) {
            viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => Redraw();
        }
    }

    public async void OpenRouteClicked(object source, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open GPX Route",
            AllowMultiple = false,
        });

        await using var stream = await files[0].OpenReadAsync();
        using var streamReader = new StreamReader(stream);
        var track = await CyclingMaps.Models.GpxReader.ParseFileAsync(streamReader);

        Debug.WriteLine($"Track: {track.Name} ({track.Type}) consists of {track.Positions.Count} points");

        (this.DataContext as MainWindowViewModel).Track = track;

        Redraw();
    }

    private void Redraw()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var canvas = topLevel.GetControl<Canvas>("canvas");
        MainWindowViewModel viewModel = (this.DataContext as MainWindowViewModel);

        var track = viewModel.Track;
        if (track == null || track.Positions == null)
        {
            return;
        }

        double watts = Double.Parse(viewModel.Watts);
        double weight = Double.Parse(viewModel.Weight);
        double seconds = Double.Parse(viewModel.Seconds);

        var deviceWidth = topLevel.ClientSize.Width;
        var deviceHeight = topLevel.ClientSize.Height;

        // TODO: Maintain aspect ratio
        var trackLeft = track.MinLongitude();
        var trackTop = track.MinLatitude();
        var trackWidth = track.Width();
        var trackHeight = track.Height();

        Avalonia.Point ScaleLatLongToCanvas(CyclingMaps.Models.Point point) {
            double x = ((point.Longitude - trackLeft) / trackWidth) * deviceWidth;
            double y = deviceHeight - ((point.Latitude - trackTop) / trackHeight) * deviceHeight;

            return new Avalonia.Point(x, y);
        };

        Rider rider = new(weight, 0.509, 0.63);
        Bike bike = new(8, 2, 2);
        Surface surface = new(0.005);
        Weather weather = new(0, 0.0, 1.22601);
        Impulse impulse = new(watts, TimeSpan.FromSeconds(1));

        Avalonia.Point zero = new Avalonia.Point(0, 0);
        Avalonia.Point prev = zero;
        Point prevP = new Point();
        int i = 0;
        double totalTime = 0.0;
        foreach (var point in track.Positions) {
            var curr = ScaleLatLongToCanvas(point);
            if (prev != zero) {
                var line = new Line() { StartPoint = prev, EndPoint = curr, Stroke = Brushes.Green };
                canvas.Children.Add(line);

                double timeUsed = prevP.DistanceSeconds(point, impulse, rider, bike, surface, weather);
                if (!double.IsNaN(timeUsed)) {
                    totalTime += timeUsed;
                }
                if (totalTime > seconds)
                {
                    Debug.WriteLine($"Found: {curr.X} {curr.Y}");
                    seconds = double.MaxValue;
                    var circle = new Ellipse() { Width = 40, Height = 40, Stroke = Brushes.Red, Fill = Brushes.Red, Opacity = 0.5 };
                    Canvas.SetLeft(circle, curr.X - circle.Width / 2);
                    Canvas.SetTop(circle, curr.Y - circle.Height / 2);
                    canvas.Children.Add(circle);
                }

                //    Debug.WriteLine($"Point: {i} time: {totalTime} (+ {timeUsed} seconds)");

                i++;
            }

            prev = curr;
            prevP = point;
        }
    }
}