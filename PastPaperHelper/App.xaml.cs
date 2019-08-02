using PastPaperHelper.Sources;
using System.Windows;

namespace PastPaperHelper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            PaperSource source = null;
            switch (PastPaperHelper.Properties.Settings.Default.PaperSource)
            {
                default: source = PaperSources.GCE_Guide; break;
                case "GCE Guide": source = PaperSources.GCE_Guide; break;
                case "PapaCambridge": source = PaperSources.PapaCambridge; break;
                case "CIE Notes": source = PaperSources.CIE_Notes; break;
            }
            SubscriptionManager.CurrentPaperSource = source;
        }
    }
}
