using System.Diagnostics;
using System.Windows.Controls;

namespace PastPaperHelper.PrismTest.Views
{
    /// <summary>
    /// Interaction logic for ReferenceView
    /// </summary>
    public partial class ReferenceView : UserControl
    {
        public ReferenceView()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Process.Start("https://www.e-iceblue.com/Introduce/free-pdf-component.html");

    }
}
