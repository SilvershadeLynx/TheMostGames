using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using Words_counter.Api;

namespace Words_counter
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

        private async void button_Calculate_Click(object sender, RoutedEventArgs e)
        {
            //resulting list
            List<ResultTable> result = new List<ResultTable>();
            string stringIdentifiers = StringProcessor.StringFromRichTextBox(rtb_StringIdentifiers);

            //check if all identifiers correct
            if (StringProcessor.TryHighlightIncorrectIdentifiers(rtb_StringIdentifiers))
                MessageBox.Show("Обнаружены некорректные идентификаторы", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);

            //check if identifiers are not empty
            if (stringIdentifiers == string.Empty || stringIdentifiers == null)
            {
                rtb_StringIdentifiers.Focus();
            }

            try
            {
                //call api, count words and vowels, add to table
                foreach (string value in StringProcessor.GetUniqueIdentifiersList(stringIdentifiers))
                {
                    TextStrings textString = await ConnectApi.GetInfo(value.ToString());
                    int wordsCount = StringProcessor.CountWords(textString.Text);
                    int vowelsCount = StringProcessor.CountVowels(textString.Text);
                    result.Add(new ResultTable(textString.Text, wordsCount, vowelsCount));
                }

                //send result table to grid
                dataGrid_ResultTable.ItemsSource = result;
            }
            //catch server connection exceptions
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Server connection error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //catch any other exception
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error); ;
            }
        }
    }
}
