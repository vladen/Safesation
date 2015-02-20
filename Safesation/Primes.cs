using System;

namespace Safesation
{
    internal static class Primes
    {
        #region FIELDS

        private static readonly int[] Numbers =
        {
            3, 7, 17, 37, 79, 163, 331, 673, 1361, 2729, 5471, 10949, 21911, 43853, 87719, 175447, 350899, 701819,
            1403641, 2807303, 5614657, 11229331, 22458671, 44917381, 89834777, 179669557, 359339171, 718678369
        }; 

        #endregion

        #region PROPERTIES

        public static int First
        {
            get { return Numbers[0]; }
        } 

        #endregion

        #region METHODS

        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
                return candidate == 2;
            var limit = (int)Math.Sqrt(candidate);
            for (var divisor = 3; divisor <= limit; divisor += 2)
                if (candidate % divisor == 0)
                    return false;
            return true;
        }

        public static int Next(int current)
        {
            for (var i = 0; i < Numbers.Length; i++)
            {
                var prime = Numbers[i];
                if (prime > current)
                    return prime;
            }
            for (var i = (current * 2) | 1; i < Int32.MaxValue; i += 2)
                if (IsPrime(i))
                    return i;
            throw new OverflowException();
        } 

        #endregion
    }
}