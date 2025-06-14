using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class DataPageService : IDataPageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public DataPageService(ApplicationDbContext context, IMapper mapper, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
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

        public async Task<ServiceResult<bool>> AboutUpdateAsync(AboutRequest request, string language)
        {
            var dataAbout = await _context.DataPages
                .Where(d => d.NamePage == "About")
            .ToListAsync();

            var card1 = dataAbout.Where(x => x.NameData == "Card1").FirstOrDefault();
            var cardTitle1 = dataAbout.Where(x => x.NameData == "CardTitle1").FirstOrDefault();
            var card2 = dataAbout.Where(x => x.NameData == "Card2").FirstOrDefault();
            var cardTitle2 = dataAbout.Where(x => x.NameData == "CardTitle2").FirstOrDefault();
            var card3 = dataAbout.Where(x => x.NameData == "Card3").FirstOrDefault();
            var cardTitle3 = dataAbout.Where(x => x.NameData == "CardTitle3").FirstOrDefault();
            var leadTitle = dataAbout.Where(x => x.NameData == "LeadTitle").FirstOrDefault();
            var leadText = dataAbout.Where(x => x.NameData == "LeadText").FirstOrDefault();

            if(language == "ru")
            {
                card1.InfRu = request.Card1;
                cardTitle1.InfRu = request.CardTitle1;
                card2.InfRu = request.Card2;
                cardTitle2.InfRu = request.CardTitle2;
                card3.InfRu = request.Card3;
                cardTitle3.InfRu = request.CardTitle3;
                leadTitle.InfRu = request.LeadTitle;
                leadText.InfRu = request.LeadText;
            }
            else
            {
                    card1.InfEng = request.Card1;
                    cardTitle1.InfEng = request.CardTitle1;
                    card2.InfEng = request.Card2;
                    cardTitle2.InfEng = request.CardTitle2;
                    card3.InfEng = request.Card3;
                    cardTitle3.InfEng = request.CardTitle3;
                    leadTitle.InfEng = request.LeadTitle;
                    leadText.InfEng = request.LeadText;
            }

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> AboutImageAsync(AboutImageRequest request, int numImage, string lang)
        {
            var dataAbout = await _context.DataPages
                .Where(d => d.NamePage == "About")
                .ToListAsync();

            if (request.Image != null)
            {
                var existImage = await _context.Images
                    .Where(i => i.ImageType == "image" && i.EntityId == numImage && i.EntityTarget == "About")
                    .FirstOrDefaultAsync();

                if (existImage != null)
                {
                    await _imageService.RemoveImageFromServer(existImage.Id);
                }

                await _imageService.AddUploadedImageAsync("About", numImage, "image", request.Image);
            }

            DataPage imageTitle = null, imageContent = null;

            switch (numImage) {
                case 1:
            imageTitle = dataAbout.Where(x => x.NameData == "ImageTitle1").FirstOrDefault();
            imageContent = dataAbout.Where(x => x.NameData == "ImageContent1").FirstOrDefault();
                    break;
                case 2:
            imageTitle = dataAbout.Where(x => x.NameData == "ImageTitle2").FirstOrDefault();
            imageContent = dataAbout.Where(x => x.NameData == "ImageContent2").FirstOrDefault();
                    break;

                case 3:
            imageTitle = dataAbout.Where(x => x.NameData == "ImageTitle3").FirstOrDefault();
            imageContent = dataAbout.Where(x => x.NameData == "ImageContent3").FirstOrDefault();
                    break;

                case 4:
            imageTitle = dataAbout.Where(x => x.NameData == "ImageTitle4").FirstOrDefault();
            imageContent = dataAbout.Where(x => x.NameData == "ImageContent4").FirstOrDefault();
                    break;

                case 5:
            imageTitle = dataAbout.Where(x => x.NameData == "ImageTitle5").FirstOrDefault();
            imageContent = dataAbout.Where(x => x.NameData == "ImageContent5").FirstOrDefault();
                    break;

            }

            if(lang == "ru")
            {
                imageTitle.InfRu = request.Title;
                imageContent.InfRu = request.Content;
            }
            else
            {
                    imageTitle.InfEng = request.Title;
                    imageContent.InfEng = request.Content;
            }

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);

        }

        public async Task<ServiceResult<AboutImages>> AboutEditViewsAsync()
        {
            var images = await _context.Images
                .Where(i => i.EntityTarget == "About")
                .OrderBy(i => i.EntityId)
                .Take(5)
                .ToListAsync();


            var viewImages = new AboutImages()
            {
                Images = _mapper.Map<List<ImageResponse>>(images)
            };

            return ServiceResult<AboutImages>.Success(viewImages);
        }


    }
}
