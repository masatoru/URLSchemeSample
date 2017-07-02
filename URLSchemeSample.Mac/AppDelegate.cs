using AppKit;
using Foundation;

namespace URLSchemeSample.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application


        }

        public override void WillFinishLaunching(NSNotification notification)
        {
			//NG 呼んではだめ
			//base.WillFinishLaunching(notification);

			// Objective-Cでの書き方
			// https://stackoverflow.com/questions/41724031/handlegeturlevent-doesnt-get-called-in-safari-extension-mac-os
			//- (void)applicationWillFinishLaunching:(NSNotification *)notification
			//{
			//	NSAppleEventManager* appleEventManager = [NSAppleEventManager sharedAppleEventManager];
			//	[appleEventManager setEventHandler:self
			//			andSelector:@selector(handleGetURLEvent: withReplyEvent:)
			//			forEventClass:kInternetEventClass andEventID:kAEGetURL];
			//}

			var manager= NSAppleEventManager.SharedAppleEventManager;
            manager.SetEventHandler(this,
                                   new ObjCRuntime.Selector("handleGetURLEvent:withReplyEvent:"),
                                   AEEventClass.Internet,
                                   AEEventID.GetUrl);
        }

        // Objective-Cでの書き方
		//- (void)handleGetURLEvent:(NSAppleEventDescriptor *)event*)
		//         withReplyEvent:(NSAppleEventDescriptor *)reply*) replyEvent
		//		{
		//			rl = [NSURL URLWithString:[[event paramDescriptorForKeyword:keyDirectObject] stringValue]];
		//}
		//}

		[Export("handleGetURLEvent:withReplyEvent:")]
        private void HandleGetURLEvent(NSAppleEventDescriptor descriptor,NSAppleEventDescriptor replyEvent)
        {
			// stackoverflow.com/questions/1945
			// https://forums.xamarin.com/discussion/9774/custom-url-schema-handling
			var keyDirectObject = "----";
            var keyword =
                (((uint)keyDirectObject[0]) << 24 |
                ((uint)keyDirectObject[1]) << 16 |
                ((uint)keyDirectObject[2]) << 8 |
                ((uint)keyDirectObject[3]));

            var urlString = descriptor.ParamDescriptorForKeyword(keyword).StringValue;

			using(var alert =new NSAlert()){
                alert.MessageText = urlString;
                alert.RunSheetModal(null);
            }
        }

		public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
