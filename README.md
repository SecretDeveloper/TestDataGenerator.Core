

# ![TestDataGenerator.Core](https://raw.githubusercontent.com/SecretDeveloper/TestDataGenerator.Core/master/icon.png) TestDataGenerator.Core
A library that can be used to generate data for testing, templating and other uses.  You provide it with a pattern containing symbols defining the output you want to produce and it will create random data to match that pattern.

[<img src="https://ci.appveyor.com/api/projects/status/p3ctf74q1brhklim?svg=true">](https://ci.appveyor.com/project/SecretDeveloper/testdatagenerator)
[<img src="https://img.shields.io/nuget/dt/TestDataGenerator.Core.svg">](https://www.nuget.org/packages/TestDataGenerator.Core/)

## Features
- Easy to use syntax.
- Regex-like expressions, familiar to many.
- Support for simple patterns or advanced multi pattern templates. 
- Random generation which can be seeded.
- Support for Named Patterns making reuse easy.
- Ability to add your own named pattern collection files.
- Fully featured library.
- 100% code coverage!

## Pattern Syntax
### Pattern Composition
If you are familiar with Regular Expressions then most of the syntax used will be familiar but there are significant differences in place given that regex is used to match a string against a pattern.  The generator instead uses simple patterns of symbols to produce strings, because of the difference in usage the syntaxes cannot match up entirely.  Patterns define what the generated values will be and can be composed using text and symbols.  Sections of the pattern can be repeated a specific number of times (they can also be repeated a random number of times by providing a min and max).  Patterns can also include alternate items that will be randomly selected, helping to produce relatively complicated outputs. 

### Symbols (Character Classes)
The following symbols are shorthand tokens which you can use in your generation patterns.  They follow most of the [Perl/Tcl](http://en.wikipedia.org/wiki/Regular_expression#Character_classes) shorthand classifications but because our focus is on text production rather than searching/matching we have extended things a little with a few more shorthand items.

|Symbol|Description|Represented characters|
|------|-----------|-------|
|`\.`|A single random character of any type.|abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789 .,;:\"'!&?£$€$%^<>{}[]()*\\+-=@#_\|~/ and space|
|`\w`|A single random upper-case character, lower-case character, number or underscore.|abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789 or _|
|`\W`|A single random non AlphaNumeric, non Whitespace character|.,;:\"'!&?£$€$%^<>{}[]()*\\+-=@#_\|~/|
|`\a`|A single random upper-case character or lower-case character.|abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ|
|`\s`|A whitespace character|SPACE TAB NEWLINE CARRAIGERETURN VERTICALTAB LINEFEED|
|`\d`|A single random number|0-9|
|`\D`|A single random non number character.|abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ .,;:\"'!&?£$€$%^<>{}[]()*\\+-=@#_\|~/ or [SPACE]|
|`\l`|A single random lower-case letter.|abcdefghijklmnopqrstuvwxyz|
|`\L`|A single random upper-case Letter.|ABCDEFGHIJKLMNOPQRSTUVWXYZ|
|`\S`|A single random non-whitespace character.|abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789 .,;:\"'!&?£$€$%^<>{}[]()*\\+-=@#_\|~/|
|`\V`|A single random upper-case Vowel.|AEIOU|
|`\v`|A single random lower-case vowel.|aeiou|
|`\C`|A single random upper-case Consonant.|BCDFGHJKLMNPQRSTVWXYZ|
|`\c`|A single random lower-case consonant.|bcdfghjklmnpqrstvwxyz|
|`\n`|A newline character.|[NEWLINE]|
|`\t`|A tab character.|[TAB]|

### Groups
Groups can contain multiple Symbols or characters and allows us to treat them as a single unit.  Their usage becomes apparent when using them with *repetitions* or *alternations*.  Groups are surrounded in () brackets.
#### Simple Groups

|Group|Description|Example|
|----|------|-----|
|`(\d)`|A number between 0 and 9.|'2'|
|`(ABC\d)`|'ABC' followed by a number between 0 and 9.|ABC1|
|`\L(\d)\L`|A upper-case letter, three digits between 0 and 9 and another upper-case letter.|M7R|

#### Alternations
When a group contains alternations the patterns are divided by the | character, during processing one of the alternated patterns is selected at random. 
Groups can contain several individual symbols or groups of symbols and randomly alternate between them when generating the output value.

**Alternatives must be contained within a Group**

|Alternation|Description|Example|
|----|------|-----|
|`(\L\L|\d\d)`|Either two upper-case letters OR two numbers.|'JY'|
|`(\L\L|\d\d|[AEIOU]|[100-120])`|Either two upper-case letters OR two digits OR an upper-case vowel OR a number between 100 and 120.|'QA'|
|`(\C|)`|Either a upper-case Consonant or nothing.||

### Ranges
Ranges can contain multiple characters or ranges of characters but no symbols (the items defined within the range will be what is used, no special symbols are allowed).  The item to be produced is selected at random.
#### Character ranges

|Character Range|Description|Example|
|----|------|-----|
|`[abc]`|Either 'a','b' or 'c'.|'a'|
|`[a-z]`|A single lower-case letter between a and z.|'z'|
|`[A-Z]`|A single upper-case letter between A and Z.|'R'|
|`[A-D]`|A single upper-case letter between A and D.|'C'|
|`[A-Da-z]`|A single character between A and D or between a and z.|'s'|

#### Numeric ranges

|Numeric Range|Description|Example|
|----|------|-----|
|`[1-5]`|A number between 1 and 5.|'1'|
|`[100-500]`|A number between 100 and 500.|'253'|
|`[1.25-5]`|A decimal number between 1.25 and 5. The scope or number of decimal places is taken from the first number defined (1.25 in this case) and the produced value will have the same number of decimal places.|'1.89'|

### Special Functions
Certain special functions are supported

#### Anagrams
You can generate an anagram from a set of characters by using the following syntax:  
`[abc]{:anagram}`  
The produced string will contain each of the provided characters (used only once) in a random order.

### Named Parameters
A named pattern is surrounded with @ characters and links to a predefined pattern loaded from a file. The `default.tdg-patterns` file located in the same directory as the tdg executable file contains a list of named patterns which can be used in other patterns you write.  For example to generate you could write something like `([1-9]\d\d-\d\d-\d\d\d\d)` or you can use the named parameter in the file `(@misc_ssn@)` to a similar value.  You can add more patterns to the file as you wish.  Named patterns can also include other named patterns if you so wish.  

Take a look at the `@address_us_type1@` pattern in the file as an example of a compound pattern than uses other patterns to produce an output e.g. '8579 Shady Lane, Howard County, Montana, 52963'

### Repetition
Repetition is a powerful feature allowing for complicated data production. A Symbol, Group or Range can be repeated a set or random number of times by using the following syntax.

#### Quantity Repetition

|Repitition syntax|Description|Example|
|----|------|-----|
|`\d{5}`|Will generate 5 number characters.|'46642'|
|`(\L\d\L){5}`|Will generate 5 upper-case letter, number, upper-case letter items.|'J5EJ6KE9CZ4QB1F'|
|`[ABC]{5}`|Will generate 5 items where each item will be either 'A','B' or 'C'.|'AABAC'|

#### Random Quantity

|Random syntax|Description|Example|
|----|------|-----|
|`\d{5,10}`|Will generate between 5 and 10 number characters.|'7211494791'|
|`(\L\d\L){5,10}`|Will generate  between 5 and 10 upper-case letter, number, upper-case letter items.|'I0MJ3JA6VW7CA8FW4HD1J'|
|`[ABC]{5,10}`|Will generate  between 5 and 10 items where each item will be either 'A','B' or 'C'.|'BBAAAAB'|


## Template Syntax

### Templates
You can create template documents that contain multiple pattern syntax placeholders.  TDG will then replace these placeholders with their appropriate random content.  Templates can also include a *configuration* directive that can be used to control the content production.  

### Placeholders
You can place your symbols and patterns within placeholders which will be replaced with the generated values.  These placeholders contain 1 or more symbols representing the desired output characters.  
Within a template all text not within a placeholder is treated as normal text with no special handling taking place.  Patterns that need to be randomly generated should be placed inside '<< >>' tokens e.g. 'This is a template <<\L\L>>'  produces 'This is a template TE'. You can also escape placeholders by placing a '\' before them which will prevent them from being processed.  To place a '\' before a placeholder in the generated content you need to place 2 backslashes before the placeholder. 

### Configuration
You can supply configuration values to the generator either as an additional parameter within the api or you can include it within the template string itself by wrapping it within '<# #>' tokens.  Configuration directives must appear as the first item with a template or else they will be ignored and removed.

#### Configuration items
- Seed - A Seed value to be used for the random data generation. By providing the same Seed value to a template, the same output values will be produced each time the template is handled.  When a Seed is not provided a random seed is used instead producing different results each time.
- PatternFiles - Files containing Named Patterns can be included here and the patterns they contain will be available during processing. An absolute or relative path can be provided.  For relative paths the current directory and any sub directory called "tdg-patterns" will be searched when trying to locate the file.

#### Configuration directive syntax
- `<#{"seed":1}#>` - This configuration provides the Seed value for the random data generation.
- `<#{ "patternfiles":["c:\mypatterns.tdg-patterns"], "seed":300 } #>` - This configuration provides a Seed value of 300 and a Pattern file that will include Named Patterns to be used by the template.

### Checkout the Examples folder for some further items and ideas.

