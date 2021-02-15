using EmployeeManagementV2.Model;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeManagementV2.Data
{
    public interface IEmployeeDataService
    {
        Task<JObject> GetEmployees(int page,string name=null);
        Task<HttpResponseMessage> AddEmployee(Employee employee);
        Task<HttpResponseMessage> UpdateEmployee(Employee employee);
        Task<HttpResponseMessage> DeleteEmployee(string Identifier);
    }
}
