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

using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace Android.NUnitLite.UI {
	
	abstract class TestElement : FormattedElement {
		
		string name;
		TestResult result;
		
        public TestElement (ITest test, AndroidRunner runner) : base (String.Empty)
		{
			if (test == null)
				throw new ArgumentNullException ("test");
		
			Test = test;
			name = test.FullName ?? test.Name;
			Runner = runner;
		}

		protected AndroidRunner Runner { get; private set; }
		
		protected string Name {
			get { return name; }
		}
				
		protected TestResult Result {
			get { return result ?? new TestCaseResult (Test as TestMethod); }
			set { result = value; }
		}
		
		protected ITest Test { get; private set; }
		
		public void Update (TestResult result)
		{
			Result = result;

			Update ();
		}

		abstract public void Update ();
	}
}