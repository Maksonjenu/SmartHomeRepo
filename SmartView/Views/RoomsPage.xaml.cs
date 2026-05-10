namespace SmartView.Views;

public partial class RoomsPage
{
    public RoomsPage()
    {
        InitializeComponent();
    }

    private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var border = sender as System.Windows.Controls.Border;
        var parent = System.Windows.LogicalTreeHelper.GetParent(border) as System.Windows.Controls.ListBoxItem;
        if (parent != null)
        {
            parent.IsSelected = true;
        }
    }
}
