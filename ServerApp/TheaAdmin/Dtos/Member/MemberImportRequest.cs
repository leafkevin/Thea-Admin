using MiniExcelLibs.Attributes;
using TheaAdmin.Domain;

namespace TheaAdmin.Dtos;

public class MemberImportRequest
{
    public string MemberId { get; set; }
    [ExcelColumnName("*会员名称")]
    public string MemberName { get; set; }
    [ExcelColumnName("*手机号")]
    public string Mobile { get; set; }
    [ExcelColumnName("性别")]
    public string GenderDescription { get; set; }
    public Gender Gender => GenderDescription switch
    {
        "未知" => Gender.Unknown,
        "男性" => Gender.Male,
        "女性" => Gender.Female,
        _ => Gender.Unknown
    };
    [ExcelColumnName("余额")]
    public double Balance { get; set; }
    [ExcelColumnName("描述")]
    public string Description { get; set; }
}
