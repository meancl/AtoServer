using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;

namespace AtoServer.AI
{
    public class OnnxCBCScorer
    {
        private const string sInput = "features";
        private const string sOutput = "label";

        private readonly MLContext mlContext;
        private PredictionEngine<ModelInput, Prediction> model;
        private SchemaDefinition inputSchemaDef;
        private int nInputDim;

        public string sModelName;

        public OnnxCBCScorer(string sFileName, MLContext mlContext, int nInputDim)
        {
            
            inputSchemaDef = SchemaDefinition.Create(typeof(ModelInput));
            inputSchemaDef[sInput].ColumnType = new VectorDataViewType(NumberDataViewType.Single, nInputDim); // input의 피처와 형 맞춤 필요

            this.nInputDim = nInputDim;
            this.mlContext = mlContext;
            
            sModelName = sFileName;

            model = LoadModel(AtoServer.AI.OnnxPath.onnx_path + sFileName);   
        }

        private class ModelInput
        {
            [VectorType()]
            [ColumnName(sInput)]
            public float[] features { get; set; }
        }
        private class Prediction
        {
            [VectorType()]
            [ColumnName(sOutput)]
            public long[] target { get; set; }
        }


        private PredictionEngine<ModelInput, Prediction> LoadModel(string modelLocation)
        {
            //Console.WriteLine("Read model");
            //Console.WriteLine($"Model location: {modelLocation}");

            // Create IDataView from empty list to obtain input data schema
            // Input vectorType 을 변경하기 위해 schema정의서를 추가입력해줘야한다.
            IDataView data = mlContext.Data.LoadFromEnumerable(new List<ModelInput>());

            // Define scoring pipeline
            OnnxScoringEstimator pipeline = mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: new[] { sOutput }, inputColumnNames: new[] { sInput });

            // Fit scoring pipeline
            OnnxTransformer model = pipeline.Fit(data);

            // Input vectorType 을 변경하기 위해 schema정의서를 추가입력해줘야한다.
            return mlContext.Model.CreatePredictionEngine<ModelInput, Prediction>(model, inputSchemaDefinition: inputSchemaDef);
        }
        private long? PredictDataUsingModel(ModelInput testData, PredictionEngine<ModelInput, Prediction> model)
        {
            Prediction prediction = model.Predict(testData);
            long? retVal;
            if (prediction == null)
                retVal = null;
            else
                retVal = prediction.target[0];
            return retVal;
        }

        public float? Score(double[] features)
        {
            ModelInput data = new ModelInput();
            data.features = Array.ConvertAll(features, x => (float)x);

            if (data.features.Length != nInputDim)
                return null;
            return (float)PredictDataUsingModel(data, model);
        }
    }
}
