using AutoMapper;
using TranslationApi.Application.DTOs;
using TranslationApi.Application.Interfaces;
using TranslationApi.Domain.Entities;
using TranslationApi.Domain.Interfaces;
namespace TranslationApi.Application.Services
{
    public class AIModelService : IAIModelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AIModelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AIModelDto>> GetAllModelDtosAsync()
        {
            var models = await _unitOfWork.AIModels.GetAllAsync();
            return _mapper.Map<IEnumerable<AIModelDto>>(models);
        }

        public async Task<IEnumerable<AIModelDto>> GetActiveModelDtosAsync()
        {
            var models = await _unitOfWork.AIModels.GetActiveModelsAsync();
            return _mapper.Map<IEnumerable<AIModelDto>>(models);
        }

        public async Task<AIModelDto?> GetModelDtoByIdAsync(Guid id)
        {
            var model = await _unitOfWork.AIModels.GetByIdAsync(id);
            return model != null ? _mapper.Map<AIModelDto>(model) : null;
        }

        public async Task<AIModelDto> CreateModelAsync(AIModelDto createDto)
        {
            var model = _mapper.Map<AIModel>(createDto);
            model.ChatSessions = new List<ChatSession>();

            await _unitOfWork.AIModels.AddAsync(model);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AIModelDto>(model);
        }

        public async Task<bool> UpdateModelAsync(Guid id, AIModelDto updateDto)
        {
            var existingModel = await _unitOfWork.AIModels.GetByIdAsync(id);
            if (existingModel == null)
                return false;

            _mapper.Map(updateDto, existingModel);
            await _unitOfWork.AIModels.UpdateAsync(existingModel);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ModelExistsAsync(string name, string version)
        {
            return await _unitOfWork.AIModels.ExistsAsync(name, version);
        }

        public async Task ActivateModelAsync(Guid id)
        {
            var model = await _unitOfWork.AIModels.GetByIdAsync(id);
            if (model != null)
            {
                model.IsActive = true;
                await _unitOfWork.AIModels.UpdateAsync(model);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeactivateModelAsync(Guid id)
        {
            await _unitOfWork.AIModels.DeactivateModelAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeleteModelAsync(Guid id)
        {
            var model = await _unitOfWork.AIModels.GetByIdAsync(id);
            if (model != null)
            {
                await _unitOfWork.AIModels.RemoveAsync(model);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            return false;
        }

        // Internal methods for service layer use
        public async Task<AIModel?> GetModelByIdAsync(Guid id)
        {
            return await _unitOfWork.AIModels.GetByIdAsync(id);
        }

        public async Task<AIModel?> GetModelByNameAndVersionAsync(string name, string version)
        {
            return await _unitOfWork.AIModels.GetByNameAndVersionAsync(name, version);
        }


    }
}