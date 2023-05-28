using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtoServer.AI
{
    public class AIStarter
    {
        public Dictionary<string, int> strategyNameDict = new Dictionary<string, int>();

        public MLContext mlContext;

        #region 머신러닝 모델 정보입력
        // (모델명, 모델피처갯수)


        // Random Forest Classfier 모델 
        public OnnxRFCScorer[] arrRFCGroup1;
        Tuple<string, int>[] arrInitRFCGroup1 = new Tuple<string, int>[] {
            new Tuple<string, int> ("rf_max30_d4_v2.onnx", 102),
            new Tuple<string, int> ("rf_maxM_d2_v2.onnx", 102),
            new Tuple<string, int> ("rf_maxM_d3_v1.onnx", 102),
            new Tuple<string, int> ("rf_maxM_d3_v2.onnx", 102),
            new Tuple<string, int> ("rf_maxM_d45_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow10_02_n100_md10.onnx", 102),
            new Tuple<string, int> ("RF_pow10_003_n150_md4.onnx", 102),
            new Tuple<string, int> ("RF_pow10_003_n200_md7.onnx", 102),
            new Tuple<string, int> ("RF_pow10_004_n300_md6.onnx", 102),
            new Tuple<string, int> ("RF_pow10_004_n300_md6_randover04.onnx", 102),
            new Tuple<string, int> ("RF_pow10_004_n300_md6_smote04.onnx", 102),
            new Tuple<string, int> ("RF_pow10_04_n100_md7.onnx", 102),
            new Tuple<string, int> ("RF_pow10_04_n100_md10.onnx", 102),
            new Tuple<string, int> ("RF_pow10_025_n100_md10.onnx", 102),
            new Tuple<string, int> ("RF_pow10_026_n120_md10.onnx", 102),
            new Tuple<string, int> ("RF_pow30_02_n120_md5_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_02_n120_md7_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_02_n120_md10_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_003_n100_md5.onnx", 102),
            new Tuple<string, int> ("RF_pow30_03_n120_md5_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_03_n120_md7_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_03_n120_md8_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_03_n120_md10_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_04_n60_md7.onnx", 102),
            new Tuple<string, int> ("RF_pow30_04_n100_md10.onnx", 102),
            new Tuple<string, int> ("RF_pow30_04_n150_md7.onnx", 102),
            new Tuple<string, int> ("RF_pow30_025_n120_md5_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_025_n120_md7_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_025_n120_md8_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_025_n120_md10_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_0035_n100_md5.onnx", 102),
            new Tuple<string, int> ("RF_pow30_035_n120_md5_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_035_n120_md7_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_035_n120_md8_v2.onnx", 102),
            new Tuple<string, int> ("RF_pow30_035_n120_md10_v2.onnx", 102),

        };

        // LightGBM Classfier 모델
        public OnnxLGBMCScorer[] arrLGBMCGroup1;
        Tuple<string, int>[] arrInitLGBMCGroup1 = new Tuple<string, int>[] {
            //new Tuple<string, int> ("pipeline_lightgbm.onnx", 102),
        };

        // CatBoost Classfier 모델
        public OnnxCBCScorer[] arrCBCGroup1;
        Tuple<string, int>[] arrInitCBCGroup1 = new Tuple<string, int>[] {
            //new Tuple<string, int> ("catboost_test.onnx", 102),
        };

        // XGBoost Classfier 모델
        public OnnxXGBCScorer[] arrXGBCGroup1;
        Tuple<string, int>[] arrInitXGBCGroup1 = new Tuple<string, int>[] {
            //new Tuple<string, int> ("xgboost_test.onnx", 102),
        };

        // SVM Classfier 모델
        public OnnxSVMCScorer[] arrSVMCGroup1;
        Tuple<string, int>[] arrInitSVMCGroup1 = new Tuple<string, int>[] {
            //new Tuple<string, int> ("svm_tmp.onnx", 102),
            //new Tuple<string, int> ("svm_8.onnx", 102),
        };

        #endregion

        public AIStarter()
        {
            FirstInit();
        }

        private void FirstInit()
        {
            List<string> modelList = new List<string>();

            #region ONNX INITIALIZATION
            mlContext = new MLContext();
            OnnxModelGetter onnxModelGetter = new OnnxModelGetter();

          
            arrSVMCGroup1 = new OnnxSVMCScorer[arrInitSVMCGroup1.Length];
            for (int i = 0; i < arrInitSVMCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitSVMCGroup1[i].Item1 });
                arrSVMCGroup1[i] = new OnnxSVMCScorer(sFileName: arrInitSVMCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitSVMCGroup1[i].Item2); // Load trained model
                modelList.Add(arrInitSVMCGroup1[i].Item1);
            }
          
            arrRFCGroup1 = new OnnxRFCScorer[arrInitRFCGroup1.Length];
            for (int i = 0; i < arrInitRFCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitRFCGroup1[i].Item1 });
                arrRFCGroup1[i] = new OnnxRFCScorer(sFileName: arrInitRFCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitRFCGroup1[i].Item2); // Load trained model
            }

            arrLGBMCGroup1 = new OnnxLGBMCScorer[arrInitLGBMCGroup1.Length];
            for (int i = 0; i < arrInitLGBMCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitLGBMCGroup1[i].Item1 });
                arrLGBMCGroup1[i] = new OnnxLGBMCScorer(sFileName: arrInitLGBMCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitLGBMCGroup1[i].Item2); // Load trained model
            }

            arrCBCGroup1 = new OnnxCBCScorer[arrInitCBCGroup1.Length];
            for (int i = 0; i < arrInitCBCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitCBCGroup1[i].Item1 });
                arrCBCGroup1[i] = new OnnxCBCScorer(sFileName: arrInitCBCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitCBCGroup1[i].Item2); // Load trained model
            }

            arrXGBCGroup1 = new OnnxXGBCScorer[arrInitXGBCGroup1.Length];
            for (int i = 0; i < arrInitXGBCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitXGBCGroup1[i].Item1 });
                arrXGBCGroup1[i] = new OnnxXGBCScorer(sFileName: arrInitXGBCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitXGBCGroup1[i].Item2); // Load trained model
            }
            #endregion

            

        }


    

    }

}
