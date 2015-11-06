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
using Android.OS;
using Android.Widget;

using MonoDroid.Dialog;
using NUnit.Framework.Internal;
using NUnit.Framework.Api;
using System.Collections.Generic;

namespace Android.NUnitLite.UI {

	[Activity (Label = "Tests")]			
	public class TestSuiteActivity : Activity {
		
		string test_suite;
		TestSuite suite;
		Section main;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			test_suite = Intent.GetStringExtra ("TestSuite");
            if (!AndroidRunner.Suites.TryGetValue(test_suite, out suite)) {
                suite = null;
            }

			var menu = new RootElement (String.Empty);
			
			main = new Section (test_suite);
            if (suite != null) {
                foreach (ITest test in suite.Tests) {
                    TestSuite ts = test as TestSuite;
                    if (ts != null)
                        main.Add(new TestSuiteElement(ts, AndroidRunner.Runner));
                    else
                        main.Add(new TestCaseElement(test as TestMethod, AndroidRunner.Runner));
                }
            }
			menu.Add (main);

			Section options = new Section () {
				new ActionElement ("Run all", Run),
			};
			menu.Add (options);

			var da = new DialogAdapter (this, menu);
			var lv = new ListView (this) {
				Adapter = da
			};
			SetContentView (lv);
		}
		
		public void Run ()
		{
			AndroidRunner runner = AndroidRunner.Runner;
			if (!runner.OpenWriter ("Run " + test_suite, this))
				return;
			
            var results = default(TestResult);
			try {
                results = AndroidRunner.Runner.Run(suite);
			}
			finally {
				runner.CloseWriter ();
			}
			
            int index = 0;
			foreach (TestElement te in main) {
                if (results == null || index >= results.Children.Count) {
                    te.Update();
                    continue;
                }

                te.Update (results.Children[index++] as TestResult);
			}
		}
	}
}