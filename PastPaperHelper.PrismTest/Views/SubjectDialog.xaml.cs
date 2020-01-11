using System.Windows;
using System.Windows.Controls;

namespace PastPaperHelper.PrismTest.Views
{
    /// <summary>
    /// Interaction logic for SubjectDialog
    /// </summary>
    public partial class SubjectDialog : UserControl
    {
        public SubjectDialog()
        {
            InitializeComponent();
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            //if (!(selectionTreeView.SelectedItem is Subject)) return;
            //Subject item = (Subject)selectionTreeView.SelectedItem;
            //SettingsViewModel vm = ((DataContext as MainWindowViewModel).ListItems.Last().Content as SettingsView).DataContext as SettingsViewModel;
            //vm.AddSubjectCommand.Execute(item);
        }
    }
}
