using System;

public class Shuffle
{
    private int n;
    private int[] deck;
    private int counter;

    public Shuffle(int n)
    {
        this.n = n;
        this.deck = new int[n];
        this.counter = 0;

        // initialize array to ordered sequence
        for (int i=0; i<n; i++)
        {
            deck[i] = i;
        }

        // shuffle deck
        int max = n - 1;
        Random random = new Random();
        while (max != 0)
        {
            int r = random.Next(0, max + 1);
            int temp = deck[max];
            deck[max] = deck[r];
            deck[r] = temp;
            max--;
        }
    }

    // pick the next value from the shuffled deck and return it
    // returns -1 if deck has been exhausted
    public int GetNext()
    {
        if (counter < n)
        {
            int result = deck[counter];
            counter++;
            return result;
        } else
        {
            return -1;
        }
    }
}
