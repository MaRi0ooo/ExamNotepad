using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UI.SyntaxBox;

namespace ExamNotepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SwitchLanguage("en");

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick!;
            timer.Start();
        }

        String? path;

        /* SWITCH LANGUAGE */
        private void EnglishLanguage_Click(object sender, RoutedEventArgs e)
        {
            SwitchLanguage("en");
        }
        private void PolishLanguage_Click(object sender, RoutedEventArgs e)
        {
            SwitchLanguage("pl");
        }
        private void UkrainianLanguage_Click(object sender, RoutedEventArgs e)
        {
            SwitchLanguage("ukr");
        }
        private void SwitchLanguage(string languageCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (languageCode)
            {
                case "en":
                    dict.Source = new Uri("res\\StringResources.en.xaml", UriKind.Relative);
                    break;
                case "ukr":
                    dict.Source = new Uri("res\\StringResources.ukr.xaml", UriKind.Relative);
                    break;
                case "pl":
                    dict.Source = new Uri("res\\StringResources.pl.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("res\\StringResources.en.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }



        /* Menu File Buttons */
        private void NewButtonMenu_Click(object sender, RoutedEventArgs e) => textBox.Clear();
        private void OpenButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|C files (*.c)|*.c|C++ files (*.cpp)|*.cpp|C# files (*.cs)|*.cs|JavaScript files (*.js)|*.js|HTML files (*.html)|*.html|CSS files (*.css)|*.css",
                FilterIndex = 1,
                ValidateNames = true,
                Multiselect = false,
                AddExtension = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using StreamReader streamReader = new StreamReader(openFileDialog.FileName);
                    path = openFileDialog.FileName;
                    Task<String> text = streamReader.ReadToEndAsync();
                    textBox.Text = text.Result;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        ex.Message,
                        "Message",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
        private async void SaveButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(path))
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|C files (*.c)|*.c|C++ files (*.cpp)|*.cpp|C# files (*.cs)|*.cs",
                    FilterIndex = 1,
                    ValidateNames = true
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        path = saveFileDialog.FileName;
                        using StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName);
                        await streamWriter.WriteLineAsync(textBox.Text);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(
                            ex.Message,
                            "Message",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }
        private async void SaveAsButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|C files (*.c)|*.c|C++ files (*.cpp)|*.cpp|C# files (*.cs)|*.cs",
                FilterIndex = 1,
                ValidateNames = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName);
                    await streamWriter.WriteLineAsync(textBox.Text);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        ex.Message,
                        "Message",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
        private void ExitButtonMenu_Click(object sender, RoutedEventArgs e) => App.Current.Shutdown();

        /* Menu Edit Buttons */
        private void TimeAndDateButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = DateTime.Now.ToString("HH:mm dd.MM.yyyy");
        }
        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            string text = textBox.SelectedText;
            text = text.Insert(0, "//");
            int index = text.IndexOf('\n', 0);
            while (index != -1)
            {
                text = text.Insert(index + 1, "//");
                index = text.IndexOf('\n', index + 1, text.Length - index - 2);
            }
            textBox.SelectedText = text;
        }
        private void UnCommentButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.SelectedText = textBox.SelectedText.Replace("//", "");
        }

        /* Menu Format Buttons */
        private void WordWrapButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.TextWrapping = (textBox.TextWrapping == TextWrapping.NoWrap) ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }
        private void FontMenuButton_Click(object sender, RoutedEventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog()
            { ShowApply = true, ShowEffects = true })
            {
                if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Debug.WriteLine(fontDialog.Font);

                    textBox.Foreground = new SolidColorBrush(Color.FromArgb(fontDialog.Color.A, fontDialog.Color.R, fontDialog.Color.G, fontDialog.Color.B));

                    textBox.FontFamily = new FontFamily(fontDialog.Font.Name);
                    textBox.FontSize = fontDialog.Font.Size * 96.0 / 72.0;
                    textBox.FontWeight = (fontDialog.Font.Bold) ? FontWeights.Bold : FontWeights.Regular;

                    TextDecorationCollection textDecorationsCollection = new TextDecorationCollection();
                    if (fontDialog.Font.Underline)
                    {
                        textDecorationsCollection.Add(TextDecorations.Underline);
                    }
                    if (fontDialog.Font.Strikeout)
                    {
                        textDecorationsCollection.Add(TextDecorations.Strikethrough);
                    }
                    textBox.TextDecorations = textDecorationsCollection;
                }
            }
        }
        private void SyntaxHighlighting_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush blackColorBrush = new SolidColorBrush(), whiteColorBrush = new SolidColorBrush();
            blackColorBrush.Color = Colors.Black;
            whiteColorBrush.Color = Colors.White;

            textBox.Background = blackColorBrush;
            if (syntaxHighlighting.IsChecked == true)
            {
                SyntaxBox.SetEnable(textBox, true);
                syntaxHighlighting.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                buttonSyntaxHighlighting.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                textBox.Foreground = blackColorBrush;
            }
            else
            {
                SyntaxBox.SetEnable(textBox, false);
                syntaxHighlighting.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                buttonSyntaxHighlighting.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                textBox.Foreground = whiteColorBrush;
            }
        }

        /* Menu Views Buttons */
        private void StatusBarButton_Click(object sender, RoutedEventArgs e)
        {
            statusBarPanel.Visibility = (statusBarPanel.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        /* Menu Help Buttons */
        private void AboutMenuButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        /* StatusBar Panel */
        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int row = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
            int col = textBox.CaretIndex - textBox.GetCharacterIndexFromLineIndex(row);
            labelCursorPosition.Text = $"Ln: {row + 1}, Ch: {col + 1}";
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            currentDate.Text = $"{DateTime.Now.ToShortTimeString()} / {DateTime.Now.ToShortDateString()}";
        }
        private void ButtonSyntaxHighlighting_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush blackColorBrush = new SolidColorBrush(), whiteColorBrush = new SolidColorBrush();
            blackColorBrush.Color = Colors.Black;
            whiteColorBrush.Color = Colors.White;

            textBox.Background = blackColorBrush;
            syntaxHighlighting.IsChecked = !syntaxHighlighting.IsChecked;
            if (syntaxHighlighting.IsChecked == true)
            {
                SyntaxBox.SetEnable(textBox, true);
                syntaxHighlighting.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                buttonSyntaxHighlighting.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                textBox.Foreground = blackColorBrush;
            }
            else
            {
                SyntaxBox.SetEnable(textBox, false);
                syntaxHighlighting.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                buttonSyntaxHighlighting.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                textBox.Foreground = whiteColorBrush;
            }
        }
    }
}
