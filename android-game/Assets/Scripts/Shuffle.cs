
using System.Collections;

public class Shuffle
{

    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random rng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int rndIndex = rng.Next(i, array.Length);
            T temp = array[rndIndex];
            array[rndIndex] = array[i];
            array[i] = temp;
        }
        return array;

    }


}
