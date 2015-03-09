# Safesation
=====

Safesation is a small library for safe (exceptionless) and unified (single paradigm) type conversions 
based on an option/maybe type extended with a set of useful operations.
The name "Safesation" reflects the idea of safe (reliable and exceptionless) programming.

## Type Conversions

The main purpose of this library is to unify a wide range 
of simple type conversion methods provided by .NET framework:
* assignments and implicit operators
  * a = b
* explicit operators
  * (int)short.MaxValue
* constructors
  * new Uri(string)
  * new Something(string, CultureInfo)
* static Parse methods
  * int.Parse(string)
  * int.Parse(string, IFormatProvider)
  * int.Parse(string, NumberStyles)
  * int.Parse(string, NumberStyles, IFormatProvider)
  * DateTime.Parse(string, IFormatProvider, DateTimeStyles)
* static ParseExact methods
  * DateTime.ParseExact(string, string, IFormatProvider)
  * DateTime.ParseExact(string, string, IFormatProvider, DateTimeStyles)
* static TryParse methods
  * int.TryParse(string, out int)
  * DateTime.TryParse(string, IFormatProvider, DateTimeStyles, out DateTime)
* static TryParseExact methods
  * DateTime.TryParse(string, string, IFormatProvider, DateTimeStyles, out DateTime)
* instance To* methods
  * object.ToString()
  * int.ToString(string)
  * int.ToBoolean(IFormatProvider)
  * DateTime.ToString(string, IFormatProvider)
* TypeConverter
  * TypeConverter.ConvertFrom(ITypeDescriptorContext, CultureInfo, object)
  * TypeConverter.ConvertTo(ITypeDescriptorContext, CultureInfo, Type, object)

### Some examples:
```csharp
"http://test.org/path.file.ext".Safe().To<System.Uri>(); // will invoke new Uri
"True".Safe().To<bool>(); // will invoke bool.TryParse
"12/31/9999".Safe().To<DateTime>(CultureInfo.InvariantCulture, "d"); // will invoke DateTime.TryParseExact
```

Additional goals achieved were:
* introduce handy (fluent) syntax
* discover available conversion methods automatically, on the fly
* provide decent performance (avoid excessive boxing and reflection)
* supress obvious exceptions (replace exceptional states with default values)
  * but retain ability to catch exceptions when needed (weave exception handling delegates into call chain)
* keep optional parametrization for various conversion methods
  * formatting strings
  * IFormatProvider
  * DateTimeStyles, NumberStyles, TimeSpanStyles

### How it works?
Reflection is used to discover available conversion methods between two types.
If more than one method is able to perform a conversion, the winner will be choosen
with help of the ConversionWays enum (upper is better):
  * Assignment
  * TypeConverter
  * Constructor
  * Operator
  * StaticTryMethod
  * StaticMethod
  * InstanceMethod
Standardizing wrapping of converter method is performed dynamically via code emitter.
Then dynamic wrapper is cached within thread-safe Dictionary clone 
which implements blocking-writes/non-blocking-reads paradigm.

### Option Type
... to be done ...

## Detailed Example

```csharp
var result = value
    .Safe() // 1
    .To<int>() // 2
    .Check(
        check: result => result > 0) // 3
    .Match(
        some: result => { }, // 4
        none: () => { }) // 5
    .Switch(
        @switch => @switch
            .Case(result => result % 2 == 0, "even") // 6
            .Case(result => result % 2 != 0, "odd")) // 7
    .Catch(
        exception => { }) // 8
    .Otherwise("error"); // 9
```
1. Safely wrap some value with 'optional' type;
2. Safely convert wrapped value to int (no exception will be thrown);
3. Safely verify whether converted value satisfies a 'check' predicate (no exception will be thrown even if the check delegate throws);
4. Safely invoke a 'some' delegate when both conversion and validation are successfull (no exception will be thrown even if the 'some' delegate throws);
5. Safely invoke a 'none' delegate when either conversion or validation are unsuccessfull (no exception will be thrown even if the 'none' delegate throws);
6. Safely switch to first case when the result is divisible by 2;
7. Safely switch to second case when the result is not divisible by 2;
8. Safely invoke a 'catch' delegate when an exception was thrown or value itself is an exception object (no exception will be thrown even if the 'catch' delegate throws);
9. Safely return final result when all is well, or default value ("error") otherwise.

## Features

* Safe programming ideology.
* Unified conversion and validation.
* Simplified exception handling.
* Extended pattern matching.