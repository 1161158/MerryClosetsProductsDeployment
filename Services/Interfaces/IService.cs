using System.Collections.Generic;
using MerryClosets.Models;
using MerryClosets.Models.DTO;

namespace MerryClosets.Services.Interfaces
{
    //https://www.vrddit.com/?v=r/portugal/comments/9imfc4
    public interface IService<Entity, DTO> where Entity : BaseEntity where DTO : BaseEntityDto
    {
        ValidationOutput Register(DTO dto);
        IEnumerable<DTO> GetAll();
        ValidationOutput GetByReference(string reference);
        ValidationOutput ClientGetByReference(string reference);
        ValidationOutput Update(string reference, DTO dto);
        ValidationOutput Remove(string reference);
    }
}