---
layout: lab
title:  Lab 3 - Testing and Mutexes
date:   2020-10-11 12:00:00
authors: [C. Antonio Sánchez, Ali Mousavifar]
categories: [labs, threads, multithread, shakespeare, testing]

---


# Lab 3 -- Testing and Mutexes

In this lab, we will first learn about unit testing, then will get some practice with thread synchronization using mutexes.  First we will create a method that counts the number of words in a string, and test this thoroughly to ensure reliability under a variety of circumstances.  Next, we will use this method to help us count how many words each character has to memorize in a collection of plays by Shakespeare.

Feel free to discuss approaches and solutions with your classmates, but labs are to be completed individually.  Each student is expected to be able to answer questions about the content, describe their work, and reproduce their code (or parts thereof).


## Shakespearean Word Counts

### Counting Words and Unit Tests

Consider the function:
```csharp
/**
 * Counts number of words, separated by spaces, in a line.
 * @param line string in which to count words
 * @param start_idx starting index to search for words (>= 0)
 * @return number of words in the line
 */
int word_count(ref string line, int start_idx);
```
The method specification tells us that it counts the number of words in a string, with each new word separated by a space from the previous.  The method also allows us to specify a starting index within the string to start searching for words.  Think about how you might test this function to see if it is working correctly.  What are some sample strings you think you should test it with?

Once you've thought a bit about the problem for a few minutes, start the implementation.  We will separate the declaration and the implementation of this method in a separate public class  `HelperFunctions` and write the implementation in a `WordCount` method in this class.

`HelperFunctions.cs`:
```csharp

/**
 * Counts number of words, separated by spaces, in a line.
 * @param line string in which to count words
 * @param start_idx starting index to search for words
 * @return number of words in the line
 */
int WordCount(ref string line, int start_idx){
  // YOUR IMPLEMENTATION HERE

  return 0;


}

```




Do you think you've implemented the function correctly?  Now we're going to test it.  Unit tests are usually written in a separate testing source file so that your tests don't clutter your actual production code.  We are going to manually create some unit tests in `WordCountTester` in a file called `WordCountTester.cs` and implement a custom exception class called `UnitTestException` in `UnitTestException.cs` for this test:

```csharp

// Exception class to throw for unit test failures
public class UnitTestException: Exception
    {
        private string line_;
        private int idx_;
        private int results_;
        private int expected_;

        public UnitTestException(ref string line, int idx, int results, int expected, string message) :
          base(message)
        {
            line_ = line;
            idx_ = idx;
            results_ = results;
            expected_ = expected;
        }

    }
```

```csharp  
/**
 * Tests word_count for the given line and starting index
 * @param line line in which to search for words
 * @param start_idx starting index in line to search for words
 * @param expected expected answer
 * @throws UnitTestException if the test fails
 */
static void WCTester(string line, int start_idx, int expected) {

  // TO DO
  // Call your WordCount(ref line, start_idx) method

  // if not what we expect, throw an error
  if (result != expected) {
    throw new Q1.UnitTestException(ref line, start_idx, result, expected, String.Format("UnitTestFailed: result:{0} expected:{1}, line: {2} starting from index {3}", result, expected, line, start_idx));
  }

}
```
In the same file as `WordCountTester.cs`, implement your main function; the entry point to your unit testing.

```csharp
int main() {

  try {

    // To DO:
    // YOUR TESTS HERE. Create a large list which includes the line, the // starting index and the expected result. You would want to check //// all the edge case scenarios.

    WCTester(line, startIdx, expectedResults);

  } catch(UnitTestException e) {
    Console.WriteLine(e);
  }

}
```

We start by creating an exception class called `UnitTestException` that will be `thrown` whenever one of our tests fails.  This exception stores everything we need to know about the failed test to help us debug.  In the above code, `UnitTestException` captures all the inputs to the `WordCount` function, the result, and the expected result.

We then define a *tester* function that takes the inputs along with an expected answer, calls our `WordCount` function, then throws the exception if we get something unexpected.

Finally, our main method has a `try - catch` block that will call our tester function with a variety of inputs and expected outputs.  If any tests fail, the `catch` part catches the exception and prints out information about the failed test so we can debug our code.

**Your task:** add a variety of tests.  Try to "break" your own code.  It is very rare to have a method that runs perfectly, for all inputs, on a first or second try.  You should have enough tests so that you've covered all expected *behaviours* of your method.  Think about possible partitions of the input space, as well as boundary/edge/corner cases.  In particular, think about:
- different numbers of words in a string
- different starting indices
- different lengths of strings
- extra spaces at the start, end, or between words
- ...?

Note, you must specify the main entry point you want to choose in <StartupObject><your_project_name>.WordCountTester</StartupObject> in the <your_project_name>.csproj file and Rebuild All and Run.
### Dictionaries

Dictionaries are *incredibly* useful data structures.  Conceptually, they are quite simple: they associate pairs of data together.  Think about an array or vector.  When you set `array[10] = "Steve"`, in some sense you are associating the number 10 with the name *Steve*.  You can later retrieve the name associated with number 10 using `string name = array[10]`.

Dictionay generalizes this, associating pretty much any type of data with any other type of data. A dictionary *maps* data from a *key* to a *value*.
```csharp
System.Collection.Generics
//...
Dictionary<TKey,TValue>
```
For example, we can have a map of strings to numbers:
```csharp
Dictionary<string, int> myDictionary = new Dictionary<string, int>();

```
In the above, the keys are of type `string`, and the values are of type `int`.
We can add key-value pairs directly to the dictionary using
```csharp
myDictionary.Add("Rob", 5); // Adding a dictionary
```
Keys are considered unique in a dictionary, so if you later assign
```csharp
myDictionary["Rob"] = 10; // Editing a dictionary
```
this will overwrite the original value.

You can check a dictionary to see if a particular key exists using the `.ContainsKey("Rob")` member function:
```csharp
myDictionary.ContainsKey("Rob")
```

You can iterate through all the entries in a dictionary just like for a vector or any other iterable:
```csharp
foreach( KeyValuePair<string, int> pairItem in myDictionary )
{
    Console.WriteLine("Key = {0}, Value = {1}",
        pairItem.Key, pairItem.Value);
}

```
You should find that the dictionary iterates through the keys in no particular order.

### Sorting

Each item in the dictionary is treated as a KeyValuePair structure representing a value and its key. The order in which the items are returned is undefined. In C#, there is also OrderedDictionary which respects the order of the keys (Not the value).

Create a method `SortCharactersByWordcount` in `HelperFunctions.cs` which takes the dictionary and stores the key-value pairs in a List of Tuples `List<Tuple<int, string>>` in the order of count descending.  

```csharp
public static List<Tuple<int, string>> SortCharactersByWordcount(Dictionary<string, int> wordcount)
{
  // Implement sorting by word count here
}

```
In the implementation, you may iterate over the items of the dictionary and use `OrderByDescending(key => key.Value)` function.

For the purpose of this lab, it is sufficient to get the list sorted by value only. However, there may be scenarios where the *values* are the same by the *keys* are not sorted, e.g. {"Rob", 10} comes before {"Bob", 10} but it is preferred for the order to be reversed. One way to do this is to initially store the *key*-*value* pairs in the orderedDictionary which orders them by *key*. Then when the content of the orderedDictionary is being retrieved in `SortCharactersByWordcount` and a list of `Tuple<int, string>` is created, then the sorting list respects *value* (integer) descending and *key* (string) alphabetically.  

### Counting Words in Shakespeare

Shakespeare's characters talk a lot in his plays.  For example, Othello says approximately 12000 words, and Romeo about 10000.  That's a lot of memorization.  Which characters say the most in all of Shakespeare's plays?

We already have a word count function that we can make use of.  Now all we need are some files to use it on.

[Project Gutenburg](https://www.gutenberg.org/) offers free digital copies of many literary works for which the copyright has either expired or never existed.  It's a great resource for reading many of the classics.  In the GitHub `data` folder [here](https://github.com/alimousavifar/lab3_public/tree/master/data) you will find all of Shakespeare's works.  Note that not all of them are plays though.

If you open one of the plays, such as Romeo and Juliet, you will notice that *most* lines containing dialogue either begin with two spaces or with four spaces.  If the line starts with two, the line contains the name (or short-form) of the character speaking, followed by a period, followed by the character's dialogue.  If the line starts with four spaces, it is usually a continuation of the previous character's lines.  This approach is by no means perfect, but should be sufficient for us to determine which Shakespearean character is the most verbose.

We have provided much of the code for you in this exercise.  We will map *Character* → *Word Count* in a `Dictionary` or ``orderedDictionary``.  To populate the map, we will parse a selection of Shakespeare's plays, extract the speakers of each line of dialogue, count the number of words, and update the counts.  This should all be done in a thread-safe multithreaded way.

```csharp

/**
 * Checks if the line specifies a character's dialogue, returning
 * the index of the start of the dialogue.  If the
 * line specifies a new character is speaking, then extracts the
 * character's name.
 *
 * Assumptions: (doesn't have to be perfect)
 *     Line that starts with exactly two spaces has
 *       CHARACTER. <dialogue>
 *     Line that starts with exactly four spaces
 *       continues the dialogue of previous character
 *
 * @param line line to check
 * @param character extracted character name if new character,
 *        otherwise leaves character unmodified
 * @return index of start of dialogue if a dialogue line,
 *      -1 if not a dialogue line
 */
int IsDialogueLine(string line, ref string character) {

  // new character
  if (line.Length >= 3 && line[0] == ' '
      && line[1] == ' ' && line[2] != ' ')
  {
      // extract character name

      int start_idx = 2;
      int end_idx = 3;
      while (end_idx <= line.Length && line[end_idx - 1] != '.')
      {
          ++end_idx;
      }

      // no name found
      if (end_idx >= line.Length)
      {
          return 0;
      }

      // extract character's name
      character = line.Substring(start_idx, end_idx - start_idx - 1);
      return end_idx;
  }

  // previous character
  if (line.Length >= 5 && line[0] == ' '
      && line[1] == ' ' && line[2] == ' '
      && line[3] == ' ' && line[4] != ' ')
  {
      // continuation
      return 4;
  }

  return 0;
}
```
Note ```IsDialogueLine``` method is implemented already. Although it is not perfect, it is a good start. Feel free to debug if you see issues. But fixing the bugs for ```IsDialogueLine``` is not required for this lab. 

```csharp
/**
 * Reads a file to count the number of words each actor speaks.
 *
 * @param filename file to open
 * @param mutex mutex for protected access to the shared wcounts map
 * @param wcounts a shared map from character -> word count
 */
 public static void CountCharacterWords(string filename,
                          Mutex mutex,
                          Dictionary<string, int> wcounts)
  {

  //===============================================
  //  IMPLEMENT THREAD SAFETY IN THIS METHOD
  //===============================================

    string line;  // for storing each line read from the file
    string character = "";  // empty character to start
    System.IO.StreamReader file = new System.IO.StreamReader(filename);

    while ((line = file.ReadLine()) != null)
    {
      //=================================================
      // YOUR JOB TO ADD WORD COUNT INFORMATION TO MAP
      //=================================================

        // Is the line a dialogueLine?
        //    If yes, get the index and the character name.
        //      if index > 0 and character not empty
        //        get the word counts
        //          if the key exists, update the word counts
        //          else add a new key-value to the dictionary
        //    reset the character   

      }
      // Close the file
  }
```

```csharp
int main() {

  // map and mutex for thread safety
  Mutex mutex = new Mutex();
  Dictionary<string, int> wordcountSingleThread = new Dictionary<string, int>();

  Dictionary<string, int> wordcountMultiThread = new Dictionary<string, int>();

  var filenames = new List<string> {
        "data/shakespeare_antony_cleopatra.txt"
        ,
        "data/shakespeare_hamlet.txt"
        ,
        "data/shakespeare_julius_caesar.txt",
        "data/shakespeare_king_lear.txt",
        "data/shakespeare_macbeth.txt",
        "data/shakespeare_merchant_of_venice.txt",
        "data/shakespeare_midsummer_nights_dream.txt",
        "data/shakespeare_much_ado.txt",
        "data/shakespeare_othello.txt",
        "data/shakespeare_romeo_and_juliet.txt",
   };

  //=============================================================
  // YOUR IMPLEMENTATION HERE TO COUNT WORDS IN SINGLE AND MULTIPLE THREADS
  //=============================================================



  Console.WriteLine( "Done");
  return 0;
}
```
The code leaves several sections blank.  It is your job to fill in these sections.  In particular, you will need to

1. Modify `CountCharacterWords` to insert word counts into the *Character* → *Word Count* map in a thread-safe way. Note that the pseudocode is provided as comments in this function. Note that this function should be implemented in `HelperFunctions.cs` file.
2. Implement the `SortCharactersByWordcount` for sorting characters by number of spoken words, descending. Note that this function should be implemented in `HelperFunctions.cs` file.
3. Write multithreading code in the `main` method to parse each file and populate the word-count map.
4. Implement `PrintListofTuples` method. Note that this function should be implemented in `HelperFunctions.cs` file.

Note, that each C# project can only have one entry point (i.e. main method), so after implementing the Program.cs you must specify the main entry point you want to choose in <StartupObject><your_project_name>.Program</StartupObject> in the <your_project_name>.csproj file and Rebuild All and Run.
#### Accessing the Data

The data files for this exercise need to be someplace where your program can find them.  One way is to hard-code the path in the list of filenames.  However, this will not be very portable between machines.

The easier way is to copy the data over to the *working directory* of your executable (in my mac machine my executables are in <project_dir>/bin/Debug/netcoreapp<version>/*), then use relative path names in code to load the files. By default this is set to the project directory, so as long as the `data` folder is in the project directory itself, it can be found using relative pathnames.

#### Thread Safety

If two threads try to modify the *Character* → *Word Count* dictionary concurrently, the behaviour is undefined.  Sometimes this will throw an error immediately, sometimes everything will *appear* to work correctly -- as in it won't crash -- but your results will seem to vary slightly every time you run your program.  If this happens, you'll often get an error when the program exits.

If the program doesn't crash right away, these concurrent modification errors can be very difficult to track down and fix.  If ever your program crashes when it closes, it's because you have overrun some memory somewhere.  It was possibly overwriting some other variables in the process, but didn't do enough damage to trigger an error at the time.  You see the error at the end as the program tries to clean up its allocated memory and notices... hey, this vector/map/array was supposed to be done by now, but seems to keep going past the end!

You need to protect access to your dictionary if it is shared between threads by applying *mutual exclusion*.  For this, you have access to the standard library implementation `System.Threading`, which provides `mutex`.

As you are protecting access to your shared dictionary, think about critical section *localization*.  What is the smallest section of code that needs to be protected?  The narrower you can make this region, the more benefit you will gain by having multiple threads executing in parallel.  If you lock your mutex at the beginning of a method and unlock it at the end, then calls to your method will behave as if it was called sequentially.

### Questions

- How can you test your program without needing to manually go through all the dialogue in Shakespeare's plays?
- Has writing this code multithreaded helped in any way? Show some numbers in your observations. If your answer is no, under what conditions can multithreading help?
- As written, if a character in one play has the same name as a character in another -- e.g. *King* -- it will treat them as the same and artificially increase the word count.  How can you modify your code to treat them as separate, but still store all characters in the single dictionary (you do not need to implement this... just think about how you would do it)?
