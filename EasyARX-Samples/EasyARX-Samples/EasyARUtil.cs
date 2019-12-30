using EasyAR;
using Urho;

namespace EasyARX
{
    /// <summary>
    /// Utilities and shared data for working with EasyAR in Xamarin
    /// </summary>
    public class EasyARUtil
    {
        /// <summary>
        /// The key provided by EasyAR
        /// 
        /// Steps to create the key for this sample:
        /// 1. login www.easyar.com
        /// 2. create app with
        ///     Name: EasyARXSamples
        ///     Package Name and Bundle Identifier: com.joejensen.easyarxsamples
        /// 3. Find the created item in the list and show key set key string bellow
        /// Note that these values can be changed by editing the Info.plist in iOS and/or the Android Manifest options.
        /// </summary>
        public const string KEY = "===PLEASE ENTER YOUR KEY HERE===";

        /// <summary>
        /// Converts EasyAR matrix objects to Urho matrix objects, note that this does not change the handedness of the matrix. EasyAR matrices
        /// are in OpenGL format except they are row-major while Urho matrices are in DirectX format.
        /// </summary>
        /// <param name="arMatrix">The matrix as an EasyAR object</param>
        /// <returns>The matrix as an Urho object</returns>
        public static Matrix4 ConvertEasyArToUrho(Matrix44F arMatrix)
        {
            Matrix4 m4 = new Matrix4(
                arMatrix.data_0, arMatrix.data_1, arMatrix.data_2, arMatrix.data_3,
                arMatrix.data_4, arMatrix.data_5, arMatrix.data_6, arMatrix.data_7,
                arMatrix.data_8, arMatrix.data_9, arMatrix.data_10, arMatrix.data_11,
                arMatrix.data_12, arMatrix.data_13, arMatrix.data_14, arMatrix.data_15);
            return m4;
        }
    }
}
