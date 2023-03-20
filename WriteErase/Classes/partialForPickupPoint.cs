using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteErase
{
    partial class PickupPoint
    {
        public string fullNameOfPoint
        {
            get { return $"{PointIndex}, г.{PointCity}, ул.{PointStreet}, д.{PointHouse}"; }
        }

    }
}
