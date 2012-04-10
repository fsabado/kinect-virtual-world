using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WpfApplication_KinectProjectToPHP
{
    /// <summary>
    /// Christian Williams 010537189 ccw005@uark.edu
    /// 
    /// This class is used to represent the color of a prim.
    /// Note that the Equals and ToString methods are overridden
    /// and defined here in the color object so that prims can be 
    /// compared to one another in a familiar manner to other types
    /// for ease of working with the object within the main program. 
    /// </summary>
    class PrimColor
    {
        private double[] _primColor = new double[3];
        private double _red;
        private double _green;
        private double _blue;

        public PrimColor(double red, double green, double blue)
        {
            this._red = red;
            this._green = green;
            this._blue = blue;
            this._primColor 
                = new double[] {this._red,this._green,this._blue};
           
        }

        public override String ToString()
        {
            return "Red: " + this._red + " Green: " + this._green 
                             + " Blue: " + this._blue + "\n";
        }

        public bool Equals(PrimColor otherColor)
        {
            if(this._red == otherColor._red
                && this._green == otherColor._green
                && this._blue == otherColor._blue)
               return true;
            return false;
        }

        public double[] GetColor()
        {
            return this._primColor;
        }
    }
}