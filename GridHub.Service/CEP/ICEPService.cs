using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_InsightWise.Service.CEP
{    public interface ICEPService
    {
        Task<AddressResponse> GetAddressbyCEP(string CEP); /*08738-240*/
    }
}
