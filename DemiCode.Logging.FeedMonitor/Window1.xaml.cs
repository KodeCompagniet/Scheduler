using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ServiceModel.Syndication;
using System.Xml;

namespace DemiCode.Logging.FeedMonitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatusFeed();
        }

        private void LoadStatusFeed()
        {
            Uri feedUri = new Uri("http://localhost:8731/DemiCode/StatusFeed");

            using (XmlTextReader reader = new XmlTextReader(feedUri.ToString()))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                var items =
                            from item in feed.Items
                            orderby item.LastUpdatedTime
                            select new
                            {
                                Title = item.Title.Text,
                                Summary = (item.Summary == null ? String.Empty : item.Summary.Text),
                                Content = item.Content as TextSyndicationContent == null ? String.Empty : ((TextSyndicationContent)item.Content).Text,
                                LastUpdatedTime = item.LastUpdatedTime,
                                PublishDate = item.PublishDate,
                                ExceptionText =
                                (from element in item.ElementExtensions
                                 where (element.OuterName == "Exception") && (element.OuterNamespace == "http://demicode.com/logging")
                                 select element.GetObject<string>()
                                 ).FirstOrDefault()
                            };

                FeedViewer.ItemsSource = items;
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStatusFeed();
        }
    }
}
