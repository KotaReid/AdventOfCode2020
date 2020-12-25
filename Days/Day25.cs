using System;

namespace AdventOfCode2020.Days
{
    public static class Day25
    {
        public static void Run()
        {
            var cardPublicKey = 2959251;
            var doorPublicKey = 4542595;

            var cardLoopSize = GetLoopSize(cardPublicKey);
            var doorLoopSize = GetLoopSize(doorPublicKey);

            var encryptionKey1 = GetEncryptionKey(cardPublicKey, doorLoopSize);
            var encryptionKey2 = GetEncryptionKey(doorPublicKey, cardLoopSize);

            if (encryptionKey1 != encryptionKey2)
                throw new Exception("Encryption keys do not match");

            Console.WriteLine($"Answer: {encryptionKey1}");
        }

        private static int GetLoopSize(int publicKey)
        {
            var value = 1L;
            var loopSize = 0;

            while (value != publicKey)
            {
                value = TransformSubjectNumber(value, 7);
                loopSize++;
            }

            return loopSize;
        }

        private static long GetEncryptionKey(long publicKey, int loopSize)
        {
            var value = 1L;

            for (var i = 0; i < loopSize; i++)
                value = TransformSubjectNumber(value, publicKey);

            return value;
        }

        private static long TransformSubjectNumber(long value, long subjectNubmer) => (value * subjectNubmer) % 20201227;
    }
}