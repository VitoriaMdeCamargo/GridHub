using ERP_InsightWise.Service.CEP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridHub.Test.tests.integration
{
    public class CEPServiceTest
    {
        private readonly CEPService _cepService;
        private readonly string cepValid = "08737-250";
        private readonly string cepInvalid = "00000-000";
        private readonly string logradouroExpected = "Rua Maria das Dores da Conceição";

        public CEPServiceTest()
        {
            //A - Arrange
            _cepService = new CEPService();
        }

        [Fact]
        public async Task GetAddressbyCEP_ReturnAddressResponse_WhenCEPIsValid()
        {
            //A - Action
            AddressResponse addressResponse = await _cepService.GetAddressbyCEP(cepValid);

            //A - Assert
            Assert.NotNull(addressResponse);
            Assert.Equal(logradouroExpected, addressResponse.Logradouro);

        }

        [Fact]
        public async Task GetAddressbyCEP_ReturnNull_WhenCEPIsInvalid()
        {
            //A - Action 
            AddressResponse addressResponse = await _cepService.GetAddressbyCEP(cepInvalid);

            //A - Assert
            Assert.NotNull(addressResponse);
        }
    }
}
