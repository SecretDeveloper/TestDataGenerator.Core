TestDataGenerator
=================

This is a very basic library which can be used to create data of varying types. 
The main purpose is to generate dummy data which can be used for testing.

You provide it with a pattern which you want to generate and it will create 
random data to match that pattern.

The pattern is as follows:
- * - An uppercase or lowercase letter or number.
- L - An uppercase Letter.
- l - A lowercase letter.
- V - An uppercase Vowel.
- v - A lowercase vowel.
- C - An uppercase Consonant.
- c - A lowercase consonant.
- X - Any number, 0-9.
- x - Any number, 1-9.

Patterns can be repeated a specific number of times using the format "[pattern]{repeat}".  For example:
- [L]{5} - Will generate 5 random uppercase characters.
- [LLX]{24}  - Will generate 24 repeating letter-letter-number values.

The 'Process' method allows you to provide a string containing ((placeholders)) which will be replaced with generated values.  
For example "This is a ((LL)) string" will produce something similar to "This is a AQ string" where 'AQ' are randomly generated.

Profiling results:
- It takes 981ms to generate 1000 strings that match the following large pattern:
- "[L]{1}[X]{1}[L]{2}[X]{2}[L]{4}[X]{4}[L]{8}[X]{8}[L]{16}[X]{16}[L]{32}[X]{32}[L]{64}[X]{64}[L]{128}[X]{128}[L]{256}[X]{256}[L]{512}[X]{512}[L]{1024}[X]{1024}"

See the unit tests for other examples.