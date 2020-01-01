using EasyAR;
using System;
using Urho;
using Xamarin.Essentials;

namespace EasyARX
{
    /// <summary>
    /// Component which creates a background scene to show the camera and call ARFrameUpdates when new data is received to update the foreground.
    /// 
    /// Note that this class requires the foreground scene to be set at index 1 rather than 0.
    /// </summary>
    class EasyARComponent : Component
    {
        private bool paused;
        private readonly EasyARBackgroundUpdater backgroundUpdater = new EasyARBackgroundUpdater();
        public event Action<OutputFrame,CameraParameters,float,int> ARFrameUpdated;

        // Urho components for the background
        private Scene bgScene;
        private Camera bgCamera;
        private Zone bgZone;
        private Viewport bgViewport;

        public OutputFrameBuffer OutputFrameBuffer;

        private int previousInputFrameIndex = -1;

        [Preserve]
        public EasyARComponent() 
        { 
            ReceiveSceneUpdates = true;
            Console.WriteLine("EasyARComponent Created.");
        }

        [Preserve]
        public EasyARComponent(IntPtr handle) : base(handle) 
        { 
            ReceiveSceneUpdates = true;
            Console.WriteLine("EasyARComponent Created.");
        }

        /// <summary>
        /// Initializes the background scene, note that this required the foreground to be at viewport 1.
        /// </summary>
        /// <param name="node"></param>
        public override void OnAttachedToNode(Node node)
        {
            Console.WriteLine("EasyAR Component Attached to Node.");
            Application.Paused += OnPause;
            Application.Resumed += OnResume;

            bgScene = new Scene();
            bgScene.CreateComponent<Octree>();
            bgZone = bgScene.CreateComponent<Zone>();
            bgZone.AmbientColor = Color.White;

            var cameraNode = bgScene.CreateChild();
            cameraNode.Position = new Vector3(0, 0, 0);
            cameraNode.LookAt(new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            bgCamera = cameraNode.CreateComponent<Camera>();

            bgViewport = new Viewport(Context, bgScene, bgCamera, null);
            bgViewport.SetClearColor(Color.Transparent);
            Application.Renderer.SetViewport(0, bgViewport);

            var planeNode = new EasyARBackgroundNode(backgroundUpdater.Material);
            var bgNode = bgScene.CreateChild();
            bgNode.AddComponent(planeNode);
        }

        /// <summary>
        /// Detects new output frames and updates the camera, then passes the output frame through an event to update the foreground
        /// </summary>
        /// <param name="timeStep"></param>
        protected override void OnUpdate( float timeStep)
        {
            if (paused) 
            { 
                return; 
            }

            Optional<OutputFrame> optionalOframe = OutputFrameBuffer.peek();
            if (optionalOframe.OnSome)
            {
                OutputFrame oframe = optionalOframe.Some;
                Optional<InputFrame> optionalIframe = oframe.inputFrame();
                if (optionalIframe.OnSome)
                {
                    InputFrame iframe = optionalIframe.Some;
                    CameraParameters cameraParameters = iframe.cameraParameters();
                    if (cameraParameters != null)
                    {
                        Image image = iframe.image();
                        float aspectRatio = (float)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Height);

                        int rotation = 0;
                        switch (DeviceDisplay.MainDisplayInfo.Rotation)
                        {
                            case DisplayRotation.Rotation90:
                                rotation = 90;
                                break;
                            case DisplayRotation.Rotation180:
                                rotation = 180;
                                break;
                            case DisplayRotation.Rotation270:
                                rotation = 270;
                                break;
                        }

                        if (iframe.index() != previousInputFrameIndex)
                        {
                            Matrix44F ip = cameraParameters.imageProjection(aspectRatio, rotation, true, false);
                            Matrix4 iprj = ip.ToUrhoMatrix();
                            bgCamera.SetProjection(iprj);
                            EasyAR.Buffer buffer = image.buffer();
                            try
                            {
                                backgroundUpdater.UpdateTexture(Application, image.format(), image.width(), image.height(), buffer);
                            }
                            finally
                            {
                                buffer.Dispose();
                            }
                            previousInputFrameIndex = iframe.index();
                        }

                        ARFrameUpdated?.Invoke(oframe, cameraParameters, aspectRatio, rotation);

                        image.Dispose();
                        cameraParameters.Dispose();
                    }
                    iframe.Dispose();
                }
                oframe.Dispose();
            }

            base.OnUpdate(timeStep);
        }

        void OnPause()
        {
            Console.WriteLine("Pausing AR Component.");
            paused = true;
            EasyAR.Engine.onPause();
        }

        void OnResume()
        {
            Console.WriteLine("Resuming AR Component.");
            paused = false;
            EasyAR.Engine.onResume();
        }
    }
}
