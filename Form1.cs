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
                        outsideEntities.Add("IS x" + Math.Round(Convert.ToDecimal(convertList[i + 12]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4));
                        outsideEntities.Add("IF x" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 20]), 4));

                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("OS X" + Math.Round(Convert.ToDecimal(convertList[i + 12]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4));
                        outsideEntities.Add("OF X" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 20]), 4));
                    }
                }

                if (current == "ARC")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("IA x" + Math.Round(Convert.ToDecimal(convertList[i + 12]), 4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4) + " R" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4));
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("OA x" + Math.Round(Convert.ToDecimal(convertList[i + 12]), 4) + 
                            " Y" + Math.Round(Convert.ToDecimal(convertList[i + 14]), 4) + 
                            " R" + Math.Round(Convert.ToDecimal(convertList[i + 18]), 4) + 
                            " SA" + Math.Round(Convert.ToDecimal(convertList[i + 22]), 4) +
                            " EA" + Math.Round(Convert.ToDecimal(convertList[i + 24]), 4));

                        // X Start point = x center point (10) + (radius(40) * cos(startAngle(50))
                        // Y Start point = y center point (20) + (radius(40) * sin(starttAngle(50))
                        // X End point = x center point (10) + (radius(40) * cos(endAngle(51))
                        // Y End point = y center point (20) + (radius(40) * sin(endAngle(51))
                    }
                }

                if (current == "CIRCLE")
                {
                    if (convertList[i + 8] == "INSIDE")
                    {
                        outsideEntities.Add("IC x" + Math.Round(Convert.ToDecimal(convertList[i + 12]),4) + " Y" + Math.Round(Convert.ToDecimal(convertList[i + 14]),4) + " R" + convertList[i + 18]);
                    }

                    if (convertList[i + 8] == "OUTSIDE")
                    {
                        outsideEntities.Add("OC x" + convertList[i + 12] + " Y" + convertList[i + 14] + " R" + convertList[i + 18]);

                    }
                }

            }

            outsideEntities.Add("G93 X0.0Y0.0Z0.0");
            outsideEntities.Add("G130");
            outsideEntities.Add("/M707");
            outsideEntities.Add("G50");
            converted_code.Lines = outsideEntities.ToArray();
        }

        public string x = "0.0000";
        public void CircleData()
        {
            /// CIRCLE
            /// 8 - layer
            /// 10 - center x
            /// 20 - center y
            /// 40 - radius
            /// 50 - Start Angle
            /// 51 - End Angle
            
            /// X = Center(10) - radius(40)
            /// Y = Center(20)
            /// I = radius(40)
            /// J = Y direction distance from starting point to arc center
           
            /// We will always pierce at center and move left to quad, then run the x, y, i, j command
        }

        public void ArcData()
        {
            /// Pass in these variables:
            
            /// ARC
            /// 8 - layer
            /// 10 - center x
            /// 20 - center y
            /// 40 - radius
            /// 50 - start angle
            /// 51 - end angle
           
            /// Convert these variable to the start and end points:
            
            // X Start point = x center point (10) + (radius(40) * cos(startAngle(50))
            // Y Start point = y center point (20) + (radius(40) * sin(starttAngle(50))
            // X End point = x center point (10) + (radius(40) * cos(endAngle(51))
            // Y End point = y center point (20) + (radius(40) * sin(endAngle(51))

            /// save these points for use in code 
        }

        public void LineData()
        {
            /// Pass in these variables:

            /// Line
            /// 8 - layer
            /// 10 - x1
            /// 20 - y1
            /// 11 - x2
            /// 21 - y2

            /// Convert these variable to the start and end points:

            /// save these points for use in code 
        }
    }
}
