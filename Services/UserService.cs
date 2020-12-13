using AutoMapper;
using Microsoft.EntityFrameworkCore;
using simpleAPI.Data;
using simpleAPI.DTOs.V1;
using simpleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simpleAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public UserService(DataContext dataContext, IMapper mapper) 
        {
            _context = dataContext;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetUserDTO>>> GetAllUsers()
        {
            ServiceResponse<List<GetUserDTO>> serviceResponse = new ServiceResponse<List<GetUserDTO>>();
            List<User> dbUsers = await _context.Users.ToListAsync();
            serviceResponse.Data = (dbUsers.Select(c => _mapper.Map<GetUserDTO>(c))).ToList();
            return serviceResponse;
        }
    }
}
