using EmployeeManagementV2.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EmployeeManagementV2.Data
{
    public class EmployeeDataService: IEmployeeDataService
    {
        private Uri _baseAddress;
        private string _apiToken = "Bearer fa114107311259f5f33e70a5d85de34a2499b4401da069af0b1d835cd5ec0d56";
        public EmployeeDataService()
        {
            _baseAddress = new Uri("https://gorest.co.in/public-api");
        }

        public async Task<JObject> GetEmployees(int page,string name=null)
        {
            using (HttpClient client = new HttpClient())
            {
                SetHeaders(client);
                var response = await client.GetAsync(_baseAddress + "/users?page="+ page + "&name="+name);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseBody);
            }
        }

        public async Task<HttpResponseMessage> AddEmployee(Employee employee)
        {
            using (HttpClient client = new HttpClient())
            {
                SetHeaders(client);
                var response = await client.PostAsJsonAsync(_baseAddress + "/users", employee);
                return response;
            }
        }

        public async Task<HttpResponseMessage> UpdateEmployee(Employee employee)
        {
            using (HttpClient client = new HttpClient())
            {
                SetHeaders(client);
                var response =  await client.PutAsJsonAsync<Employee>(_baseAddress + "/users/" + employee.Id, employee);
                return response;
            }
        }

        public async Task<HttpResponseMessage> DeleteEmployee(string Identifier)
        {
            using (HttpClient client = new HttpClient())
            {
                SetHeaders(client);
                var response = await client.DeleteAsync(_baseAddress + "/users/" + Identifier);
                return response;
            }
        }

        private void SetHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", _apiToken);
        }
    }
}
