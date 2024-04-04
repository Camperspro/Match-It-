using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace MatchIt
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    /// 
    public partial class GameWindow : Window
    {
        GameState nGame = new GameState();
        List<Card> cardDeck = new List<Card>();
        List<Card> paired = new List<Card>();
        DispatcherTimer timer = new DispatcherTimer();
        bool endless = false;
        Card cFlip = null, cFlip2 = null;
        int tseconds = 0;
        int fastestTime = 0;
        int pairs = 0;
        Image flip = null;
        Image flip2 = null;
        string gamemode = "Quickplay";

        public GameWindow(bool nEndless, GameState ng)
        {
            nGame= ng;
            endless = nEndless;
            if (endless)
            {gamemode = "Endless";}

            start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            tseconds++;
            tableTime.Content = tseconds.ToString();
        }

        public void start()
        {
            InitializeComponent();
            nGame.setState("NG");
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            updateTable();
        }

        private void updateTable()
        {
            if (nGame.getHearts() <= 0) { nGame.setState("GO"); }
            else if(paired.Count == cardDeck.Count && endless && nGame.getState() != "NG")
            {
                this.Dispatcher.Invoke(() =>
                {
                    warningLabel.Content = "TABLE CLEARED!";
                    warningLabel.Background = Brushes.LightGreen;
                    warningLabel.Visibility = Visibility.Visible;
                    updateImages();
                    showFlipped();
                    nGame.setState("NT"); timer.Stop();
                });
            }
            else if(paired.Count == cardDeck.Count && !endless && nGame.getState() != "NG") { nGame.setState("GO"); fastestTime = tseconds; }

            switch (nGame.getState())
            {
                case "GO": //GAME OVER STATE
                    this.Dispatcher.Invoke(() =>
                    {
                        if (endless)
                        {
                            GameOverMenu.Visibility = Visibility.Visible;
                            FinalText.Text = "Highest Table: #" + nGame.getTable() + ", Best Time: " + fastestTime + "s, Total Pairs: " + pairs + ".";
                        }
                        else
                        {
                            GameOverMenu.Visibility = Visibility.Visible;
                            goTitle.Text = "Finished!";
                            FinalText.Text = "Best Time: " + fastestTime + "s.";
                        }
                    });
                    break;
                case "NG": //NEW GAME STATE
                    nGame.setState("GS");
                    generateBackground();
                    if (cardDeck.Count == 0)
                     { createPairs(); }
                    //Now Find User Theme & Set our Images. Then Start Timer
                    foreach (Card y in cardDeck)
                     { showTheme(y); }
                    timer.Start();
                    updateHud();
                    break;
                case "NT": //NEW TABLE STATE
                    this.Dispatcher.Invoke(() =>
                    {
                        generateBackground();
                        cardDeck.Clear();
                        paired.Clear();
                        if(fastestTime == 0) { fastestTime = tseconds; }
                        else if(tseconds < fastestTime) { fastestTime = tseconds; }
                        prevTime.Content = "Previous Table Time: " + tseconds.ToString()+"s";
                        tseconds = 0;
                        { createPairs(); }
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += timer_Tick;
                        timer.Start();
                        nGame.setTable(nGame.getTable()+1);
                        nGame.setHearts(3);
                        nGame.setState("GS");
                        updateHud();
                    });
                    break;
                case "GS": //GAME START or ONGOING
                    updateHud();
                    break;
            }
        }

        private void generateBackground()
        {
            Random rnd = new Random();
            int index = rnd.Next(0,2);
            if (nGame.getTheme() != "none")
            {
                string[] location = Directory.GetFiles(@"../../Assets/" + nGame.getTheme() + "/background/", "*.jpg", SearchOption.TopDirectoryOnly);
                BitmapImage backG = new BitmapImage(new Uri(location[index], UriKind.Relative));
                switch (nGame.getTheme())
                {
                    case "animal":
                        this.Background = new ImageBrush(backG);
                        break;

                    case "anime":
                        this.Background = new ImageBrush(backG);
                        break;
                }
            }
            else
            {
                this.Background = Brushes.LightGray;
            }
        }

        private void showImage(Card card)
        {card.setImage(new BitmapImage(new Uri("Assets/" + nGame.getTheme() + "/card" + card.getValue() + ".png", UriKind.Relative)));}

        private void showTheme(Card card)
        { card.setImage(new BitmapImage(new Uri("Assets/" + nGame.getTheme() + "/cover.png", UriKind.Relative)));}

        private List<Card> createDeck(List<Card> deck)
        {
            Random r = new Random();
            int x = 0;
            Card card = null;
            List<int> numbers = new List<int>();
            do
            {
                card = new Card(r.Next(1, 10));
                if (!numbers.Contains(card.getValue()))
                {deck.Add(card); numbers.Add(card.getValue());}

            } while (deck.Count != 5);
                
            do
            {
                x = 0;
                card = new Card(r.Next(1, 10));
                if (numbers.Contains(card.getValue()))
                {
                    foreach(Card compare in deck)
                    {
                        if(compare.getValue() == card.getValue())
                        {x++;}
                    }
                }
                if(x < 2 && numbers.Contains(card.getValue())) { deck.Add(card);}
            } while (deck.Count != 10);

            return deck;
        }

        private void createPairs()
        {
            Task.Delay(60);
            createDeck(cardDeck);
            int t = cardDeck.Count;
            Random r = new Random();
            while (t > 1)
            {
                t--;
                int n = r.Next(t + 1);
                var value = cardDeck[n];
                cardDeck[n] = cardDeck[t];
                cardDeck[t] = value;
            }
        }

        private async void checkFlipped(Card f1, Card f2)
        {
            if (f1.getValue() == f2.getValue() && f1.getFlipped() && f2.getFlipped())
            {
                paired.Add(f1); paired.Add(f2);
                this.Dispatcher.Invoke(() =>
                {
                    Image one = (Image)tableCards.Children[cardDeck.FindIndex(x => x.Equals(f1))];
                    Image two = (Image)tableCards.Children[cardDeck.FindIndex(x => x.Equals(f2))];
                    showImage(f1); showImage(f2);
                    one.Source = f1.getImage();
                    two.Source = f2.getImage();
                    pairs++;
                    if (endless && nGame.getHearts() < 3) { nGame.setHearts(nGame.getHearts() + 1); }
                    cFlip = null;
                    cFlip2 = null;
                    updateImages();
                    updateHud();
                    Task.Delay(1100);
                });
            }
            else
            {
                updateImages();
                await Task.Delay(1050);
                this.Dispatcher.Invoke(() =>
                {
                    f1.setFlipped(false);
                    f2.setFlipped(false);
                    showTheme(f1); showTheme(f2);
                    flip.Source = f1.getImage();
                    flip2.Source = f2.getImage();
                    if (endless) { nGame.setHearts(nGame.getHearts() - 1); }
                    cFlip = null;
                    cFlip2 = null;
                    updateImages();
                    updateHud();
                });
            }
        }

        private void showFlipped()
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach(Card i in paired)
                {
                    Image flip = (Image)tableCards.Children[cardDeck.FindIndex(x => x.Equals(i))];
                    showImage(i);
                    flip.Source = i.getImage();
                }
            });
        }

        public void updateImages()
        {
            for (int i = 0; i < cardDeck.Count; i++)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Image flip = (Image)tableCards.Children[i];
                    flip.Visibility = Visibility.Visible;
                    Card temp = cardDeck[i];
                    if (temp.getFlipped())
                    {
                        showImage(temp);
                        flip.Source = temp.getImage();
                    }
                    else
                    {
                        showTheme(temp);
                        flip.Source = temp.getImage();
                    }
                });
            }
        }

        private void updateHud()
        {
            this.Dispatcher.Invoke(async () =>
            {
                this.IsHitTestVisible = true;
                tableMode.Content = gamemode;
                tableInfo.Content = "Table " + nGame.getTable();
                tablePairs.Content = paired.Count + " / " + cardDeck.Count;
                if (!endless) //Quickplay
                {
                    heartCont.Visibility = Visibility.Collapsed;
                    tableInfo.Visibility = Visibility.Collapsed;
                }
                else
                {
                    switch (nGame.getHearts())
                    {
                        case 0:
                            hc1.Source = new BitmapImage(new Uri("Assets/heart-outline.png", UriKind.Relative));
                            hc2.Source = new BitmapImage(new Uri("Assets/heart-outline.png", UriKind.Relative));
                            hc3.Source = new BitmapImage(new Uri("Assets/heart-outline.png", UriKind.Relative));
                            heartCont.Background = Brushes.DarkRed;
                            nGame.setState("GO");
                            updateTable();
                            break;

                        case 1:
                            hc1.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc2.Source = new BitmapImage(new Uri("Assets/heart-outline.png", UriKind.Relative));
                            hc3.Source = new BitmapImage(new Uri("Assets/heart-outline.png", UriKind.Relative));
                            heartCont.Background = Brushes.Transparent;
                            break;
                        case 2:
                            hc1.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc2.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc3.Source = new BitmapImage(new Uri("Assets/heart-outline.png", UriKind.Relative));
                            heartCont.Background = Brushes.Transparent;
                            break;
                        case 3:
                            hc1.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc2.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc3.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            heartCont.Background = Brushes.Transparent;
                            break;

                        case 4:
                            hc1.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc2.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            hc3.Source = new BitmapImage(new Uri("Assets/heart.png", UriKind.Relative));
                            heartCont.Background = Brushes.LightSkyBlue;
                            break;
                    } //Life Bar
                }

                await Task.Run(() =>
                {
                    updateImages();
                    showFlipped();
                });
            });
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            this.Dispatcher.Invoke(() =>
            {
                nGame.setState("GO");
                fastestTime = tseconds;
                updateTable();
            });
        }

        private void Mainmenu_Click(object sender, RoutedEventArgs e) //Endless
        {
            //Do proper Cleanup
            cardDeck.Clear();
            paired.Clear();
            nGame = null;
            timer = null;
            flip = null;
            flip2 = null;
            pairs = 0;
            fastestTime = 0;
            tseconds = 0;
            MainWindow nMM = new MainWindow();
            this.Close();
            nMM.Show();
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e) //Endless
        {
            cardDeck.Clear();
            paired.Clear();
            timer = null;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            flip = null;
            flip2 = null;
            pairs = 0;
            fastestTime = 0;
            tseconds = 0;
            nGame.reset();
            GameOverMenu.Visibility = Visibility.Hidden;
            updateTable();
        }

        private async void card_MouseDown(object sender, MouseButtonEventArgs e) //get clicked cards and used flipped bool for all flipped cards or make 2 list 
        {
            await Task.Run(() =>
            {
                Task.Delay(30);
                if (cFlip == null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        int index = tableCards.Children.IndexOf(sender as UIElement);
                        cFlip = cardDeck[index];
                        cFlip.setFlipped(true);
                        flip = (Image)tableCards.Children[cardDeck.FindIndex(x => x.Equals(cFlip))];
                        showImage(cFlip);
                        flip.Source = cFlip.getImage();
                        warningLabel.Visibility = Visibility.Collapsed;
                    });
                }
                else if (cFlip2 == null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        int index = tableCards.Children.IndexOf(sender as UIElement);
                        cFlip2 = cardDeck[index];
                        cFlip2.setFlipped(true);
                        flip2 = (Image)tableCards.Children[cardDeck.FindIndex(x => x.Equals(cFlip))];
                        showImage(cFlip2);
                        flip2.Source = cFlip2.getImage();
                        warningLabel.Visibility = Visibility.Collapsed;
                    });
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.IsHitTestVisible = false;
                    if (paired.Contains(cFlip))
                    {
                        cFlip = null;
                        warningLabel.Visibility = Visibility.Visible;
                    }
                    else if (paired.Contains(cFlip2) || cFlip2 == cFlip)
                    {
                        cFlip2 = null;
                        warningLabel.Visibility = Visibility.Visible;
                    }
                });


                if (cFlip != null && cFlip2 != null)
                { checkFlipped(cFlip, cFlip2); }

                updateTable();
            });
        }
    }
}

