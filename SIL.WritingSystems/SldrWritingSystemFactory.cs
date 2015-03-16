﻿using System.IO;
using System.Net;

namespace SIL.WritingSystems
{
	public class SldrWritingSystemFactory : SldrWritingSystemFactory<WritingSystemDefinition>
	{
		protected override WritingSystemDefinition ConstructDefinition()
		{
			return new WritingSystemDefinition();
		}

		protected override WritingSystemDefinition ConstructDefinition(string ietfLanguageTag)
		{
			return new WritingSystemDefinition(ietfLanguageTag);
		}

		protected override WritingSystemDefinition ConstructDefinition(WritingSystemDefinition ws)
		{
			return new WritingSystemDefinition(ws);
		}
	}

	public abstract class SldrWritingSystemFactory<T> : WritingSystemFactoryBase<T> where T : WritingSystemDefinition
	{
		public override T Create(string ietfLanguageTag)
		{
			// check SLDR for template
			string sldrCachePath = Path.Combine(Path.GetTempPath(), "SldrCache");
			Directory.CreateDirectory(sldrCachePath);
			string templatePath = Path.Combine(sldrCachePath, ietfLanguageTag + ".ldml");
			string filename;
			if (GetLdmlFromSldr(templatePath, ietfLanguageTag, out filename) == SldrStatus.FileNotFound)
			{
				// check SLDR cache for template
				if (!File.Exists(templatePath))
					templatePath = null;
			}

			// check template folder for template
			if (string.IsNullOrEmpty(templatePath) && !string.IsNullOrEmpty(TemplateFolder))
			{
				templatePath = Path.Combine(TemplateFolder, ietfLanguageTag + ".ldml");
				if (!File.Exists(templatePath))
					templatePath = null;
			}

			T ws;
			if (!string.IsNullOrEmpty(templatePath))
			{
				ws = ConstructDefinition();
				var loader = new LdmlDataMapper(this);
				loader.Read(templatePath, ws);
				ws.Template = templatePath;
			}
			else
			{
				ws = ConstructDefinition(ietfLanguageTag);
			}

			return ws;
		}

		/// <summary>
		/// The folder in which the repository looks for template LDML files when a writing system is wanted
		/// that cannot be found in the local store, global store, or SLDR.
		/// </summary>
		public string TemplateFolder { get; set; }

		/// <summary>
		/// Gets the a LDML file from the SLDR.
		/// </summary>
		protected virtual SldrStatus GetLdmlFromSldr(string path, string id, out string filename)
		{
			
				return Sldr.GetLdmlFile(path, id, out filename);
		}
	}
}