using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ncCreate
{
    public partial class Form1 : Form
    {
        // Declare the filename variable
        private string dxfFileName = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Find the dxf file to open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_OpenFile_Click(object sender, EventArgs e)
        {
            // Choose the nc file you want to edit
            var openFileDialog1 = new OpenFileDialog
            {
                // Filter the results so you only see .dxf files
                Filter = "dxf files (*.dxf)|*.dxf|All files (*.*)|*.*"
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((openFileDialog1.OpenFile()) != null)
                {
                    // Store the file path of the dxf file
                    dxfFileName = openFileDialog1.FileName;

                    // Display the dxf file code in the window
                    original_code.Text = File.ReadAllText(dxfFileName);
                }

                FindEntities();
            }

        }

        List<string> dxfList = new List<string>();
        List<string> coordinateList = new List<string>();
        int entitiesLine = 0;
        int endsecLine = 0;

        /// <summary>
        /// Get Part entities out of dxf file, lines, arcs, circles
        /// </summary>
        private void FindEntities()
        {
            int counter = 0;
            string line;

            StreamReader file = new StreamReader(dxfFileName);
            while ((line = file.ReadLine()) != null)
            {
                // Check if we are at the entities line...
                if (line.Contains("ENTITIES"))
                {
                    // then save the line number
                    entitiesLine = counter;
                }
                // Check if we are at the endsec of the entitie section...
                else if (line.Contains("ENDSEC") && entitiesLine != 0)
                {
                    // Save the line number of the end of the section
                    endsecLine = counter;
                    // Break out of the while loop
                    break;
                }
                counter++;
            }

            file.Close();
        }


        private void Btn_NcCreate_Click(object sender, EventArgs e)
        {
            // Convert the original file to a list
            // Break it up so that we only copy the entities section using entitiesLine as the start and endsecLine as the end
            // Make sure the list is empty to start
            dxfList.Clear();

            // add dxf file to list
            dxfList = File.ReadAllLines(dxfFileName).ToList();

            // Get the number of lines in List
            int convertLength = dxfList.Count;

            // Remove from the end section of entities to the end of the list
            dxfList.RemoveRange(endsecLine, (convertLength - endsecLine));

            // Remove from the start of the list to the entities section
            dxfList.RemoveRange(0, entitiesLine);

            convertLength = dxfList.Count;

            for (int i = 1; i < convertLength; i += 1)
            {
                // Get current line on list
                string current = dxfList[i];

                if (current == "LINE")
                {
                    /// Call Line Data Method
                    /// Pass in i so we know how to offset to get info needed
                    LineData(i);
                }

                if (current == "ARC")
                {
                    ArcData(i);
                }

                if (current == "CIRCLE")
                {
                    CircleData(i);
                }

            }

            // Add the End Sequence to the code
            coordinateList.Add("G93 X0.0Y0.0Z0.0");
            coordinateList.Add("G130");
            coordinateList.Add("/M707");
            coordinateList.Add("G50");
            // Need to add M102(), G90, G92, G93, M100 to start
            converted_code.Lines = coordinateList.ToArray();
        }

        #region EntityMethods

        /// <summary>
        /// Get circle data from dxf file
        /// </summary>
        /// <param name="iCircle"></param>
        public void CircleData(int iCircle)
        {
            /// CIRCLE
            /// 8 - layer
            /// 10 - center x
            /// 20 - center y
            /// 40 - radius
            /// X = Center(10) - radius(40)
            /// Y = Center(20)
            /// I = radius(40)
            /// J = Y direction distance from starting point to arc center

            /// We will always pierce at center and move left to quad, then run the x, y, i, j command
            double xc = Convert.ToDouble(dxfList[iCircle + 12]);
            double yc = Convert.ToDouble(dxfList[iCircle + 14]);
            double rc = Convert.ToDouble(dxfList[iCircle + 18]);
            if (dxfList[iCircle + 8] == "INSIDE")
            {
                // Inside circle Pierce, always pierce in center, G01 to edge, then process circle
                coordinateList.Add("ICP x" + Math.Round(xc, 4) +
                    " Y" + Math.Round(yc, 4) +
                    " R" + Math.Round(rc, 4));
            }

            if (dxfList[iCircle + 8] == "OUTSIDE")
            {
                // Outside circle pierce, always pierce outside 0.125 horizontal
                coordinateList.Add("OCP x" + Math.Round(xc, 4) +
                    " Y" + Math.Round(yc, 4) +
                    " R" + Math.Round(rc, 4));
            }
        }

        /// <summary>
        /// Get arc data for all arcs in dxf program
        /// </summary>
        /// <param name="iArc"></param>
        public void ArcData(int iArc)
        {
            /// ARC
            /// 8 - layer
            /// 10 - center x
            /// 20 - center y
            /// 40 - radius
            /// 50 - start angle
            /// 51 - end angle
            // X Start point = x center point (10) + (radius(40) * cos(startAngle(50))
            // Y Start point = y center point (20) + (radius(40) * sin(starttAngle(50))
            // X End point = x center point (10) + (radius(40) * cos(endAngle(51))
            // Y End point = y center point (20) + (radius(40) * sin(endAngle(51))

            var arcLayer = dxfList[iArc + 8];

            var centerX = double.Parse(dxfList[iArc + 12]);
            centerX = Math.Round(centerX, 5);

            var centerY = double.Parse(dxfList[iArc + 14]);
            centerY = Math.Round(centerY, 5);

            var radiusArc = Math.Round(double.Parse(dxfList[iArc + 18]), 4);

            // Convert angle to radians
            var startAngleArc = (Math.Round(double.Parse(dxfList[iArc + 22]), 4)) * (Math.PI / 180);
            var endAngleArc = (Math.Round(double.Parse(dxfList[iArc + 24]), 4)) * (Math.PI / 180);

            var startX = centerX + (radiusArc * (Math.Cos(startAngleArc)));
            var startY = centerY + (radiusArc * (Math.Sin(startAngleArc)));
            var endX = centerX + (radiusArc * (Math.Cos(endAngleArc)));
            var endY = centerY + (radiusArc * (Math.Sin(endAngleArc)));

            if (arcLayer == "INSIDE")
            {
                coordinateList.Add($"IAS X{startX} Y{startY} R{radiusArc}");
                coordinateList.Add($"IAF X{endX} Y{endY} R{radiusArc}");
            }

            if (arcLayer == "OUTSIDE")
            {
                coordinateList.Add($"OAS X{startX} Y{startY} R{radiusArc}");
                coordinateList.Add($"OAF X{endX} Y{endY} R{radiusArc}");
            }
        }

        /// <summary>
        /// Get start and end points for each line in dxf file
        /// </summary>
        /// <param name="iLine"></param>
        public void LineData(int iLine)
        {
            /// Line
            /// 8 - layer
            /// 10 - x1
            /// 20 - y1
            /// 11 - x2
            /// 21 - y2
            var lineLayer = dxfList[iLine + 8];
            var x1 = Convert.ToDouble(dxfList[iLine + 12]);
            var y1 = Convert.ToDouble(dxfList[iLine + 14]);
            var x2 = Convert.ToDouble(dxfList[iLine + 18]);
            var y2 = Convert.ToDouble(dxfList[iLine + 20]);

            if (lineLayer == "INSIDE")
            {
                coordinateList.Add("IS x" + Math.Round(x1, 4) + " Y" + Math.Round(y1, 4));

                coordinateList.Add("IF x" + Math.Round(x2, 4) + " Y" + Math.Round(y2, 4));
            }

            if (lineLayer == "OUTSIDE")
            {
                coordinateList.Add("OS X" + Math.Round(x1, 4) + " Y" + Math.Round(y1, 4));

                coordinateList.Add("OF X" + Math.Round(x2, 4) + " Y" + Math.Round(y2, 4));
            }
        }

        #endregion
    }
}
