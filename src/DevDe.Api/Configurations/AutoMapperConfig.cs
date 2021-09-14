using AutoMapper;
using DevDe.Api.ViewModels;
using DevIO.Business.Models;

namespace DevDe.Api.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap<ProdutoViewModel, Produto>();
            CreateMap<ProdutoImagemViewModel, Produto>().ReverseMap();

            CreateMap<Produto, ProdutoViewModel>().
                ForMember(destinationMember => destinationMember.NomeFornecedor, memberOptions => memberOptions.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}
