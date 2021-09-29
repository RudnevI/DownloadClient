using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DownloadClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private readonly Regex _uriRegEx = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");

        private readonly Regex _fileRegEx = new Regex(@"^[\w\-. ]+$");

        private readonly Regex _proxyRegEx = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b:\d{2,5}");
        

        public MainWindow()
        {
            InitializeComponent();
            DownloadButton.IsEnabled = false;
        }

        private bool IsUriValid()
        {
            return _uriRegEx.IsMatch(URITextBox.Text);

        }

        private bool DoesFilePathExist()
        {
            return Directory.Exists(FilePathTextBox.Text);
        }

        private bool IsFilenameValid()
        {
            return _fileRegEx.IsMatch(FilenameTextBox.Text) && IsNoFileWithSameName(); 
        }

        private bool IsNoFileWithSameName()
        {
            return !Directory.GetFiles(FilePathTextBox.Text).Any(filename => filename.Equals(FilenameTextBox.Text));
        }

        private bool IsProxyValid()
        {
            return _proxyRegEx.IsMatch(ProxyTextBox.Text);
        }
        private void URITextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
             HighlightValidity ();
        }

        private void HighlightValidity()
        {
            

            bool validity = IsUriValid() && DoesFilePathExist() && IsFilenameValid();
            
            List<TextBox>textBoxes = new List<TextBox>
            {
                URITextBox, FilePathTextBox, FilenameTextBox
            };
            if (ProxyCheckBox.IsChecked ?? false)
            {
                validity = validity && IsProxyValid();
                textBoxes.Add(ProxyTextBox);
            }
            DownloadButton.IsEnabled = validity;

            Brush highlightBrush = (validity) ? (Brushes.LightGreen) : (Brushes.IndianRed);

            foreach(TextBox textBox in textBoxes)
            {
                textBox.BorderBrush = highlightBrush;
            }

        }

   

   
       

        private void Client_DownloadProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar progress = (ProgressBar)MainPanel.FindName("Progress");

            progress.Value = e.ProgressPercentage;

            
            
        }

        private string GetFilePath()
        {
            return FilePathTextBox.Text;
        }

        private string GetFileName()
        {
            return FilenameTextBox.Text;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {

            string currentUriValue = URITextBox.Text;

            ProgressBar progress = new ProgressBar { Margin = new Thickness(0, 10, 0, 0)};
            MainPanel.Children.Add(progress);

            try
            {
                WebClient client = new WebClient();





                client.DownloadProgressChanged +=
                    new DownloadProgressChangedEventHandler((object downloadSender, DownloadProgressChangedEventArgs downloadEventsArgs) =>
                    {
                        progress.Value = downloadEventsArgs.ProgressPercentage;

                    });
                
                if(ProxyCheckBox.IsChecked ?? false)
                {
                    client.Proxy = new WebProxy(ProxyTextBox.Text);
                }

                
                client.DownloadFileCompleted += new AsyncCompletedEventHandler((object completedSender, AsyncCompletedEventArgs completedArgs) =>
                {
                    progress.Visibility = Visibility.Collapsed;

                });

                client.DownloadFileAsync(new Uri(currentUriValue), 
                    new StringBuilder().Append(GetFilePath()).Append("\\").Append(GetFileName()).ToString()
                    
                    );
                
                client.Dispose();
     
                

            }

            catch(ArgumentNullException)
            {
                MessageBox.Show(ErrorMessages.ArgumentNullMessage);
            }
            
            catch(WebException)
            {
                MessageBox.Show(ErrorMessages.WebErrorMessage);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(ErrorMessages.InvalidOperationMessage);
            }




        }

        private void FilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightValidity();
        }

        private void FilenameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightValidity();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ProxyTextBox.IsEnabled = ProxyCheckBox.IsChecked ?? false;
            HighlightValidity();
        }
    }
}
