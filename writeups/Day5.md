**⚠ THIS WRITEUP CONTAINS SPOILERS ON DAY 5 OF ADVENT OF CODE 2023, INCLUDING MY SOLUTION METHODS. While I'm okay with it being copied (would like credit if so, please!), I don't want to spoil anyone unwillingly. DO NOT READ if you want to solve it on your own first!**

Also, this post uses excerpts of the actual examples from [the site](https://adventofcode.com/2023/day/5).

*This writeup was last edited December 17, 2023, as I noticed some typographical mistakes I made in my code.*

---

The code I'm about to explain relied very heavily on a class I made called an `AVLTreeDictionary`. I created that class because I'm a Java transplant, and Java has its [`TreeMap`](https://docs.oracle.com/javase/8/docs/api/java/util/TreeMap.html) class that includes methods for retrieving the next-nearest entry from any given key — `lower`, `floor`, `ceiling`, and `higher` (`Key`/`Entry` depending on what exactly you want). C#'s [`SortedDictionary`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?view=net-8.0) doesn't have those methods, and I couldn't just say goodbye to them when I've made a lot of things that need them. So while it's not the best-documented code (I'm working on writing it still), it's available [here](https://github.com/StevenH237/CSharp.Nixill/blob/master/CSharp.Nixill/src/Collections/AVLTree/AVLTreeDictionary.cs) and the "Nixill" package is available on NuGet [here](https://www.nuget.org/packages/Nixill/).

I'm also using an enclosing program on my Advent of Code so that I don't have to write certain boilerplate for every individual day; this code handles running tests automatically on all provided example input and only passing me an answer to the real input if those tests pass.

---

# Part 1
Here's what the challenge for Part 1 entailed:

You get a list of seeds, such as `seeds: 79 14 55 13`, and a list of seven maps that explain how to convert one value to the next, each separated by a blank line, such as

```
seed-to-soil map:
50 98 2
52 50 48
```

Each rule (one line of numbers) is given as the lowest output number of the rule, then the lowest input number of the rule, then the length of the rule's range. For example, the first rule above, `50 98 2`, means that an input of 98 becomes an output of 50. The rule has a length of 2, which manes that only two distinct inputs apply to it (98 or 99), which both have the same offset (-48, to become 50 and 51 respectively).

For an input number not referenced by any rules, the output should be the same unchanged. For example, 10 above would stay 10.

In other words, inputs would be mapped to outputs according to the following rules. All values not otherwise noted are inclusive, and the values in **bold** are those actually given.

| Input starts at | + Range size | Input ends at (exclusive) | Output starts at | Output - Input = Offset | Output ends at (exclusive) |
| :-------------: | :----------: | :-----------------------: | :--------------: | :---------------------: | :------------------------: |
|        0        |     50†      |            50†            |        0         |            0            |             50             |
|     **50**      |    **48**    |            98             |      **52**      |           +2            |            100             |
|     **98**      |    **2**     |            100            |      **50**      |           -48           |             52             |
|       100       |      ∞†      |            ∞†             |       100        |            0            |             ∞†             |

†The ranges that start at 0 and 100 weren't explicitly given, but were created by a *lack* of rules that cover those inputs.

The object of the puzzle is to put all the initial seeds through all seven maps, in order, and then find the lowest output that results from them. With just the portion of the example I gave you, that'd be the 13 we put in (because it wasn't changed by tne one map), but if you were to use the full example from the site, it would come out to 35.

## How I solved it
My method for solving it revolved around a class I called `D5Map`, backed by an `AVLTreeDictionary<int, int>`.

The keys in that dictionary correspond to the start of a rule's input range, including the "0 offset" "ghost rules" made by the areas not affected by a rule. Where a rule ends, another rule starts, even if that other rule is a "ghost rule" - so if the end of one rule isn't already the start of another rule, then another "ghost rule" has to be added at the end.

I decided to make the values be the offsets of that rule (as seen in the fifth column of the table above) so that you only need to get the value of the rule applicable at a given input and then add that value to the input to get the output.

1. The `D5Map` is initialized to just {0 → 0}.
2. The first rule is `50 98 2`, which means input starts at 98, output starts at 50, and the size is 2. `50 - 98 = -48`, which makes -48 the offset; and `98 + 2 = 100`, which makes 100 the end of input. Therefore, the map gains the entries {98 → -48} and {100 → 0}.
3. The second rule is `52 50 48`, which means that input starts at 50, output starts at 52, and the size is 48. `52 - 50 = 2`, which makes 2 the offset; and `50 + 48 = 98`, which makes 98 the end of input. Therefore, the map gains the entry {50 → 2} — it does *not* add {98 → 0} because the Dictionary already contains an entry at 98.

At the end of this all, the map looks like this:

```
0 → 0
50 → 2
98 → -48
100 → 0
```

When it comes to actually mapping seeds to the correct output values, it's easy: The `FloorEntry` function of an AVLTreeDictionary gets the entry with the highest key that's less than or equal to the parameter passed into it. Therefore, to get any output, you take the input, and add the value of the FloorEntry of that input. For example, the FloorEntry of 99 is `{98 → -48}`, the value of that entry is -48, and 99 + -48 = 51.

After passing each value through all seven maps, the next step is to just find the lowest final output value, and then return it, and that's your Part 1 answer!

I finish writing all the code out and...

```
Test file: example1.txt / Expected output: 35 / Actual output: 31
```

... what?

Side note, this is exactly the reason I *made* the automatic test runner. Instant confirmation I'd done something wrong without confidently giving an answer anyway and I put it into the site and get told "no".

I decided to take a break at that point, walked home from the coffee shop I'd thus far coded at, and re-read the puzzle. I nearly instantly figured out the error at that point — I'd swapped the input and output range starts. (I also initially did that when I was writing this post! I'm glad I noticed before committing it. *Edit Dec 17: No, I didn't fully notice everything! A couple paragraphs above have been fixed.*)

```
Test file: example1.txt / Expected output: 35 / Actual output: 35
All tests passed!
Result on puzzle input: [redacted]
```

I plugged that into the site and it was right!

The code at this point is available [here](https://github.com/StevenH237/AdventOfCode2023/blob/f9cb8e276aea757c1b8d8e52c60ed15b9acca35c/src/days/Day5.cs).

# Part 2
Part 2 presents a small but powerful change to the puzzle: The list of seeds is not necessarily `n` seeds. It's `n/2` *ranges* of seeds. You are now tasked with finding the lowest final output given from *any seed in any of those ranges*.

Consider the example seed-list again: `79 14 55 13`. Out of every *pair* of numbers, the first of the pair is now the start of a range of seeds, and the second is the length. One range starts at 79 and, including 79 itself, contains 14 seeds (79 80 .. 91 92). The other range starts at 55 and, including 55 itself, contains 13 seeds (55 56 .. 66 67).

The tiny excerpt of sample I've given isn't very exciting (all the existing seeds just become +2), but with the full set of input data, the expected answer is now 46.

## How I solved it: Brute Force edition
My first instinct was to literally loop over every possible seed from all the ranges. That couldn't be too bad, right? Besides, it'd be a good backup in case I couldn't get my other code working.

I also had to change most of the `int`s in this code to `long`s since the seed numbers routinely exceeded 2147483648.

So with that in mind, I just edited my code to do exactly that, and set it running. After I had to restart it once because I had forgotten to actually *output the found number*, whoops, I did run it again and then get set to work figuring out the graceful solution.

The bruteforce solution took only 23 minutes. By that time I hadn't even written a single character of code in the graceful solution, but I threw the value the website gave me onto the website and I was awarded a gold star for my efforts.

I originally had that in just a copy of my repository that wasn't linked back to this repo so that I could keep editing as it ran, but I did eventually upload it as a separate branch, and you can see it [here](https://github.com/StevenH237/AdventOfCode2023/blob/d5bruteforce/src/days/Day5.cs).

## How I solved it: Graceful edition
This one was the real meat of the puzzle, a way to actually find the answer fast. I ended up making another class, which I called `D5Chart`.

To make the terminology clear here:
- A **"chart"** is one step of input/output ranges. For example, the input chart in the example is `{ 55 → true, 68 → false, 79 → true, 93 → false }`. Whether or not any particular index is part of the ranges of that chart can be determined by whether that index's `FloorEntry` has a value of `true`.
- A **"map"** is the set of rules that translates input values from one chart to output values that become the next. For example, the given map in the example is `{0 → 0, 50 → 2, 98 → -48, 100 → 0}`. How maps work is explained more above.

Now, in the website's example, the first two maps don't do anything super interesting with our data, so I'm going to skip them. After those maps are processed, the third chart is as follows:

```
57 → true
70 → false
81 → true
95 → false
```

(which corresponds to the ranges `[57, 70)` and `[81, 95)`)

The third map, however, has the following values:

```
0 → 42
7 → 50
11 → -11
53 → -4
61 → 0
```

One of those range splits happens right in the middle of one of the ranges of data. What should happen there is that the seeds 57 to 60 get decreased by 4, and the seeds 61 to 69 don't get changed at all. The second range also doesn't change.

So therefore, I figured the play for each map was to start a new chart that all the values could be copied to, and then, for each range on the existing chart, find all the rules of that map that could apply to that range.

In the example above, the first range on the chart is `[57, 70)`. The rules that apply to that chart are `-4 @ [53, 61)` and `0 @ [61, ∞)`.

For each of the rules, you then add the affected portion of the range to the new place in the new chart as it should be properly listed. This results in two separate ranges, `[53, 57)` and `[61, 70)`.

The other range on the older chart is `[81, 95)`, which isn't affected.

Once all the maps have been processed in this way, the lower bound of the lowest range then needs to actually be returned as the puzzle answer.

Including the time I spent coding it, this took about 2h37m *longer* than bruteforcing. But in terms of the actual time it took to find the answer, it was about 23 minutes faster - near-instant since it didn't have to do twenty million (my usual exaggeration is actually a couple orders of magnitude *short* here, it was a couple billion in total) map processes.

# Looking forward: What would I improve?

## Bruteforce number printing
I did this code in tandem with DragonGodGrapha, whose Advent of Code repository can be found [here](https://github.com/DragonGodGrapha/AoCRedux) (as of this writing, his Day 5 answer isn't up yet). We didn't collaborate on the actual code (or even write in the same language), but we were vaguely talking about our answers as we went.

His bruteforce solution included an optimization I should have put in mine: Print any newly found lowest number. His took about an hour and a half to do about 30% of the possible seeds, but he decided to submit as a guess the first low number that lasted about 10% of the input, and that happened to be the correct answer. If it hadn't, there would've been a forced delay before submitting another guess, which could've grown if incorrect guesses kept being made. However, the low speed at which new numbers were found would've already forced him to wait out the delay.

If I had made that change myself, I'm not sure how many tries it would've taken me, but it turned out not to matter since my code was somehow drastically faster than his.

## AreaMap
This puzzle gave me a new idea for a collection class, called `AreaMap`. An `AreaMap<K, V>` would let you specify key-ranges and values, and then you can just retrieve the value for any key given even if it's not the explicit edge of a range. The maps would've been an `AreaMap<long, long>` and the charts would've been an `AreaMap<long, bool>` (or maybe even just an `AreaSet<long>`?). I'll need to implement that at some point.