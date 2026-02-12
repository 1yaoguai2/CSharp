using InterfaceStuday;

class Program
{
    static void Main(string[] args)
    {
        //多态核心，通过接口引用指向不同的实现
        IPayment alipay = new Alipayment();
        IPayment wechat = new WeChatPayment();
        IPayment bankCard = new BankCardPayment();

        //统一调用pay方法，无需关心是哪种支付
        alipay.Pay(199.9m);
        wechat.Pay(99.3m);
        bankCard.Pay(500m);

        Console.WriteLine("--------------");

        ProcessPayment(new Alipayment(), 299.2m, "code_001");
        ProcessPayment(new WeChatPayment(), 69.2m, "code_002");
        ProcessPayment(new BankCardPayment(), 229m, "code_003");

        Console.ReadKey();

    }

    //通过支付处理方法：接口IPayment接口类型，适配所有支付方式
    static void ProcessPayment(IPayment payment,decimal amount,string orderId)
    {
        bool payResult = payment.Pay(amount);
        if (payResult)
        {
            string status = payment.QueryPaymentStatus(orderId);
            Console.WriteLine($"支付状态查询结果：{status}");
        }
    }
 
}

