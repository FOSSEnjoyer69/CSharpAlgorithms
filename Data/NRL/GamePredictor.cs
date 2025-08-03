using Microsoft.ML;

namespace CSharpAlgorithms.Data.NRL;

public class GamePredictor
{
    private readonly MLContext context = new();
    private ITransformer model;

    public void Train(Game[] games)
    {
        IDataView data = context.Data.LoadFromEnumerable(games);
        var pipeline = context.Transforms.Categorical.OneHotEncoding(new[]
        {
            new InputOutputColumnPair("HomeTeam"),
            new InputOutputColumnPair("AwayTeam")
        })
        .Append(context.Transforms.Concatenate("Features", "HomeTeam", "AwayTeam"))
        .Append(context.Transforms.Conversion.MapValueToKey("Label", "Result"))
        .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"))
        .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        model = pipeline.Fit(data);
    }

    public string Predict(string homeTeam, string awayTeam)
    {
        Game game = new();
        game.HomeTeam = homeTeam;
        game.AwayTeam = awayTeam;

        var predictionEngine = context.Model.CreatePredictionEngine<Game, Game>(model);
        var prediction = predictionEngine.Predict(game);

        return prediction.Winner;
    }
}
