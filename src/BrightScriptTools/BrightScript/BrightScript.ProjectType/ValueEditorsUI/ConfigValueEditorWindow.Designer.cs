namespace BrightScript.ValueEditorsUI
{
    partial class ConfigValueEditorWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelBnt = new System.Windows.Forms.Button();
            this.okBnt = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.configParamsGrid = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.replacesGrid = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configParamsGrid)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.replacesGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelBnt);
            this.panel1.Controls.Add(this.okBnt);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 437);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(652, 39);
            this.panel1.TabIndex = 1;
            // 
            // cancelBnt
            // 
            this.cancelBnt.Dock = System.Windows.Forms.DockStyle.Right;
            this.cancelBnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelBnt.Location = new System.Drawing.Point(328, 0);
            this.cancelBnt.Name = "cancelBnt";
            this.cancelBnt.Size = new System.Drawing.Size(324, 39);
            this.cancelBnt.TabIndex = 1;
            this.cancelBnt.Text = "Cancel";
            this.cancelBnt.UseVisualStyleBackColor = true;
            this.cancelBnt.Click += new System.EventHandler(this.cancelBnt_Click);
            // 
            // okBnt
            // 
            this.okBnt.Dock = System.Windows.Forms.DockStyle.Left;
            this.okBnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okBnt.Location = new System.Drawing.Point(0, 0);
            this.okBnt.Name = "okBnt";
            this.okBnt.Size = new System.Drawing.Size(322, 39);
            this.okBnt.TabIndex = 0;
            this.okBnt.Text = "Ok";
            this.okBnt.UseVisualStyleBackColor = true;
            this.okBnt.Click += new System.EventHandler(this.okBnt_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(652, 437);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.configParamsGrid);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 217);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config params";
            // 
            // configParamsGrid
            // 
            this.configParamsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configParamsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configParamsGrid.Location = new System.Drawing.Point(3, 22);
            this.configParamsGrid.Name = "configParamsGrid";
            this.configParamsGrid.Size = new System.Drawing.Size(646, 192);
            this.configParamsGrid.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.replacesGrid);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(652, 216);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Replaces";
            // 
            // replacesGrid
            // 
            this.replacesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.replacesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replacesGrid.Location = new System.Drawing.Point(3, 22);
            this.replacesGrid.Name = "replacesGrid";
            this.replacesGrid.Size = new System.Drawing.Size(646, 191);
            this.replacesGrid.TabIndex = 0;
            // 
            // ConfigValueEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 476);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "ConfigValueEditorWindow";
            this.Text = "EditorWindow";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.configParamsGrid)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.replacesGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelBnt;
        private System.Windows.Forms.Button okBnt;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView configParamsGrid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView replacesGrid;
    }
}