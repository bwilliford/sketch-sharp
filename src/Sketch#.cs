using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//Sketch# Version 1.0
//Sketch Recognition Framework for C# / Unity
//Copyright 2018 Blake Williford, All Rights Reserved



public class Point
    {
        //Properties
        public float x;
        public float y;
        public long time;
        public long pressure;
        public bool corner;

        //Constructors
        public Point()
        {

        }
        public Point(float xCoord, float yCoord, long timeStamp)
        {
            x = xCoord;
            y = yCoord;
            time = timeStamp;
        }
        public void randomize(float x1, float x2, float y1, float y2)
        {
            x = Random.Range(x1, x2);
            y = Random.Range(y1, y2);
            time = 0;
        }

        //Methods
        public bool isNear(Point p2, double threshold)
        {
            bool result = false;
            float xDistance = Mathf.Abs(p2.x - x);
            float yDistance = Mathf.Abs(p2.y - y);
            float totalDist = Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2));
            if (totalDist <= threshold)
            {
                result = true;
            }
            return result;
        }
}

    public class Segment
    {
        //Properties
        public Point point1 = new Point(0, 0, 0);
        public Point point2 = new Point(0, 0, 0);
        public float length;
        public float angle;

        //Constructors
        public Segment()
        {
        }
        public Segment(Point p1, Point p2)
        {
            point1 = p1;
            point2 = p2;
            length = this.getLength();
        }

        //Methods
        public float getLength()
        {
            float xDistance = point2.x - point1.x;
            float yDistance = point2.y - point1.y;
            float totalDist = Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2));
            length = totalDist;
            return totalDist;
        }
        public float getDuration() //Duration of Segment F13
        {
            long result = 0;
            result = point2.time - point1.time;
            return result;
        }
    }

    public class Stroke
    {
        //Properties
        public Point[] points = new Point[1000];
        public Segment[] segments = new Segment[1000];
        public Point[] corners = new Point[1000];
        public int numPoints;
        public int numCorners;
        public string shape;

        //Constructor
        public Stroke()
        {
        }

        //Methods
        public void add(Point p1)
        {
            points[numPoints] = new Point(p1.x, p1.y, p1.time);
            numPoints++;
            if (numPoints == 2)
            {
                segments[0] = new Segment(points[0], p1);
            }
            if (numPoints > 2)
            {
                segments[numPoints - 2] = new Segment(points[numPoints - 2], p1);
            }
        }
        public void insert(Point p1, int index)
        {

        }

        //Gesture Methods
        public float getInitialAngleCos() //Initial angle COS F1
        {
            float opp = points[2].x - points[0].x;
            float adj = points[2].y - points[0].y;
            float hyp = Mathf.Sqrt((opp * opp) + (adj * adj));
            float result = opp / hyp;
            return result;
        }
        public float getInitialAngleSin() //Initial angle SIN F2
        {
            float opp = points[2].x - points[0].x;
            float adj = points[2].y - points[0].y;
            float hyp = Mathf.Sqrt((opp * opp) + (adj * adj));
            float result = adj / hyp;
            return result;
        }
        public float getBoundingBoxLength() //Length of bounding box diaganol F3 //Accomadate negatives
        {
            float[] x = new float[1000];
            float[] y = new float[1000];
            for (int i = 0; i < numPoints; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }
            float xmax = x.Max();
            float xmin = x.Min();
            float ymax = y.Max();
            float ymin = y.Min();

            float result = Mathf.Sqrt(((xmax - xmin) * (xmax - xmin)) + ((ymax - ymin) * (ymax - ymin)));
            return result;
        }
        public float getBoundingBoxAngle() //Bounding box angle F4
        {
            float[] x = new float[1000];
            float[] y = new float[1000];
            for (int i = 0; i < numPoints; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }
            float xmax = x.Max();
            float xmin = x.Min();
            float ymax = y.Max();
            float ymin = y.Min();

            float result = Mathf.Atan((ymax - ymin) / (xmax - xmin));
            return result;
        }
        public float getDistanceFirstLast() //Distance from first point to last point F5
        {
            float[] x = new float[1000];
            float[] y = new float[1000];
            for (int i = 0; i < numPoints; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }
            float result = Mathf.Sqrt(Mathf.Pow(points[numPoints - 1].x - points[0].x, 2) + Mathf.Pow(points[numPoints - 1].y - points[0].y, 2));
            return result;
        }
        public float getFirstLastAngleCos() //Cosine of angle from first point to last point F6
        {
            float result = (points[numPoints - 1].x - points[0].x) / getDistanceFirstLast();
            return result;
        }
        public float getFirstLastAngleSin() //Sine of angle from point to last point F7
        {
            float result = (points[numPoints - 1].y - points[0].y) / getDistanceFirstLast();
            return result;
        }
        public float getLength() //Length of stroke F8
        {
            float totalDist = 0;
            for (int i = 0; i < numPoints - 1; i++)
            {
                totalDist = totalDist + segments[i].getLength();
            }
            return totalDist;
        }
        public float getTotalAngleTraversed() //Total Angles Traversed F9
        {
            float result = 0;
            for (int i = 1; i < numPoints - 2; i++)
            {
                float deltax = points[i + 1].x - points[i].x;
                float deltay = points[i + 1].y - points[i].y;
                float prevdeltax = points[i].x - points[i - 1].x;
                float prevdeltay = points[i].y - points[i - 1].y;
                result = result + Mathf.Atan(((deltax * prevdeltay) - (prevdeltax * deltay)) / ((deltax * prevdeltax) + (prevdeltay * deltay)));
            }
            return result;
        }
        public float getTotalAngleSum() //Total Angles Sum F10
        {
            float result = 0;
            for (int i = 1; i < numPoints - 2; i++)
            {
                float deltax = points[i + 1].x - points[i].x;
                float deltay = points[i + 1].y - points[i].y;
                float prevdeltax = points[i].x - points[i - 1].x;
                float prevdeltay = points[i].y - points[i - 1].y;
                result = result + Mathf.Abs(Mathf.Atan(((deltax * prevdeltay) - (prevdeltax * deltay)) / ((deltax * prevdeltax) + (prevdeltay * deltay))));
            }
            return result;
        }
        public float getTotalAngleSumSquared() //Total Angles Sum Squared F11
        {
            float result = 0;
            for (int i = 1; i < numPoints - 2; i++)
            {
                float deltax = points[i + 1].x - points[i].x;
                float deltay = points[i + 1].y - points[i].y;
                float prevdeltax = points[i].x - points[i - 1].x;
                float prevdeltay = points[i].y - points[i - 1].y;
                result = result + Mathf.Pow(Mathf.Atan(((deltax * prevdeltay) - (prevdeltax * deltay)) / ((deltax * prevdeltax) + (prevdeltay * deltay))), 2);
            }
            return result;
        }
        public float getMaxSpeedSquared() //Maximum Speed Squared F12
        {
            float result = 0;
            for (int i = 0; i < numPoints - 1; i++)
            {
                float deltax = points[i + 1].x - points[i].x;
                float deltay = points[i + 1].y - points[i].y;
                long deltat = points[i + 1].time - points[i].time;
                //long prevdeltat = points[i].time - points[i - 1].time;
                if (deltat > 0)
                {
                    float speed = ((deltax * deltax) + (deltay * deltay)) / (deltat * deltat);
                    if (speed > result)
                    {
                        result = speed;
                    }
                }
            }
            return result;
        }
        public float getDuration() //Duration of Gesture F13
        {
            long result = 0;
            if (points[1].time == points[0].time)
            {
                result = points[numPoints - 1].time - points[1].time;
            }
            else
            {
                result = points[numPoints - 1].time - points[0].time;
            }
            return result;
        }
        public int getCorners() // Corner Detection
        {
            int result = 0;
            return result;
        }

        //Recognition Methods
        public bool isClosed() //Is it a closed Shape?
        {
            bool result = false;
            if (points[0].isNear(points[numPoints - 1], 0.5))
            {
                result = true;
            }
            return result;
        }
        public bool isLine() //Is it a line?
        {
            bool result = false;
            float ratio = getDistanceFirstLast() / getLength();
            if (ratio > 0.95 && ratio < 1.05)
            {
                result = true;
                shape = "line";
            }
            return result;
        }
        public bool isArrow() //Is it an arrow?
        {
            bool result = false;
            return result;
        }
        public bool isSquare() //Is it a square?
        {
            bool result = false;
            return result;
        }
        public bool isCircle() //Is it a circle?
        {
            bool result = false;
            return result;
        }   
}

    public class Sketch
    {
        //Properties
        public Stroke[] strokes = new Stroke[1000];
        public int numStrokes = 0;

        //Constructors
        public Sketch()
        {
        }

        //Methods
        public void add(Stroke stroke)
        {
            strokes[numStrokes] = stroke;
            numStrokes++;
        }
        public Stroke flatten()
        {
            Stroke flattenedStroke = new Stroke();

            return flattenedStroke;
        }
    }