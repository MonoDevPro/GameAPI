using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Specifications;

/// <summary>
/// Seleciona todos os personagens que estão inativos, para serem removidos permanentemente.
/// </summary>
public class InactiveCharactersForPurgeSpec : BaseSpecification<Character>
{
    public InactiveCharactersForPurgeSpec() : base(c => !c.IsActive)
    {
        IgnoreQueryFiltersEnabled();
        // A entidade será apagada, então o tracking é necessário.
    }
}
