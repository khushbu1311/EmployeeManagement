using EmployeeManagementV2.Model;
using EmployeeManagementV2.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmployeeManagementV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private Employee _employee;
        private bool _isUpdateMode = false;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Page = 1;
            _viewModel.GetAllEmployees();

            dgEmployeeMgmt.ItemsSource = _viewModel.Employees;

            comboGender.ItemsSource = Enum.GetValues(typeof(Gender)).Cast<Gender>();
            comboGender.SelectedValue = Enum.GetValues(typeof(Gender)).Cast<Gender>().First();

            comboStatus.ItemsSource = Enum.GetValues(typeof(Status)).Cast<Status>();
            comboStatus.SelectedValue = Enum.GetValues(typeof(Status)).Cast<Status>().First();
        }

        private async void dgEmployeeMgmt_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var response =  await _viewModel.UpdateEmployee(_employee);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Row updated successfully.");
            }
            else
            {
                MessageBox.Show("Error Code" +
                response.StatusCode + "Error" + response.ReasonPhrase);
            }

        }

        private void dgEmployeeMgmt_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_isUpdateMode)
            {
                Employee TempEmp = (from emp in _viewModel.Employees
                                    where emp.Id == _employee.Id
                                    select emp).First();

                var _nameElement = dgEmployeeMgmt.Columns[1].GetCellContent(e.Row);
                if (_nameElement.GetType() == typeof(TextBox))
                {
                    var name = ((TextBox)_nameElement).Text;
                    _employee.Name = name;
                }
                var _emailElement = dgEmployeeMgmt.Columns[2].GetCellContent(e.Row);
                if (_emailElement.GetType() == typeof(TextBox))
                {
                    var email = ((TextBox)_emailElement).Text;
                    _employee.Email = email;
                }

                var _genderElement = dgEmployeeMgmt.Columns[3].GetCellContent(e.Row);
                if (_genderElement.GetType() == typeof(TextBox))
                {
                    var gender = ((TextBox)_genderElement).Text;
                    _employee.Gender = gender;
                }
                var _statusElement = dgEmployeeMgmt.Columns[4].GetCellContent(e.Row);
                if (_statusElement.GetType() == typeof(TextBox))
                {
                    var status = ((TextBox)_statusElement).Text;
                    _employee.Status = status;
                }

            }
        }

        private void dgEmployeeMgmt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _employee = dgEmployeeMgmt.SelectedItem as Employee;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            _isUpdateMode = true;
           
            dgEmployeeMgmt.Columns[1].IsReadOnly = false;
            dgEmployeeMgmt.Columns[2].IsReadOnly = false;
            dgEmployeeMgmt.Columns[3].IsReadOnly = false;
            dgEmployeeMgmt.Columns[4].IsReadOnly = false;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_employee == null)
            {
                MessageBox.Show("Cannot delete the blank row");
            }
            else
            {
                var response = await _viewModel.DeleteEmployee(_employee.Id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Row deleted successfully.");
                    _viewModel.GetAllEmployees();
                }
                else
                {
                    MessageBox.Show("Error Code" +
                    response.StatusCode + "Error" + response.ReasonPhrase);
                }
            }

        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = null;
            if (txtName.Text.Length == 0)
            {
                errorMessage = "Blank Name.Please enter correct text!!";
                txtName.Focus();
            }
            else if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                errorMessage = "Invalid Email!!";
                txtEmail.Select(0, txtEmail.Text.Length);
                txtEmail.Focus();
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                var employee = new Employee();
                employee.Name = txtName.Text;
                employee.Email = txtEmail.Text;
                employee.Gender = comboGender.Text;
                employee.Status = comboStatus.Text;

                var response = await _viewModel.AddEmployee(employee);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Row added successfully.");
                    _viewModel.GetAllEmployees();
                }
                else
                {
                    MessageBox.Show("Error Code" +
                    response.StatusCode + "Error" + response.ReasonPhrase);
                }
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Page = Convert.ToInt32(txtTotalPage.Text);
            _viewModel.GetAllEmployees();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Page = txtPage.Text == "1"? Convert.ToInt32(txtPage.Text) : (Convert.ToInt32(txtPage.Text) - 1);
            _viewModel.GetAllEmployees();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Page = Convert.ToInt32(txtPage.Text) + 1;
            _viewModel.GetAllEmployees();
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Page = 1;
            _viewModel.GetAllEmployees();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            dgEmployeeMgmt.SelectAllCells();
            dgEmployeeMgmt.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dgEmployeeMgmt);
            dgEmployeeMgmt.UnselectAllCells();
            var result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            var filePath = "C:\\EmployeeDetails.csv";
            File.AppendAllText(filePath, result, UnicodeEncoding.UTF8);

            MessageBox.Show(string.Format("Data exported at path: {0}", filePath));
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchName = txtSearch.Text;
            _viewModel.GetAllEmployees(searchName);
        }
    }
}
