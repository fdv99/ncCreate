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

            /// dxf file info
            /// LINE
            /// 10 - start x
            /// 20 - start y
            /// 30 - start z - should always be zero for us
            /// 11 - end x
            /// 21 - end y
            /// 8 - layer
            /// 
            /// ARC
            /// 8 - layer
            /// 10 - start x
            /// 20 - start y
            /// 40 - radius
            /// 50 - start angle
            /// 51 - end angle
            /// 
            
            
            for (int i=1; i< convertLength; i+=1)
            {
                // Get current line on list
                string current = convertList[i];

                if (current == "LINE")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add(Math.Round(Convert.ToDecimal(convertList[i + 12]), 4) + " " + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4));
                        //outsideEntities.Add(Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + " " + Math.Round(Convert.ToDecimal(convertList[i + 20]), 4));

                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add(Math.Round(Convert.ToDecimal(convertList[i + 12]), 4) + " " + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4));
                        //outsideEntities.Add("OX" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 20]), 4));
                    }
                }

                if (current == "ARC")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("i" + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + " R" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4));
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("arc " + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + " R" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4));
                    }
                }

                if (current == "CIRCLE")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("iCIRC X" + Math.Round(Convert.ToDecimal(convertList[i + 12]),4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 14]),4) + " R" + convertList[i + 18]);
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("oCIRC X" + convertList[i + 12] + " Y" + convertList[i + 14] + " R" + convertList[i + 18]);

                    }
                }
                int number = 27;
                outsideEntities.Add((Decimal.Round(number), 5).ToString());

            }

            outsideEntities.Add("G93 X0.0Y0.0Z0.0");
            outsideEntities.Add("G130");
            outsideEntities.Add("/M707");
            outsideEntities.Add("G50");
            converted_code.Lines = outsideEntities.ToArray();
        }

        public string x = "0.0000";
        public void CircleData(string xcen, string ycen, string r)
        {
            /// CIRCLE
            /// 8 - layer
            /// 10 - center x
            /// 20 - center y
            /// 40 - radius
            /// 10 = 1.5
            /// 20 = 3.5
            /// 40 = 0.25
            /// Need:
            /// x = 1.25
            /// y = 3.5
            /// I = 0.25
            /// J = 0.0005
            /// 
            /// X = Center(10) - radius(40)
            /// Y = Center(20)
            /// I = radius(40)
            /*
            decimal centerX = Convert.ToDecimal(xcen, 4);
            decimal centerY = Convert.ToDecimal(ycen, 4);
            decimal radius = Convert.ToDecimal(r, 4);
            decimal xCircleStart = centerX - radius;
           */
        }

    }
}
