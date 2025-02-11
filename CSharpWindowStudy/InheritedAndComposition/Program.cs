using InheritedAndComposition;

Console.WriteLine("继承与组合");
//继承
InheritedTest.Animal animal = new InheritedTest.Animal("老虎");
animal.MakeSound();

InheritedTest.Cat cat = new InheritedTest.Cat("狸花猫");
cat.MakeSound();

//组合
CompositionTest.Engin engin85 = new CompositionTest.Engin(400);
engin85.Start();

CompositionTest.Engin engin89 = new CompositionTest.Engin(600);
CompositionTest.Car car = new CompositionTest.Car(engin89);
car.Start();



/*
继承与组合
Animal - 老虎 Make Sound!
Cat - 狸花猫 Make Sound!
Engin with 400 HP is Start!
Engin with 600 HP is Start!
Car is Starting!
 */