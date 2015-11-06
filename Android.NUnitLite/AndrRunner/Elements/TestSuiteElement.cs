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
using NUnit.Framework;
using NUnit.Framework.Api;
using MonoDroid.NUnit;

namespace Android.NUnitLite.UI {
	
	class TestSuiteElement : TestElement {

		public TestSuiteElement (TestSuite suite, AndroidRunner runner) : base (suite, runner)
		{
			if (Suite.TestCaseCount > 0)
				Indicator = ">"; // hint there's more

            Caption = suite.FullName;
		}
		
		public TestSuite Suite {
			get { return Test as TestSuite; }
		}
		
		public override void Update ()
		{
            if (Result.IsIgnored()) {
                Caption = String.Format("<b>{0}</b>:\n <font color='#FF7700'>{1}</font>", 
                    Suite.FullName, Result.GetMessage()); 
            } else if (Result.IsSuccess()) {
                int counter = Result.AssertCount;
                Caption = String.Format("<b>{0}</b>:\n <font color='green'>Success! {1} ms for {2} assertion{3}</font>",
                    Suite.FullName,
                    Result.Duration.TotalMilliseconds, counter,
                    counter == 1 ? String.Empty : "s");
            } else if (Result.IsInconclusive()) {
                Caption = String.Format("<b>{0}</b>:\n <font color='blue'>{1}</font>", Suite.FullName, Result.GetMessage ());
            } else if (Result.IsFailure ()) {
                Caption = String.Format("<b>{0}</b>:\n <font color='red'>{1}</font>", Suite.FullName, Result.GetMessage ());
			} else {
				// Assert.Ignore falls into this
				Caption = Result.GetMessage ();
			}

            SetCaption(Caption);
		}

		public override View GetView (Context context, View convertView, ViewGroup parent)
		{
			View view = base.GetView (context, convertView, parent);
			// if there are test cases inside this suite then create an activity to show them
			if (Suite.TestCaseCount > 0) {		
				view.Click += delegate {
					Intent intent = new Intent(context, typeof (TestSuiteActivity));
					intent.PutExtra ("TestSuite", Name);
					intent.AddFlags (ActivityFlags.NewTask);			
					context.StartActivity (intent);
				};
			}
			return view;
		}
	}
}