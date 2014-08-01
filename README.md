TestDataGenerator
=================

Generate dummy data which can be used for testing, You provide it with a pattern which you want to generate and it will create 
random data to match that pattern.


## Patterns
The pattern is as follows:
- `*` - An upper-case or lower-case letter or number.
- `L` - An upper-case Letter.
- `l` - A lower-case letter.
- `V` - An upper-case Vowel.
- `v` - A lower-case vowel.
- `C` - An upper-case Consonant.
- `c` - A lower-case consonant.
- `X` - Any number, 0-9.
- `x` - Any number, 1-9.

Patterns can be repeated a specific number of times using the format "[pattern]{repeat}".  

For example:
- `[L]{5}` - Will generate 5 random upper-case characters.
- `[LLX]{24}`  - Will generate 24 repeating letter-letter-number values.


## Placeholders
The 'GenerateFromTemplate' method allows you to provide a string containing ((placeholders)) which will be replaced with generated values.

For example: 
- `"This is a ((LL)) string"` will produce something similar to `"This is a AQ string"` where `'AQ'` is randomly generated.
- `"This is a (([XX]{19})) string"` will produce something similar to `"This is a 3698145258142562124 string"` where `'3698145258142562124'` is randomly generated.

## Commandline Tool
You can use the `tdg.exe` application to generate test data from the command line.  You can provide templates directly from the command line or from a file and 
the tool also supports exporting the generated output to either the command line or another file.
Example usage:
- Generate 100 SSN like values and output to console window
  - `tdg.exe -t "((Xxx-xx-xxxx))" -c 100`
- Generate 100 strings with random name like values and output to file 
  - `tdg.exe -t "Hi there ((Lvlv Lvllvvllv)) how are you doing?" -c 100 -o C:\test1.txt`


## Profiling results
Profiling results:
- It takes 981ms to generate 1000 strings that match the following large pattern:
  - `"[L]{1}[X]{1}[L]{2}[X]{2}[L]{4}[X]{4}[L]{8}[X]{8}[L]{16}[X]{16}[L]{32}[X]{32}[L]{64}[X]{64}[L]{128}[X]{128}[L]{256}[X]{256}[L]{512}[X]{512}[L]{1024}[X]{1024}"`


See the unit tests for other examples.