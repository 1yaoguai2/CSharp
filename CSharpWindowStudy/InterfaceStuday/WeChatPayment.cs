using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStuday
{
    //微信支付
    public class WeChatPayment : IPayment
    {
        public bool Pay(decimal amount)
        {
            Console.WriteLine($"【微信支付】正在出库{amount:C}的支付请求...");
            bool isSuccess = true;
            Console.WriteLine("微信支付成功!");
            return isSuccess;
        }

        public string QueryPaymentStatus(string orderId)
        {
            Console.WriteLine($"【微信】查询订单{orderId}的支付状态...");
            return $"订单{orderId}：微信支付-已经完成";
        }
    }
}
