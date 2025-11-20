using NextJob.Api.Services;
using Xunit;

namespace NextJob.Tests
{
    public class MatchMlServiceTests
    {
        private readonly MatchMlService _service = new MatchMlService();

        [Fact]
        public void PredictCompatibility_ReturnsHigherScore_ForBetterCandidate()
        {
            // candidato fraco
            var low = _service.PredictCompatibility(
                requiredScore: 30,
                desiredScore: 20,
                softScore: 40,
                yearsOfExperience: 1
            );

            // candidato forte
            var high = _service.PredictCompatibility(
                requiredScore: 90,
                desiredScore: 80,
                softScore: 85,
                yearsOfExperience: 5
            );

            Assert.InRange(low, 0, 100);
            Assert.InRange(high, 0, 100);
            Assert.True(high > low);
        }

        [Fact]
        public void PredictCompatibility_AlwaysReturnsBetween0And100()
        {
            var score = _service.PredictCompatibility(
                requiredScore: 200,   
                desiredScore: -50,
                softScore: 500,
                yearsOfExperience: 50
            );

            Assert.InRange(score, 0, 100);
        }

        [Fact]
        public void PredictCompatibility_IsDeterministic_ForSameInput()
        {
            var s1 = _service.PredictCompatibility(70, 60, 80, 3);
            var s2 = _service.PredictCompatibility(70, 60, 80, 3);

            Assert.Equal(s1, s2);
        }
    }
}

