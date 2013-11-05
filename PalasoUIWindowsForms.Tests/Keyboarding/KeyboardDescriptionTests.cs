// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: KeyboardDescriptionTests.cs
// Responsibility: eberhard
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.UI.WindowsForms.Keyboarding;
using Palaso.UI.WindowsForms.Keyboarding.Types;
using Palaso.WritingSystems;
using Palaso.Tests.Code;

namespace PalasoUIWindowsForms.Tests.Keyboarding
{
	[TestFixture]
	public class KeyboardDescriptionIClonableGenericTests : IClonableGenericTests<KeyboardDescription>
	{
		public override KeyboardDescription CreateNewClonable()
		{
			return new KeyboardDescription("foo", "foo", "en-US", null, null);
		}

		public override string ExceptionList
		{
			get { return "|Engine|InputLanguage|"; }
		}

		public override string EqualsExceptionList
		{
			get { return "|Type|Name|OperatingSystem|IsAvailable|"; }
		}

		protected override List<ValuesToSet> DefaultValuesForTypes
		{
			get
			{
				return new List<ValuesToSet>
					{
						new ValuesToSet(false, true),
						new ValuesToSet("to be", "!(to be)"),
						new ValuesToSet(PlatformID.Win32NT, PlatformID.Unix),
						new ValuesToSet(KeyboardType.System, KeyboardType.OtherIm)
					};
			}
		}
	}
}
