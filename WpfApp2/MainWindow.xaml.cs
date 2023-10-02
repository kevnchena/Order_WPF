using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   

        Dictionary<string, int>  drinks = new Dictionary<string, int>();
        Dictionary<string, int>  orders = new Dictionary<string, int>();
        string takeout="";

        public MainWindow()
        {   
            InitializeComponent();
            //新增飲料
            AddNewDrink(drinks);
            //顯示所有飲料
            DisplayDrinkMenu(drinks);

        }

        private void DisplayDrinkMenu(Dictionary<string, int> mydrinks)
        {
            foreach(var drink in mydrinks)
            {
                StackPanel sp = new StackPanel();
                CheckBox cb = new CheckBox(); //打勾勾
                Slider sl = new Slider();
                Label lb = new Label();

                cb.Content = $"{drink.Key}:{drink.Value}元";
                cb.FontFamily = new FontFamily("Consolas");
                cb.Foreground = Brushes.Blue;
                cb.FontSize = 18;
                cb.Width = 200;
                cb.Margin= new Thickness(5);

                sl.Width = 100;
                sl.Value = 0;
                sl.Minimum = 0;
                sl.Maximum = 10;
                sl.IsSnapToTickEnabled = true;//tick是小數點
                //sl.TickPlacement = TickPlacement.BottomRight;

                lb.Width = 50;
                lb.Content = "0";

                sp.Orientation  = Orientation.Horizontal;
                sp.Margin  = new Thickness(4);
                sp.Children.Add(cb);
                sp.Children.Add(sl);
                sp.Children.Add(lb); 

                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);




                DrinkMenu.Children.Add(sp);
                DrinkMenu.Height = (mydrinks.Count+1)*35;
            }


        }

        private void AddNewDrink(Dictionary<string, int> mydrinks)
        {
            //mydrinks.Add("紅茶大杯", 60);
            //mydrinks.Add("紅茶小杯", 40);
            //mydrinks.Add("綠茶大杯", 60);
            //mydrinks.Add("綠茶小杯", 40);
            //mydrinks.Add("青茶大杯", 60);
            //mydrinks.Add("青茶小杯", 40);
            //mydrinks.Add("咖啡大杯", 80);
            //mydrinks.Add("咖啡小杯", 60);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV檔案|*.csv|文字檔案|*.txt|所有檔案|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(fileName);
                //string text =File.ReadAllText(fileName); 會讀到所有文字包含分割
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = Convert.ToInt32(tokens[1]);
                    mydrinks.Add(drinkName, price);
                }
            }
        }

        private void PlaceOrder(object sender, TextChangedEventArgs e)
        {
            var targetTextBox = sender as TextBox;
            bool success = int.TryParse(targetTextBox.Text, out int amount);
            if (!success)MessageBox.Show("請輸入整數", "輸入錯誤");
            else if (amount < 0) MessageBox.Show("輸入數值必須大於0", "輸入錯誤");
            else
            {
                StackPanel targetPanel = targetTextBox.Parent as StackPanel;
                Label targetLabel = targetPanel.Children[0] as Label;
                string drinkName = targetLabel.Content.ToString();

                if (orders.ContainsKey(drinkName)) orders.Remove(drinkName);
                orders.Add(drinkName, amount);
            }
        }
        private void Click_botton(object sender, RoutedEventArgs e)
        {
            //將訂購的飲料加入訂單
            OrderList(orders);
            //顯示訂單細項
            DisplayOrderDetail(orders);

        }

        private void DisplayOrderDetail(Dictionary<string, int> myorders)
        {
            displaytextblock.Inlines.Clear();
            Run titleString = new Run();
            titleString.Text = "你所訂購的品項";
            titleString.Foreground = Brushes.Blue;
            titleString.FontSize = 16;
            
            displaytextblock.Inlines.Add(titleString);

            
            //double total = 0.0;
            //double sellPrice = 0.0;
            //string message = "";
            //string displayMessage = "訂購清單如下:\n";

            //foreach (var item in myorders)
            //{   

            //    string drinkName = item.Key;
            //    int quantity = myorders[drinkName];
            //    int price = drinks[drinkName];

            //    total += quantity * price;
            //    displayMessage += $"{drinkName}   {quantity} 杯， 每杯{price} 共{quantity * price}元\n";
            //}

            //if (total >= 500)
            //{
            //    sellPrice = total * 0.8;
            //    message = "訂購500元以上者8折";

            //}
            //else if (total >= 300)
            //{
            //    sellPrice = total * 0.9;
            //    message = "訂購300元以上者9折";
            //}
            //else if (total >= 200)
            //{
            //    sellPrice = total * 0.95;
            //    message = "訂購200元以上者95折";
            //}
            //else
            //{
            //    sellPrice = total;
            //    message = "訂購未滿200元不打折";
            //}

            //displayMessage += $"{message}\n原訂單總價為{total}元，折扣後為{sellPrice}元";
            //displaytextblock.Text = displayMessage;
        }

        private void OrderList(Dictionary<string, int> myorders)
        {
           myorders.Clear();
           for(int i = 0; i < DrinkMenu.Children.Count; i++)
            {
                var sp = DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var sl = sp.Children[1] as Slider;
                String drinkName = cb.Content.ToString().Substring(0, 4);
                int quantity = Convert.ToInt32(sl.Value);

                if(cb.IsChecked ==true && quantity != 0)
                {
                    myorders.Add(drinkName, quantity);
                }
            }
        }

        private void RadioBotton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                takeout = rb.Content.ToString();
                MessageBox.Show(takeout);
                
            }
        }
    }
}
