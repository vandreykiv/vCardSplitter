using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VCardSplitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ofdSource_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnSelectSource_Click(object sender, EventArgs e)
        {
            if (this.ofdSource.ShowDialog() == DialogResult.OK)
            {
                this.tbSource.Text = this.ofdSource.FileName;
            }
        }

        private void btnSelectTarget_Click(object sender, EventArgs e)
        {
            if (this.fbdTarget.ShowDialog() == DialogResult.OK)
            {
                this.tbTarget.Text = this.fbdTarget.SelectedPath;
            }
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            String source = this.tbSource.Text;
            String target = this.tbTarget.Text;

            if (!File.Exists(source)) return;
            if (!Directory.Exists(target)) return;

            String[] lines = File.ReadAllLines(source);
            String contents = String.Empty;
            String name = String.Empty;
            String lineEdited = String.Empty;

            Boolean isBegin = false;
            Boolean isEnd = false;
            Int32 iFiles = 0;
            this.progress.Maximum = lines.Length;
            foreach (String line in lines)
            {
                if (line == "BEGIN:VCARD") isBegin = true;
                if (line == "END:VCARD") isEnd = true;
                if (line.StartsWith("FN:")) name = line.Substring(3).Replace("\"", "'") + ".vcf";
                if (isBegin) EditVcardEncoding(ref contents, ref lineEdited, line);
                if (isEnd)
                {
                    File.WriteAllText(Path.Combine(target, name), contents);
                    isBegin = false;
                    isEnd = false;
                    contents = String.Empty;
                    iFiles++;
                }
                this.progress.PerformStep();
            }

            MessageBox.Show("Your VCard was split into " + iFiles.ToString() + " files.", "Success", MessageBoxButtons.OK);
        }

        private static void EditVcardEncoding(ref String contents, ref String lineEdited, String line)
        {
            lineEdited = line;
            if (line.StartsWith("FN:")) lineEdited = line.Substring(0, 2) + ";ENCODING=QUOTED-PRINTABLE;CHARSET=UTF-8:" + line.Substring(3);
            if (line.StartsWith("N:")) lineEdited = line.Substring(0, 1) + ";ENCODING=QUOTED-PRINTABLE;CHARSET=UTF-8:" + line.Substring(2).Replace("\\","");
            if (line.StartsWith("ORG:")) lineEdited = line.Substring(0, 3) + ";ENCODING=QUOTED-PRINTABLE;CHARSET=UTF-8:" + line.Substring(4);
            contents += System.Environment.NewLine + lineEdited;
        }
    }
}
