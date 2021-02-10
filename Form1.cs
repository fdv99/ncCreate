﻿using EntitiesModel;
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
        List<string> dxfList = new List<string>();
        List<string> coordinateList = new List<string>();
        List<Entities> entitiesList = new List<Entities>();
        int entitiesLine = 0;
        int endsecLine = 0;

        public Form1()
        {
            InitializeComponent();
        }

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
            coordinateList.Add("G130");
            coordinateList.Add("/M707");
            coordinateList.Add("G50");
            converted_code.Lines = coordinateList.ToArray();
        }

        #region EntityMethods

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
            Circle circle = new Circle
            {
                Layer = dxfList[iCircle + 8],
                CenterX = double.Parse(dxfList[iCircle + 12]),
                CenterY = double.Parse(dxfList[iCircle + 14]),
                Radius = double.Parse(dxfList[iCircle + 18])
            };

            entitiesList.Add(circle);

            if (circle.Layer == "INSIDE")
            {
                // Inside circle Pierce, always pierce in center, G01 to edge, then process circle
                coordinateList.Add("Inside Circle x" + Math.Round(circle.CenterX, 4) +
                    " Y" + Math.Round(circle.CenterY, 4) +
                    " R" + Math.Round(circle.Radius, 4));
            }

            if (circle.Layer == "OUTSIDE")
            {
                // Outside circle pierce, always pierce outside 0.125 horizontal
                coordinateList.Add("Outside Circle x" + Math.Round(circle.CenterX, 4) +
                    " Y" + Math.Round(circle.CenterY, 4) +
                    " R" + Math.Round(circle.Radius, 4));
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
            // X Start point = x center point (10) + (radius(40) * cos(startAngle(50))
            // Y Start point = y center point (20) + (radius(40) * sin(starttAngle(50))
            // X End point = x center point (10) + (radius(40) * cos(endAngle(51))
            // Y End point = y center point (20) + (radius(40) * sin(endAngle(51))

            Arc arc = new Arc();
            arc.Layer = dxfList[iArc + 8];
            arc.CenterX = double.Parse(dxfList[iArc + 12]);
            arc.CenterY = double.Parse(dxfList[iArc + 14]);
            arc.Radius = double.Parse(dxfList[iArc + 18]);
            arc.StartAngle = (double.Parse(dxfList[iArc + 22])) * (Math.PI / 180);
            arc.EndAngle = (double.Parse(dxfList[iArc + 24])) * (Math.PI / 180);
            arc.StartX = arc.CenterX + (arc.Radius * (Math.Cos(arc.StartAngle)));
            arc.StartY = arc.CenterY + (arc.Radius * (Math.Sin(arc.StartAngle)));
            arc.EndX = arc.CenterX + (arc.Radius * (Math.Cos(arc.EndAngle)));
            arc.EndY = arc.CenterY + (arc.Radius * (Math.Sin(arc.EndAngle)));

            entitiesList.Add(arc);

            if (arc.Layer == "INSIDE")
            {
                coordinateList.Add($"Inside Arc Start X{arc.StartX} Y{arc.StartY} R{arc.Radius}");
                coordinateList.Add($"Inside Arc Finish X{arc.EndX} Y{arc.EndY} R{arc.Radius}");
            }

            if (arc.Layer == "OUTSIDE")
            {
                coordinateList.Add($"Outside Arc Start X{arc.StartX} Y{arc.StartY} R{arc.Radius}");
                coordinateList.Add($"Outside Arc Finish X{arc.EndX} Y{arc.EndY} R{arc.Radius}");
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
            /// 

            Line line = new Line();
            line.Layer = dxfList[iLine + 8];
            line.StartX = Convert.ToDouble(dxfList[iLine + 12]);
            line.StartY = Convert.ToDouble(dxfList[iLine + 14]);
            line.EndX = Convert.ToDouble(dxfList[iLine + 18]);
            line.EndY = Convert.ToDouble(dxfList[iLine + 20]);
            entitiesList.Add(line);

            if (line.Layer == "INSIDE")
            {
                coordinateList.Add("Inside Line Start x" + Math.Round(line.StartX, 4) + " Y" + Math.Round(line.StartY, 4));
                coordinateList.Add("Inside Line Finish x" + Math.Round(line.EndX, 4) + " Y" + Math.Round(line.EndY, 4));
            }

            if (line.Layer == "OUTSIDE")
            {
                coordinateList.Add("Outside Line Start X" + Math.Round(line.StartX, 4) + " Y" + Math.Round(line.StartY, 4));

                coordinateList.Add("Outside Line Finish X" + Math.Round(line.StartX, 4) + " Y" + Math.Round(line.StartY, 4));
            }
        }
        #endregion
    }


}
