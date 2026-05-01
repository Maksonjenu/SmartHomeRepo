using SmartHome.Core.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartView
{
    /// <summary>
    /// Логика взаимодействия для SimpleDemo.xaml
    /// </summary>
    public partial class SimpleDemo : Window
    {

        // 1. Создаем голый HttpClient прямо тут (хорошо, что хотя бы статический)
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000/api/") // Порт API
        };

        public SimpleDemo()
        {
            InitializeComponent();
        }

        // 2. Обработчик клика кнопки
        private async void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApartmentsList.ItemsSource = null;


                LoadBtn.IsEnabled = false; // Блокируем кнопку, чтобы не накликали
                LoadBtn.Content = "Грузим...";


                var response = await _http.GetAsync("apartments");
                response.EnsureSuccessStatusCode(); // Проверка на 200 OK
                var jsonString = await response.Content.ReadAsStringAsync(); // Читаем текст

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Вот она, волшебная кнопка!
                };

                var apartments = JsonSerializer.Deserialize<List<ApartmentDto>>(jsonString, options); // Парсим


                // 3. Скачиваем данные напрямую из API
                //var apartments = await _http.GetFromJsonAsync<List<ApartmentDto>>("apartments");

                // 4. ЖЕСТКО пихаем данные напрямую в элемент управления (ListBox)
                ApartmentsList.ItemsSource = apartments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Бракоделы", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadBtn.IsEnabled = true;
                LoadBtn.Content = "Скачать квартиры";
            }
        }
    }

}
