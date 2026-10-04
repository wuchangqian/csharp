using Avalonia.Controls;

namespace CSharpLearningProject.AuthDemo;

public partial class DataView : Window
{
    public DataView()
    {
        InitializeComponent();
        DataContext = new DataViewModel();
    }
}
