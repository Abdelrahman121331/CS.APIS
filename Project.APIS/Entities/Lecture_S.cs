using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Lecture_S : BaseEntity
    {
        public string Lecture_Name { get; set; }
        [ForeignKey(nameof(Students))] 
        public int St_ID { get; set; }
        public Students Students { get; set; }

        [ForeignKey(nameof(Rooms))] 
        public int Room_ID { get; set; }
        public Rooms Rooms { get; set; }
        [ForeignKey(nameof(Subjects))]  
        public int Sub_ID { get; set; }
        public Subjects Subjects { get; set; }
        public DateTime LectureDate { get; set; } 
        public int Degree { get; set; } 
    }
}
