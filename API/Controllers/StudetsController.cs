using API.DTOs;
using AutoMapper;
using Core;
using Core.Entities;
using Core.Repositories.Contract;
using Core.Specifications.studetsSpecifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.APIS.Erorrs;
using System.Globalization;
using Repository.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{
    public class StudetsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly StoreContext context;

        public StudetsController(IUnitOfWork unitOfWork, IMapper mapper ,StoreContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.context = context;
        }


        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  
        [HttpGet("GetAllStudets")]   //Studets
        public async Task<ActionResult<IReadOnlyList<StudentsDTO>>> GetAllStudets()
        {
            var Studets = await _unitOfWork.Repository<Students>().GetAll();
            var data = _mapper.Map<IReadOnlyList<Students>, IReadOnlyList<StudentsDTO>>(Studets);
            return Ok(data); //200
        }
        [HttpGet("DashBordStudets")]   //Studets
        public async Task<ActionResult<IReadOnlyList<StudentsDTO>>> DashBordStudets(int id)
        {
            var query = (from l in context.Lecture
                         join su in context.Subjects on l.Sub_ID equals su.ID into lectureGroup
                         from su in lectureGroup.DefaultIfEmpty()
                         join r in context.Rooms on l.Room_ID equals r.ID into roomGroup
                         from r in roomGroup.DefaultIfEmpty()
                         join d in context.Doctors on su.Dr_ID equals d.ID
                         where l.St_ID == id
                         select new
                         {
                             su.Sub_Name,
                             d.Dr_NameAr,
                             LectureDate = l.LectureDate,
                             RoomNo = r.Room_Num
                         }).ToList()  // نزل الداتا من SQL إلى الذاكرة
                 .Select(x => new
                 {
                     x.Sub_Name,
                     x.Dr_NameAr,
                     Day = x.LectureDate.DayOfWeek.ToString(),
                     x.RoomNo
                 }).ToList();

            return Ok(query); //200
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Students>> GetStudet(int id)
        {
            try
            {
                var Studet = await _unitOfWork.Repository<Students>().GetById(id);
                if (Studet == null)
                {
                    return NotFound(new ApiResponse(404));// 404
                }
                var data = _mapper.Map<Students, StudentsDTO>(Studet);
                return Ok(data); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);// 400
            }

        }

        [HttpPost("Add_OR_UpdateStudent")] 
        public async Task<ActionResult<Students>> AddStudet(StudentsDTO students)
        {
            //var mappedStudents = _mapper.Map<StudentsDTO, Students>(students);  

            var mappedStudents = new Students
            {
                ID = students.ID,
                St_Code = students.St_Code,
                St_NameAr = students.St_NameAr,
                St_NameEn = students.St_NameEn,
                St_Email = students.St_Email,
                St_Image = students.St_Image,
                Phone = students.Phone,
                Fac_ID = students.Fac_ID,
                FacYearSem_ID = students.FacYearSem_ID,
            };
            if (mappedStudents.ID != 0)
            {
                var data = await _unitOfWork.Repository<Students>().UpdateAsync(mappedStudents);
                if (data is null) return BadRequest(new ApiResponse(400));
                await _unitOfWork.CompleteAsync();
                return Ok(data);
            }
            else
            {
                mappedStudents.ID = 0;
                var data = await _unitOfWork.Repository<Students>().AddAsync(mappedStudents);
                if (data is null) return BadRequest(new ApiResponse(400));
                await _unitOfWork.CompleteAsync();
                return Ok(data);
            }


        } 
        [HttpDelete("DeleteStudent")]
        public async Task DeleteStudents(int id)
        { 
            var Studet = await _unitOfWork.Repository<Students>().GetById(id);
            if (Studet is not null)
            {
            _unitOfWork.Repository<Students>().Delete(Studet);
            await _unitOfWork.CompleteAsync();
                 
            }
            else
            {
              NotFound(new ApiResponse(404));// 404
            }

        }

    }
}
