using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_TourPlanner.Commands
{
    public class DeleteTourMessage
    {
        public int TourId { get; }

        public DeleteTourMessage(int tourId)
        {
            TourId = tourId;
        }
    }
}
