namespace RFID_Backend.Application.Models
{
    public class TotalInTime
    {
        public int UserId { get; set; }
        public string Uid { get; set; }
        public string FullName { get; set; }
        public DateTime? AttendanceDate { get; set; }  //  Nullable
        public DateTime? FirstIn { get; set; }         //  Nullable
        public DateTime? LastOut { get; set; }         //  Nullable
        public string TotalWorkedTime { get; set; }
        public string Designation { get; set; }
    }


    public class AttendanceModel
    {
        public string Uid { get; set; }
        public string FullName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? FirstIn { get; set; }
        public DateTime? LastOut { get; set; }
        public string TotalHoursFormatted { get; set; }


    }


}


