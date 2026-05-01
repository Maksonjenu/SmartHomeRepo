using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;

namespace SmartView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // Делаем провайдер публичным (на всякий случай, хотя по-хорошему всё инжектится само)
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Создаем коллекцию сервисов (как builder.Services в API)
            var services = new ServiceCollection();

            // 2. Инициализируем всю нашу "хуйню" (Сервисы, Клиенты)
            // Это УЛЬТРА-НОРМИС способ работы с HttpClient. Он сам им управляет.
            services.AddHttpClient("ApiClient", client =>
            {
                // ПОМЕНЯЙ ПОРТ НА СВОЙ!
                client.BaseAddress = new Uri("http://localhost:5000/api/");
            });

            // 3. Регистрируем ViewModel
            services.AddTransient<MainViewModel>();

            // 4. Регистрируем Главное окно
            services.AddTransient<MainWindow>();

            // 5. Собираем всё это в контейнер
            ServiceProvider = services.BuildServiceProvider();

            // 6. Достаем окно из контейнера (оно само подтянет MainViewModel) и показываем
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

    }

}
