using rabbitWpf.MessageBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace rabbitWpf
{
    public partial class MainWindow : Window
    {
        MailQueueProducer producer = new("localhost", "guest", "guest", "/");

        static List<string>  avaiablePlatforms = new List<string>
            {
                "Smtp",
                "Mailgun",
                "SendGrid"
            };
        
        MailModel msg = new(avaiablePlatforms[0]);

        public MainWindow()
        {
            InitializeComponent();
            platformSelector.ItemsSource = avaiablePlatforms;
            platformSelector.SelectedIndex = 0;
        }

        public void ClearMail(object sender, RoutedEventArgs e)
        {
            recipients.Clear();
            subject.Clear();
            body.Clear();
            msg = new MailModel(platformSelector.SelectedItem.ToString());
            
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            var recipientsList = recipients.Text.Split(';');
            msg.Sender = senderAddr.Text;
            msg.Recipients = recipientsList.ToList();
            if (ValidateMailAddresses(msg.Recipients, msg.Sender))
            {
                producer.SendMessage(msg);
            }
            
        }

        private bool ValidateMailAddresses(List<string> recipients, string sender)
        {
            if (recipients.Count < 1)
            {
                MessageBox.Show($"Brak adresu odbiorcy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (sender.Length < 2 )
            {
                MessageBox.Show($"Niepoprawny adres nadawcy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            recipients.Add(sender);

            foreach (string addr in recipients)
            {
                try
                {
                    var mailAddress = new System.Net.Mail.MailAddress(addr);
                }
                catch (FormatException)
                {
                    MessageBox.Show($"Adres {addr} jest nieprawidłowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show($"Adres email jest pusty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            recipients.Remove(sender);
            return true;
        }

        private void AddAttachment(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileOk += FileDialog_FileOk;
            var result = ofd.ShowDialog();
            if (result == false) return;

            string selectedFilePath = ofd.FileName;
            Attachment attachment = new Attachment(selectedFilePath);

            using (var stream = attachment.ContentStream)
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                msg.Attachments.Add(new AttachmentData(attachment.Name, buffer));
            }
            attachmentsList.Text += attachment.Name + ("; ");

        }

        private void platform_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            msg.PlatformType = platformSelector.SelectedItem.ToString();
        }

        private void FileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e) // checks if file's size isnt too big
        {
            var fileDialog = (Microsoft.Win32.OpenFileDialog)sender;
            foreach (string fileName in fileDialog.FileNames)
            {
                var fileInfo = new System.IO.FileInfo(fileName);
                long fileSize = fileInfo.Length;

                if (!ValideFileSize(fileSize))
                {
                    e.Cancel = true;
                    return;
                }
                
            }
        }

        private bool ValideFileSize(long fileSize)
        {
            long maxFileSize = 50 * 1024 * 1024; // 50MB

            if (fileSize > maxFileSize)
            {
                MessageBox.Show("Zbyt duży rozmiar pliku.", "Błąd",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) // Moving through TextBoxes using TAB
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                int currentIndex;
                int nextIndex;
                var textBox = (TextBox)sender;
                var parent = textBox.Parent as Panel;

                if (parent != null)
                {
                    var textBoxes = parent.Children.OfType<TextBox>().ToList();

                    currentIndex = textBoxes.IndexOf(textBox);
                    nextIndex = (currentIndex + 1) % textBoxes.Count;
                    textBoxes[nextIndex].Focus();
                }
            }
        }

    }
}
