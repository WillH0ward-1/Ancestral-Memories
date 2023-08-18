﻿using UnityEngine.Assertions;

namespace Phonix.Tests
{
    public class SoundexTests
    {
        private static readonly string[] Words = new[] { "Spotify", "Spotfy", "Sputfi", "Spotifi" };
        private static readonly string[] Words2 = new[] { "United Air Lines", "United Aire Lines", "United Air Line" };

        readonly Soundex _generator = new Soundex();

        public void Should_Be_Similar()
        {
            Assert.IsTrue(_generator.IsSimilar(Words));
            Assert.IsTrue(_generator.IsSimilar(Words2));
        }
    }
}
