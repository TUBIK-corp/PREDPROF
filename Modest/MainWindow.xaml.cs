using Modest.Helpers;
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

namespace Modest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetData(object sender, RoutedEventArgs e)
        {
            var dates = await HttpHelper.GetDates("/dates");
            var token = await HttpHelper.GetRooms("?day=30&month=10&year=23");
            if (token.Item2 != null)
            {
                var parser = new DataParser();
                var superParser = new SuperParse();
                var i = parser.ParseFloors(token.Item2);
                var data = superParser.Parse(token.Item1);
                Draw(i, i.RoomCount, i.FloorCount, data.rooms);
            }
        }

        public void Draw(DataParser.NeededData needed, int width, int height, int[] rooms)
        {
            Rooms.Children.Clear();
            Rooms.RowDefinitions.Clear();
            Rooms.ColumnDefinitions.Clear();
            for (int i = 0; i < width; i++)
            {
                Rooms.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < height; i++)
            {
                Rooms.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
            int floor_k = 0;
            foreach(var floor in needed.Floors)
            {
                int currentRow = Rooms.RowDefinitions.Count-floor_k-1;
                int currentColumn = 0;
                int temp = 1;
                int iterator = 0;
                int current = 0;
                foreach(var light in floor.Lights)
                {
                    current++;
                    temp = rooms[iterator];
                    var border = new Border();
                    border.Background = light ? Brushes.LightYellow : Brushes.DarkGray;
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    TextBlock block = new TextBlock();
                    block.VerticalAlignment = VerticalAlignment.Center;
                    block.HorizontalAlignment = HorizontalAlignment.Center; 
                    block.Text = ((iterator+1)+(floor_k)*rooms.Length).ToString();
                    border.Child = block;
                    Rooms.Children.Add(border);
                    Grid.SetRow(border, currentRow);
                    Grid.SetColumn(border, currentColumn);
                    if(current >= temp)
                    {
                        iterator++;
                        current = 0;
                        if(iterator < rooms.Length)
                            temp = rooms[iterator];
                    }
                    currentColumn++;
                }
                floor_k++;
            }
        }
    }
}