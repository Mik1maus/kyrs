using kyrs.Models;
using kyrs.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace kyrs
{
    public partial class MainWindow : Window
    {
        private readonly EmployeesService _employeesService;

        // В конструктор мы передаем наш сервис для работы с БД
        public MainWindow(EmployeesService employeesService)
        {
            InitializeComponent();
            _employeesService = employeesService;

            // Подписываемся на событие загрузки окна (чтобы сразу вывести данные)
            Loaded += MainWindow_Loaded;

            // Подписываемся на событие выбора строчки в таблице
            EmployeesDataGrid.SelectionChanged += EmployeesDataGrid_SelectionChanged;
        }

        // Метод срабатывает один раз при открытии программы
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        // Вытягиваем данные из БД и кидаем в таблицу
        private async Task LoadDataAsync()
        {
            var employees = await _employeesService.GetAllEmployeesAsync();
            EmployeesDataGrid.ItemsSource = employees;
        }

        // Метод срабатывает каждый раз, когда ты кликаешь на новую строчку в таблице
        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, что действительно выбран сотрудник
            if (EmployeesDataGrid.SelectedItem is Employees selectedEmployee)
            {
                // Заполняем Label на панели справа
                LblLastName.Content = selectedEmployee.LastName;
                LblFirstName.Content = selectedEmployee.FirstName;
                LblMiddleName.Content = selectedEmployee.MiddleName;

                // Проверка на null для связанных таблиц (на случай если отдел или посада удалены)
                LblDepartment.Content = selectedEmployee.Department?.Name ?? "Не вказано";
                LblPosition.Content = selectedEmployee.Position?.Name ?? "Не вказано";

                // Форматируем дату, чтобы выводилось без времени (только день.месяц.год)
                LblAge.Content = selectedEmployee.Age.ToString("dd.MM.yyyy");
                LblStartWork.Content = selectedEmployee.StartWork.ToString("dd.MM.yyyy");

                // Форматируем зарплату (добавит знак валюты)
                LblMoney.Content = selectedEmployee.Money.ToString("C");
            }
            else
            {
                // Если выделение снято, очищаем панель
                LblLastName.Content = "-";
                LblFirstName.Content = "-";
                LblMiddleName.Content = "-";
                LblDepartment.Content = "-";
                LblPosition.Content = "-";
                LblAge.Content = "-";
                LblStartWork.Content = "-";
                LblMoney.Content = "-";
            }
        }
    }
}