using MaterialDesignThemes.Wpf;
using PastPaperHelper.ViewModels;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// SearchView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchView : Grid
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public SearchStatus SearchStatus { get => (SearchStatus)GetValue(SearchStatusProperty); set => SetValue(SearchStatusProperty, value); }
        public static readonly DependencyProperty SearchStatusProperty = DependencyProperty.Register("SearchStatus", typeof(SearchStatus), typeof(SearchView), new UIPropertyMetadata(SearchStatus.Standby, new PropertyChangedCallback(searchStatusChanged)));

        private static void searchStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchView view = (SearchView)d;
            view.SearchStatusChanged_Callback((SearchStatus)e.NewValue);
        }

        public SearchView()
        {
            InitializeComponent();
            SearchViewModel model = new SearchViewModel();
            resultsGrid.DataContext = model;
            model.SearchPath = Properties.Settings.Default.Path;
            Binding binding = new Binding("SearchStatus") { Source = model, Mode = BindingMode.TwoWay };
            SetBinding(SearchStatusProperty, binding);
            DataContext = model;
            SearchStatusChanged_Callback(SearchStatus.Standby);
        }

        private void ViewMarkScheme(object sender, RoutedEventArgs e)
        {
            Question question = (sender as Button).DataContext as Question;
            if (question != null) Process.Start(question.FilePath.Replace("qp", "ms"));
        }

        private void ViewQuestionPaper(object sender, RoutedEventArgs e)
        {
            Question question = (sender as Button).DataContext as Question;
            if (question != null) Process.Start(question.FilePath);
        }

        public void SearchStatusChanged_Callback(SearchStatus newStatus)
        {
            if (newStatus == SearchStatus.Standby)
            {
                search.Content = "Search";
                ButtonProgressAssist.SetIsIndeterminate(search, false);
                keyword.IsEnabled = true;
                progress.IsIndeterminate = true;
            }
            else
            {
                search.Content = "Cancel";
                ButtonProgressAssist.SetIsIndeterminate(search, true);
                keyword.IsEnabled = false;
                progress.IsIndeterminate = false;
            }
        }
    }
}
