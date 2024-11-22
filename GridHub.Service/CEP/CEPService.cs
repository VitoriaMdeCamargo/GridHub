using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_InsightWise.Service.CEP
{
    public class CEPService : ICEPService
    {
        public async Task<AddressResponse> GetAddressbyCEP(string cep)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://viacep.com.br/");

            HttpResponseMessage response = await client.GetAsync($"ws/{cep}/json/");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AddressResponse>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
