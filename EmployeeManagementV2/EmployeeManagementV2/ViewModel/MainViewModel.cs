using EmployeeManagementV2.Data;
using EmployeeManagementV2.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeManagementV2.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        private IEmployeeDataService _employeeDataService;
        public MainViewModel(IEmployeeDataService employeeDataService)
        {
            Employees = new ObservableCollection<Employee>();
            EmployeeDataService = employeeDataService;
        }

        public async void GetAllEmployees(string name=null)
        {
            var response = await EmployeeDataService.GetEmployees(Page,name);
            var meta = response["meta"]["pagination"].ToObject<Pagination>();
            Page = meta.Page;
            TotalPages = meta.Pages;

            Employees.Clear();
            foreach (var employee in response["data"].ToObject<List<Employee>>())
            {
                Employees.Add(employee);
            }
        }

        public async Task<HttpResponseMessage> AddEmployee(Employee e)
        {
            return await EmployeeDataService.AddEmployee(e);
        }
        public async Task<HttpResponseMessage> UpdateEmployee(Employee e)
        {
            return await EmployeeDataService.UpdateEmployee(e);
        }

        public async Task<HttpResponseMessage> DeleteEmployee(string Id)
        {
            return await EmployeeDataService.DeleteEmployee(Id);
        }

        public ObservableCollection<Employee> Employees { get; set; }
        private Employee _selectedEmployee;

        private int _page;

        public int Page
        {
            get { return _page; }
            set
            {
                if (value != _page)
                {
                    _page = value;
                    OnPropertyChanged("Page");
                }
            }
        }


        private int _totalpages;

        public int TotalPages
        {
            get { return _totalpages; }
            set
            {
                if (value != _totalpages)
                {
                    _totalpages = value;
                    OnPropertyChanged("TotalPages");
                }
            }
        }


        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public IEmployeeDataService EmployeeDataService { get => _employeeDataService; set => _employeeDataService = value; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Status
    {
        Active,
        Inactive
    }
}


