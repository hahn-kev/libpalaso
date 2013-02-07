﻿using System;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Palaso.Media.Naudio.UI
{
	public partial class RecordingDeviceButton : UserControl
	{
		private IAudioRecorder _recorder;

		public RecordingDeviceButton()
		{
			InitializeComponent();
		}

		public IAudioRecorder Recorder
		{
			get { return _recorder; }
			set
			{
				_recorder = value;
				if (_recorder != null)
				{
					toolTip1.SetToolTip(_recordingDeviceImage, value.SelectedDevice.Capabilities.ProductName);
					if (IsHandleCreated)
						UpdateDisplay();
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			UpdateDisplay();
			base.OnLoad(e);
		}

		public void UpdateDisplay()
		{
			if (_recorder == null)
				return;

			if(_recorder.SelectedDevice.GenericName.Contains("Internal"))
				_recordingDeviceImage.Image = AudioDeviceIcons.Computer;
			else if (_recorder.SelectedDevice.GenericName.Contains("USB Audio Device"))
				_recordingDeviceImage.Image = AudioDeviceIcons.HeadSet;
			else if (_recorder.SelectedDevice.GenericName.Contains("Microphone"))
				_recordingDeviceImage.Image = AudioDeviceIcons.Microphone;

			var deviceName = _recorder.SelectedDevice.ProductName;

			if(deviceName.Contains("ZOOM"))
				_recordingDeviceImage.Image = AudioDeviceIcons.Recorder;
			else if (deviceName.Contains("Plantronics") || deviceName.Contains("Andrea"))
				_recordingDeviceImage.Image = AudioDeviceIcons.HeadSet;
			else if (deviceName.Contains("Line"))
				_recordingDeviceImage.Image = AudioDeviceIcons.ExternalAudioDevice;

			// REVIEW: For some reason, the icons used to represent the different devices are all different sizes
			// and proportions. Best approach seems to be to scale them down to fit but not scale them up
			// because they will pixelate. It would probably be better to get somebody with an eye for design to
			// come up with consistent looking icons that are of the same size and scale nicely.
			_recordingDeviceImage.SizeMode =
				(_recordingDeviceImage.Image.Height > Height || _recordingDeviceImage.Image.Width > Width) ?
				PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
		}
	}
}
