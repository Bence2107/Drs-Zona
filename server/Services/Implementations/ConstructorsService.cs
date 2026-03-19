using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Types;

namespace Services.Implementations;

public class ConstructorsService(
    IConstructorsRepository constructorRepo, 
    IBrandsRepository brandsRepo,
    IContractsRepository contractsRepo
) : IConstructorsService
{
    public async Task<ResponseResult<ConstructorDetailDto>> GetById(Guid id)
    {
        if (!await constructorRepo.CheckIfIdExists(id)) return ResponseResult<ConstructorDetailDto>.Failure("Constructor not found");

        var constructor = await constructorRepo.GetByIdWithBrand(id);
        if (constructor is { Brand: null }) return ResponseResult<ConstructorDetailDto>.Failure("Brand is invalid");
        if (constructor != null && !await brandsRepo.CheckIfIdExists(constructor.BrandId)) return ResponseResult<ConstructorDetailDto>.Failure("Brand not found");

        var contracts = await contractsRepo.GetByTeamId(id);
        var driverNameRecord = new List<DriverNameRecord>();
        if (contracts.Count > 0)
        {
            driverNameRecord.AddRange(contracts.Select(contract => new DriverNameRecord(Id: contract.DriverId, Name: contract.Driver!.Name)));
        } 
        
        var dto = new ConstructorDetailDto(
            Id:  constructor!.Id,
            BrandId: constructor.BrandId,
            BrandName: constructor.Brand!.Name,
            BrandDescription: constructor.Brand.Description,
            Name: constructor.Name,
            Nickname: constructor.Nickname,
            FoundedYear: constructor.FoundedYear,
            HeadQuarters: constructor.HeadQuarters,
            TeamChief: constructor.TeamChief,
            TechnicalChief: constructor.TechnicalChief,
            TotalPodiums: constructor.Podiums,
            TotalWins: constructor.Wins,
            Championships: constructor.Championships,
            DriverNames: driverNameRecord,
            Seasons: constructor.Seasons
        );
        
        return ResponseResult<ConstructorDetailDto>.Success(dto);
    }
    
    public async Task<ResponseResult<List<ConstructorListDto>>> GetAllConstructors()
    {
        var constructors = await constructorRepo.GetAllConstructor();
    
        var dto = constructors.Select(c => new ConstructorListDto(
            Id: c.Id,
            Name: c.Name
        ))
            .OrderBy(c => c.Name)
            .ToList();
    
        return ResponseResult<List<ConstructorListDto>>.Success(dto);
    }
    
    public async Task<ResponseResult<bool>> Create(ConstructorCreateDto constructorCreateDto)
    {
        var existingConstructor = await constructorRepo.GetByName(constructorCreateDto.Name);
        if (existingConstructor != null) return ResponseResult<bool>.Failure("Constructor already is in the Championship");
        
        var constructor = new Constructor
        {
            Name = constructorCreateDto.Name,
            Nickname = constructorCreateDto.Nickname,
            FoundedYear = constructorCreateDto.FoundedYear,
            HeadQuarters = constructorCreateDto.HeadQuarters,
            TeamChief = constructorCreateDto.TeamChief,
            TechnicalChief = constructorCreateDto.TechnicalChief,
            Wins = constructorCreateDto.Wins,
            Championships = constructorCreateDto.Championships,
            Podiums = constructorCreateDto.Podiums,
            Seasons = constructorCreateDto.Seasons
        };

        if (await brandsRepo.CheckIfIdExists(constructorCreateDto.BrandId))
        {
            constructor.BrandId = constructorCreateDto.BrandId;
        }

        await constructorRepo.Create(constructor);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Update(ConstructorUpdateDto constructorUpdateDto)
    {
        var constructor = await constructorRepo.GetConstructorById(constructorUpdateDto.Id);
        if (constructor == null) return ResponseResult<bool>.Failure("Constructor not found");

        if (constructorUpdateDto.BrandId != constructor.BrandId)
        {
            if (!await brandsRepo.CheckIfIdExists(constructorUpdateDto.BrandId))
            {
                return ResponseResult<bool>.Failure("Brand not found");
            }
        }
        
        constructor.Id = constructorUpdateDto.Id;
        constructor.BrandId = constructorUpdateDto.BrandId;
        constructor.Name = constructorUpdateDto.Name;
        constructor.Nickname = constructorUpdateDto.Nickname;
        constructor.FoundedYear = constructorUpdateDto.FoundedYear;
        constructor.HeadQuarters = constructorUpdateDto.HeadQuarters;
        constructor.TeamChief = constructorUpdateDto.TeamChief;
        constructor.TechnicalChief = constructorUpdateDto.TechnicalChief;
        constructor.Wins = constructorUpdateDto.Wins;
        constructor.Championships = constructorUpdateDto.Championships;
        constructor.Podiums = constructorUpdateDto.Podiums;
        constructor.Seasons = constructorUpdateDto.Seasons;
        
        await constructorRepo.Update(constructor);
        return ResponseResult<bool>.Success(true);
    }
}