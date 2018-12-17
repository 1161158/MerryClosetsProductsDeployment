using AutoMapper;
using MerryClosets.Models.Animation;
using MerryClosets.Models.Category;
using MerryClosets.Models.Collection;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Material;
using MerryClosets.Models.Product;
using MerryClosets.Models.Restriction;

namespace MerryClosets.AutoMapper
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<Material, MaterialDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Part, PartDto>();
            CreateMap<Color, ColorDto>();
            CreateMap<Finish, FinishDto>();
            CreateMap<Price, PriceDto>();
            CreateMap<SlotDefinition, SlotDefinitionDto>();
            CreateMap<ConfiguredProduct, ConfiguredProductDto>();
            CreateMap<ConfiguredProduct, ProductOrderDto>();
            CreateMap<Catalog, CatalogDto>();
            CreateMap<Collection, CollectionDto>();
            CreateMap<ConfiguredMaterial, ConfiguredMaterialDto>();
            CreateMap<ConfiguredPart, ConfiguredPartDto>();
            CreateMap<ConfiguredSlot, ConfiguredSlotDto>();
            CreateMap<ProductCollection, ProductCollectionDto>();
            CreateMap<ProductMaterial, ProductMaterialDto>();
            CreateMap<DimensionValues, DimensionValuesDto>();
            CreateMap<ConfiguredDimension, ConfiguredDimensionDto>();
            CreateMap<DiscreteValue, DiscreteValueDto>();
            CreateMap<ContinuousValue, ContinuousValueDto>();
            CreateMap<PriceHistory, PriceHistoryDto>();
            CreateMap<Values, ValuesDto>().Include<DiscreteValue, DiscreteValueDto>().Include<ContinuousValue, ContinuousValueDto>();
            CreateMap<MaterialFinishPartAlgorithm, MaterialFinishPartAlgorithmDto>();
            CreateMap<RatioAlgorithm, RatioAlgorithmDto>();
            CreateMap<SizePercentagePartAlgorithm, SizePercentagePartAlgorithmDto>();
            CreateMap<DimensionAlgorithm, DimensionAlgorithmDto>().Include<RatioAlgorithm, RatioAlgorithmDto>();
            CreateMap<PartAlgorithm, PartAlgorithmDto>().Include<SizePercentagePartAlgorithm, SizePercentagePartAlgorithmDto>().Include<MaterialFinishPartAlgorithm, MaterialFinishPartAlgorithmDto>();
            CreateMap<Algorithm, AlgorithmDto>().Include<DimensionAlgorithm, DimensionAlgorithmDto>().Include<PartAlgorithm, PartAlgorithmDto>();
            CreateMap<ModelGroup, ModelGroupDto>();
            CreateMap<Component, ComponentDto>();
            CreateMap<FrontalOpenAnimation, FrontalOpenAnimationDto>();
            CreateMap<LateralOpenAnimation, LateralOpenAnimationDto>();
            CreateMap<SlidingLeftAnimation, SlidingLeftAnimationDto>();
            CreateMap<SlidingRightAnimation, SlidingRightAnimationDto>();
            CreateMap<SlidingOpenAnimation, SlidingOpenAnimationDto>().Include<SlidingLeftAnimation, SlidingLeftAnimationDto>().Include<SlidingRightAnimation, SlidingRightAnimationDto>();
            CreateMap<Animation, AnimationDto>().Include<FrontalOpenAnimation, FrontalOpenAnimationDto>().Include<LateralOpenAnimation, LateralOpenAnimationDto>().Include<SlidingOpenAnimation, SlidingOpenAnimationDto>();
        }
    }
}
