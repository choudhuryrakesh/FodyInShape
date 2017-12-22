using Newtonsoft.Json;
using System;
using System.Diagnostics;
using WeavableAssembly.DTO;

namespace WeavableAssembly
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string seperator = "***************";
            try
            {
                var ssn = "123-12-1234";

                var test = new Person
                {
                    SSN = ssn,
                    SSNCustom = "321#21#4321",
                    Name = "James Bond",
                };

                var newtonSoft = JsonConvert.SerializeObject(test);
                Console.WriteLine($"{ seperator}{ Environment.NewLine}Newtonsoft:{ Environment.NewLine}{ newtonSoft}");

                Console.WriteLine("Input Json to deserialize:");
                var toDeserialize = Console.ReadLine();

                var testDeserialized = JsonConvert.DeserializeObject<Person>(toDeserialize);
                Console.WriteLine($"{ seperator}{ Environment.NewLine}Newtonsoft:{ Environment.NewLine}{ testDeserialized.SSN}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }
    }
}