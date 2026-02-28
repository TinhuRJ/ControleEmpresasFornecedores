using AutoMapper;
using ControleEmpresasFornecedores.Api.DTOs;
using ControleEmpresasFornecedores.Api.Entities;

namespace ControleEmpresasFornecedores.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Fornecedor -> Response
        CreateMap<Fornecedor, FornecedorResponseDto>();

        // CreateDto -> Entity
        CreateMap<FornecedorCreateDto, Fornecedor>();

        // Update (CreateDto -> Entity existente)
        CreateMap<FornecedorCreateDto, Fornecedor>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}