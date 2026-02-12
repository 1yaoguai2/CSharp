using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStuday
{
    public class BankCardPayment : IPayment
    {

        public bool Pay(decimal amount)
        {
            Console.WriteLine($"【银行卡支付】正在处理{amount:C}的支付请求...");
            Console.WriteLine("请输入银行卡密码...");
            bool isSuccess = true;
            Console.WriteLine("银行卡支付成功!");
            return isSuccess;
        }

        public string QueryPaymentStatus(string orderId)
        {
            Console.WriteLine($"【银行卡】查询订单{orderId}的支付状态...");
            return $"订单{orderId}：银行卡支付-已经完成";
        }
        
    }
}
