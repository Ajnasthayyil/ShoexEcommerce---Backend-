using System;
using System.Collections.Generic;
using Razorpay.Api;

class Program
{
    static void Main()
    {
        try
        {
            var dict = new Dictionary<string, string>();
            dict.Add("razorpay_order_id", "order_123");
            dict.Add("razorpay_payment_id", "pay_123");
            dict.Add("razorpay_signature", "sig");
            // Also test if it needs the secret in the dict, wait Utils.verifyPaymentSignature takes 1 arg or 2?
            Utils.verifyPaymentSignature(dict);
            Console.WriteLine("No exception");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }
}
