using DotNetBungieAPI.Models.Destiny.Definitions.Activities;
using DotNetBungieAPI.Models.Destiny;
using Stopwatch.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotNetBungieAPI.Models.Destiny.HistoricalStats;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.WinForms;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace list
{
    /// <summary>
    /// Логика взаимодействия для ListActivity.xaml
    /// </summary>
    public partial class ListActivity : UserControl
    {
        private string Url {  get; set; }
        private DestinyHistoricalStatsPeriodGroup Activity {  get; set; }

        public void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleWebViewVisibility();    
            
        }

        public WebView2 GetWebView2Control()
        {
            return webView;           
        }


        public void ToggleWebViewVisibility()
        {
            var blankUri = new Uri("about:blank");
            // Check the current visibility of the webView
            if (webView.Visibility == Visibility.Visible)
            {
                // If currently visible, collapse it and clear the source
                webView.Visibility = Visibility.Collapsed;
                webView.Source = blankUri;
                ((MainWindow)Application.Current.MainWindow).CheckVisibleWebViews();
            }
            else if (!string.IsNullOrEmpty(Url))
            {
                webView.Source = new Uri(Url);
                webView.Visibility = Visibility.Visible;
                Application.Current.MainWindow.Width = 1280;
                Application.Current.MainWindow.Height = 720;
                webView.BringIntoView();
            }
            
        }




        // Event handler for when the WebView2 control finishes loading a page







        public static string InsertSpacesBeforeCapitals(string input)
        {
            // Check if the input string has more than one character and starts with a capital letter
            if (!string.IsNullOrEmpty(input) && char.IsUpper(input[0]) && input.Length > 1)
            {
                // Use a regular expression to find uppercase characters that are not at the start of the string
                var pattern = @"(?<=\p{Ll})\p{Lu}";
                // Replace matched uppercase characters with a space followed by the uppercase character
                return Regex.Replace(input, pattern, m => " " + m.Value);
            }
            return input; // Return the original string if no changes are needed
        }

        public ListActivity(DestinyHistoricalStatsPeriodGroup activity)
        {
            Activity = activity;
            InitializeComponent();            
            DisplayInfo();
        }

        private async void DisplayInfo()
        {

            if (Activity.Values.TryGetValue("activityDurationSeconds", out var destinyHistoricalStatsValue))
            {
                uint hash = (uint)Activity.ActivityDetails.ActivityReference.Hash;
                var display = await D2CharacterTracker.client.ApiAccess.Destiny2.GetDestinyEntityDefinition<DestinyActivityDefinition>(DefinitionsEnum.DestinyActivityDefinition, hash);
                var Mode = Convert.ToString(Activity.ActivityDetails.Mode);
                Mode = InsertSpacesBeforeCapitals(Mode);
                string activityName = display.Response.DisplayProperties.Name;

                var Class = D2CharacterTracker.CharacterActivities.First(x => x.Value.Contains(Activity)).Key;

                var Char = Class.ClassType.ToString();
                IconClass.Data = Application.Current.Resources[Char] as Geometry;



                ActMode.Text = Mode;
                ActName.Text = activityName;
                string duration = destinyHistoricalStatsValue.BasicValue.DisplayValue;
                ActTime.Text = duration;
                string completed = "";
                var Modes = Convert.ToString(Activity.ActivityDetails.Modes[0]);
                if (Activity.Values.TryGetValue("completed", out var compl))
                {
                    completed = compl.BasicValue.DisplayValue;
                    var converter = new BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(completed == "Yes" ? "#EE67C06B" : "#c06b67");
                    Dot.Fill = brush;
                }

                switch (Mode)
                {
                    case "Raid":
                    case "Dungeon":
                    case "LostSector":
                    case "Gambit":
                    case "Patrol":
                        IconActivity.Data = Application.Current.Resources[Mode] as Geometry;
                        break;
                    case "Strike":
                    case "Nightfall":
                    case "Heroic Nightfall":
                    case "Scored Nightfall":
                    case "Scored Heroic Nightfall":
                    case "All Strikes":
                    case "":
                        IconActivity.Data = Application.Current.Resources["Strike"] as Geometry;
                        break;
                    default:
                        IconActivity.Data = Application.Current.Resources["Patrol"] as Geometry;
                        break;
                }

                if (Modes == "PrivateMatchesAll")
                {
                    IconActivity.Data = Application.Current.Resources["AllPvP"] as Geometry;
                }
                else if (Modes == "AllPvP")
                {
                    IconActivity.Data = Application.Current.Resources[Modes] as Geometry;
                }

                if(Mode == "Trials Of Osiris" )
                {
                    IconActivity.Data = Application.Current.Resources["TrialsOfOsiris"] as Geometry;
                }

                var modeUrlMap = new Dictionary<string, string>
                {
                    { "Raid", "https://raid.report/pgcr/" },
                    { "Dungeon", "https://dungeon.report/pgcr/" },
                    { "LostSector", "https://lostsector.report/pgcr/" },
                    { "ScoredNightfall", "https://gm.report/pgcr/" },
                    { "PrivateMatchesAll", "https://crucible.report/pgcr/" }
                };

                var mode = Convert.ToString(Activity.ActivityDetails.Mode);
                var instanceId = Convert.ToString(Activity.ActivityDetails.InstanceId);

                if (modeUrlMap.TryGetValue(mode, out string? value))
                {
                    Url = value + instanceId;
                }
                else if (Modes == "AllPvP")
                {
                    Url = "https://crucible.report/pgcr/" + instanceId;
                }
                else
                {
                    Url = "https://destinytracker.com/destiny-2/pgcr/" + instanceId;
                }
                
                


            }
        }       
    }
}
