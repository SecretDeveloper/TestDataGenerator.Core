
#TestDataGenerator
A library and command line tool that can be used to generate data for testing or other uses.  You provide it with a pattern containing symbols defining the output you want to produce and it will create random data to match that pattern.

## Quick Start with examples

### Placeholders
When using the commandline tool place all patterns and symbols inside '\<<' tokens **without the backslash** e.g. 'Generate some Letters \<<\L\L>>'. 

### Symbols
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
- `\n` - A newline character.

### Groups
- `(\d){5}` - Five digits between 1 and 9.
- `\L(\d){3}\L` - A upper-case letter, five digits between 1 and 9 and another upper-case letter.
- `(.[100-101]){3}` - Three items, each will include a dot '.' and either 100 or 101 e.g. *'.101.100.100'*

### Ranges
- `[a-z]` - A single lower-case letter between a and z.
- `[a-z]{5}` - Five lower-case letter between a and z.
- `[a-z]{5,10}` - Between five and ten lower-case letter between a and z.
- `[A-Z]` - A single upper-case letter between Z and Z.
- `[1-5]` - A number between 1 and 5.
- `[100-500]` - A number between 100 and 500.
- `[1-28]/[1-12]/[1960-2013]` - A date value between 1960 and 2013.

### Alternations
**Alternatives must be contained within a Group**
- `(\L\L|\d\d)` - Either two upper-case letters OR two numbers.
- `(\L\L|\d\d|[AEIOU]|[100-120])` - Either two upper-case letters OR two digits OR an upper-case vowel OR a number between 100 and 120.

### CommandLine tool
You can use the `tdg.exe` application to generate test data from the command line.  It can handle provided templates directly from the command line or from a file. The tool also supports exporting the generated output to either the command line or another file.

### Parameters:
- `-t, --template`    The template to use when producing data
- `-f, --file`        The path of the input file.
- `-o, --output`      The path of the output file.
- `-c, --count`       (Default: 1) The number of items to produce.
- `-v, --verbose`     (Default: False) Verbose output including debug and performance information.
- `--help`            Display the help screen.

### Examples
- Single repeating symbols using the following syntax
  - `tdg -t 'Letters \<<\L{20}>> and Numbers \<<\D{12}>>'`
  - Produces items like *'Letters EEENSZEUHVHELDHYMPIY and Numbers 489050141711'*.
- Repeating patterns containing multiple letters or numbers of random length.
  - `tdg -t '\<<(\L){5}>>'` - Will generate 5 random upper-case characters. e.g. *'DWFIQ'*
  - `tdg -t '\<<(\L\L\D){24}>>'`  - Will generate 24 repeating letter-letter-number values e.g. *'AF2QQ7PO3QU0KD3XA5JG1AQ7SP4RE0OX9HM3DH8CJ4DL3LU6VF1HV0MZ5KX0GY3IC8BC6YU7'*
- Variable length data can be generated also
  - `tdg -t '\<<(\L){10,20}>>'` - Will generate a string containing between 10 and 20 characters of random value e.g. *'HXRXFKGVVVZXGCWUNV'*
  - `tdg -t 'Letters \<<\L{2,20}>> and Numbers \<<\D{2,12}>>'` produces items like *'Letters KUWKCUJNJSAXKJN and Numbers 28030624'*
- Input can contain several placeholders.
  - `tdg -t 'Hi there \<<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing?  Your SSN is \<<\D\d\d-\d\d-\d\d\d\d>>.' -c 100` 
  - Produces 100 items like *'Hi there Ciufe Damp how are you doing?  Your SSN is 089-92-8965.'*
- Generate 100 SSN like values and output to console window.
  - `tdg -t '\<<\D\d\d-\d\d-\d\d\d\d>>' -c 100`
  - Produces 100 items like *'756-94-5776'*.
- Generate 100 strings with random name like values and output to file.
  - `tdg -t 'Hi there \<<\L\v\l\v \L\v\l\l\v\v\l\l\v>> how are you doing?' -c 100 -o C:\test1.txt`
  - Produces 100 items like *'Lese Nodkuoiai'*.
- `tdg -t '\<<Letters \w{2,20} and Numbers \D{2,12}\n>>'` produces the following output: *'Letters MlZA and Numbers 706
'*

## More Information

### Placeholders
You can place your symbols and patterns within placeholders which will be replaced with the generated values.  These placeholders contain 1 or more symbols representing the desired output characters.  Note that placeholders are wrapped in double angle brackets `\<<PATTERN>>` without the backslash.
*Please note I am supplying a backslash at the start of all placeholder examples so that they appear as examples correctly.  You do not need to add these.*

### Pattern Composition
If you are familiar with Regular Expressions then most of the syntax used will be familiar but there are significant differences in place given that regex is used to match a string against a pattern.  The generator instead uses simple patterns opf symbols to produce strings, because of the difference in usage the syntaxes cannot match up entirely.  Patterns define what the generated values will be and can be composed using text and symbols.  Sections of the pattern can be repeated a specific number of times (they can also be repeated a random number of times by providing a min and max).  Patterns can also include alternate items that will be randomly selected, helping to produce relatively complicated outputs. 

`\<<\L\v\l\v>>` is a placeholder containing the pattern of symbols `\L\v\l\v`.

### Symbol Repetition
Individual symbols can be repeated by a supplying a repeat section immediately after the symbol.  For example `\L{5}` will produce 5 upper case letters.  You can also add some randomness to the mix by supplying a range: `\L{min,max}`.  The pattern `\L{1,100}` will produce between 1 and 100 upper case letters. Here's one *'VXBHOHSQJPLZDXSI'*

### Symbol Grouping
Individual symbols can be grouped together using parenthesis characters.  When grouped together they can be repeated using the same repeat syntax.  `(\l\D){5}` will produce something like *'f7s8d2m2k9'*.
You can also include the random range syntax from above.

### Alternating Symbols and Groups
Patterns can contain several individual symbols or groups of symbols and randomly alternate between them when generating the output value.  `\<<\C|\c{10}|\V\V\V|(\v\v){2,3}>>` will produce either a single upper-case consonant, 10 lower-case consonants, 3 upper-case vowels or between 10 and 15 lower-case vowels.  Which one gets outputed is randomly selected when processing the pattern.

### Other patterns:
- `'\<<This is a \L\L string>>'` will produce something similar to *'This is a TL string'*.
- `'\<<This is a \D{19} string>>'` will produce something similar to *'This is a 9694818444511194016 string'*.
- Individual symbols can be repeated a specific number of times using the syntax `\L{10}` which will generate 10 upper case letters.
- Individual symbols can be repeated a random number of times using the syntax `\L{10,20}` which will generate between 10 and 20 upper case letters.
- 1 or more Symbols can be combined into patterns by wrapping them in parenthesis e.g. `(\*\L\D)`.
- Patterns can be repeated a specific number of times using the syntax `(\L\D){10}` which will generate 10 repeated letter-number pairs e.g. *'I2E8A3S0Q3V8R5H6T6Y1'*.
- Patterns can be repeated a random number of times using the syntax `(\L\D){10,20}` which will generate between 10 and 20 repeated letter-number pairs e.g. *'B2W7E6S9G0E2H4D7S9D1D2C6N9O8N2V1Y6H0I0V7'*.


### Profiling results
*These timings are taken from unit tests making direct API calls, the command line tool will have higher times as it has additional IO work to output the values to screen or file.  Should still be fast.*
- 1000 instances of the following template generated in 172 milliseconds.
  - `\<<\L{1}\D{1}\L{2}\D{2}\L{4}\D{4}\L{8}\D{8}\L{16}\D{16}\L{32}\D{32}\L{64}\D{64}\L{128}\D{128}\L{256}\D{256}\L{512}\D{512}\L{1024}\D{1024}>>`
- 1000 instances of the following template generated in 3 milliseconds.
  - `\<<\L{50}>>`
- 1000 instances of the following template generated in 3 milliseconds.
  - `\<<\L{50,51}>>`

## This README was generated using the generator.  See the unit tests for other examples.
