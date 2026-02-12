using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStuday
{
   public interface IPayment
    {
        bool Pay(decimal amount);

        string QueryPaymentStatus(string orderId);
    }
}
