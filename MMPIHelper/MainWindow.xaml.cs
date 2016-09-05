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
using System.Diagnostics;
using System.Threading;

namespace MMPIHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private Brush red, green;
        private MMPIDataHandler dataHandler;
        private bool userEdit;
        private int sleepTime;

        private void Init()
        {
            sleepTime = 100;
            mainWin.Width = 800;
            mainWin.Height = 700;
            InitDatabase();
            InitGrid();
        }

        private void InitGrid()
        {
            userEdit = false;
            red = new SolidColorBrush(Color.FromArgb(255, 255, 198, 198));
            green = new SolidColorBrush(Color.FromArgb(255, 207, 255, 198));
            red.Opacity = 0.5;
            green.Opacity = 0.5;
            int rows = 19;
            int cols = 30;

            for (int i = 0; i < rows; i++)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                blocks.RowDefinitions.Add(rd);
            }

            for (int i = 0; i < cols; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                blocks.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < rows; i++)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                sequence.RowDefinitions.Add(rd);
            }

            ColumnDefinition cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(40, GridUnitType.Pixel);
            sequence.ColumnDefinitions.Add(cd2);

            int idx = 1;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Label seqnum = new Label();
                    seqnum.Content = idx;
                    seqnum.FontSize = 9;
                    seqnum.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    seqnum.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    TextBox tb = new TextBox();
                    tb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                    tb.Name = "tb_" + idx;
                    tb.ToolTip = "Cella " + idx;
                    tb.Background = red;
                    tb.FontWeight = FontWeights.Bold;
                    tb.TextChanged += TextBox_TextChanged;
                    tb.PreviewKeyDown += TextBox_KeyDown;
                    tb.GotFocus += TextBox_GotFocus;
                    blocks.Children.Add(tb);
                    blocks.Children.Add(seqnum);
                    Grid.SetColumn(tb, j);
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(seqnum, j);
                    Grid.SetRow(seqnum, i);
                    idx++;
                    if (idx == 567)
                    {
                        i = rows;
                        break;
                    }
                }
            }

            int seq = 1;
            for (int i = 0; i < rows; i++)
            {
                Label tb = new Label();
                tb.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                tb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.FontSize = 14;
                tb.Content = seq;
                sequence.Children.Add(tb);
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, i);
                seq += 30;
            }
        }

        private void LoadData()
        {
            List<MMPIData> data = dataHandler.GetSavedData();
            LoadNumbers(data);
        }

        private void LoadNumbers(List<MMPIData> Numbers)
        {
            userEdit = false;
            TextBox cell;
            int index, last;
            String num;
            last = -1;
            for (int i = 0; i < blocks.Children.Count; i++)
            {
                if (blocks.Children[i] is TextBox)
                {
                    cell = blocks.Children[i] as TextBox;
                    index = int.Parse(cell.Name.Split('_')[1]);
                    num = Numbers[index - 1].Number;
                    if (num == MMPIDataHandler.DEFAULT_VALUE)
                    {
                        cell.Text = String.Empty;
                        if (last == -1) last = i;
                    }
                    else if (num == "0" || num == "1" || num == "2") cell.Text = num.ToString();
                }
            }
            if (last != -1) blocks.Children[last].Focus();
            userEdit = true;
        }

        private void InitDatabase()
        {
            dataHandler = MMPIDataHandler.GetDataHandler("mmpidata", Message);
        }

        private void ClearDataCells()
        {
            userEdit = false;
            foreach (UIElement cell in blocks.Children)
            {
                if (cell is TextBox) (cell as TextBox).Text = String.Empty;
            }
            blocks.Children[0].Focus();
            userEdit = true;
        }

        private void ClearDataFile()
        {
            dataHandler.EraseData();
        }

        private void Message(string Message)
        {
            MessageBox.Show(Message);
        }

        private void menu_newmmpi_Click(object sender, RoutedEventArgs e)
        {
            ClearDataFile();
            ClearDataCells();
        }

        private void menu_senddata_Click(object sender, RoutedEventArgs e)
        {
            Thread keysender = new Thread(Send);
            keysender.Start();
        }

        private void Send()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string processname = "ntvdm";
                Process[] procs = KeystrokeSimulator.GetProcessByName(processname);
                if (procs.Length == 0)
                {
                    MessageBox.Show(String.Format("Process [{0}] not found!", processname));
                    return;
                }

                Process cmd = procs[0];
                List<char> keys = new List<char>();
                foreach (UIElement element in blocks.Children)
                {
                    if (element is TextBox)
                    {
                        TextBox item = element as TextBox;
                        if (item.Text == "1" || item.Text == "2")
                        {
                            keys.Add(char.Parse(item.Text));
                        }
                        else keys.Add('0');
                    }
                }
                MSDosKeyStrokeSimulator.SendCharsNative(cmd, keys.ToArray(), sleepTime);
            }));
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            TraversalRequest tr = null;
            switch (e.Key)
            {
                case Key.Up: tr = new TraversalRequest(FocusNavigationDirection.Up); e.Handled = true; break;
                case Key.Down: tr = new TraversalRequest(FocusNavigationDirection.Down); e.Handled = true; break;
                case Key.Left: tr = new TraversalRequest(FocusNavigationDirection.Previous); e.Handled = true; break;
                case Key.Right: tr = new TraversalRequest(FocusNavigationDirection.Next); e.Handled = true; break;
            }

            if (tr != null) tb.MoveFocus(tr);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox cell = (sender as TextBox);
            if (userEdit && cell.Text != String.Empty)
            {
                TraversalRequest tr = new TraversalRequest(FocusNavigationDirection.Next);
                cell.MoveFocus(tr);
            }
            if (cell.Text.Equals("i") || cell.Text.Equals("I")) cell.Text = "1";
            else if (cell.Text.Equals("h") || cell.Text.Equals("H")) cell.Text = "2";

            String data = cell.Text;
            
            if (data.Equals("1") || data.Equals("2")) cell.Background = green;
            else
            {
                if (!data.Equals("0"))
                {
                    data = MMPIDataHandler.DEFAULT_VALUE;
                    cell.Text = String.Empty;
                }
                cell.Background = red;
            }
            if (userEdit) dataHandler.SaveNewNumber(int.Parse(cell.Name.Split('_')[1]), data);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void mainWin_ContentRendered(object sender, EventArgs e)
        {
            LoadData();
        }


    }
}
