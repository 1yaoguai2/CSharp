using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InheritedAndComposition
{
    public class InheritedTest
    {
        public class Animal
        {
            public string Name { get; set; }
            public Animal(String nameStr)
            {
                Name = nameStr;
            }
            public virtual void MakeSound()
            {
                Console.WriteLine($"Animal - {Name} Make Sound!");
            }
        }

        public class Cat : Animal
        {
            public Cat(string nameStr) : base(nameStr)
            {
                
            }

            public override void MakeSound()
            {
                Console.WriteLine($"Cat - {Name} Make Sound!");
            }
        }
    }
}
