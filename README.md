
TestDataGenerator
=================

Generate dummy data which can be used for testing, You provide it with a pattern which you want to generate and it will create 
random data to match that pattern.

## Symbols
The pattern is as follows:
- `\.` - A single random upper-case, lower-case letter or number.
- `\w` - A single random upper-case or lower-case letter.
- `\L` - A single random upper-case Letter.
- `\l` - A single random lower-case letter.
- `\V` - A single random upper-case Vowel.
- `\v` - A single random lower-case vowel.
- `\C` - A single random upper-case Consonant.
- `\c` - A single random lower-case consonant.
- `\D` - A single random number, 0-9.
- `\d` - A single random number, 1-9.

#### For example:
- Individual symbols can be repeated a specific number of times using the syntax `\L{10}` which will generate 10 upper case letters.
- Individual symbols can be repeated a random number of times using the syntax `\L{10,20}` which will generate between 10 and 20 upper case letters.
- 1 or more Symbols can be combined into patterns by wrapping them in parenthesis e.g. `(\*\L\D)`.
- Patterns can be repeated a specific number of times using the syntax `(\L\D){10}` which will generate 10 repeated letter-number pairs e.g. 'G3D3G4Z4T6A3I4Q6E6S3'.
- Patterns can be repeated a random number of times using the syntax `(\L\D){10,20}` which will generate between 10 and 20 repeated letter-number pairs e.g. 'K1R3V0S2L8L5S9Y7U8T2H6U2S7M9J9R4Y2V2M9K4'.

## Placeholders
The 'GenerateFromTemplate' method allows you to provide a string containing placeholders values, these placeholders are 
comprised of 1 or more symbols representing the desired output characters.  Note that placeholders are wrapped in double 
parenthesis e.g. `Hi ((\L\v\l\v))` where `((\L\v\l\v))` is the placeholder pattern containing the symbols `\L\v\l\v`.

#### For example
- `'\<<This is a \L\L string>>'` will produce something similar to 'This is a GD string'.
- `'\<<This is a \D{19} string>>'` will produce something similar to 'This is a 3758656838906733615 string'.

## Commandline Tool
You can use the `tdg.exe` application to generate test data from the command line.  You can provide templates directly from the command line or from a file and 
the tool also supports exporting the generated output to either the command line or another file.

#### Example usage:
- Generate 100 SSN like values and output to console window
  - `tdg -t '\<<\D\d\d-\d\d-\d\d\d\d>>' -c 100`
- Generate 100 strings with random name like values and output to file 
  - `tdg -t 'Hi there \<<\L\v\l\v \L\v\l\l\v\v\l\l\v>> how are you doing?' -c 100 -o C:\test1.txt`
  - Produces 100 items like 'Sune Aildoevke'.
- Combine several placeholders to produce more complicated content
  - `tdg -t 'Hi there \<<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing?  Your SSN is \<<Ddd-dd-dddd>>.' -c 100` 
  - Produces 100 items like 'Hi there \<<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing?  Your SSN is \<<Ddd-dd-dddd>>.'
- Single repeating symbols using the following syntax
  - `tdg -t 'Letters \<<\L{20}>> and Numbers \<<\D{12}>>' -c 100`
  - Produces 100 items like 'Letters ZDIASXCYHNOULWPBZCII and Numbers 852855213055'
- Repeating patterns containing multiple letters or numbers of random length.
  - `tdg -t 'MQCOM'` - Will generate 5 random upper-case characters. e.g. 'HRTZP'
  - `tdg -t 'LJ9KP7VC6FN0KM8NM1QX5GA0HV7BP6KO3HC8WD9OI2JH0KF6QY8MC0AE3YD7YU4ZV9UD3AR3'`  - Will generate 24 repeating letter-letter-number values e.g. 'GC5HX7ZX5EK6XY4GG9MX0EQ5UJ4BQ0GQ2MG0NT2RY2ET3WC8NO9LN7HB4AM5FY2MX7JK6LS4'
- Variable length data can be generated also
  - `tdg -t 'VYVZWEYGPOBEVCNEE'` - Will generate a string containing between 10 and 20 characters of random value e.g. 'MGTLAXRVPNRFCVKU'
  - `tdg -t 'Letters \<<\L{2,20}>> and Numbers \<<\D{2,12}>>' -c 100` produces items like 'VMCPVOGPJFICRVG and Numbers 8228399'

## Profiling results
Profiling results:
- 1000 instances of the following template generated in 295 milliseconds.
  - `\<<\L{1}\D{1}\L{2}\D{2}\L{4}\D{4}\L{8}\D{8}\L{16}\D{16}\L{32}\D{32}\L{64}\D{64}\L{128}\D{128}\L{256}\D{256}\L{512}\D{512}\L{1024}\D{1024}>>`
- 1000 instances of the following template generated in 5 milliseconds.
  - `\<<\L{50}>>`
- 1000 instances of the following template generated in 5 milliseconds.
  - `\<<\L{50,51}>>`

##Examples
Executing the following `tdg -t '\<<Letters \w{2,20} and Numbers \D{2,12}\n>>'` produces the following output:
```
Letters bEwEvyn and Numbers 0376096648\n
```

See the unit tests for other examples.

