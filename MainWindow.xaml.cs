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

namespace IshTaluy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int maxFail = 7;
        int chances = 7;
        int numOfGuessedWords = 0;
        int lettersCount = 0;//letters that found while one game
        string selectedWord;
        Random rnd = new Random();// random that helps us to choose the word
        List<Button> btnObj = new List<Button>();//letters alphbet buttons list
        List<TextBox> txtObj = new List<TextBox>();//letters selected word buttons list
        List<char> letters = new List<char>()
        { 'א','ב','ג','ד','ה','ו','ז','ח','ט','י','כ','ל','מ','נ','ס','ע','פ','צ','ק','ר','ש','ת'};
        List<string> wordsList = new List<string>()
        { "שלום", "לא", "אני"};

        public MainWindow()
        {
            InitializeComponent();
            lettersCount = 0;
            chances = 7;
            selectedWord = "";
            //There're all the func that are runnig at the beggining the game ti build all the needed fields
            SelectWord(null, null);
            BuildSelectedletters();
            BuildBtnLetters();
        }

        //The func is selecting the word from word list
        private void SelectWord(object sender, RoutedEventArgs e)
        {
            List<string> words = new List<string>(wordsList);
            int inx = rnd.Next(words.Count());
            selectedWord = words[inx];

            if (selectedWord.Contains(' '))
            {
                lettersCount++;
                txtObj[selectedWord.IndexOf(' ')].BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

           ChangingEndLetters();
        }

        private void ChangingEndLetters()
        {
            if (selectedWord.Contains('ם'))
            {
                selectedWord = selectedWord.Replace('ם', 'מ');
            }
            if (selectedWord.Contains('ף'))
            {
                selectedWord = selectedWord.Replace('ף', 'פ');
            }
            if (selectedWord.Contains('ך'))
            {
                selectedWord = selectedWord.Replace('ך', 'כ');
            }
            if (selectedWord.Contains('ץ'))
            {
                selectedWord = selectedWord.Replace('ץ', 'צ');
            }
            if (selectedWord.Contains('ן'))
            {
                selectedWord = selectedWord.Replace('ן', 'נ');
            }
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
            BuildSelectedletters();
            BuildBtnLetters();
        }

        //The func is building all the letters in selected word
        private void BuildSelectedletters()
        {
            var bc = new BrushConverter();
            this.theWord.Children.Clear();
            this.txtObj.Clear();

            if (this.theWord == null) { return; }

            for (int i = selectedWord.Length-1; i >= 0;i--)
            {
                Grid grid = new Grid();                
                grid.Margin = new Thickness(70,20,70,20);

                TextBox textBox = new TextBox();
                textBox.TextAlignment = TextAlignment.Center;   
                textBox.Background = (Brush)bc.ConvertFrom("#C9ADA7");
                textBox.Foreground = (Brush)bc.ConvertFrom("#4A4E69");
                textBox.IsReadOnly = true;//making sure the client not going to edit what he don't need to
                grid.Children.Add(textBox);
                this.theWord.Children.Add(grid);

                txtObj.Add(textBox);
            }
        }

        //The func is building all the alphebet letters 
        private void BuildBtnLetters()
        {
            var bc = new BrushConverter();
            this.lettersGrid.Children.Clear();

            if (this.lettersGrid == null) { return; }

            for (int i = 0; i < letters.Count; i++)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(8,1,8,1);

                Button btn = new Button();
                btn.Background = (Brush)bc.ConvertFrom("#C9ADA7");
                btn.Foreground = (Brush)bc.ConvertFrom("#4A4E69");
                btn.Content = letters[i].ToString();
                btn.Click += BtnClick;

                grid.Children.Add(btn);
                this.lettersGrid.Children.Add(grid);

                btnObj.Add(btn);
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
                        txtObj[inx].Text = ch.ToString();
                        lettersCount++;//counting guessed letter
                        txtObj[inx].BorderBrush = new SolidColorBrush(Colors.Transparent);
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
                txtObj[selectedWord.Length - i - 1].Text = selectedWord[i].ToString();
            }

            result.Text = numOfGuessedWords.ToString();
            SelectNewWord(null,null);
        }
    }
}
