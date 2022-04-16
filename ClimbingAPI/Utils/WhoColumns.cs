using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Boulder;

namespace ClimbingAPI.Utils
{
    public static class WhoColumns
    {
        public static void Fill(Boulder boulder)
        {
            boulder.Author = "Admin"; //TODO: implement after authentication
            boulder.ModificationTime = DateTime.Now;
        }
    }
}
