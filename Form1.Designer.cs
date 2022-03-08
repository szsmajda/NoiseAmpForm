namespace NoiseAmpControlApp
{
    partial class Form1
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
            this.SpeakOut_button = new System.Windows.Forms.Button();
            this.NoiseMeasure_button = new System.Windows.Forms.Button();
            this.SerialConsoleTextBox = new System.Windows.Forms.TextBox();
            this.keepAlive_button = new System.Windows.Forms.Button();
            this.OutputConsoleTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SpeakOut_button
            // 
            this.SpeakOut_button.Enabled = false;
            this.SpeakOut_button.Location = new System.Drawing.Point(132, 12);
            this.SpeakOut_button.Name = "SpeakOut_button";
            this.SpeakOut_button.Size = new System.Drawing.Size(114, 23);
            this.SpeakOut_button.TabIndex = 0;
            this.SpeakOut_button.Text = "SPEAK OUT";
            this.SpeakOut_button.UseVisualStyleBackColor = true;
            this.SpeakOut_button.Click += new System.EventHandler(this.SpeakOut_button_Click);
            // 
            // NoiseMeasure_button
            // 
            this.NoiseMeasure_button.Enabled = false;
            this.NoiseMeasure_button.Location = new System.Drawing.Point(252, 12);
            this.NoiseMeasure_button.Name = "NoiseMeasure_button";
            this.NoiseMeasure_button.Size = new System.Drawing.Size(113, 23);
            this.NoiseMeasure_button.TabIndex = 1;
            this.NoiseMeasure_button.Text = "NOISE MEASURE";
            this.NoiseMeasure_button.UseVisualStyleBackColor = true;
            this.NoiseMeasure_button.Click += new System.EventHandler(this.NoiseMeasure_button_click);
            // 
            // SerialConsoleTextBox
            // 
            this.SerialConsoleTextBox.Location = new System.Drawing.Point(13, 56);
            this.SerialConsoleTextBox.Multiline = true;
            this.SerialConsoleTextBox.Name = "SerialConsoleTextBox";
            this.SerialConsoleTextBox.ReadOnly = true;
            this.SerialConsoleTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SerialConsoleTextBox.Size = new System.Drawing.Size(568, 100);
            this.SerialConsoleTextBox.TabIndex = 2;
            // 
            // keepAlive_button
            // 
            this.keepAlive_button.Location = new System.Drawing.Point(12, 13);
            this.keepAlive_button.Name = "keepAlive_button";
            this.keepAlive_button.Size = new System.Drawing.Size(114, 23);
            this.keepAlive_button.TabIndex = 3;
            this.keepAlive_button.Text = "KEEPALIVE";
            this.keepAlive_button.UseVisualStyleBackColor = true;
            this.keepAlive_button.Click += new System.EventHandler(this.KeepAlive_button_Click);
            // 
            // OutputConsoleTextBox
            // 
            this.OutputConsoleTextBox.Location = new System.Drawing.Point(13, 164);
            this.OutputConsoleTextBox.Multiline = true;
            this.OutputConsoleTextBox.Name = "OutputConsoleTextBox";
            this.OutputConsoleTextBox.ReadOnly = true;
            this.OutputConsoleTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputConsoleTextBox.Size = new System.Drawing.Size(568, 100);
            this.OutputConsoleTextBox.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 276);
            this.Controls.Add(this.OutputConsoleTextBox);
            this.Controls.Add(this.keepAlive_button);
            this.Controls.Add(this.SerialConsoleTextBox);
            this.Controls.Add(this.NoiseMeasure_button);
            this.Controls.Add(this.SpeakOut_button);
            this.Name = "Form1";
            this.Text = "Noise Controlled Streamer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SpeakOut_button;
        private System.Windows.Forms.Button NoiseMeasure_button;
        private System.Windows.Forms.TextBox SerialConsoleTextBox;
        private System.Windows.Forms.Button keepAlive_button;
        private System.Windows.Forms.TextBox OutputConsoleTextBox;
    }
}

