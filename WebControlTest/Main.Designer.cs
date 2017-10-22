namespace ForeFuelSimulator
{
    partial class Main
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
            if (disposing && (components != null))
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.MainPage = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // MainPage
            // 
            this.MainPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPage.Location = new System.Drawing.Point(0, 0);
            this.MainPage.MinimumSize = new System.Drawing.Size(20, 20);
            this.MainPage.Name = "MainPage";
            this.MainPage.ScrollBarsEnabled = false;
            this.MainPage.Size = new System.Drawing.Size(1264, 682);
            this.MainPage.TabIndex = 0;
            this.MainPage.Url = new System.Uri("", System.UriKind.Relative);
            this.MainPage.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.MainPage_DocumentCompleted);
            this.MainPage.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MainPage_PreviewKeyDown);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.MainPage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "ForeFuel Simulator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser MainPage;
    }
}

