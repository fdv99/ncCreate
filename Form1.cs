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
using System.Data.OleDb;

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

            // Filter the results so you only see .dxf files
            openFileDialog1.Filter = "dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    // Store the file path of the dxf file
                    ncFileName = openFileDialog1.FileName;

                    // Display the dxf file code in the window
                    original_code.Text = File.ReadAllText(ncFileName);
                }
                FindEntities();
            }

        }

        List<string> dxfList = new List<string>();
        List<string> coordinateList = new List<string>();
        List<string> part1List = new List<string>();
        int entitiesLine = 0;
        int endsecLine = 0;

        private void FindEntities()
        {
            int counter = 0;
            string line;
            //int entitiesLine = 0;
            //int endsecLine = 0;

            StreamReader file = new StreamReader(ncFileName);
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
            dxfList = File.ReadAllLines(ncFileName).ToList();

            // Get the number of lines in List
            int convertLength = dxfList.Count;

            // Remove from the end section of entities to the end of the list
            dxfList.RemoveRange(endsecLine, (convertLength - endsecLine));

            // Remove from the start of the list to the entities section
            dxfList.RemoveRange(0, entitiesLine);

            convertLength = dxfList.Count;   

            // Go through list and copy all Outside lines/arcs to outside list
            //List<string> coordinateList = new List<string>();          
            
            for (int i=1; i< convertLength; i+=1)
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

            coordinateList.Add("G93 X0.0Y0.0Z0.0");
            coordinateList.Add("G130");
            coordinateList.Add("/M707");
            coordinateList.Add("G50");
            converted_code.Lines = coordinateList.ToArray();
        }

        public void CircleData(int iCircle)
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
            double xc = Convert.ToDouble(dxfList[iCircle + 12]);
            double yc = Convert.ToDouble(dxfList[iCircle + 14]);
            double rc = Convert.ToDouble(dxfList[iCircle + 18]);
            if (dxfList[iCircle + 8] == "INSIDE")
            {
                coordinateList.Add("IC x" + Math.Round(xc, 4) +
                    " Y" + Math.Round(yc, 4) +
                    " R" + Math.Round(rc, 4));
            }

            if (dxfList[iCircle + 8] == "OUTSIDE")
            {
                coordinateList.Add("OC x" + Math.Round(xc, 4) +
                    " Y" + Math.Round(yc, 4) +
                    " R" + Math.Round(rc, 4));
            }
        }

        public void ArcData(int iArc)
        {
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
            var centerX = 0.0;
            var centerY = 0.0;
            var radiusArc = 0.0;
            var startAngleArc = 0.0;
            var endAngleArc = 0.0;

            centerX = double.Parse(dxfList[iArc + 12]);
            centerX = Math.Round(centerX, 5);

            centerY = double.Parse(dxfList[iArc + 14]);
            centerY = Math.Round(centerY, 5);

            radiusArc = Math.Round(double.Parse(dxfList[iArc + 18]),4);

            // Convert angle to radians
            startAngleArc = (Math.Round(double.Parse(dxfList[iArc + 22]),4))*(Math.PI / 180);
            endAngleArc = (Math.Round(double.Parse(dxfList[iArc + 24]),4))*(Math.PI / 180);

            var startX = centerX + (radiusArc * (Math.Cos(startAngleArc)));
            var startY = centerY + (radiusArc * (Math.Sin(startAngleArc)));
            if (dxfList[iArc + 8] == "INSIDE")
            { 
                coordinateList.Add($"IA X{startX} Y{startY} R{radiusArc}");
            }

            if (dxfList[iArc + 8] == "OUTSIDE")
            {
                coordinateList.Add($"OA X{startX} Y{startY} R{radiusArc}");

                // X Start point = x center point (10) + (radius(40) * cos(startAngle(50))
                // Y Start point = y center point (20) + (radius(40) * sin(starttAngle(50))
                // X End point = x center point (10) + (radius(40) * cos(endAngle(51))
                // Y End point = y center point (20) + (radius(40) * sin(endAngle(51))
            }
        }

        public void LineData(int iLine)
        {
            /// Line
            /// 8 - layer
            /// 10 - x1
            /// 20 - y1
            /// 11 - x2
            /// 21 - y2
            /// save these points for use in code 
            var lineType = "Line";
            var lineLayer = dxfList[iLine + 8];
            var x1 = Convert.ToDouble(dxfList[iLine + 12]);
            var y1 = Convert.ToDouble(dxfList[iLine + 14]);
            var x2 = Convert.ToDouble(dxfList[iLine + 18]);
            var y2 = Convert.ToDouble(dxfList[iLine + 20]);

            //con.Open();
            //OleDbCommand cmd = new OleDbCommand("INSERT INTO featureList(Type, Layer, X1, Y1, X2, Y2, R, Angle1, Angle2) values(lineType, lineLayer, x1, y1, x2, y2, radius, angle1, angle2)", con);
            //cmd.ExecuteNonQuery();
            //con.Close();

            if (dxfList[iLine + 8] == "INSIDE")
            {
                coordinateList.Add("IS x" + Math.Round(x1, 4) + " Y" + Math.Round(y1, 4));

                coordinateList.Add("IF x" + Math.Round(x2, 4) + " Y" + Math.Round(y2, 4));  

            }

            if (dxfList[iLine + 8] == "OUTSIDE")
            {
                coordinateList.Add("OS X" + Math.Round(x1, 4) + " Y" + Math.Round(y1, 4));

                coordinateList.Add("OF X" + Math.Round(x2, 4) + " Y" + Math.Round(y2, 4));
            }
        }
    }
}
