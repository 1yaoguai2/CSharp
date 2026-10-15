using TypeConversion;

Console.WriteLine("Hello, World!");
//***********************************************
//隐式转换，无需添加代码，编译器会自动完成转换
//将范围小的数据类型赋值给范围大的数据类型时，数据不会丢失
byte b = 1;
// Implicit conversion from byte to int
int i = b; 
// Implicit conversion from int to long
long l = i; 
float f = 1.2f;
// Implicit conversion from float to double
double d = f;

//***********************************************
//显式转换，需要添加代码，编译器不会自动完成转换
//将范围大的数据类型赋值给范围小的数据类型时，可能会丢失数据
//不同数据类型之间的转换，可能会丢失数据
double doubleValue = 1.22;
// Explicit conversion from double to int
int intValue = (int)doubleValue;
// Explicit conversion from double to float
float floatValue = (float)doubleValue;
// Explicit conversion from int to byte
string stringValue = intValue.ToString();

//***********************************************
//ToString()方法将数值类型显式转换为字符串类型
// Example of converting an integer to a string using ToString() method
string str = 123.ToString();

//Parse()方法将字符串类型显式转换为数值类型
// Example of converting a string to an integer using Parse() method
string strValue = "456";
int parsedValue = int.Parse(strValue);

//***********************************************
//自定义类型转换--隐式转换和显式转换的高级用法
Fahrenheit fahrenheit = new Fahrenheit(98.6);
fahrenheit.TestConversion();

/*
 * 结果：
 * Fahrenheit: 98.6
 * Implicit conversion: 98.6 Fahrenheit is 98.6 in double.
 * Explicit conversion: 100 in double is 100 Fahrenheit.
 */