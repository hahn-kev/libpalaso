// ---------------------------------------------------------------------------------------------
using SIL.Windows.Forms.Keyboarding;

#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2013' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using SIL.Windows.Forms.SuperToolTip;

namespace SIL.Windows.Forms.TestApp
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	partial class TestAppForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_KeyboardControllerInitialized)
				{
					KeyboardController.Shutdown();
					_KeyboardControllerInitialized = false;
				}

				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            SIL.Windows.Forms.SuperToolTip.SuperToolTipInfoWrapper superToolTipInfoWrapper1 = new SIL.Windows.Forms.SuperToolTip.SuperToolTipInfoWrapper();
            SIL.Windows.Forms.SuperToolTip.SuperToolTipInfo superToolTipInfo1 = new SIL.Windows.Forms.SuperToolTip.SuperToolTipInfo();
            SIL.Windows.Forms.SuperToolTip.SuperToolTipInfoWrapper superToolTipInfoWrapper2 = new SIL.Windows.Forms.SuperToolTip.SuperToolTipInfoWrapper();
            SIL.Windows.Forms.SuperToolTip.SuperToolTipInfo superToolTipInfo2 = new SIL.Windows.Forms.SuperToolTip.SuperToolTipInfo();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestAppForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._uiLanguageMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnThrowException = new System.Windows.Forms.Button();
            this.btnShowFormWithModalChild = new System.Windows.Forms.Button();
            this.btnTestContributorsList = new System.Windows.Forms.Button();
            this.recordPlayButton = new System.Windows.Forms.Button();
            this.btnFlexibleMessageBox = new System.Windows.Forms.Button();
            this.btnSettingProtectionDialog = new System.Windows.Forms.Button();
            this._silAboutBoxGecko = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.btnMetaDataEditor = new System.Windows.Forms.Button();
            this.btnShowReleaseNotes = new System.Windows.Forms.Button();
            this.btnSilAboutBox = new System.Windows.Forms.Button();
            this.btnImageToolbox = new System.Windows.Forms.Button();
            this.btnWritingSystemSetupDialog = new System.Windows.Forms.Button();
            this.btnLookupISOCodeDialog = new System.Windows.Forms.Button();
            this.btnFolderBrowserControl = new System.Windows.Forms.Button();
            this.superToolTip1 = new SIL.Windows.Forms.SuperToolTip.SuperToolTip(this.components);
            this.superToolTip2 = new SIL.Windows.Forms.SuperToolTip.SuperToolTip(this.components);
            this.btnMediaFileInfo = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._uiLanguageMenu});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(249, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // _uiLanguageMenu
            // 
            this._uiLanguageMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._uiLanguageMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._uiLanguageMenu.Name = "_uiLanguageMenu";
            this._uiLanguageMenu.Size = new System.Drawing.Size(58, 22);
            this._uiLanguageMenu.Text = "English";
            this._uiLanguageMenu.ToolTipText = "User-interface Language";
            // 
            // btnThrowException
            // 
            this.btnThrowException.Location = new System.Drawing.Point(16, 542);
            this.btnThrowException.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnThrowException.Name = "btnThrowException";
            this.btnThrowException.Size = new System.Drawing.Size(209, 28);
            this.btnThrowException.TabIndex = 8;
            this.btnThrowException.Text = "Throw Unhandled Exception";
            this.btnThrowException.UseVisualStyleBackColor = true;
            this.btnThrowException.Click += new System.EventHandler(this.btnThrowException_Click);
            // 
            // btnShowFormWithModalChild
            // 
            this.btnShowFormWithModalChild.Location = new System.Drawing.Point(16, 506);
            this.btnShowFormWithModalChild.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnShowFormWithModalChild.Name = "btnShowFormWithModalChild";
            this.btnShowFormWithModalChild.Size = new System.Drawing.Size(209, 28);
            this.btnShowFormWithModalChild.TabIndex = 7;
            this.btnShowFormWithModalChild.Text = "Form w/ Modal Child...";
            this.btnShowFormWithModalChild.UseVisualStyleBackColor = true;
            this.btnShowFormWithModalChild.Click += new System.EventHandler(this.btnShowFormWithModalChild_Click);
            // 
            // btnTestContributorsList
            // 
            this.btnTestContributorsList.Location = new System.Drawing.Point(16, 470);
            this.btnTestContributorsList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTestContributorsList.Name = "btnTestContributorsList";
            this.btnTestContributorsList.Size = new System.Drawing.Size(209, 28);
            this.btnTestContributorsList.TabIndex = 6;
            this.btnTestContributorsList.Text = "Test Contributors List...";
            this.btnTestContributorsList.UseVisualStyleBackColor = true;
            this.btnTestContributorsList.Click += new System.EventHandler(this.btnTestContributorsList_Click);
            // 
            // recordPlayButton
            // 
            this.recordPlayButton.Location = new System.Drawing.Point(16, 434);
            this.recordPlayButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.recordPlayButton.Name = "recordPlayButton";
            this.recordPlayButton.Size = new System.Drawing.Size(209, 28);
            this.recordPlayButton.TabIndex = 5;
            this.recordPlayButton.Text = "Record and play back sound";
            this.recordPlayButton.UseVisualStyleBackColor = true;
            this.recordPlayButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.recordPlayButton_MouseDown);
            this.recordPlayButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.recordPlayButton_MouseUp);
            // 
            // btnFlexibleMessageBox
            // 
            this.btnFlexibleMessageBox.Location = new System.Drawing.Point(16, 399);
            this.btnFlexibleMessageBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFlexibleMessageBox.Name = "btnFlexibleMessageBox";
            this.btnFlexibleMessageBox.Size = new System.Drawing.Size(209, 28);
            this.btnFlexibleMessageBox.TabIndex = 4;
            this.btnFlexibleMessageBox.Text = "Flexible Message Box";
            this.btnFlexibleMessageBox.UseVisualStyleBackColor = true;
            this.btnFlexibleMessageBox.Click += new System.EventHandler(this.btnFlexibleMessageBox_Click);
            // 
            // btnSettingProtectionDialog
            // 
            this.btnSettingProtectionDialog.Location = new System.Drawing.Point(16, 362);
            this.btnSettingProtectionDialog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSettingProtectionDialog.Name = "btnSettingProtectionDialog";
            this.btnSettingProtectionDialog.Size = new System.Drawing.Size(209, 28);
            this.btnSettingProtectionDialog.TabIndex = 3;
            this.btnSettingProtectionDialog.Text = "SettingProtectionDialog";
            this.btnSettingProtectionDialog.UseVisualStyleBackColor = true;
            this.btnSettingProtectionDialog.Click += new System.EventHandler(this.btnSettingProtectionDialog_Click);
            // 
            // _silAboutBoxGecko
            // 
            this._silAboutBoxGecko.Location = new System.Drawing.Point(16, 220);
            this._silAboutBoxGecko.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._silAboutBoxGecko.Name = "_silAboutBoxGecko";
            this._silAboutBoxGecko.Size = new System.Drawing.Size(209, 28);
            this._silAboutBoxGecko.TabIndex = 2;
            this._silAboutBoxGecko.Text = "SIL AboutBox (Gecko)";
            this._silAboutBoxGecko.UseVisualStyleBackColor = true;
            this._silAboutBoxGecko.Click += new System.EventHandler(this.OnSilAboutBoxGeckoClicked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 617);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 16);
            superToolTipInfo1.BackgroundGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            superToolTipInfo1.BackgroundGradientEnd = System.Drawing.Color.Blue;
            superToolTipInfo1.BackgroundGradientMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            superToolTipInfo1.BodyText = "This is the body text";
            superToolTipInfo1.FooterForeColor = System.Drawing.Color.Lime;
            superToolTipInfo1.FooterText = "And this is the footer";
            superToolTipInfo1.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            superToolTipInfo1.HeaderText = "The header can serve as a title";
            superToolTipInfo1.OffsetForWhereToDisplay = new System.Drawing.Point(0, 0);
            superToolTipInfo1.ShowFooter = true;
            superToolTipInfo1.ShowFooterSeparator = true;
            superToolTipInfoWrapper1.SuperToolTipInfo = superToolTipInfo1;
            superToolTipInfoWrapper1.UseSuperToolTip = true;
            this.superToolTip1.SetSuperStuff(this.label1, superToolTipInfoWrapper1);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hover over me to see a tooltip";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 641);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 16);
            superToolTipInfo2.BackgroundGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            superToolTipInfo2.BackgroundGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(218)))), ((int)(((byte)(239)))));
            superToolTipInfo2.BackgroundGradientMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            superToolTipInfo2.BodyText = resources.GetString("superToolTipInfo2.BodyText");
            superToolTipInfo2.OffsetForWhereToDisplay = new System.Drawing.Point(0, 0);
            superToolTipInfo2.ShowHeader = false;
            superToolTipInfoWrapper2.SuperToolTipInfo = superToolTipInfo2;
            superToolTipInfoWrapper2.UseSuperToolTip = true;
            this.superToolTip2.SetSuperStuff(this.label2, superToolTipInfoWrapper2);
            this.label2.TabIndex = 1;
            this.label2.Text = "Hover for simple, long tooltip";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(16, 326);
            this.btnSelectFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(209, 28);
            this.btnSelectFile.TabIndex = 0;
            this.btnSelectFile.Text = "Select File";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.OnSelectFileClicked);
            // 
            // btnMetaDataEditor
            // 
            this.btnMetaDataEditor.Location = new System.Drawing.Point(16, 290);
            this.btnMetaDataEditor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMetaDataEditor.Name = "btnMetaDataEditor";
            this.btnMetaDataEditor.Size = new System.Drawing.Size(209, 28);
            this.btnMetaDataEditor.TabIndex = 0;
            this.btnMetaDataEditor.Text = "Meta Data Editor";
            this.btnMetaDataEditor.UseVisualStyleBackColor = true;
            this.btnMetaDataEditor.Click += new System.EventHandler(this.OnShowMetaDataEditorClicked);
            // 
            // btnShowReleaseNotes
            // 
            this.btnShowReleaseNotes.Location = new System.Drawing.Point(16, 255);
            this.btnShowReleaseNotes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnShowReleaseNotes.Name = "btnShowReleaseNotes";
            this.btnShowReleaseNotes.Size = new System.Drawing.Size(209, 28);
            this.btnShowReleaseNotes.TabIndex = 0;
            this.btnShowReleaseNotes.Text = "Show Release Notes";
            this.btnShowReleaseNotes.UseVisualStyleBackColor = true;
            this.btnShowReleaseNotes.Click += new System.EventHandler(this.OnShowReleaseNotesClicked);
            // 
            // btnSilAboutBox
            // 
            this.btnSilAboutBox.Location = new System.Drawing.Point(16, 185);
            this.btnSilAboutBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSilAboutBox.Name = "btnSilAboutBox";
            this.btnSilAboutBox.Size = new System.Drawing.Size(209, 28);
            this.btnSilAboutBox.TabIndex = 0;
            this.btnSilAboutBox.Text = "SIL AboutBox";
            this.btnSilAboutBox.UseVisualStyleBackColor = true;
            this.btnSilAboutBox.Click += new System.EventHandler(this.OnSilAboutBoxClicked);
            // 
            // btnImageToolbox
            // 
            this.btnImageToolbox.Location = new System.Drawing.Point(16, 149);
            this.btnImageToolbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnImageToolbox.Name = "btnImageToolbox";
            this.btnImageToolbox.Size = new System.Drawing.Size(209, 28);
            this.btnImageToolbox.TabIndex = 0;
            this.btnImageToolbox.Text = "Image Toolbox";
            this.btnImageToolbox.UseVisualStyleBackColor = true;
            this.btnImageToolbox.Click += new System.EventHandler(this.OnImageToolboxClicked);
            // 
            // btnWritingSystemSetupDialog
            // 
            this.btnWritingSystemSetupDialog.Location = new System.Drawing.Point(16, 113);
            this.btnWritingSystemSetupDialog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnWritingSystemSetupDialog.Name = "btnWritingSystemSetupDialog";
            this.btnWritingSystemSetupDialog.Size = new System.Drawing.Size(209, 28);
            this.btnWritingSystemSetupDialog.TabIndex = 0;
            this.btnWritingSystemSetupDialog.Text = "WritingSystemSetupDialog";
            this.btnWritingSystemSetupDialog.UseVisualStyleBackColor = true;
            this.btnWritingSystemSetupDialog.Click += new System.EventHandler(this.OnWritingSystemSetupDialogClicked);
            // 
            // btnLookupISOCodeDialog
            // 
            this.btnLookupISOCodeDialog.Location = new System.Drawing.Point(16, 78);
            this.btnLookupISOCodeDialog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLookupISOCodeDialog.Name = "btnLookupISOCodeDialog";
            this.btnLookupISOCodeDialog.Size = new System.Drawing.Size(209, 28);
            this.btnLookupISOCodeDialog.TabIndex = 0;
            this.btnLookupISOCodeDialog.Text = "LanguageLookupDialog";
            this.btnLookupISOCodeDialog.UseVisualStyleBackColor = true;
            this.btnLookupISOCodeDialog.Click += new System.EventHandler(this.OnLanguageLookupDialogClicked);
            // 
            // btnFolderBrowserControl
            // 
            this.btnFolderBrowserControl.Location = new System.Drawing.Point(16, 42);
            this.btnFolderBrowserControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFolderBrowserControl.Name = "btnFolderBrowserControl";
            this.btnFolderBrowserControl.Size = new System.Drawing.Size(209, 28);
            this.btnFolderBrowserControl.TabIndex = 0;
            this.btnFolderBrowserControl.Text = "FolderBrowserControl";
            this.btnFolderBrowserControl.UseVisualStyleBackColor = true;
            this.btnFolderBrowserControl.Click += new System.EventHandler(this.OnFolderBrowserControlClicked);
            // 
            // superToolTip1
            // 
            this.superToolTip1.FadingInterval = 10;
            // 
            // superToolTip2
            // 
            this.superToolTip2.FadingInterval = 10;
            // 
            // btnMediaFileInfo
            // 
            this.btnMediaFileInfo.Location = new System.Drawing.Point(19, 577);
            this.btnMediaFileInfo.Name = "btnMediaFileInfo";
            this.btnMediaFileInfo.Size = new System.Drawing.Size(206, 28);
            this.btnMediaFileInfo.TabIndex = 10;
            this.btnMediaFileInfo.Text = "Get Media File Info";
            this.btnMediaFileInfo.UseVisualStyleBackColor = true;
            this.btnMediaFileInfo.Click += new System.EventHandler(this.btnMediaFileInfo_Click);
            // 
            // TestAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 671);
            this.Controls.Add(this.btnMediaFileInfo);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnThrowException);
            this.Controls.Add(this.btnShowFormWithModalChild);
            this.Controls.Add(this.btnTestContributorsList);
            this.Controls.Add(this.recordPlayButton);
            this.Controls.Add(this.btnFlexibleMessageBox);
            this.Controls.Add(this.btnSettingProtectionDialog);
            this.Controls.Add(this._silAboutBoxGecko);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.btnMetaDataEditor);
            this.Controls.Add(this.btnShowReleaseNotes);
            this.Controls.Add(this.btnSilAboutBox);
            this.Controls.Add(this.btnImageToolbox);
            this.Controls.Add(this.btnWritingSystemSetupDialog);
            this.Controls.Add(this.btnLookupISOCodeDialog);
            this.Controls.Add(this.btnFolderBrowserControl);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TestAppForm";
            this.Text = "SIL.Windows.Forms.TestApp";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnFolderBrowserControl;
		private System.Windows.Forms.Button btnLookupISOCodeDialog;
		private System.Windows.Forms.Button btnWritingSystemSetupDialog;
		private System.Windows.Forms.Button btnImageToolbox;
		private System.Windows.Forms.Button btnSilAboutBox;
		private System.Windows.Forms.Button btnShowReleaseNotes;
		private SuperToolTip.SuperToolTip superToolTip1;
		private System.Windows.Forms.Label label1;
		private SuperToolTip.SuperToolTip superToolTip2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnMetaDataEditor;
		private System.Windows.Forms.Button btnSelectFile;
		private System.Windows.Forms.Button _silAboutBoxGecko;
		private System.Windows.Forms.Button btnSettingProtectionDialog;
		private System.Windows.Forms.Button btnFlexibleMessageBox;
		private System.Windows.Forms.Button recordPlayButton;
		private System.Windows.Forms.Button btnTestContributorsList;
		private System.Windows.Forms.Button btnShowFormWithModalChild;
		private System.Windows.Forms.Button btnThrowException;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripDropDownButton _uiLanguageMenu;
		private System.Windows.Forms.Button btnMediaFileInfo;
	}
}
