using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class BrandService(IBrandsRepository brandsRepo) : IBrandService
{
    public ResponseResult<BrandDetailDto> GetBrandById(Guid id)
    {
        var brand = brandsRepo.GetBrandById(id);
        if (brand == null) return ResponseResult<BrandDetailDto>.Failure("Brand not exist");
        
        return ResponseResult<BrandDetailDto>.Success(new BrandDetailDto(
            Id: brand.Id,
            Name: brand.Name,
            Description: brand.Description,
            Principal: brand.Principal,
            HeadQuarters: brand.HeadQuarters
        ));
    }
    
    public ResponseResult<BrandDetailDto> GetBrandByName(string name)
    {
        var brand = brandsRepo.GetByName(name);
        if (brand == null) return ResponseResult<BrandDetailDto>.Failure("Brand not exist");
        
        return ResponseResult<BrandDetailDto>.Success(new BrandDetailDto(
            Id: brand.Id,
            Name: brand.Name,
            Description: brand.Description,
            Principal: brand.Principal,
            HeadQuarters: brand.HeadQuarters
        ));
    }

    public ResponseResult<List<BrandListDto>> ListBrands()
    {
        var brands = brandsRepo.GetAllBrands().Select(d => new BrandListDto(
            Id: d.Id,
            Name: d.Name)
        ).ToList();
        return ResponseResult<List<BrandListDto>>.Success(brands);
    }
    
    public ResponseResult<bool> CreateBrand(BrandCreateDto dto)
    {
        var nameExits = brandsRepo.GetByName(dto.Name);
        if (nameExits is not null)
        {
            return ResponseResult<bool>.Failure(nameof(dto.Name), "Brand with this name already exist");
        }

        var brand = new Brand
        {
            Name = dto.Name,
            Description = dto.Description,
            Principal = dto.Principal,
            HeadQuarters = dto.HeadQuarters
        };

        brandsRepo.Create(brand);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateBrand(BrandUpdateDto dto)
    {
        var editable = brandsRepo.GetBrandById(dto.Id);
        if (editable is null) return ResponseResult<bool>.Failure("Series not exist");

        var brand = new Brand
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Principal = dto.Principal,
            HeadQuarters = dto.HeadQuarters
        };

        brandsRepo.Update(brand);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> DeleteBrand(Guid id)
    {
        var brand = brandsRepo.GetBrandById(id);
        if (brand is null) return ResponseResult<bool>.Failure("Series not exist");

        brandsRepo.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
}