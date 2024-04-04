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

namespace MatchIt
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

        //CREATE THEME FINDING TAB TO SET THEME FOR GAMESTATE

        private void Button_Click(object sender, RoutedEventArgs e) //Quickplay
        {
            if (comboboxTheme.Text == "" || comboboxTheme.Text == null) { comboboxTheme.Text = "none"; }
            GameState nState = new GameState(comboboxTheme.Text.ToLower());
            GameWindow nGW = new GameWindow(false, nState);
            this.Close();
            nGW.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //Endless
        {
            if (comboboxTheme.Text == "" || comboboxTheme.Text == null) { comboboxTheme.Text = "none"; }
            GameState nState = new GameState(comboboxTheme.Text.ToLower());
            GameWindow nGW = new GameWindow(true, nState);
            this.Close();
            nGW.Show();
        }
    }
}
