using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Контейнер сам вставит сюда MainViewModel при создании окна!
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel; // Привязываем UI к ViewModel
        }
    }
}