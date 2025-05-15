﻿using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;

namespace API.Helpers
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Students, StudentsDTO>()
                .ForMember(d => d.Faculty, o => o.MapFrom(s => s.Faculty.Fac_Name))
                .ForMember(d => d.FacultyYearSemister, o => o.MapFrom(s => s.FacultyYearSemister.Sem_Name))
                .ForMember(d => d.St_Image, o => o.MapFrom<StudentsPictureUrlResolver>()).ReverseMap(); 

              CreateMap<Doctors, DoctorsDTO>()
                .ForMember(d => d.Dr_Image, o => o.MapFrom<DoctorsPictureUrlResolver>()).ReverseMap();

            CreateMap<Studets_Rooms, Studets_RoomsDTO>()
             .ForMember(d => d.Rooms, o => o.MapFrom(s => s.Rooms.Room_Num))
             .ForMember(d => d.Students, o => o.MapFrom(s => s.Students.St_NameAr)).ReverseMap();

            CreateMap<Subjects, SubjectsDTO>()
             .ForMember(d => d.Doctors, o => o.MapFrom(s => s.Doctors.Dr_NameAr))
             .ForMember(d => d.Instructors, o => o.MapFrom(s => s.Instructors.Ins_NameAr))
             .ForMember(d => d.FacultyYearSemister, o => o.MapFrom(s => s.FacultyYearSemister.Sem_Name)).ReverseMap(); 




        }
    }
}
