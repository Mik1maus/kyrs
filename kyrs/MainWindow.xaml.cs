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

        // А этот метод добавь куда-нибудь ниже в классе MainWindow:
        private void BtnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно
            var employeeWindow = new EmployeeWindow(_employeesService);
            employeeWindow.ShowDialog();

            // Когда окно закроется (сотрудник добавлен), заново загружаем таблицу
            _ = LoadDataAsync();
        }

        // НОВЫЙ МЕТОД: Клик по кнопке "Видалити співробітника"
        private async void BtnDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли кто-то в таблице
            if (EmployeesDataGrid.SelectedItem is Employees selectedEmployee)
            {
                // Спрашиваем подтверждение перед удалением
                var result = MessageBox.Show($"Ви дійсно хочете видалити співробітника {selectedEmployee.LastName} {selectedEmployee.FirstName}?",
                                             "Підтвердження видалення",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                // Если пользователь нажал "Да"
                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем из базы
                    await _employeesService.DeleteEmployeeAsync(selectedEmployee.Id);

                    // Обновляем таблицу, чтобы удаленный сотрудник исчез с экрана
                    await LoadDataAsync();

                    MessageBox.Show("Співробітника успішно видалено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Если никто не выбран, выводим предупреждение
                MessageBox.Show("Будь ласка, оберіть співробітника для видалення зі списку.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // НОВЫЙ МЕТОД: Клик по кнопке "Редагувати інформацію"
        private void BtnEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли кто-то в таблице
            if (EmployeesDataGrid.SelectedItem is Employees selectedEmployee)
            {
                // Открываем второе окно, но теперь передаем в него и выбранного сотрудника!
                var employeeWindow = new EmployeeWindow(_employeesService, selectedEmployee);
                employeeWindow.ShowDialog();

                // Обновляем таблицу после закрытия окна редактирования
                _ = LoadDataAsync();
            }
            else
            {
                // Если никто не выбран, выводим предупреждение
                MessageBox.Show("Будь ласка, оберіть співробітника для редагування зі списку.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        // 4. Клик по кнопке "Статистика"
        private void BtnOpenStatistics_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно статистики и передаем туда наш сервис с данными
            var statWindow = new StatisticsWindow(_employeesService);
            statWindow.ShowDialog();
        }
    }
}