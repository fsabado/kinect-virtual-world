using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication_KinectProjectToPHP
{
    /// <summary>
    /// Christian Williams Uark 010537189
    /// ccw005@uark.edu
    /// 
    /// This class represents a Second Life prim and is used to
    /// store properties of 
    /// </summary>
    class ModelPrim
    { 
        public String PrimName { get; set; }
        public String PrimUrl { get; set; }
        public String PrimShape { get; set; }   
       
             
                
#region Rotation Properties
        private double _primRotationX;
        private double _primRotationY;
        private double _primRotationZ;
        public double PrimRotationX
        {
            get { return this._primRotationX; }
            set
            {
                if (IsRotationRange(value))
                {
                    this._primRotationX = value;
                }
                else
                    this._primRotationX = 0.0;
            }
        }
        public double PrimRotationY
        {
            get { return this._primRotationY; }
            set
            {
                if (IsRotationRange(value))
                {
                    this._primRotationY = value;
                }
                else
                    this._primRotationY = 0.0;
            }
        }
        public double PrimRotationZ
        {
            get { return this._primRotationZ; }
            set
            {
                if (IsRotationRange(value))
                {
                    this._primRotationZ = value;
                }
                else
                    this._primRotationZ = 0.0;
            }
        }
        public bool IsRotationRange(double value)
        {
            if (value >= 0.0 && value <= 359)
            {
                return true;
            }
            return false;
        }

#endregion
       
#region Color Properties
        private double _primColorRed;
        private double _primColorGreen;
        private double _primColorBlue;
        public double PrimColorRed
        {
            get { return this._primColorRed; }
            set
            {
                if (IsColorRange(value))
                {
                    
                    this._primColorRed = value;
                }
            }
        }

        public double PrimColorGreen
        {
            get { return this._primColorGreen; }
            set
            {
                if (IsColorRange(value))
                {
                    
                    this._primColorGreen = value;
                }
            }
        }

        public double PrimColorBlue
        {
            get { return this._primColorBlue; }
            set
            {
                if (IsColorRange(value))
                {
                   
                    this._primColorBlue = value;
                }
            }
        }

        public bool IsColorRange(double value)
        {
            if (value >= 0.0 && value <= 4.0)
            {
                return true;
            }
            return false;
        }

#endregion       

#region Size Properties
        private double _primSizeX;
        private double _primSizeY;
        private double _primSizeZ;

        public double PrimSizeX
        {
            get { return this._primSizeX; }
            set { 
                    if (IsSizeRange(value))
                    {
                        if (value < 0.1)
                        {
                            value = 0.1;
                        }
                        if (value > 4.0)
                        {
                            value = 4.0;
                        }
                        this._primSizeX = value;
                    }                 
                }
        }

        public double PrimSizeY
        {
            get { return this._primSizeY; }
            set
            {
                if (IsSizeRange(value))
                {
                    if (value < 0.1)
                    {
                        value = 0.1;
                    }
                    if (value > 4.0)
                    {
                        value = 4.0;
                    }
                    this._primSizeY = value;
                }
            }
        }

        public double PrimSizeZ
        {
            get { return this._primSizeZ; }
            set
            {
                if (IsSizeRange(value))
                {
                    if (value < 0.1)
                    {
                        value = 0.1;
                    }
                    if (value > 4.0)
                    {
                        value = 4.0;
                    }
                    this._primSizeZ = value;
                }
            }
        }

        public bool IsSizeRange(double value)
        {
            if (value >= 0.0 && value <= 4.0)
            {
                return true;
            }
            return false;
        }

#endregion

        
               
        
        
        public override String ToString()
        {
            return this.PrimName + " " + this.PrimUrl;
        }

        public bool Equals(ModelPrim otherPrim)
        {
            // compare the relevant prim data 
            if (otherPrim._primColorBlue == this._primColorBlue
                && otherPrim._primColorGreen == this._primColorGreen
                && otherPrim._primColorRed == this._primColorRed
                && PrimAxisCompare(this,otherPrim)
                && otherPrim.PrimShape == this.PrimShape)
                return true;
            return false;
        }

        public bool PrimAxisCompare(ModelPrim aPrim,ModelPrim bPrim)
        {
            if (Math.Abs(aPrim._primSizeX - bPrim._primSizeX) < 1.0)
            {
                if (Math.Abs(aPrim._primSizeY - bPrim._primSizeY) < 1.0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
