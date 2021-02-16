using System.Diagnostics;
using System.Windows.Controls;

namespace ExamRevisionHelper.Views
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

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Process.Start((sender as TextBlock).Text);

    }
}
