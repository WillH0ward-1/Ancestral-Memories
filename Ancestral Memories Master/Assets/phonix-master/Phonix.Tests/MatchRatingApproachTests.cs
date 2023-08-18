using UnityEngine.Assertions;

namespace Phonix.Tests
{
    public class MatchRatingApproachTests
    {
        private static readonly string[] Words = new[] { "Spotify", "Spotfy", "Sputfy","Sputfi" };

        readonly MatchRatingApproach _generator = new MatchRatingApproach();

        public void Should_Be_Similar()
        {
            Assert.IsTrue(_generator.IsSimilar(Words));
        }
    }
}
