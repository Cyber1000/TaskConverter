using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;

namespace Converter.Core.Mapper
{
    public class Converter
    {
        public static TaskInfoModel MapToModel(TaskInfo taskInfo)
        {
            return GTDMapper.Mapper.Map<TaskInfoModel>(taskInfo);
        }

        public static TaskInfo MapFromModel(TaskInfoModel model)
        {
            return GTDMapper.Mapper.Map<TaskInfo>(model);
        }
    }
}
