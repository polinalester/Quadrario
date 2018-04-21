namespace GameClient
{
    partial class GameWindow
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ConnectButton = new System.Windows.Forms.Button();
            this.IDTextbox = new System.Windows.Forms.TextBox();
            this.IPTextbox = new System.Windows.Forms.TextBox();
            this.IDLabel = new System.Windows.Forms.Label();
            this.IPLabel = new System.Windows.Forms.Label();
            this.NotifLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(362, 14);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 0;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // IDTextbox
            // 
            this.IDTextbox.Location = new System.Drawing.Point(61, 16);
            this.IDTextbox.Name = "IDTextbox";
            this.IDTextbox.Size = new System.Drawing.Size(100, 20);
            this.IDTextbox.TabIndex = 1;
            // 
            // IPTextbox
            // 
            this.IPTextbox.Location = new System.Drawing.Point(237, 16);
            this.IPTextbox.Name = "IPTextbox";
            this.IPTextbox.Size = new System.Drawing.Size(100, 20);
            this.IPTextbox.TabIndex = 2;
            // 
            // IDLabel
            // 
            this.IDLabel.AutoSize = true;
            this.IDLabel.Location = new System.Drawing.Point(12, 19);
            this.IDLabel.Name = "IDLabel";
            this.IDLabel.Size = new System.Drawing.Size(43, 13);
            this.IDLabel.TabIndex = 3;
            this.IDLabel.Text = "Your ID";
            // 
            // IPLabel
            // 
            this.IPLabel.AutoSize = true;
            this.IPLabel.Location = new System.Drawing.Point(180, 19);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(51, 13);
            this.IPLabel.TabIndex = 4;
            this.IPLabel.Text = "Server IP";
            // 
            // NotifLabel
            // 
            this.NotifLabel.AutoSize = true;
            this.NotifLabel.Location = new System.Drawing.Point(462, 18);
            this.NotifLabel.Name = "NotifLabel";
            this.NotifLabel.Size = new System.Drawing.Size(0, 13);
            this.NotifLabel.TabIndex = 5;
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(684, 561);
            this.Controls.Add(this.NotifLabel);
            this.Controls.Add(this.IPLabel);
            this.Controls.Add(this.IDLabel);
            this.Controls.Add(this.IPTextbox);
            this.Controls.Add(this.IDTextbox);
            this.Controls.Add(this.ConnectButton);
            this.KeyPreview = true;
            this.Name = "GameWindow";
            this.Text = "Quadrario";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameWindow_FormClosed);
            this.Load += new System.EventHandler(this.GameWindow_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GameWindow_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.TextBox IDTextbox;
        private System.Windows.Forms.TextBox IPTextbox;
        private System.Windows.Forms.Label IDLabel;
        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.Label NotifLabel;
    }
}

