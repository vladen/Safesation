Safesation
=====

Safesation is a small library for safe (exceptionless) and unified (single paradigm) type conversions 
based on an option/maybe type extended with a set of additional useful operations.

## Example

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