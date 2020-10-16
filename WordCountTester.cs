using System;
using System.Collections.Generic;
namespace Lab3Q1
{
    public class WordCountTester
    {
        static int Main()
        {
          try {


              //=================================================
              // Implement your tests here. Check all the edge case scenarios.
              // Create a large list which iterates over WCTester
              //=================================================

              WCTester(line, startIdx, expectedResults);

            } catch(UnitTestException e) {
              Console.WriteLine(e);
            }

        }


        /**
         * Tests word_count for the given line and starting index
         * @param line line in which to search for words
         * @param start_idx starting index in line to search for words
         * @param expected expected answer
         * @throws UnitTestException if the test fails
         */
          static void WCTester(string line, int start_idx, int expected) {

            //=================================================
            // Implement: comparison between the expected and
            // the actual word counter results
            //=================================================

            if (result != expected) {
              throw new Lab3Q1.UnitTestException(ref line, start_idx, result, expected, String.Format("UnitTestFailed: result:{0} expected:{1}, line: {2} starting from index {3}", result, expected, line, start_idx));
            }

           }
    }
}
