using EasyAR;
using Urho;

namespace EasyARX
{
    /// <summary>
    /// Utilities and shared data for working with EasyAR in Xamarin
    /// </summary>
    public static class EasyARUtil
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
        /// <param name="matrix">The matrix as an EasyAR object</param>
        /// <returns>The matrix as an Urho object</returns>
        public static Matrix4 ToUrhoMatrix( this Matrix44F matrix)
        {
            Matrix4 m4 = new Matrix4(
                matrix.data_0, matrix.data_1, matrix.data_2, matrix.data_3,
                matrix.data_4, matrix.data_5, matrix.data_6, matrix.data_7,
                matrix.data_8, matrix.data_9, matrix.data_10, matrix.data_11,
                matrix.data_12, matrix.data_13, matrix.data_14, matrix.data_15);
            return m4;
        }

        /// <summary>
        /// Extension method for converting an EasyAR vector to an Urho Vector
        /// </summary>
        /// <param name="vector">The vector to convert</param>
        /// <returns></returns>
        public static Vector2 ToUrhoVector( this Vec2F vector)
        {
            return new Vector2(vector.data_0, vector.data_1);
        }

        /// <summary>
        /// Extension method for converting an EasyAR vector to an Urho Vector
        /// </summary>
        /// <param name="vector">The vector to convert</param>
        /// <returns></returns>
        public static Vector3 ToUrhoVector( this Vec3F vector)
        {
            return new Vector3(vector.data_0, vector.data_1, vector.data_2);
        }

        /// <summary>
        /// Extension method for converting an Urho vector to an EasyAR Vector
        /// </summary>
        /// <param name="vector">The vector to convert</param>
        /// <returns></returns>
        public static Vec2F toEasyARVector( this Vector2 vector)
        {
            return new Vec2F(vector.X, vector.Y);
        }

        /// <summary>
        /// Extension method for converting an Urho vector to an EasyAR Vector
        /// </summary>
        /// <param name="vector">The vector to convert</param>
        /// <returns></returns>
        public static Vec3F toEasyARVector( this Vector3 vector)
        {
            return new Vec3F(vector.X, vector.Y, vector.Z);
        }
    }
}
