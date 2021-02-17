using ExamRevisionHelper.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace ExamRevisionHelper.Dialogs
{
    public sealed partial class SubjectDialog : ContentDialog
    {
        SubjectSelectionNode IGSubjects = new SubjectSelectionNode { Content = "IGCSE" };
        SubjectSelectionNode ALSubjects = new SubjectSelectionNode { Content = "A Level" };

        public SubjectDialog()
        {
            this.InitializeComponent();
            //initialize size
            var windowSize = Window.Current.CoreWindow.Bounds;
            subjectSelector.Width = windowSize.Width - 200;
            subjectSelector.Height = windowSize.Height - 300;

            subjectSelector.RootNodes.Add(IGSubjects);
            subjectSelector.RootNodes.Add(ALSubjects);

            //TODO: MVVM
            //FIXME: Error if App.SubjectsLoaded is null
            foreach (var subj in from subj in App.SubjectsLoaded
                                 where subj.Curriculum == Curriculums.IGCSE
                                 select new SubjectSelectionNode
                                 {
                                     Content = $"{subj.SyllabusCode} {subj.Name}",
                                     Subject = subj
                                 })
            {
                IGSubjects.Children.Add(subj);
                if (App.SubscribedSubjects.Contains(subj.Subject))
                    subjectSelector.SelectedNodes.Add(subj);
            }

            foreach (var subj in from subj in App.SubjectsLoaded
                                 where subj.Curriculum == Curriculums.ALevel
                                 select new SubjectSelectionNode
                                 {
                                     Content = $"{subj.SyllabusCode} {subj.Name}",
                                     Subject = subj
                                 })
            {
                ALSubjects.Children.Add(subj);
                if (App.SubscribedSubjects.Contains(subj.Subject))
                    subjectSelector.SelectedNodes.Add(subj);
            }
        }

        #region Content size control
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void ContentDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            subjectSelector.Width = e.Size.Width - 200;
            subjectSelector.Height = e.Size.Height - 300;
        }
        #endregion

        private async void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var tmp = from node in subjectSelector.SelectedNodes
                       where node.Parent != null
                       select (node as SubjectSelectionNode).Subject;
            var list = tmp.ToList();

            //Unsubscribe removed subjects; eliminate existing subjects
            List<Subject> removed = new List<Subject>();
            foreach (Subject subj in App.SubscribedSubjects)
            {
                if (list.Contains(subj))
                    list.Remove(subj);
                else
                    removed.Add(subj);
            }

            removed.ForEach((subj) =>
                    SubjectSubscriptionUtils.Unsubscribe(subj));

            //Add new subjects to subscription
            foreach (Subject subj in list)
                await SubjectSubscriptionUtils.SubscribeAsync(subj).ConfigureAwait(false);

            XmlDocument dataDocument = App.PaperSource.SaveDataToXml();
            using (Stream outputStream = App.SourceDataFile.OpenStreamForWriteAsync().GetAwaiter().GetResult())
            {
                dataDocument.Save(outputStream);
            }
        }
    }

    public class SubjectSelectionNode : Microsoft.UI.Xaml.Controls.TreeViewNode
    {
        public Subject Subject { get; set; }
        public SubjectSelectionNode() : base() { }
    }
}
