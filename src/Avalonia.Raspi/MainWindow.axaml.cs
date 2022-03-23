using Avalonia.Controls;

namespace Avalonia.Raspi
{
    public partial class MainWindow : Window
    {
        CpuViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new CpuViewModel();
            DataContext = vm;
            vm.Start();
        }
    }
}
