//
// Copyright 2011-2012 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;

using Android.App;
using Android.Content;
using Android.Views;

using NUnit.Framework.Internal;
using NUnit.Framework.Api;
using NUnit.Framework;
using MonoDroid.NUnit;

namespace Android.NUnitLite.UI {
	
	class TestCaseElement : TestElement {
		
		public TestCaseElement (TestMethod test, AndroidRunner runner) : base (test, runner)
		{
			if (test.RunState == RunState.Runnable)
				Indicator = "..."; // hint there's more

            Caption = test.Name;
		}

		public override void Update ()
		{
            if (Result.IsIgnored ()) {
                Caption = String.Format ("<b>{0}</b>:\n <font color='#FF7700'>{1}</font>", 
                    TestCase.FullName, Result.GetMessage()); 
            } else if (Result.IsSuccess () || Result.IsInconclusive ()) {
                int counter = Result.AssertCount;
                Caption = String.Format ("<b>{0}</b>:\n <font color='green'>{1} {2} ms for {3} assertion{4}</font>",
                    TestCase.FullName,
                    Result.IsInconclusive () ? "Inconclusive." : "Success!",
                    Result.Duration.TotalMilliseconds, counter,
                    counter == 1 ? String.Empty : "s");
            } else if (Result.IsFailure ()) {
                Caption = String.Format("<b>{0}</b>:\n <font color='red'>{1}</font>", TestCase.FullName, Result.GetMessage ());
            } else {
                // Assert.Ignore falls into this
                Caption = Result.GetMessage ();
            }

            SetCaption(Caption);
		}
		
		public TestMethod TestCase {
			get { return Test as TestMethod; }
		}
		
		public override View GetView (Context context, View convertView, ViewGroup parent)
		{
			View view = base.GetView (context, convertView, parent);
			view.Click += delegate {
				if (TestCase.RunState != RunState.Runnable)
					return;
								
				AndroidRunner runner = AndroidRunner.Runner;
				if (!runner.OpenWriter ("Run " + TestCase.FullName, context))
					return;

				var suite = (TestCase.Parent as TestSuite);
                while(suite != null && suite.arguments == null) {
                    suite = suite.Parent as TestSuite;
                }

				var testContext = TestExecutionContext.CurrentContext;
                testContext.TestObject = Reflect.Construct (TestCase.Method.ReflectedType, suite.arguments);

				
				try {
					suite.GetOneTimeSetUpCommand ().Execute (testContext);
					Run ();
					suite.GetOneTimeTearDownCommand ().Execute (testContext);
				}
				finally {
					runner.CloseWriter ();
				}

				if ((TestCase.RunState == RunState.Runnable) && !Result.IsSuccess()) {
					Intent intent = new Intent (context, typeof (TestResultActivity));
					intent.PutExtra ("TestCase", Name);
					intent.AddFlags (ActivityFlags.NewTask);			
					context.StartActivity (intent);
				} 
			};
			return view;
		}

		public void Run ()
		{
			Update (Runner.Run (TestCase));
		}
	}
}