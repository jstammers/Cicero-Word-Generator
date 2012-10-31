﻿namespace WordGenerator.Controls.Dialogs
{
    partial class PulseManager
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
            this.doneButton = new System.Windows.Forms.Button();
            this.AvailablePulseListBox = new System.Windows.Forms.ListBox();
            this.UsedPulseListBox = new System.Windows.Forms.ListBox();
            this.UsedPulseLabel = new System.Windows.Forms.Label();
            this.AvailablePulseLabel = new System.Windows.Forms.Label();
            this.UsedToAvButton = new System.Windows.Forms.Button();
            this.AvToUsedButton = new System.Windows.Forms.Button();
            this.ClearPulsesButton = new System.Windows.Forms.Button();
            //this.pulseVisualizer1 = new WordGenerator.Controls.Dialogs.PulseVisualizer();
            this.SuspendLayout();
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(184, 384);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 0;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.closeWindow);
            // 
            // AvailablePulseListBox
            // 
            this.AvailablePulseListBox.FormattingEnabled = true;
            this.AvailablePulseListBox.Location = new System.Drawing.Point(29, 259);
            this.AvailablePulseListBox.Name = "AvailablePulseListBox";
            this.AvailablePulseListBox.Size = new System.Drawing.Size(120, 95);
            this.AvailablePulseListBox.TabIndex = 1;
            this.AvailablePulseListBox.DoubleClick += new System.EventHandler(this.addPulseToUsedList);
            // 
            // UsedPulseListBox
            // 
            this.UsedPulseListBox.FormattingEnabled = true;
            this.UsedPulseListBox.Location = new System.Drawing.Point(283, 259);
            this.UsedPulseListBox.Name = "UsedPulseListBox";
            this.UsedPulseListBox.Size = new System.Drawing.Size(120, 95);
            this.UsedPulseListBox.TabIndex = 2;
            this.UsedPulseListBox.DoubleClick += new System.EventHandler(this.removePulseFromUsedList);
            // 
            // UsedPulseLabel
            // 
            this.UsedPulseLabel.AutoSize = true;
            this.UsedPulseLabel.Location = new System.Drawing.Point(280, 243);
            this.UsedPulseLabel.Name = "UsedPulseLabel";
            this.UsedPulseLabel.Size = new System.Drawing.Size(66, 13);
            this.UsedPulseLabel.TabIndex = 3;
            this.UsedPulseLabel.Text = "Used Pulses";
            // 
            // AvailablePulseLabel
            // 
            this.AvailablePulseLabel.AutoSize = true;
            this.AvailablePulseLabel.Location = new System.Drawing.Point(26, 243);
            this.AvailablePulseLabel.Name = "AvailablePulseLabel";
            this.AvailablePulseLabel.Size = new System.Drawing.Size(84, 13);
            this.AvailablePulseLabel.TabIndex = 4;
            this.AvailablePulseLabel.Text = "Available Pulses";
            // 
            // UsedToAvButton
            // 
            this.UsedToAvButton.Location = new System.Drawing.Point(184, 315);
            this.UsedToAvButton.Name = "UsedToAvButton";
            this.UsedToAvButton.Size = new System.Drawing.Size(75, 23);
            this.UsedToAvButton.TabIndex = 5;
            this.UsedToAvButton.Text = "<----";
            this.UsedToAvButton.UseVisualStyleBackColor = true;
            this.UsedToAvButton.Click += new System.EventHandler(this.removePulseFromUsedList);
            // 
            // AvToUsedButton
            // 
            this.AvToUsedButton.Location = new System.Drawing.Point(184, 275);
            this.AvToUsedButton.Name = "AvToUsedButton";
            this.AvToUsedButton.Size = new System.Drawing.Size(75, 23);
            this.AvToUsedButton.TabIndex = 6;
            this.AvToUsedButton.Text = "---->";
            this.AvToUsedButton.UseVisualStyleBackColor = true;
            this.AvToUsedButton.Click += new System.EventHandler(this.addPulseToUsedList);
            // 
            // ClearPulsesButton
            // 
            this.ClearPulsesButton.Location = new System.Drawing.Point(306, 360);
            this.ClearPulsesButton.Name = "ClearPulsesButton";
            this.ClearPulsesButton.Size = new System.Drawing.Size(75, 23);
            this.ClearPulsesButton.TabIndex = 7;
            this.ClearPulsesButton.Text = "Clear Pulses";
            this.ClearPulsesButton.UseVisualStyleBackColor = true;
            this.ClearPulsesButton.Click += new System.EventHandler(this.clearUsedList);
            // 
            // pulseVisualizer1
            //
            //Temporarily Removed pulseVisualizer
           /* 
            this.pulseVisualizer1.Location = new System.Drawing.Point(29, 12);
            this.pulseVisualizer1.Name = "pulseVisualizer1";
            this.pulseVisualizer1.Size = new System.Drawing.Size(374, 210);
            this.pulseVisualizer1.TabIndex = 8;
            */
            // 
            // PulseManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 429);
            //this.Controls.Add(this.pulseVisualizer1);
            this.Controls.Add(this.ClearPulsesButton);
            this.Controls.Add(this.AvToUsedButton);
            this.Controls.Add(this.UsedToAvButton);
            this.Controls.Add(this.AvailablePulseLabel);
            this.Controls.Add(this.UsedPulseLabel);
            this.Controls.Add(this.UsedPulseListBox);
            this.Controls.Add(this.AvailablePulseListBox);
            this.Controls.Add(this.doneButton);
            this.Name = "PulseManager";
            this.Text = "PulseManager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.ListBox AvailablePulseListBox;
        private System.Windows.Forms.ListBox UsedPulseListBox;
        private System.Windows.Forms.Label UsedPulseLabel;
        private System.Windows.Forms.Label AvailablePulseLabel;
        private System.Windows.Forms.Button UsedToAvButton;
        private System.Windows.Forms.Button AvToUsedButton;
        private System.Windows.Forms.Button ClearPulsesButton;
        // private PulseVisualizer pulseVisualizer1;
    }
}