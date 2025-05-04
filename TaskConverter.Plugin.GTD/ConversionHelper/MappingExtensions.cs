using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Configuration;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public static class MappingExtensions
{
    public static IMappingExpression<TDestination, TSource> ReverseMapWithValidation<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping) =>
        mapping.ReverseMap().ValidateMemberList(MemberList.Destination);

    public static IMappingExpression<TSource, TDestination> IgnoreMembers<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> mapping,
        params Expression<Func<TDestination, object>>[] members
    )
    {
        foreach (var member in members)
        {
            mapping.ForMember(member, opt => opt.Ignore());
        }

        return mapping;
    }
}
