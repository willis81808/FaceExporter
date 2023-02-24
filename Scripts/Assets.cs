using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Jotunn.Utils;

internal static class Assets
{
    internal static AssetBundle Bundle = AssetUtils.LoadAssetBundleFromResources("faceexporter", typeof(FaceExporter).Assembly);

    internal static FaceButtonOptions faceButtonOptions = Bundle.LoadAsset<GameObject>("Face Options").GetComponent<FaceButtonOptions>();
}