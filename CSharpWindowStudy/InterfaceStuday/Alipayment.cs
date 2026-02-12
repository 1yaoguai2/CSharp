using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStuday
{
    //支付宝支付
    public class Alipayment : IPayment
    {
        public bool Pay(decimal amount)
        {
            Console.WriteLine($"【支付宝支付】正在处理{amount:C}的支付请求...");
            // 模拟支付逻辑
            Random random = new Random();
            bool isSuccess = random.Next(0, 2) == 1; //模拟支付成功/失败
            Console.WriteLine(isSuccess ? "支付宝支付成功" : "支付宝支付失败");
            return isSuccess;
        }

        public string QueryPaymentStatus(string orderId)
        {
            Console.WriteLine($"【支付宝】查询订单{orderId}的支付状态...");
            return $"订单{orderId}:支付宝支付-已经完成";
        }
    }
}
