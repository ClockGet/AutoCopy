using AutoCopyLib;
using System;
using System.Diagnostics;

namespace Benchmark
{
    public class Address
    {
        public string ZipCode { get; set; }
    }
    public class Telephone
    {
        public string Number { get; set; }
    }

    public class Customer
    {
        public Address Address { get; set; }
        public Telephone Phone { get; set; }
        public string Memo { get; set; }
    }
    public class CustomerInfo
    {
        public string zipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Memo { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var autoCopy = AutoCopy.CreateMap<Customer, CustomerInfo>();
            autoCopy
                .ForMember(p => p.zipCode, opt => opt.MapFrom(p => p.Address.ZipCode))
                .ForMember(p => p.PhoneNumber, opt => opt.MapFrom(p => p.Phone.Number));
            autoCopy.Register();

            AutoMapper.Mapper.Initialize(cfg => 
            cfg.CreateMap<Customer, CustomerInfo>()
                .ForMember(p => p.zipCode, opt => opt.MapFrom(p => p.Address.ZipCode))
                .ForMember(p => p.PhoneNumber, opt => opt.MapFrom(p => p.Phone.Number))
            );
#if !DEBUG
            int loop = 100000;
#else
            int loop = 10;
#endif
            Clock.BenchmarkTime("hand map",() => 
            {
                Customer customer = new Customer();
                CustomerInfo customerInfo = new CustomerInfo();
                customer.Address = new Address { ZipCode = "1234567890" };
                customer.Phone = new Telephone { Number = "17791704580" };
                customer.Memo = "customer";
                ShallowCopy(customerInfo, customer);
            }, loop);
            
            Clock.BenchmarkTime("AutoCopy",() =>
            {
                Customer customer = new Customer();
                CustomerInfo customerInfo = new CustomerInfo();
                customer.Address = new Address { ZipCode = "1234567890" };
                customer.Phone = new Telephone { Number = "17791704580" };
                customer.Memo = "customer";
                autoCopy.ShallowCopy(customer, customerInfo);
            }, loop);

            Clock.BenchmarkTime("AutoMapper",() =>
            {
                Customer customer = new Customer();
                customer.Address = new Address { ZipCode = "1234567890" };
                customer.Phone = new Telephone { Number = "17791704580" };
                customer.Memo = "customer";
                var customerInfo=AutoMapper.Mapper.Map<CustomerInfo>(customer);
            }, loop);
        }
        static bool ShallowCopy(CustomerInfo customerInfo, Customer customer)
        {
            try
            {
                customerInfo = customerInfo ?? new CustomerInfo();
                customerInfo.PhoneNumber = customer.Phone.Number;
                customerInfo.zipCode = customer.Address.ZipCode;
                customerInfo.Memo = customer.Memo;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
