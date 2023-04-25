using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTradier_AI_Server.AI
{
    public class AIStarter
    {
        public Dictionary<string, int> strategyNameDict = new Dictionary<string, int>();

        public Dictionary<string, Dictionary<int, ScaleDatas>> scaleDict;
        public MLContext mlContext;

        #region 머신러닝 모델 정보입력
        // (모델명, 모델피처갯수)

        // Deep Neural Network Classifier 모델
        public OnnxDNNCScorer[] arrDNNCGroup1;
        Tuple<string, int>[] arrIniDNNCGroup1 = new Tuple<string, int>[] {
            //new Tuple<string, int> ("fMax30_50_drop_Robust_ls_min.onnx", 102),
        };

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

            //new Tuple<string, int> ("RF_pow10_01_n150_md8_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_02_n150_md8_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_02_n100_md8_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_016_n100_md8_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_powMin_01_n200_md13_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow30_04_n400_md13_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow30_015_n200_md12_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_015_n100_md12_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_015_n100_md8_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_015_n200_md7_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow10_01_n200_md7_2023-04-01.onnx", 102),
            //new Tuple<string, int> ("RF_pow30_04_n250_md10_2023-04-01.onnx", 102),
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
            //new Tuple<string, int> ("svm_standard_pca20_undersampling_rbf_pow10_025_pow30_03.onnx", 23),
            //new Tuple<string, int> ("svm_8.onnx", 102),
        };

        // PCA 변환 모델
        public OnnxPCAModel[] arrPCAGroup1;
        Tuple<string, int>[] arrInitPCAGroup1 = new Tuple<string, int>[] {
            // new Tuple<string, int> ("svm_standard_pca20_undersampling_rbf_pow10_025_pow30_03_PCA.onnx", 102),
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

            #region 스케일 필요한 모델 
            arrDNNCGroup1 = new OnnxDNNCScorer[arrIniDNNCGroup1.Length];
            for (int i = 0; i < arrIniDNNCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrIniDNNCGroup1[i].Item1 });
                arrDNNCGroup1[i] = new OnnxDNNCScorer(sFileName: arrIniDNNCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrIniDNNCGroup1[i].Item2); // Load trained model
                modelList.Add(arrIniDNNCGroup1[i].Item1);
            }

            arrSVMCGroup1 = new OnnxSVMCScorer[arrInitSVMCGroup1.Length];
            for (int i = 0; i < arrInitSVMCGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitSVMCGroup1[i].Item1 });
                arrSVMCGroup1[i] = new OnnxSVMCScorer(sFileName: arrInitSVMCGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitSVMCGroup1[i].Item2); // Load trained model
                modelList.Add(arrInitSVMCGroup1[i].Item1);
            }
            #endregion

            #region 스케일 필요하지 않은 모델
            arrRFCGroup1 = new OnnxRFCScorer[arrInitRFCGroup1.Length];
            for (int i = 0; i < arrInitRFCGroup1.Length; i++)
            {
                // onnxModelGetter.DownloadOnnxModels(new[] { arrInitRFCGroup1[i].Item1 });
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

            #region PCA 모델
            arrPCAGroup1 = new OnnxPCAModel[arrInitPCAGroup1.Length];
            for (int i = 0; i < arrInitPCAGroup1.Length; i++)
            {
                onnxModelGetter.DownloadOnnxModels(new[] { arrInitPCAGroup1[i].Item1 });
                arrPCAGroup1[i] = new OnnxPCAModel(sFileName: arrInitPCAGroup1[i].Item1, mlContext: mlContext, nInputDim: arrInitPCAGroup1[i].Item2); // Load trained model
            }
            #endregion
            #endregion

            using (var db = new myDbContext())
            {
                #region Read Scale Data
                scaleDict = new Dictionary<string, Dictionary<int, ScaleDatas>>(); // modelName : {variableName : scaleData}

                for (int sc = 0; sc < modelList.Count; sc++)
                {
                    ScaleDatas nLastUpdateT = db.scaleDatasDict.OrderByDescending(o => o.dTime).FirstOrDefault(x => x.sModelName.Equals(modelList[sc]));

                    if (nLastUpdateT != null) // 해당 모델의 정보가 있을때
                    {
                        scaleDict[modelList[sc]] = new Dictionary<int, ScaleDatas>();
                        var modelFeatures = db.scaleDatasDict.Where(x => x.dTime == nLastUpdateT.dTime && x.sModelName.Equals(modelList[sc]));

                        foreach (var feature in modelFeatures)
                        {
                            if (feature != null)
                                scaleDict[modelList[sc]][feature.nSeq] = feature;
                        }
                    }
                }
                #endregion
            }
        }


        #region Scale 작업
        public double[] GetScaledData(double[] input, string sModelName)
        {
            double denom;
            try
            {
                double[] scaledData = new double[input.Length];
                var specificScaleDict = scaleDict[sModelName];

                for (int i = 0; i < input.Length; i++)
                {
                    ScaleDatas scale = specificScaleDict[i];
                    denom = (scale.fD1 - scale.fD2 == 0) ? 1 : (scale.fD1 - scale.fD2);
                    scaledData[i] = ((input[i] - scale.fD0) / denom);
                }
                return scaledData;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region DNN 투표
        public bool VoteDNN(DNNGroup[] ds)
        {
            DNNGroup testGroup;
            bool isPass = false;
            int nPNum;
            // superpass , point
            // min.. 

            for (int i = 0; i < ds.Length; i++)
            {
                testGroup = ds[i];
                List<float> testVal = testGroup.group;

                nPNum = 0;

                for (int p = 0; p < testVal.Count; p++)
                {
                    if (testVal[p] > testGroup.fScoreToPass)
                        nPNum++;
                }

                if (nPNum < testGroup.nMinPassNum)
                {
                    isPass = false;
                    break;
                }
                else
                {
                    isPass = true;
                    if (nPNum >= testGroup.nSuperPassNum)
                        break;
                }
            }
            return isPass;
        }
        #endregion

        #region DNN Test 
        public float GetDNN(OnnxDNNCScorer onnx, double[] data)
        {
            float fRet = 0f;
            var scaledData = GetScaledData(data, onnx.sModelName);
            if (scaledData != null)
            {
                float? pred_profit15 = onnx.Score(scaledData);
                if (pred_profit15 != null)
                    fRet = (float)pred_profit15;
            }
            return fRet;
        }
        #endregion
    }


    public struct DNNGroup
    {
        public List<float> group;
        public double fMinToPass;
        public double fScoreToPass;
        public double fSuperToPass; // 슈퍼패스 율
        public int nSuperPassNum;
        public int nMinPassNum;

        public DNNGroup(double fScoreToPass, double fMinToPass = 1.0, double fSuperToPass = 2.0)
        {
            group = new List<float>();
            this.fMinToPass = fMinToPass;
            this.fScoreToPass = fScoreToPass;
            this.fSuperToPass = fSuperToPass;
            nSuperPassNum = 0;
            nMinPassNum = 0;
        }
        public void Append(float fPred)
        {
            group.Add(fPred);
        }
        public DNNGroup Done()
        {
            // ex) if 1개
            nSuperPassNum = (int)Math.Round(group.Count * fSuperToPass); // default 1 * 2.0 = 2
            nMinPassNum = (int)Math.Round(group.Count * fMinToPass); // default 1 * 1.0 = 1

            return this;
        }

    }

}
