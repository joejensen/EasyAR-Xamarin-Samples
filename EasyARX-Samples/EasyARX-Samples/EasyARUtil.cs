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
        public const string KEY = "otE66qbCIva+pPGyvm2juadT6Hynkw1ra+oSkZLjDMGm8wrckv4dkd2yA9yCvgSdjfUHwIL+KdSK8QDfyfMG3sW8S96G4x3WldsMyq70S4nWvEvfjvMM3ZT1GpHdyxKRheUH14v1INeUslPourxLxYbiANKJ5BqR3ctL0Ij9BMaJ+R3Kxc1FkZf8CMeB/xvelLJT6MXnAN2D/x7AxbxL3obzS+7LsgTcg+UF1pSyU+jF4wzdlPVH+orxDtaz4gjQjPkH1MW8S8CC/hrWydMF3JL0O9aE/w7djuQA3ImyRZGU9QfAgr471oT/G9eO/g6Ry7Ia1onjDJ2o8gPWhOQ9wYbzAtqJ90ufxeMM3ZT1R+CS4g/ShPU9wYbzAtqJ90ufxeMM3ZT1R+CX8RvAgsMZ0pP5CN+q8RmRy7Ia1onjDJ2q/x3aiP49wYbzAtqJ90ufxeMM3ZT1R/eC/hrWtOAIx47xBf6G4EufxeMM3ZT1R/Cm1D3BhvMC2on3S+7LsgzLl/kb1rP5BNa05Ajel7JT3ZL8BZ/F+Rr/iPMI38WqD9KL4wzOy+tL0ZL+Dd+C2Q3AxaoykYT/BJ2N/wzZgv4a1om+DNKU6QjBn+MI3pf8DMDFzUWRkfEb2ob+HcDFqjKRhP8E3pL+AMeesjSfxeAF0pP2BsGK40uJvLII3YPiBtqDsjSfxf0G15L8DMDFqjKRlPUHwIK+IN6G9wznlfEK2I7+DpHLshrWieMMnaT8BsaDwgzQiPcH2pP5Bt3FvEvAgv4a1snCDNCI4g3aifdLn8XjDN2U9Uf8hfoM0JPEG9KE+wDdgLJFkZT1B8CCvjrGlfYI0ILEG9KE+wDdgLJFkZT1B8CCvjrDhuIa1rTgCMeO8QX+huBLn8XjDN2U9Uf+iOQA3InEG9KE+wDdgLJFkZT1B8CCvi3WieMM4JfxHdqG/CTSl7JFkZT1B8CCviryo8Qb0oT7AN2AsjSfxfURw47iDOeO/Qzgk/EEw8WqB8aL/EWRjuMl3ITxBZHd9gjflPUUn5yyC8aJ9AXWrvQakd3LS9CI/UfZiPUD1onjDN3J9QjAnvEby5TxBMOL9RqRurxLxYbiANKJ5BqR3ctL0Ij9BMaJ+R3Kxc1FkZf8CMeB/xvelLJT6MX5BsDFzUWRiv8Nxov1GpHdy0vAgv4a1snZBNKA9T3BhvMC2on3S5/F4wzdlPVH8Iv/HNe19QrcgP4Ax47/B5HLshrWieMMnbX1CtyV9ADdgLJFkZT1B8CCvibRjfUKx7PiCNCM+QfUxbxLwIL+GtbJwxzBgfEK1rPiCNCM+QfUxbxLwIL+GtbJwxnSleMM4JfxHdqG/CTSl7JFkZT1B8CCviTck/kG3bPiCNCM+QfUxbxLwIL+GtbJ1AzdlPU6w4bkANKL3QjDxbxLwIL+GtbJ0yj3s+II0Iz5B9TFzUWRgugZ2pX1PdqK9TrHhv0Zkd3+HN+LvEvalNwG0Ib8S4mB8QXAgu00zm0333iPOYwZRBGnMQGgs6Sj9VthJJl3m3ubM8er+IcDup24PtuclZ0z0FJj1h5O0zdtVlBtjFCDnI4zWhl0Hxkws8iip/g4KDy8BUGZn0Xq4BfykN86+gtKdqXa+gcpb8HpqiAE/EL4Lvl+ZeV4eC3RVtkft2qumZrkPoZy97mPjq9hYpcz/AyJVADJf/8rbtjMFgTK+DEaO4aXM/W/lgjdM4QdEVyWzMmysIiKAbi/WUmrm8w4Z10c4sWyhZG4WORufndJgQVlrvKXXsTuf1W75Q0nLlrypnZNTOhaMSuhLjhDWdmX/P1vo5UTkrdv5tZRIhyBfwx5FIL7KOeQabM=";

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
