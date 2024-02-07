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

            if (!string.IsNullOrEmpty(Url))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = "/c start " + Url
                });
            }


        }

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

                if (Modes == "AllPvP")
                {
                    IconActivity.Data = Application.Current.Resources[Modes] as Geometry;
                }
                else 
                {
                    switch (Mode)
                    {
                        case "Raid":
                        case "Dungeon":
                        case "LostSector":
                        case "Gambit":
                        case "TrialsOfOsiris":
                        case "Patrol":
                            IconActivity.Data = Application.Current.Resources[Mode] as Geometry;
                            break;
                        case "Strike":
                        case "Nightfall":
                        case "HeroicNightfall":
                        case "ScoredNightfall":
                        case "ScoredHeroicNightfall":
                        case "AllStrikes":
                            IconActivity.Data = Application.Current.Resources["Strike"] as Geometry;
                            break;
                        default:
                            IconActivity.Data = Application.Current.Resources["Patrol"] as Geometry;
                            break;
                    }
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




/*if (Act[val].Values.TryGetValue("activityDurationSeconds", out var destinyHistoricalStatsValue))
                    {
                        uint hash = (uint)Act[val].ActivityDetails.ActivityReference.Hash;

                        var display = await D2CharacterTracker.client.ApiAccess.Destiny2.GetDestinyEntityDefinition<DestinyActivityDefinition>(DefinitionsEnum.DestinyActivityDefinition, hash);

                        string activityName = display.Response.DisplayProperties.Name;
                        string duration = destinyHistoricalStatsValue.BasicValue.DisplayValue;
                        string completed = "";

                        if (Act[val].Values.TryGetValue("completed", out var compl))
                        {
                            completed = compl.BasicValue.DisplayValue;
                        }

                        
                    }
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * private Border CreateBorderElement(string activityName, string duration, string completed)
        {
            Border border = new Border
            {
                Margin = new Thickness(5, 5, 5, 5),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(20)

            };

            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#22000000");

            border.Background = brush;

            TextBlock txtBlk = new TextBlock
            {
                Text = $"{activityName}, {duration} ",
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.AliceBlue,
                FontFamily = new FontFamily("Microsoft JhengHei UI Light"),
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Normal,
                FontSize = 14,
            };
            Ellipse Dot = new Ellipse
            {
                Width = 12,
                Height = 12,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            converter = new System.Windows.Media.BrushConverter();
            brush = (Brush)converter.ConvertFromString(completed == "Yes" ? "#EE67C06B" : "#c06b67");
            Dot.Fill = brush;
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };

            stackPanel.Children.Add(txtBlk);
            stackPanel.Children.Add(Dot);

            border.Child = stackPanel;

            return border;
        }

 
 
 
 
 
 
 
 
 
 
 */
