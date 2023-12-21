using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;

namespace IshTaluy
{
    
    public partial class MainPage : Page
    {
        int maxFail = 7;
        int chances = 7;
        int numOfGuessedWords = 0;
        int lettersCount = 0;//letters that found while one game
        string selectedWord;
        Random rnd = new Random();// random that helps us to choose the word
        List<TextBox> SelectedLetters = new List<TextBox>();//letters selected word textBox list
        DataTable wordDataTable = new DataTable();
        string playerName;

        public MainPage(string receivedData)
        {
            InitializeComponent();
            playerName = receivedData;
            MessageBox.Show(playerName);
            lettersCount = 0;
            chances = 7;
            selectedWord = "";
            this.SubjectCbox.ItemsSource = DAL.GetDataView("Select * from subjectTbl");
            BuildBtnLetters();
        }

        //The func is selecting the word from word list
        private void SelectWord(object sender, RoutedEventArgs e)
        {
            int inx = rnd.Next(wordDataTable.Rows.Count);
            selectedWord = wordDataTable.Rows[inx]["word"].ToString();

            BuildSelectedletters();
        }

        private void SubjectCboxSelectionChanged(object sender, RoutedEventArgs e)
        {
            int subjId = (int)SubjectCbox.SelectedValue;

            string sqlStr = $"SELECT * FROM WordTbl WHERE num = {subjId}";
            wordDataTable = DAL.GetDataTable(sqlStr);

            SelectWord(sender , e);
        }

        private void SelectNewWord(object sender, RoutedEventArgs e)
        {
            lettersCount = 0;
            chances = 7;
            selectedWord = "";
            var uri = new Uri("pack://application:,,,/Hungman/white.png");
            var bitmap = new BitmapImage(uri);
            manImage.Source = bitmap;

            SelectWord(sender, e);
            BuildBtnLetters();
        }

        //The func is building all the letters in selected word
        private void BuildSelectedletters()
        {
            var bc = new BrushConverter();
            this.theWord.Children.Clear();
            this.SelectedLetters.Clear();

            if (this.theWord == null) { return; }

            for (int i = 0; i < selectedWord.Length; i++)
            {
                Grid grid = new Grid();                
                grid.Margin = new Thickness(20, 50, 20, 50);

                TextBox textBox = new TextBox();
                textBox.TextAlignment = TextAlignment.Center;   
                textBox.Background = (Brush)bc.ConvertFrom("#C9ADA7");
                textBox.Foreground = (Brush)bc.ConvertFrom("#4A4E69");
                textBox.IsReadOnly = true;//making sure the client not going to edit what he don't need to
                grid.Children.Add(textBox);
                this.theWord.Children.Add(grid);

                SelectedLetters.Add(textBox);
            }
        }

        //The func is building all the alphebet letters 
        private void BuildBtnLetters()
        {
            var bc = new BrushConverter();
            this.lettersGrid.Children.Clear();

            if (this.lettersGrid == null) { return; }

            for(char i = 'א';i < 'ת'; i++)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(8,18,8,18);

                Button btn = new Button();
                btn.Background = (Brush)bc.ConvertFrom("#C9ADA7");
                btn.Foreground = (Brush)bc.ConvertFrom("#4A4E69");
                btn.Content = i;
                btn.Click += BtnClick;

                grid.Children.Add(btn);
                this.lettersGrid.Children.Add(grid);
            }
        }

        //The func is controling all the buttons the clicked
        private void BtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.IsEnabled = false;//turning off clicking func on the cliked btn

            char ch = char.Parse(btn.Content.ToString());//getting the letter
            if (selectedWord.Contains(ch))//checking if the guessed letter is in selected word
            {
                for(int i = selectedWord.Length - 1; i >= 0; i--)//finding all the appereance of the letter in word
                {
                    if (selectedWord[i] == ch)//appearing the word on the screen
                    {
                        int inx = selectedWord.Length - 1 - selectedWord.IndexOf(ch, i);
                        SelectedLetters[inx].Text = ch.ToString();
                        lettersCount++;//counting guessed letter
                        SelectedLetters[inx].BorderBrush = new SolidColorBrush(Colors.Transparent);
                    }
                }
            }
            else//if the letters isn't in selected word
            {
                DisplayFailImage();
            }

            if(lettersCount == selectedWord.Length)//checking if all the letters was guessed
            {
                numOfGuessedWords++;
                result.Text = "Result: " + numOfGuessedWords++.ToString();
                SelectNewWord(null, null);
            }
            else if(chances == 0)//checking if the client used all his chances
            {
                result.Text = "Result: " + numOfGuessedWords.ToString();
                MessageBox.Show("You lose");
                SelectNewWord(null, null);
            }
        }

        //The func is updating the picture of the hungman every time when the client is mistaken
        private void DisplayFailImage()
        {
            var uri = new Uri($"pack://application:,,,/Hungman/hungman0{maxFail - chances}.png");
            var bitmap = new BitmapImage(uri);

            manImage.Source = bitmap;
            chances--;
        }

        //The func is showing the word that was selected from words list
        private void ShowWord(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < selectedWord.Length; i++)
            {
                SelectedLetters[selectedWord.Length - i - 1].Text = selectedWord[i].ToString();
            }
            SelectNewWord(null,null);
        }

        //the func is creating history table page
        private void CreateHistoryPage(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HistoryPage());
            AddData(sender , e);
        }

        //the func is saving player data
        private void AddData(object sender, RoutedEventArgs e)
        {
            string str = $"INSERT INTO historyTbl(playerName, result, timee) VALUES('{playerName}', {numOfGuessedWords}, '#{DateTime.Today}#')";
            int row = DAL.ExecuteNonQuery(str);
        }
    }
}
