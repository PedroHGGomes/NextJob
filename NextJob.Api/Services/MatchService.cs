using Microsoft.ML;
using Microsoft.ML.Data;
using NextJob.Api.ML;

namespace NextJob.Api.Services
{
    // Serviço de ML.NET 
    public class MatchMlService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<MatchModelInput, MatchModelOutput> _predictionEngine;

        public MatchMlService()
        {
            _mlContext = new MLContext();

            // Dados de treino
            var trainingData = new List<MatchTrainingRow>
            {
         
                new MatchTrainingRow { RequiredSkillsScore = 90, DesiredSkillsScore = 80, SoftSkillsScore = 85, YearsOfExperience = 5, Label = 95 },
                new MatchTrainingRow { RequiredSkillsScore = 80, DesiredSkillsScore = 70, SoftSkillsScore = 80, YearsOfExperience = 4, Label = 88 },

                
                new MatchTrainingRow { RequiredSkillsScore = 60, DesiredSkillsScore = 50, SoftSkillsScore = 70, YearsOfExperience = 3, Label = 70 },
                new MatchTrainingRow { RequiredSkillsScore = 55, DesiredSkillsScore = 40, SoftSkillsScore = 60, YearsOfExperience = 2, Label = 65 },

                
                new MatchTrainingRow { RequiredSkillsScore = 30, DesiredSkillsScore = 20, SoftSkillsScore = 40, YearsOfExperience = 1, Label = 40 },
                new MatchTrainingRow { RequiredSkillsScore = 20, DesiredSkillsScore = 10, SoftSkillsScore = 30, YearsOfExperience = 0, Label = 30 }
            };

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Pipeline
            var pipeline = _mlContext.Transforms.Concatenate(
                                "Features",
                                nameof(MatchModelInput.RequiredSkillsScore),
                                nameof(MatchModelInput.DesiredSkillsScore),
                                nameof(MatchModelInput.SoftSkillsScore),
                                nameof(MatchModelInput.YearsOfExperience))
                            .Append(_mlContext.Regression.Trainers.Sdca(
                                labelColumnName: "Label",
                                featureColumnName: "Features"));

            _model = pipeline.Fit(dataView);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MatchModelInput, MatchModelOutput>(_model);
        }

        
        public float PredictCompatibility(
            double requiredScore,
            double desiredScore,
            double softScore,
            int yearsOfExperience)
        {
            var input = new MatchModelInput
            {
                RequiredSkillsScore = (float)requiredScore,
                DesiredSkillsScore = (float)desiredScore,
                SoftSkillsScore = (float)softScore,
                YearsOfExperience = yearsOfExperience
            };

            var prediction = _predictionEngine.Predict(input);

            // Garante 0–100
            var value = prediction.Score;
            if (value < 0) value = 0;
            if (value > 100) value = 100;

            return value;
        }

        
        private class MatchTrainingRow : MatchModelInput
        {
            public float Label { get; set; } 
        }
    }
}
