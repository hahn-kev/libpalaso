﻿using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Palaso.Extensions;

namespace Palaso.Tests.Extensions
{
	[TestFixture]
	public class XElementExtensionTests
	{
		private static XNamespace Sil = "urn://www.sil.org/ldml/0.1";

		private static readonly string Contents =
@"<?xml version='1.0' encoding='utf-8'?>
<ldml xmlns:sil='urn://www.sil.org/ldml/0.1'>
	<identity version='3.14' sil:date='Jan 1, 2015'>Identity description</identity>
	<sil:special version='10'>Special description</sil:special>
</ldml>".Replace("'", "\"");

		[Test]
		public void GetNullAttribute()
		{
			XElement root = XElement.Parse(Contents);
			string attribute = root.GetAttributeValue("date");
			Assert.That(attribute.Equals(string.Empty));
		}

		[Test]
		public void GetChildNullAttribute()
		{
			XElement root = XElement.Parse(Contents);
			string attribute = root.GetAttributeValue("identity", "date");
			Assert.That(attribute.Equals(string.Empty));
		}

		[Test]
		public void GetAttribute()
		{
			XElement root = XElement.Parse(Contents);
			XElement identityElem = root.Element("identity");
			string attribute = identityElem.GetAttributeValue("version");
			Assert.That(attribute.Equals("3.14"));
		}

		[Test]
		public void GetNamespaceAttribute()
		{
			XElement root = XElement.Parse(Contents);
			XElement identityElem = root.Element("identity");
			string attribute = identityElem.GetAttributeValue(Sil + "date");
			Assert.That(attribute.Equals("Jan 1, 2015"));
		}

		[Test]
		public void GetOrSetElement()
		{
			XElement root = XElement.Parse(Contents);
			// Test child previously doesn't exist and will be created
			Assert.Null(root.Element("child"));
			XElement childElem = root.GetOrCreateElement("child");
			Assert.That(root.Element("child"), Is.EqualTo(childElem));
			// Test Sil:child previously doesn't exist and will be created
			Assert.Null(root.Element(Sil + "child"));
			XElement silChildElem = root.GetOrCreateElement(Sil + "child");
			Assert.That(root.Element(Sil + "child"), Is.EqualTo(silChildElem));
		}

		[Test]
		public void SetNewChildAttribute()
		{
			XElement root = XElement.Parse(Contents);
			// Assert child element exists, but attribute does not
			Assert.That(root.Elements("identity").Any(), Is.True);
			Assert.That(root.GetAttributeValue("identity", "color"), Is.EqualTo(string.Empty));
			// Set the new attribute
			root.SetAttributeValue("identity", "color", "blue");
			Assert.That(root.GetAttributeValue("identity", "color"), Is.EqualTo("blue"));
		}

		[Test]
		public void SetNewChildNamespaceAttribute()
		{
			XElement root = XElement.Parse(Contents);
			// Assert child element exists, but attribute does not
			Assert.That(root.Elements(Sil + "special").Any(), Is.True);
			Assert.That(root.GetAttributeValue(Sil + "special", "color"), Is.EqualTo(string.Empty));
			// Set the attribute
			root.SetAttributeValue(Sil + "special", "color", "blue");
			Assert.That(root.GetAttributeValue(Sil + "special", "color").Equals("blue"));
		}

		[Test]
		public void GetAndSetChildAttribute()
		{
			XElement root = XElement.Parse(Contents);
			string attribute = root.GetAttributeValue("identity", "version");
			Assert.That(attribute.Equals("3.14"));
			root.SetAttributeValue("identity", "version", "4.0");
			Assert.That(root.GetAttributeValue("identity", "version"), Is.EqualTo("4.0"));
			// Remove the attribute
			root.SetAttributeValue("identity", "version", null);
			XAttribute attr = root.Element("identity").Attribute("version");
			Assert.Null(attr);
			// Attempt to remove the attribute again
			root.SetAttributeValue("identity", "version", null);
			attr = root.Element("identity").Attribute("version");
			Assert.Null(attr);
		}

		[Test]
		public void GetChildNamespaceNullAttribute()
		{
			XElement root = XElement.Parse(Contents);
			string attribute = root.GetAttributeValue(Sil + "special", "date");
			Assert.That(attribute.Equals(string.Empty));
			
		}

		[Test]
		public void GetAndSetChildNamespaceAttribute()
		{
			XElement root = XElement.Parse(Contents);
			string attribute = root.GetAttributeValue(Sil + "special", "version");
			Assert.That(attribute.Equals("10"));
			root.SetAttributeValue(Sil + "special", "version", "4.0");
			Assert.That(root.GetAttributeValue(Sil + "special", "version"), Is.EqualTo("4.0"));
			// Remove the attribute
			root.SetAttributeValue(Sil + "special", "version", null);
			XAttribute attr = root.Element(Sil + "special").Attribute("version");
			Assert.Null(attr);
			// Attempt to remove the attribute again
			root.SetAttributeValue(Sil + "special", "version", null);
			attr = root.Element(Sil + "special").Attribute("version");
			Assert.Null(attr);
		}
	}
}