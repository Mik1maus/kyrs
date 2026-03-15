using kyrs.Services;
using System.Windows;

namespace kyrs
{
    public partial class StatisticsWindow : Window
    {
        private readonly EmployeesService _employeesService;

        public StatisticsWindow(EmployeesService employeesService)
        {
            InitializeComponent();
            _employeesService = employeesService;

            // Подписываемся на событие загрузки окна
            Loaded += StatisticsWindow_Loaded;
        }

        private async void StatisticsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Вызываем метод сервиса и отдаем данные первой таблице (Відділи за кількістю)
            DgDepartments.ItemsSource = await _employeesService.GetDepartmentsWithMostEmployeesAsync();

            // 2. Вызываем метод сервиса и отдаем данные второй таблице (Топ 5 самых старых)
            DgOldest.ItemsSource = await _employeesService.GetTop5OldestEmployeesAsync();

            // 3. Вызываем метод сервиса и отдаем данные третьей таблице (Средняя зарплата)
            // Так как сервис возвращает Dictionary, у элементов в таблице будут свойства Key (название отдела) и Value (зарплата)
            DgAverageSalary.ItemsSource = await _employeesService.GetAverageSalaryByDepartmentAsync();
        }
    }
}