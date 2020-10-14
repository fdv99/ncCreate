using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ncCreate
{
    public partial class Form1 : Form
    {
        // Declare the filename variable
        private string ncFileName = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_OpenFile_Click(object sender, EventArgs e)
        {
            Stream myStream;

            // Choose the nc file you want to edit
            var openFileDialog1 = new OpenFileDialog();

            // Filter the results so you only see .nc files and text files
            openFileDialog1.Filter = "dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    ncFileName = openFileDialog1.FileName;
                    original_code.Text = File.ReadAllText(ncFileName);
                }
            }
        }

        private void Btn_NcCreate_Click(object sender, EventArgs e)
        {
            // Convert the original file to a list
            List<string> convertList = File.ReadAllLines(ncFileName).ToList();

            // Get the number of lines in List
            int convertLength = convertList.Count;

            // Go through list and copy all Outside lines/arcs to outside list
            List<string> outsideEntities = new List<string>();

            for (int i=1; i< convertLength; i+=1)
            {
                // Get current line on list
                string current = convertList[i];

                if (current == "LINE")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("IN X" + convertList[i + 12] + " Y" + convertList[i + 14]);
                        outsideEntities.Add("IN X" + convertList[i + 18] + " Y" + convertList[i + 20]);
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("OUT X" + convertList[i + 12] + " Y" + convertList[i + 14]);
                        outsideEntities.Add("OUT X" + convertList[i + 18] + " Y" + convertList[i + 20]);
                    }
                }

                if (current == "ARC")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("iARC X" + convertList[i + 12] + " Y" + convertList[i + 14] + " R" + convertList[i + 18]);
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("oARC X" + convertList[i + 12] + " Y" + convertList[i + 14] + " R" + convertList[i + 18]);
                    }
                }

                if (current == "CIRCLE")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("iCIRC X" + convertList[i + 12] + " Y" + convertList[i + 14] + " R" + convertList[i + 18]);
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("oCIRC X" + convertList[i + 12] + " Y" + convertList[i + 14] + " R" + convertList[i + 18]);

                    }
                }
                

            }

            outsideEntities.Add("G93 X0.0Y0.0Z0.0");
            outsideEntities.Add("G130");
            outsideEntities.Add("/M707");
            outsideEntities.Add("G50");
            converted_code.Lines = outsideEntities.ToArray();
        }
    }
}
