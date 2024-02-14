using DotNetBungieAPI.Models.Destiny.Definitions.Activities;
using DotNetBungieAPI.Models.Destiny;
using Stopwatch.Utility;
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Windows.Threading;
using static DotNetBungieAPI.HashReferences.DefinitionHashes;
using Microsoft.Web.WebView2.Wpf;
using System.Globalization;



namespace list
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int CountTabs = 0;
        List<long> instanceIdList = new List<long>();


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this; // Set the DataContext to the current instance
            UpdateInfo();
            


        }
        public void CheckVisibleWebViews()
        {
            int totalWebViewCount = MyStackPanel.Children.Count;
            int collapsedWebViewCount = 0;

            foreach (UIElement child in MyStackPanel.Children)
            {
                if (child is ListActivity userControl)
                {
                    WebView2 webView = userControl.GetWebView2Control();
                    if (webView != null)
                    {
                        // Debug output to check the visibility of each WebView2 control
                        System.Diagnostics.Debug.WriteLine($"WebView2 Visibility: {webView.Visibility}");

                        if (webView.Visibility == Visibility.Collapsed)
                        {
                            collapsedWebViewCount++;
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"Total WebView Count: {totalWebViewCount}, Collapsed WebView Count: {collapsedWebViewCount}");

            if (collapsedWebViewCount == CountTabs)
            {
                // All WebView2 controls are visible, resize the main window
                Application.Current.MainWindow.Width = 350;
                Application.Current.MainWindow.Height = 460;
                System.Diagnostics.Debug.WriteLine("Window resized to original size.");
            }
            else
            {
                // Not all WebView2 controls are visible, do not resize the main window
                // Optionally, you can resize the window back to its original size or leave it as is
            }
        }
        



        private async void UpdateInfo()
        {
            await D2CharacterTracker.Update();
            string info = await InfoAsync();
            if(D2CharacterTracker.ActiveCharacter != null)
            {
                var TimeStartActivity = D2CharacterTracker.ActiveCharacter.DateActivityStarted.ToLocalTime();

                var HoursTimeStartActivity = TimeStartActivity.Hour;
                var MinutesTimeStartActivity = TimeStartActivity.Minute;
                var SecondsTimeStartActivity = TimeStartActivity.Second;

                DateTime startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, HoursTimeStartActivity, MinutesTimeStartActivity, SecondsTimeStartActivity);
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);

                timer.Start();

                if (info == "")
                {
                    var CurrentActivityText = "Not in Activity";
                    CurrentActivityTxt.Text = CurrentActivityText;
                    timer.Stop();
                    Timetxt.Text = "0m 0s";
                }
                else
                {
                    var CurrentActivityText = info; 
                    if (CurrentActivityText.Length > 22)
                    {
                        CurrentActivityText = string.Concat(CurrentActivityText.AsSpan(0, 22), "...");
                    }
                    CurrentActivityTxt.Text = CurrentActivityText;

                    timer.Tick += (sender, e) =>
                    {
                        // Calculate the elapsed time
                        TimeSpan elapsed = DateTime.Now - startTime;
                        // Ensure the elapsed time is positive
                        if (elapsed.TotalMilliseconds < 0)
                        {
                            elapsed = TimeSpan.Zero;
                        }

                        string ElapsedTime;

                        if (elapsed.Days > 0)
                        {
                            ElapsedTime = $"{elapsed.Days}d {elapsed.Hours:D2}h {elapsed.Minutes:D2}m {elapsed.Seconds:D2}s";
                        }
                        else if (elapsed.Hours > 0)
                        {
                            ElapsedTime = $"{elapsed.Hours:D2}h {elapsed.Minutes:D2}m {elapsed.Seconds:D2}s";
                        }
                        else
                        {
                            ElapsedTime = $"{elapsed.Minutes:D2}m {elapsed.Seconds:D2}s";
                        }

                        Timetxt.Text = ElapsedTime;
                    };
                }
            } 
                       
            var Act = D2CharacterTracker.UserActivities;

            int count = Act.Count;
            int val = 0;
            string completed = "";
            int redDot = 0;
            int greenDot = 0;
            RedDot.Text = Convert.ToString(redDot);
            GreenDot.Text = Convert.ToString(greenDot);
            foreach (var act in Act)
            {
                if (count > val)
                {
                    ListActivity userControl = new ListActivity(act);
                    Rectangle blackStripe = new Rectangle
                    {
                        Fill = Brushes.Black,
                        Height = 1, 
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Bottom
                    };

                    MyStackPanel.Children.Add(userControl);
                    CountTabs = CountTabs + 1;
                    MyStackPanel.Children.Add(blackStripe);

                    if (!instanceIdList.Contains(act.ActivityDetails.InstanceId))
                    {
                        if (Act[val].Values.TryGetValue("completed", out var compl))
                        {
                            completed = compl.BasicValue.DisplayValue;
                            if (completed == "No")
                            {
                                redDot++; 
                                RedDot.Text = Convert.ToString(redDot);
                            }
                            else
                            {
                                greenDot++;
                                GreenDot.Text = Convert.ToString(greenDot);
                            }
                        }
                    }
                    instanceIdList.Add(act.ActivityDetails.InstanceId);
                    val++;
                }
            }
        }

        
        private static async Task<string> InfoAsync()
        {
            string gt = "";
            if (D2CharacterTracker.ActiveCharacter?.CurrentActivity.Hash.Value != 0)
            {
                var allinfo = D2CharacterTracker.ActiveCharacter;
                if (allinfo != null)
                {
                    var activityDefinition = await D2CharacterTracker.client.ApiAccess.Destiny2.GetDestinyEntityDefinition<DestinyActivityDefinition>(DefinitionsEnum.DestinyActivityDefinition, allinfo.CurrentActivity.Hash.Value);
                    if (activityDefinition?.Response?.DisplayProperties != null)
                    {
                        gt = activityDefinition.Response.DisplayProperties.Name;
                    }
                }
            }
            return gt;
        }



    }


}
