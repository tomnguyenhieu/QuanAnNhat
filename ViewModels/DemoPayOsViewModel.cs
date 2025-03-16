using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Net.payOS;
using Net.payOS.Types;
using QuanAnNhat.Models;

namespace QuanAnNhat.ViewModels
{
    partial class DemoPayOsViewModel
    {
        const string ClientID = "1156d3a9-26e8-4d66-af7e-5f91abadee1f";
        const string APIKey = "4a1c13ae-05a7-4f6b-bfec-cb7d8d8d6fd6";
        const string CheckSumKey = "f5180b06f70251a08fb5ca53262ce8665274cd2385a845843e64d300ff2abcec";

        private PayOS payOS = new PayOS(ClientID, APIKey, CheckSumKey);
        private PaymentData paymentData { get; set; }
        private List<ItemData> itemDatas { get; set; }

        private int PaymentAmount = 0;

        public DemoPayOsViewModel()
        {
            itemDatas = new List<ItemData>()
            {
                new ItemData("Rice", 2, 1000),
                new ItemData("Udon", 2, 2000),
                new ItemData("Ramen", 5, 3000)
            };
        }

        [RelayCommand]
        public async Task ExcuteCreatePaymentLink()
        {
            try
            {
                foreach (ItemData item in itemDatas)
                {
                    PaymentAmount += item.price * item.quantity;
                }

                paymentData = new PaymentData(17, PaymentAmount, "Demo Thanh Toan", itemDatas, "http://localhost/", "http://localhost/");
               
                CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);
                Console.WriteLine(createPayment);
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = createPayment.checkoutUrl
                };
                Process.Start(processStartInfo);

            }   
            catch (Exception)
            {
                throw;
            }
        }

        [RelayCommand]
        public async Task ExcuteGetPaymentLinkInformation()
        {
            try
            {
                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation(15);
                Console.WriteLine(paymentLinkInformation);

                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
