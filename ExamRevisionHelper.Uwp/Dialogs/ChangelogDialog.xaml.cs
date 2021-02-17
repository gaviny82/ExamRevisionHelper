using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ExamRevisionHelper.Dialogs
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ChangelogDialog : ContentDialog
    {
        public ChangelogDialog()
        {
            this.InitializeComponent();

            var ver = Package.Current.Id.Version;
            Title = $"Changelog {ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
        }
    }
}
