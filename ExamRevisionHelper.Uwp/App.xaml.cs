using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using ExamRevisionHelper.Models;
using ExamRevisionHelper.Sources;
using System.Xml;

namespace ExamRevisionHelper
{
    public enum UpdateFrequency { Disable, Always, Daily, Weekly, Montly, Auto }
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static readonly ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;
        public static StorageFolder PastPapersFolder;
        public static StorageFile SourceDataFile;
        public static Subject[] SubjectsLoaded;
        public static List<Subject> SubscribedSubjects = new List<Subject>();

        //User settings
        public static UpdateFrequency UpdateFrequency;
        public static PaperSource PaperSource;

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationLanguages.PrimaryLanguageOverride = "en-GB";
            var initTask = Task.Run(Init);
            
            Frame rootFrame = Window.Current.Content as Frame;
            // 不要在窗口已包含内容时重复应用程序初始化，只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    var a = (e.Arguments, initTask);
                    rootFrame.Navigate(typeof(MainPage), (e.Arguments, initTask));
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        private InitializationResult Init()
        {
            //Load user settings
            string paperSource;
            string[] subjectsSubscribed = null;

            try
            {
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem("PastPapersFolder"))
                    PastPapersFolder = StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PastPapersFolder").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                //TODO: Reset papers folder.
                //Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { });
            }

            if (LocalSettings.Values.ContainsKey("UpdateFrequency"))
                UpdateFrequency = (UpdateFrequency)LocalSettings.Values["UpdateFrequency"];

            if (LocalSettings.Values.ContainsKey("PaperSource"))
                paperSource = (string)LocalSettings.Values["PaperSource"];
            else paperSource = PaperSource.DEFAULT_OPTION;

            //RoamingSettings.Values["SubjectsSubscribed"] = "9709,9702,9708";//Test
            if (RoamingSettings.Values.ContainsKey("SubjectsSubscribed"))
            {
                string str = RoamingSettings.Values["SubjectsSubscribed"] as string;
                if(!string.IsNullOrWhiteSpace(str))
                    subjectsSubscribed = str.Split(',');
            }
            else
                RoamingSettings.Values["SubjectsSubscribed"] = "";


            //Load source
            try
            {
                StorageFolder localCacheFolder = ApplicationData.Current.LocalCacheFolder;
                StorageFolder sourcesFolder = localCacheFolder.CreateFolderAsync("sources", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
                SourceDataFile = sourcesFolder.TryGetItemAsync($"{paperSource}.xml").GetAwaiter().GetResult() as StorageFile;
                if (SourceDataFile == null)
                {
                    SourceDataFile = sourcesFolder.CreateFileAsync($"{paperSource}.xml").GetAwaiter().GetResult();
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(SourceDataFile.Path);
                XmlNode data = doc.SelectSingleNode("/Data");

                PaperSource = data.Attributes["Source"].Value switch
                {
                    "gce_guide" => new PaperSourceGCEGuide(doc),
                    "papacambridge" => new PaperSourcePapaCambridge(doc),
                    "cie_notes" => new PaperSourceCIENotes(doc),
                    _ => throw new NotImplementedException(),
                };
                SubjectsLoaded = PaperSource.SubjectUrlMap.Keys.ToArray();


                //Load subjects
                if (subjectsSubscribed == null) return InitializationResult.SuccessNoUpdate;
                SubjectSubscriptionUtils.ReloadSubscribedSubjects(subjectsSubscribed);
                foreach (var item in SubscribedSubjects)
                {
                    if (!PaperSource.Subscription.ContainsKey(item))
                        return InitializationResult.Error;
                }
            }
            catch (Exception)
            {
                PaperSource = paperSource switch
                {
                    "gce_guide" => new PaperSourceGCEGuide(),
                    "papacambridge" => new PaperSourcePapaCambridge(),
                    "cie_notes" => new PaperSourceCIENotes(),
                    _ => new PaperSourceGCEGuide(),
                };
                //UserDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PastPaperHelper\\PastPaperHelper\\{sourceName}.xml";
                return InitializationResult.Error;
            }

            return InitializationResult.SuccessNoUpdate;
        }


        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
