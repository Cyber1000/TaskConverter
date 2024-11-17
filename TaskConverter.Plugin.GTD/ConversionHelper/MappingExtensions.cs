using AutoMapper;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public static class MappingExtensions
{
    public static IMappingExpression<TDestination, TSource> ReverseMapWithValidation<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping) => mapping.ReverseMap().ValidateMemberList(MemberList.Destination);
}