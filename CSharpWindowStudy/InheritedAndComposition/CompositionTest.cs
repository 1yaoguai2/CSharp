using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritedAndComposition
{
    public class CompositionTest
    {
        public class Engin
        {
            public int HorsePower { get; set; }
            public Engin(int value)
            {
                HorsePower = value;
            }
            public void Start()
            {
                Console.WriteLine($"Engin with {HorsePower} HP is Start!");
            }
        }

        public class Car
        {
            private Engin engin;
            public Car(Engin engin)
            {
                this.engin = engin;
            }

            public void Start()
            {
                engin.Start();
                Console.WriteLine("Car is Starting!");
            }
        }
    }
}
