using Avalonia.Controls;
using BrineAndCoin.Launcher.ViewModels;
using Markdown.Avalonia.Full;

namespace BrineAndCoin.Launcher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}