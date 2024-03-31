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

        private async void GetData(string date)
        {
            var dateText = date.Split('-');
            var day = dateText[0];
            var month = dateText[1];
            var year = dateText[2];

            var token = await HttpHelper.GetRooms($"?day={day}&month={month}&year={year}");
            if (token.Item2 != null)
            {
                var parser = new DataParser();
                var superParser = new SuperParse();
                var i = parser.ParseFloors(token.Item2);
                var data = superParser.Parse(token.Item1);
                CountOfRooms.Text = "Количество комнат на этаже: " + data.rooms.Length;
                RoomsWindows.Text = "Окна на этаже: " + string.Join(' ', data.rooms);
                RoomsCount.Text = "Количество комнат: " + data.root.data.rooms.Count;
                Numbers.Text = "Номера комнат: " + string.Join(' ', data.root.data.rooms);
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

        private async void LoadedAll(object sender, RoutedEventArgs e)
        {
            var dates = await HttpHelper.GetDates("/date");
            foreach(var item in dates.message)
            {
                Dates.Items.Add(item);
            }
        }

        private void Reselect(object sender, SelectionChangedEventArgs e)
        {
            GetData((string)Dates.SelectedItem);
        }
    }
}