using AutoMapper;
using Transfer.Grains.Snapshot;
using Transfer.IGrains.Dto;
using Transfer.Repository.Entities;

namespace Transfer.Grains.AutoMapper
{
    public class TransferProfile : Profile
    {
        public TransferProfile()
        {
            this.CreateMap<Account, AccountSnapshot>().ReverseMap();
            this.CreateMap<ProjectSnapshot, ProjectDto>().ReverseMap();
        }
    }
}
