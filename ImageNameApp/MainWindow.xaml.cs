using ImageNameApp.Stuff;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace ImageNameApp
{
    public partial class MainWindow : Window
    {
        public DataCards _dataCards = new DataCards() { Cards = new List<Card>() };
        private Card _selectedElement = new Card();
        private int _selectedIdElement = -1;
        private BitmapImage _selectedImage = new BitmapImage();
        public MainWindow()
        {
            InitializeComponent();
            HideChangePanel();
        }
        private void HideChangePanel()
        {
            LabelText.Visibility = Visibility.Hidden;
            TextName.Visibility = Visibility.Hidden;
            imgView.Visibility = Visibility.Hidden;
            RemoveElement.Visibility = Visibility.Hidden;
            SaveElement.Visibility = Visibility.Hidden;
            CancelElement.Visibility = Visibility.Hidden;
            SelectImage.Visibility = Visibility.Hidden;
        }
        private void ShowChangePanel()
        {
            LabelText.Visibility = Visibility.Visible;
            TextName.Visibility = Visibility.Visible;
            imgView.Visibility = Visibility.Visible;
            RemoveElement.Visibility = Visibility.Visible;
            SaveElement.Visibility = Visibility.Visible;
            CancelElement.Visibility = Visibility.Visible;
            SelectImage.Visibility = Visibility.Visible;
        }
        string ImageToBase64(BitmapSource bitmap)
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        BitmapSource Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            try
            {
                MemoryStream strmImg = new MemoryStream(bytes);
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.StreamSource = strmImg;
                myBitmapImage.DecodePixelWidth = 200;
                myBitmapImage.EndInit();
                return myBitmapImage;
            }
            catch
            {
            }
            return null;
        }
        private void newSourse_Click(object sender, RoutedEventArgs e)
        {
            _dataCards = new DataCards() { Cards = new List<Card>() };
            UpdateList();
            HideChangePanel();

        }
        private async void openSourse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == false)
                return;

            var path = openFileDialog.FileName;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    _dataCards = await JsonSerializer.DeserializeAsync<DataCards>(fs);

                }
                UpdateList();
            }
            catch
            {
                MessageBox.Show("Not Correct File", "Exception");
                ListElements.Items.Clear();
            }


        }

        private async void saveSourse_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == false)
                return;
            var path = saveFileDialog.FileName;

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync<DataCards>(fs, _dataCards);
                }

        }
        private void NewElement_Click(object sender, RoutedEventArgs e)
        {
            var card = new Card() { Name = "New Element" };
            _dataCards.Cards.Add(card);
            ListElements.Items.Add(card.Name);
        }
        private void UpdateList()
        {
            ListElements.Items.Clear();
            _dataCards.Cards.ForEach(c => ListElements.Items.Add(c.Name));
        }

        private void ListElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListElements.SelectedIndex >= 0)
            {
                _selectedIdElement = ListElements.SelectedIndex;
                _selectedElement = _dataCards.Cards[_selectedIdElement];

                if (_selectedElement.Image != null)
                {
                    var img = Base64ToImage(_selectedElement.Image);
                    imgView.Source = img;
                    _selectedImage = (BitmapImage)img;
                }
                else
                {
                    imgView.Source = null;
                }


                TextName.Text = _selectedElement.Name;
                ShowChangePanel();

            }
            else
            {
                _selectedIdElement = -1;
                HideChangePanel();
            }

        }


        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Image (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == false)
                return;

            var path = openFileDialog.FileName;
            try
            {
                var bImg = new BitmapImage(new Uri(path, UriKind.Absolute));
                imgView.Source = bImg;
                _selectedImage = bImg;
            }
            catch
            {
                MessageBox.Show("Not Supported type of file", "ERROR");
            }

        }

        private void removeElement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataCards.Cards.Remove(_selectedElement);
                ListElements.Items.Remove(ListElements.Items[ListElements.SelectedIndex]);

       
              
            }
            catch { }
            UpdateList();
        }

        private void saveElement_Click(object sender, RoutedEventArgs e)
        {

            _selectedElement.Name = TextName.Text;
            try
            {
                if(imgView.Source != null)
                _selectedElement.Image = ImageToBase64(_selectedImage);
            } catch {
                MessageBox.Show("Not Correct Image", "ERROR");
            }

            ListElements.Items[ListElements.SelectedIndex] = TextName.Text;
        }

        private void cancelElement_Click(object sender, RoutedEventArgs e)
        {
            TextName.Text = _selectedElement.Name;
            if(_selectedElement.Image != null)
            imgView.Source = Base64ToImage(_selectedElement.Image);
           
        }

    }
}
