﻿using AutoMapper;
using MerryClosets.Models.Category;
using MerryClosets.Models.Collection;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Material;
using MerryClosets.Models.Product;
using MerryClosets.Models.Restriction;

namespace MerryClosets.AutoMapper
{
    public class DtoToDomainMappingProfile : Profile
    {
        public DtoToDomainMappingProfile()
        {
            CreateMap<MaterialDto, Material>();
            CreateMap<ProductDto, Product>();
            CreateMap<PartDto, Part>();
            CreateMap<CategoryDto, Category>();
            CreateMap<ColorDto, Color>();
            CreateMap<FinishDto, Finish>();
            CreateMap<PriceDto, Price>();
            CreateMap<SlotDefinitionDto, SlotDefinition>();
            CreateMap<ConfiguredProductDto, ConfiguredProduct>();
            CreateMap<CatalogDto, Catalog>();
            CreateMap<CollectionDto, Collection>();
            CreateMap<ConfiguredMaterialDto, ConfiguredMaterial>();
            CreateMap<ConfiguredSlotDto, ConfiguredSlot>();
            CreateMap<ChildConfiguredProductDto, ConfiguredProduct>();
            CreateMap<ConfiguredPartDto, ConfiguredPart>();
            CreateMap<ProductCollectionDto, ProductCollection>();
            CreateMap<ProductMaterialDto, ProductMaterial>();
            CreateMap<DimensionValuesDto, DimensionValues>();
            CreateMap<ConfiguredDimensionDto, ConfiguredDimension>();
            CreateMap<DiscreteValueDto, DiscreteValue>();
            CreateMap<ContinuousValueDto, ContinuousValue>();
            CreateMap<PriceHistoryDto, PriceHistory>();
            CreateMap<ValuesDto, Values>().Include<DiscreteValueDto, DiscreteValue>().Include<ContinuousValueDto, ContinuousValue>();
            CreateMap<MaterialFinishPartAlgorithmDto, MaterialFinishPartAlgorithm>();
            CreateMap<RatioAlgorithmDto, RatioAlgorithm>();
            CreateMap<SizePercentagePartAlgorithmDto, SizePercentagePartAlgorithm>();
            CreateMap<AlgorithmDto, Algorithm>().Include<DimensionAlgorithmDto, DimensionAlgorithm>().Include<PartAlgorithmDto, PartAlgorithm>();
            CreateMap<DimensionAlgorithmDto, DimensionAlgorithm>().Include<RatioAlgorithmDto, RatioAlgorithm>();
            CreateMap<PartAlgorithmDto, PartAlgorithm>().Include<SizePercentagePartAlgorithmDto, SizePercentagePartAlgorithm>().Include<MaterialFinishPartAlgorithmDto, MaterialFinishPartAlgorithm>();
        }

    }
}
