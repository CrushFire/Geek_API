using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models.Category;
using Core.Models.DataPage;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DataPageService : IDataPageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DataPageService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<DataPageResponse>> GetByIdAsync(int id)
        {
            var data = await _context.DataPages.FirstOrDefaultAsync(d => d.Id == id);

            if (data == null)
            {
                return ServiceResult<DataPageResponse>.Failure("Данные не найдены");
            }

            var dataResponse = _mapper.Map<DataPageResponse>(data);
            return ServiceResult<DataPageResponse>.Success(dataResponse);
        }

        public async Task<ServiceResult<Dictionary<string, DataPageRequest>>> GetByPageAsync(string page)
        {
                var data = await _context.DataPages
                    .Where(d => d.NamePage == page)
                    .ToListAsync();

                if (data == null || data.Count == 0)
                {
                    return ServiceResult<Dictionary<string, DataPageRequest>>.Failure("Данные не найдены");
                }

                var resultDict = data
                    .ToDictionary(
                        d => d.NameData,
                        d => new DataPageRequest
                        {
                            InfRu = d.InfRu,
                            InfEng = d.InfEng
                        }
                    );

                return ServiceResult<Dictionary<string, DataPageRequest>>.Success(resultDict);
        }

        public async Task<ServiceResult<DataPageResponse>> GetByNameAsync(string name)
        {
            var data = await _context.DataPages.FirstOrDefaultAsync(d => d.NameData == name);

            if (data == null)
            {
                return ServiceResult<DataPageResponse>.Failure("Данные не найдены");
            }

            var dataResponse = _mapper.Map<DataPageResponse>(data);
            return ServiceResult<DataPageResponse>.Success(dataResponse);
        }

        public async Task<ServiceResult<DataPageResponse>> GetByPageAndNameAsync(DataPageNameRequest request)
        {
            var data = await _context.DataPages.FirstOrDefaultAsync(d => d.NamePage == request.Page && d.NameData == request.Name);

            if (data == null)
            {
                return ServiceResult<DataPageResponse>.Failure("Данные не найдены");
            }

            var dataResponse = _mapper.Map<DataPageResponse>(data);
            return ServiceResult<DataPageResponse>.Success(dataResponse);
        }

        public async Task<ServiceResult<DataPageResponse>> AddDataAsync(DataPageRequest dataRequest)
        {
            DataPage data = new DataPage();
            data.NameData = data.NameData;
            data.NamePage = data.NamePage;
            data.InfRu = data.InfRu;


            var dataResponse = _mapper.Map<DataPageResponse>(data);

            await _context.AddAsync(data);
            await _context.SaveChangesAsync();
            return ServiceResult<DataPageResponse>.Success(dataResponse);
        }

        public async Task<ServiceResult<bool>> UpdateRuDataAsync(DataPageUpdateRequest dataRequest)
        {
            var data = await _context.DataPages.FirstOrDefaultAsync(c => c.Id == dataRequest.Id);

            if (data == null)
            {
                return ServiceResult<bool>.Failure("Таких данных не существует");
            }

            data.InfRu = dataRequest.Data;

            _context.DataPages.Update(data);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> UpdateEngDataAsync(DataPageUpdateRequest dataRequest)
        {
            var data = await _context.DataPages.FirstOrDefaultAsync(c => c.Id == dataRequest.Id);

            if (data == null)
            {
                return ServiceResult<bool>.Failure("Таких данных не существует");
            }

            data.InfEng = data.InfEng;

            _context.DataPages.Update(data);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> DeleteDataAsync(int id)
        {
            var data = await _context.DataPages.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
            {
                return ServiceResult<bool>.Failure("Таких данных не существует");
            }

            _context.DataPages.Remove(data);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

    }
}
