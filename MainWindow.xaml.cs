using Microsoft.Win32;
using System.Windows;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Windows.Documents;
using LandmarksAI.WPF.Learning.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LandmarksAI.WPF.Learning
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

        private void selectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(filePath));

                MakePredictionAsync(filePath);
            }

        }

        private async void MakePredictionAsync(string filePath)
        {
            string url = "https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/4e5af8da-fa69-445e-b439-fff5ad8f6279/classify/iterations/Iteration1/image";
            string prediction_key = "6b28297e7e0d4bf9b43e945d041f3bfe";
            string content_type = "application/octet-stream";
            var file = File.ReadAllBytes(filePath);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).Predictions;

                    predictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}
