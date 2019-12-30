using System;
using UIKit;

namespace EasyARX.iOS
{
    /// <summary>
    /// Main entry point to the application, unchanged from the default template
    /// </summary>
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            if( !EasyAR.Engine.initialize(EasyARUtil.KEY))
            {
                string message = $"Failed intializing EasyAR: {EasyAR.Engine.errorMessage()}.";
                Console.WriteLine(message);
                throw new Exception(message);
            }

            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
