using EasyAR;
using System.Collections.Generic;
using Urho;
using Urho.Shapes;

namespace EasyARX
{
    /// <summary>
    /// This class sets up the component graph for EasyAR and links it up to the foreground scenegraph in Urho
    /// </summary>
    class UrhoApp : Application
    {
        // Urho Fields
        Scene fgScene;
        Camera fgCamera;
        Zone fgZone;
        Viewport fgViewport;
        MonoDebugHud debugHud;

        // EasyAR Fields
        private CameraDevice camera;
        private InputFrameThrottler throttler;
        private FeedbackFrameFork feedbackFrameFork;
        private InputFrameToOutputFrameAdapter i2OAdapter;
        private InputFrameFork inputFrameFork;
        private OutputFrameJoin join;
        private OutputFrameBuffer oFrameBuffer;
        private InputFrameToFeedbackFrameAdapter i2FAdapter;
        private OutputFrameFork outputFrameFork;
        private readonly List<ImageTracker> trackers = new List<ImageTracker>();
        private readonly CallbackScheduler callbackScheduler = new DelayedCallbackScheduler();
        private EasyARComponent arComponent;

        // The list of targets that are currently in the scene
        private readonly List<Node> targetNodes = new List<Node>();

        [Preserve]
        public UrhoApp(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            base.Start();

            Renderer.NumViewports = 2;
            CreateScene();
            CreateEasyAr();

            // EasyAR camera and trackers must be started or we won't see anything rendered
            bool status = true;
            if (camera != null)
            {
                status &= camera.start();
            }
            else
            {
                status = false;
            }

            foreach (ImageTracker tracker in trackers)
            {
                status &= tracker.start();
            }
        }

        /// <summary>
        /// Stop the application using the urho callback
        /// </summary>
        protected override void Stop()
        {
            base.Stop();
            DestroyEasyAr();
        }

        /// <summary>
        /// Initialize the urho foreground scene
        /// </summary>
        void CreateScene()
        {
            fgScene = new Scene();
            fgScene.CreateComponent<Octree>();

            fgZone = fgScene.CreateComponent<Zone>();
            fgZone.AmbientColor = Color.White * 0.25f;

            var cameraNode = fgScene.CreateChild();
            cameraNode.Position = new Vector3(0, 0, 0);
            cameraNode.LookAt(new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            fgCamera = cameraNode.CreateComponent<Camera>();

            var lightNode = fgScene.CreateChild();
            lightNode.Position = new Vector3(0, 0, -10);

            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.SpecularIntensity = 0.5f;

            // Note that we delete the clear from the render path so we can see through to the first viewport that is rendering the background
            fgViewport = new Viewport(Context, fgScene, fgCamera, null);
            fgViewport.RenderPath = fgViewport.RenderPath.Clone();
            fgViewport.SetClearColor(Color.Transparent);
            fgViewport.RenderPath.RemoveCommand(0);

            // The foreground is at index 1, the background is added by the EasyAR component as index 0
            Renderer.SetViewport(1, fgViewport);

            debugHud = new MonoDebugHud(this);
            debugHud.FpsOnly = true;
            debugHud.Show(Color.Black, 40);
        }

        /// <summary>
        /// Release all the EasyAR resources
        /// </summary>
        void DestroyEasyAr()
        {
            foreach (ImageTracker tracker in trackers)
            {
                tracker.Dispose();
            }
            trackers.Clear();

            if (camera != null)
            {
                camera.Dispose();
                camera = null;
            }
        }

        /// <summary>
        /// Initialize the EasyAR component graph.
        /// See https://help.easyar.com/EasyAR%20Sense/v3/Getting%20Started/Getting-Started-with-EasyAR.html
        /// </summary>
        void CreateEasyAr()
        {
            camera = CameraDeviceSelector.createCameraDevice(CameraDevicePreference.PreferObjectSensing);
            throttler = InputFrameThrottler.create();
            inputFrameFork = InputFrameFork.create(2);
            join = OutputFrameJoin.create(2);
            oFrameBuffer = OutputFrameBuffer.create();
            i2OAdapter = InputFrameToOutputFrameAdapter.create();
            i2FAdapter = InputFrameToFeedbackFrameAdapter.create();
            outputFrameFork = OutputFrameFork.create(2);

            bool status = camera.openWithPreferredType(CameraDeviceType.Back);
            camera.setSize(new Vec2I(1280, 720));
            camera.setFocusMode(CameraDeviceFocusMode.Continousauto);
            camera.setBufferCapacity(5 + 7);
            if (!status) { return; }
            ImageTracker tracker = ImageTracker.create();
            LoadFromImage(tracker, "almondblossoms.jpg", "almondblossoms");
            LoadFromImage(tracker, "irises.jpg", "irises");
            LoadFromImage(tracker, "starrynight.jpg", "starrynight");
            tracker.setSimultaneousNum(6);
            trackers.Add(tracker);

            feedbackFrameFork = FeedbackFrameFork.create(trackers.Count);

            camera.inputFrameSource().connect(throttler.input());
            throttler.output().connect(inputFrameFork.input());
            inputFrameFork.output(0).connect(i2OAdapter.input());
            i2OAdapter.output().connect(join.input(0));

            inputFrameFork.output(1).connect(i2FAdapter.input());
            i2FAdapter.output().connect(feedbackFrameFork.input());
            int k = 0;
            foreach (ImageTracker _tracker in trackers)
            {
                feedbackFrameFork.output(k).connect(_tracker.feedbackFrameSink());
                _tracker.outputFrameSource().connect(join.input(k + 1));
                k++;
            }

            join.output().connect(outputFrameFork.input());
            outputFrameFork.output(0).connect(oFrameBuffer.input());
            outputFrameFork.output(1).connect(i2FAdapter.sideInput());
            oFrameBuffer.signalOutput().connect(throttler.signalInput());

            arComponent = fgScene.CreateComponent<EasyARComponent>();
            arComponent.ARFrameUpdated += OnARFrameUpdated;
            arComponent.OutputFrameBuffer = oFrameBuffer;
        }

        /// <summary>
        /// Loads an image and adds it to the tracker
        /// </summary>
        /// <param name="tracker"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private void LoadFromImage(ImageTracker tracker, string path, string name)
        {
            Optional<ImageTarget> optionalTarget = ImageTarget.createFromImageFile(path, StorageType.Assets, name, "", "", 1.0f);
            if (optionalTarget.OnSome)
            {
                System.Console.WriteLine($"Loaded ImageTarget Name: {name}, Aspect Ratio: {optionalTarget.Some.aspectRatio()}");
                tracker.loadTarget(optionalTarget.Some, callbackScheduler, (target, status) => {
                    System.Console.WriteLine($"Loaded Target {status}: {target.name()}({target.runtimeID()})");
                });
            }
            else
            {
                System.Console.WriteLine($"Failed loading: {path}");
            }
        }

        /// <summary>
        /// Callback from the EasyAR component whic renders the foreground nodes.
        /// </summary>
        /// <param name="oFrame"></param>
        /// <param name="cameraParameters"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="rotation"></param>
        protected void OnARFrameUpdated( OutputFrame oFrame, CameraParameters cameraParameters, float aspectRatio, int rotation)
        {
            var far = 100f;
            var near = 0.01f;
            Matrix44F sp = cameraParameters.projection(near, far, aspectRatio, rotation, true, false);
            foreach (Optional<FrameFilterResult> unTypedResult in oFrame.results())
            {
                if (unTypedResult.OnSome)
                {
                    if (unTypedResult.Some is ImageTrackerResult result)
                    {
                        int targetIndex = 0;
                        List<TargetInstance> targetInstances = result.targetInstances();
                        foreach (TargetInstance targetInstance in targetInstances)
                        {
                            TargetStatus status = targetInstance.status();
                            if (status == TargetStatus.Tracked)
                            {
                                Optional<Target> optionalTarget = targetInstance.target();
                                if (optionalTarget.OnSome)
                                {
                                    Target target = optionalTarget.Some;
                                    if (target is ImageTarget imageTarget)
                                    {
                                        List<Image> images = imageTarget.images();
                                        Image targetImage = images[0];

                                        Matrix4 prj = EasyARUtil.ConvertEasyArToUrho(sp);
                                        prj.M34 /= 2f;
                                        prj.M33 = far / (far - near);
                                        prj.M43 *= -1;
                                        fgCamera.SetProjection(prj);

                                        Matrix4 convertedPoseMatrix = EasyARUtil.ConvertEasyArToUrho(targetInstance.pose());
                                        Vector3 scale = new Vector3(imageTarget.scale(), imageTarget.scale() * targetImage.height() / targetImage.width(), 1);
                                        UpdateArScene( targetIndex, convertedPoseMatrix, scale);
                                        foreach (Image targetImageToRelease in images)
                                        {
                                            targetImageToRelease.Dispose();
                                        }
                                    }
                                    target.Dispose();
                                }
                            }
                            targetInstance.Dispose();
                            targetIndex++;
                        }

                        // Remove any targets that can no longer be located
                        while(targetNodes.Count > targetIndex)
                        {
                            Node node = targetNodes[targetIndex];
                            targetNodes.RemoveAt(targetIndex);
                            node.Remove();
                        }
                    }
                    unTypedResult.Some.Dispose();
                }
            }
        }

        /// <summary>
        /// For each target we've detected render a box over it's location
        /// </summary>
        /// <param name="index">The index of the detected target</param>
        /// <param name="poseMatrix">The pose containing the targets center</param>
        /// <param name="scale">Scale isn't considered part of the pose, we use the images original aspect ratio to scale the cube</param>
        private void UpdateArScene(int index, Matrix4 poseMatrix, Vector3 scale)
        {
            Node node;
            if (targetNodes.Count > index)
            {
                node = targetNodes[index];
            }
            else
            {
                node = fgScene.CreateChild();

                var modelNode = node.CreateChild();

                // The box is a unit cube cenered a (0,0) we need to shift it a little or cube will be partially behind the target and look wrong.
                modelNode.Position = new Vector3(0, 0, -0.5f);
                Box box = modelNode.CreateComponent<Box>();
                box.Color = Color.Blue;

                targetNodes.Add(node);
            }

            // Flatten out the cube
            scale.Z *= 0.1f;

            // Reverse handedness of pose's rotation
            var rotation = poseMatrix.Rotation;
            rotation.Z *= -1;
            rotation.W *= -1;

            var pos = poseMatrix.Column3;
            node.Position = new Vector3(pos.X, pos.Y, -pos.Z); // Reverse handedness of the poses position
            node.Rotation = rotation;
            node.Scale = scale;
        }
    }
}
