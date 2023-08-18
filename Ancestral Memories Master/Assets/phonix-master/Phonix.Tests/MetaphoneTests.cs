using UnityEngine.Assertions;

namespace Phonix.Tests
{
    public class MetaphoneTests
    {
        private static readonly string[] Words = new[] { "Spotify", "Spotfy", "Sputfi", "Spotifi" };
        readonly Metaphone _generator = new Metaphone();

        public void Should_Validate_Similar_Words()
        {
            Assert.IsTrue(_generator.IsSimilar(Words));
        }
    }
}
