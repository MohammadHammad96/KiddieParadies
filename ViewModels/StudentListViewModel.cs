using System;

namespace KiddieParadies.ViewModels
{
    public class StudentListViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherFirstName { get; set; }
        public string MotherFullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ImageName { get; set; }
        public bool IsMale { get; set; }
        public bool IsValid { get; set; }
        public bool IsParentProfileValid { get; set; }
        public string FatherIdentityImage { get; set; }
        public string MotherIdentityImage { get; set; }
    }
}