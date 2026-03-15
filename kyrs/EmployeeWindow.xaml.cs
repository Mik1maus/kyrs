using kyrs.Models;
using kyrs.Services;
using System;
using System.Linq;
using System.Windows;

namespace kyrs
{
    public partial class EmployeeWindow : Window
    {
        private readonly EmployeesService _employeesService;
        private Employees _employeeToEdit; // Тут мы храним сотрудника, если открыли окно для редактирования

        // Конструктор теперь принимает второй параметр (он необязательный, по умолчанию равен null)
        public EmployeeWindow(EmployeesService employeesService, Employees employeeToEdit = null)
        {
            InitializeComponent();
            _employeesService = employeesService;
            _employeeToEdit = employeeToEdit;

            // Прив'язуємо події
            Loaded += EmployeeWindow_Loaded;
            BtnSave.Click += BtnSave_Click;
            BtnClear.Click += BtnClear_Click;
        }

        // При відкритті вікна завантажуємо списки відділів та посад
        private async void EmployeeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var departments = await _employeesService.GetAllDepartmentsAsync();
            var positions = await _employeesService.GetAllPositionsAsync();

            CmbDepartment.ItemsSource = departments;
            CmbPosition.ItemsSource = positions;

            // ЕСЛИ МЫ ЗАШЛИ В РЕЖИМ РЕДАКТИРОВАНИЯ
            if (_employeeToEdit != null)
            {
                Title = "Редагування співробітника";
                BtnSave.Content = "Оновити"; // Меняем текст на кнопке

                // Заполняем поля старыми данными
                TxtLastName.Text = _employeeToEdit.LastName;
                TxtFirstName.Text = _employeeToEdit.FirstName;
                TxtMiddleName.Text = _employeeToEdit.MiddleName;

                // Находим нужный отдел и должность в выпадающем списке
                CmbDepartment.SelectedItem = departments.FirstOrDefault(d => d.Id == _employeeToEdit.DepartmentId);
                CmbPosition.SelectedItem = positions.FirstOrDefault(p => p.Id == _employeeToEdit.PositionId);

                DpAge.SelectedDate = _employeeToEdit.Age;
                DpStartWork.SelectedDate = _employeeToEdit.StartWork;
                TxtMoney.Text = _employeeToEdit.Money.ToString();
            }
        }

        // Збереження або оновлення співробітника
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка, чи всі поля заповнені
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                CmbDepartment.SelectedItem == null ||
                CmbPosition.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtMoney.Text) ||
                DpAge.SelectedDate == null ||
                DpStartWork.SelectedDate == null)
            {
                MessageBox.Show("Будь ласка, заповніть всі поля!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Перевірка, чи зарплата введена цифрами
            if (!double.TryParse(TxtMoney.Text, out double money))
            {
                MessageBox.Show("Зарплата повинна бути числом!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_employeeToEdit == null)
            {
                // РЕЖИМ ДОБАВЛЕНИЯ НОВОГО
                var newEmployee = new Employees
                {
                    LastName = TxtLastName.Text,
                    FirstName = TxtFirstName.Text,
                    MiddleName = TxtMiddleName.Text,
                    DepartmentId = ((Departments)CmbDepartment.SelectedItem).Id,
                    PositionId = ((Positions)CmbPosition.SelectedItem).Id,
                    Age = DpAge.SelectedDate.Value,
                    StartWork = DpStartWork.SelectedDate.Value,
                    Money = money
                };

                await _employeesService.AddEmployeeAsync(newEmployee);
                MessageBox.Show("Співробітника успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // РЕЖИМ РЕДАКТИРОВАНИЯ СТАРОГО
                _employeeToEdit.LastName = TxtLastName.Text;
                _employeeToEdit.FirstName = TxtFirstName.Text;
                _employeeToEdit.MiddleName = TxtMiddleName.Text;
                _employeeToEdit.DepartmentId = ((Departments)CmbDepartment.SelectedItem).Id;
                _employeeToEdit.PositionId = ((Positions)CmbPosition.SelectedItem).Id;
                _employeeToEdit.Age = DpAge.SelectedDate.Value;
                _employeeToEdit.StartWork = DpStartWork.SelectedDate.Value;
                _employeeToEdit.Money = money;

                await _employeesService.UpdateEmployeeAsync(_employeeToEdit);
                MessageBox.Show("Дані співробітника успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Закриваємо вікно
            this.Close();
        }

        // Очищення форми
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtLastName.Clear();
            TxtFirstName.Clear();
            TxtMiddleName.Clear();
            CmbDepartment.SelectedIndex = -1;
            CmbPosition.SelectedIndex = -1;
            DpAge.SelectedDate = null;
            DpStartWork.SelectedDate = null;
            TxtMoney.Clear();
        }
    }
}